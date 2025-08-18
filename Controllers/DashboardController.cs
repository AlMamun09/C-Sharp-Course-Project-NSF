using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.Services;
using NeighborhoodServiceFinder.ViewModels;
using System.Threading.Tasks;

namespace NeighborhoodServiceFinder.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DashboardController(UserManager<ApplicationUser> userManager, CloudinaryService cloudinaryService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _signInManager = signInManager;
        }

        // This action shows the main dashboard page.
        public IActionResult Index()
        {
            return View();
        }

        // --- NEW ACTIONS for Becoming a Provider ---

        // This GET action shows the empty form.
        [HttpGet]
        public IActionResult BecomeProvider()
        {
            return View();
        }

        // This POST action processes the submitted form data.
        [HttpPost]
        public async Task<IActionResult> BecomeProvider(BecomeProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                if (model.ProfilePicture != null)
                {
                    var uploadResult = await _cloudinaryService.UploadProfileImageAsync(model.ProfilePicture);
                    if (uploadResult.Error == null)
                    {
                        user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                // Update the user's details with all the business info
                user.BusinessName = model.BusinessName;
                user.BusinessPhoneNumber = model.BusinessPhoneNumber;
                user.BusinessAddress = model.BusinessAddress;
                user.BusinessDescription = model.BusinessDescription;

                await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "ServiceProvider");
                await _signInManager.RefreshSignInAsync(user);

                // Store a success message that will be displayed on the next page
                TempData["SuccessMessage"] = "Congratulations! You are now registered as a Service Provider.";

                // Redirect to the new ServiceProvider dashboard
                return RedirectToAction("Index", "ServiceProvider");
            }

            return View(model);
        }
    }
}