using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LocalScout.Data;
using LocalScout.Models;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LocalScout.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;

        // Updated constructor to include all necessary services
        public HomeController(
            ILogger<HomeController> logger,
            FirestoreService firestoreService,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _firestoreService = firestoreService;
            _userManager = userManager;
        }

        // This is the new, powerful Index action for our homepage
        public async Task<IActionResult> Index()
        {
            // 1. Fetch all active service categories
            var categories = await _firestoreService.GetAllCategoriesAsync();

            // 2. Fetch a batch of the latest services to feature
            var latestServices = await _firestoreService.GetRandomServicesAsync(6); // Get 6 services

            var featuredServiceCards = new List<ServiceCardViewModel>();

            // 3. For each service, get its provider's details and build a ServiceCardViewModel
            foreach (var service in latestServices)
            {
                var provider = await _userManager.FindByIdAsync(service.ProviderId);
                if (provider != null)
                {
                    var card = new ServiceCardViewModel
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        Price = service.Price,
                        PricingUnit = service.PricingUnit,
                        PrimaryImageUrl = service.ImageUrls.FirstOrDefault(), // Get the first image as the primary one
                        ProviderBusinessName = provider.BusinessName ?? "N/A",
                        ProviderProfilePictureUrl = provider.ProfilePictureUrl,
                        Location = provider.BusinessAddress,
                        BusinessHours = provider.BusinessHours,
                        JoinedDate = provider.CreatedAt
                    };
                    featuredServiceCards.Add(card);
                }
            }

            // 4. Package everything up in our HomeViewModel
            var viewModel = new HomeViewModel
            {
                Categories = categories,
                FeaturedServices = featuredServiceCards
            };

            // 5. Pass the complete ViewModel to the view
            return View(viewModel);
        }

        // This action handles the search form submission
        public async Task<IActionResult> Search(string query)
        {
            // 1. Use our service to find all services matching the query
            var matchingServices = await _firestoreService.SearchServicesAsync(query);

            var resultCards = new List<ServiceCardViewModel>();

            // 2. For each matching service, get its provider's details
            foreach (var service in matchingServices)
            {
                var provider = await _userManager.FindByIdAsync(service.ProviderId);
                if (provider != null)
                {
                    var card = new ServiceCardViewModel
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        Price = service.Price,
                        PricingUnit = service.PricingUnit,
                        PrimaryImageUrl = service.ImageUrls.FirstOrDefault(),
                        ProviderBusinessName = provider.BusinessName ?? "N/A",
                        ProviderProfilePictureUrl = provider.ProfilePictureUrl,
                        Location = provider.BusinessAddress,
                        BusinessHours = provider.BusinessHours,
                        JoinedDate = provider.CreatedAt
                    };
                    resultCards.Add(card);
                }
            }

            // 3. Package the query and the results into our new ViewModel
            var viewModel = new SearchResultsViewModel
            {
                Query = query,
                Results = resultCards
            };

            // 4. Pass the ViewModel to a new "Search" view
            return View(viewModel);
        }

        // This action shows all services for a given category
        public async Task<IActionResult> ServicesByCategory(string id)
        {
            // 1. Get the details of the category that was clicked
            var category = await _firestoreService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // 2. Use our new service method to find all services in this category
            var servicesInCategory = await _firestoreService.GetServicesByCategoryIdAsync(id);

            var serviceCards = new List<ServiceCardViewModel>();

            // 3. For each service, get its provider's details and build a ServiceCardViewModel
            foreach (var service in servicesInCategory)
            {
                var provider = await _userManager.FindByIdAsync(service.ProviderId);
                if (provider != null)
                {
                    var card = new ServiceCardViewModel
                    {
                        ServiceId = service.Id,
                        ServiceName = service.ServiceName,
                        Price = service.Price,
                        PricingUnit = service.PricingUnit,
                        PrimaryImageUrl = service.ImageUrls.FirstOrDefault(),
                        ProviderBusinessName = provider.BusinessName ?? "N/A",
                        ProviderProfilePictureUrl = provider.ProfilePictureUrl,
                        Location = provider.BusinessAddress,
                        BusinessHours = provider.BusinessHours,
                        JoinedDate = provider.CreatedAt
                    };
                    serviceCards.Add(card);
                }
            }

            // 4. Package the category name and the results into our new ViewModel
            var viewModel = new CategoryServicesViewModel
            {
                CategoryName = category.Name,
                Services = serviceCards
            };

            // 5. Pass the ViewModel to a new "ServicesByCategory" view
            return View(viewModel);
        }

        public async Task<IActionResult> ServiceDetails(string id)
        {
            // 1. Fetch the main service the user clicked on
            var mainService = await _firestoreService.GetProviderServiceByIdAsync(id);
            if (mainService == null)
            {
                return NotFound();
            }

            // 2. Fetch the provider's details using the ID from the service
            var provider = await _userManager.FindByIdAsync(mainService.ProviderId);
            if (provider == null)
            {
                return NotFound();
            }

            // 3. Fetch ALL services offered by this provider
            var allProviderServices = await _firestoreService.GetServicesByProviderIdAsync(provider.Id);

            // 4. Create a list of "other" services, excluding the one we're currently viewing
            var otherServices = allProviderServices.Where(s => s.Id != id).ToList();

            // 5. Package everything into our new ViewModel
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
