using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LocalScout.Data;
using LocalScout.Models;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalScout.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;
        private const int PageSize = 6;

        public HomeController(
            ILogger<HomeController> logger,
            FirestoreService firestoreService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _firestoreService = firestoreService;
            _userManager = userManager;
        }

        // --- INITIAL PAGE LOAD ACTIONS ---

        public async Task<IActionResult> Index()
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            var activeCategoryIds = new HashSet<string>(categories.Select(c => c.Id));

            // --- NEW LOOPING LOGIC TO ENSURE A FULL PAGE OF ACTIVE SERVICES ---
            var activeServices = new List<ProviderService>();
            string? lastDocId = null;
            bool hasMoreData = true;

            while (activeServices.Count < PageSize && hasMoreData)
            {
                var paginatedResult = await _firestoreService.GetServicesPaginatedAsync(null, PageSize, lastDocId);

                var filteredPage = paginatedResult.Services
                    .Where(s => activeCategoryIds.Contains(s.ServiceCategoryId))
                    .ToList();

                activeServices.AddRange(filteredPage);

                lastDocId = paginatedResult.LastDocumentId;
                hasMoreData = paginatedResult.HasMorePages;
            }

            var servicesToShow = activeServices.Take(PageSize).ToList();
            string? nextLastDocumentId = servicesToShow.Count > 0 ? servicesToShow.Last().Id : null;
            bool hasMorePages = activeServices.Count > PageSize || hasMoreData;
            // --- END NEW LOGIC ---

            var featuredServiceCards = await MapServicesToCards(servicesToShow);

            ViewBag.HasMorePages = hasMorePages;
            ViewBag.LastDocumentId = nextLastDocumentId;

            var viewModel = new HomeViewModel
            {
                Categories = categories,
                FeaturedServices = featuredServiceCards
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string query)
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            var activeCategoryIds = new HashSet<string>(categories.Select(c => c.Id));

            var paginatedResult = await _firestoreService.SearchServicesPaginatedAsync(query, PageSize, 1);

            // Filter search results to only include those from active categories
            var activeServices = paginatedResult.Services
                .Where(s => activeCategoryIds.Contains(s.ServiceCategoryId))
                .ToList();

            var resultCards = await MapServicesToCards(activeServices);

            ViewBag.HasMorePages = paginatedResult.HasMorePages;
            ViewBag.CurrentPage = 1;

            var viewModel = new SearchResultsViewModel
            {
                Query = query,
                Results = resultCards
            };

            return View(viewModel);
        }

        public async Task<IActionResult> ServicesByCategory(string id)
        {
            // This action is implicitly safe, because a user can only get here
            // by clicking a category from the homepage, and we only show active categories there.
            var category = await _firestoreService.GetCategoryByIdAsync(id);
            if (category == null || !category.IsActive)
            {
                return NotFound();
            }

            var paginatedResult = await _firestoreService.GetServicesPaginatedAsync(id, PageSize, null);
            var serviceCards = await MapServicesToCards(paginatedResult.Services);

            ViewBag.HasMorePages = paginatedResult.HasMorePages;
            ViewBag.LastDocumentId = paginatedResult.LastDocumentId;

            var viewModel = new CategoryServicesViewModel
            {
                CategoryName = category.Name,
                Services = serviceCards
            };

            return View(viewModel);
        }

        // --- AJAX ENDPOINTS FOR "LOAD MORE" ---

        [HttpGet]
        public async Task<IActionResult> LoadMoreIndexServices(string? lastDocumentId)
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            var activeCategoryIds = new HashSet<string>(categories.Select(c => c.Id));

            var activeServices = new List<ProviderService>();
            string? lastDocId = lastDocumentId;
            bool hasMoreData = true;

            while (activeServices.Count < PageSize && hasMoreData)
            {
                var paginatedResult = await _firestoreService.GetServicesPaginatedAsync(null, PageSize, lastDocId);
                var filteredPage = paginatedResult.Services
                    .Where(s => activeCategoryIds.Contains(s.ServiceCategoryId))
                    .ToList();
                activeServices.AddRange(filteredPage);
                lastDocId = paginatedResult.LastDocumentId;
                hasMoreData = paginatedResult.HasMorePages;
            }

            var servicesToShow = activeServices.Take(PageSize).ToList();
            string? nextLastDocumentId = servicesToShow.Count > 0 ? servicesToShow.Last().Id : null;
            bool hasMorePages = activeServices.Count > PageSize || hasMoreData;

            var serviceCards = await MapServicesToCards(servicesToShow);

            // Send final pagination data back in headers
            Response.Headers.Append("X-Has-More-Pages", hasMorePages.ToString());
            Response.Headers.Append("X-Last-Document-Id", nextLastDocumentId ?? "");

            return PartialView("_ServiceGridPartial", serviceCards);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreCategoryServices(string categoryId, string? lastDocumentId)
        {
            var paginatedResult = await _firestoreService.GetServicesPaginatedAsync(categoryId, PageSize, lastDocumentId);
            var serviceCards = await MapServicesToCards(paginatedResult.Services);

            // This action is simple because the category is already active
            Response.Headers.Append("X-Has-More-Pages", paginatedResult.HasMorePages.ToString());
            Response.Headers.Append("X-Last-Document-Id", paginatedResult.LastDocumentId ?? "");

            return PartialView("_ServiceGridPartial", serviceCards);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreSearchResults(string query, int pageNumber)
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            var activeCategoryIds = new HashSet<string>(categories.Select(c => c.Id));

            var paginatedResult = await _firestoreService.SearchServicesPaginatedAsync(query, PageSize, pageNumber);

            var activeServices = paginatedResult.Services
                .Where(s => activeCategoryIds.Contains(s.ServiceCategoryId))
                .ToList();

            var resultCards = await MapServicesToCards(activeServices);

            // This check for "hasMore" is against the original paginated result
            Response.Headers.Append("X-Has-More-Pages", paginatedResult.HasMorePages.ToString());

            return PartialView("_ServiceGridPartial", resultCards);
        }


        // --- HELPER METHOD ---
        // (This method is unchanged)
        private async Task<List<ServiceCardViewModel>> MapServicesToCards(List<ProviderService> services)
        {
            var serviceCards = new List<ServiceCardViewModel>();
            foreach (var service in services)
            {
                var provider = await _userManager.FindByIdAsync(service.ProviderId);
                if (provider != null)
                {
                    serviceCards.Add(new ServiceCardViewModel
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        Price = service.Price,
                        PricingUnit = service.PricingUnit,
                        IsNegotiable = service.IsNegotiable,
                        PrimaryImageUrl = service.ImageUrls.FirstOrDefault(),
                        ProviderBusinessName = provider.BusinessName ?? "N/A",
                        ProviderProfilePictureUrl = provider.ProfilePictureUrl,
                        Location = provider.BusinessAddress,
                        BusinessHours = provider.BusinessHours,
                        JoinedDate = provider.CreatedAt
                    });
                }
            }
            return serviceCards;
        }

        // --- Other existing actions (ServiceDetails, Privacy, Error) ---
        // (No changes needed for these methods)
        public async Task<IActionResult> ServiceDetails(string id)
        {
            var mainService = await _firestoreService.GetProviderServiceByIdAsync(id);
            if (mainService == null) return NotFound();
            var provider = await _userManager.FindByIdAsync(mainService.ProviderId);
            if (provider == null) return NotFound();
            var allProviderServices = await _firestoreService.GetServicesByProviderIdAsync(provider.Id);
            var otherServices = allProviderServices.Where(s => s.Id != id).ToList();
            var viewModel = new ServiceDetailsViewModel
            {
                MainService = mainService,
                Provider = provider,
                OtherServices = otherServices
            };
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}