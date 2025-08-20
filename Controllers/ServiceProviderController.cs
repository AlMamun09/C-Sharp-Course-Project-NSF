using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.Services;
using NeighborhoodServiceFinder.ViewModels;
using System.Threading.Tasks;

namespace NeighborhoodServiceFinder.Controllers
{
    [Authorize(Roles = "ServiceProvider, Admin")]
    public class ServiceProviderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Updated constructor to include all necessary services
        public ServiceProviderController(UserManager<ApplicationUser> userManager, CloudinaryService cloudinaryService, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _signInManager = signInManager;
        }

        // This action shows the provider's profile (View mode)
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Unable to load user.");
            }
            return View(user);
        }

        // --- NEW ACTIONS for Editing the Profile ---

        // This GET action shows the pre-filled edit form
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditProfileViewModel
            {
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessPhoneNumber = user.BusinessPhoneNumber ?? string.Empty,
                BusinessAddress = user.BusinessAddress ?? string.Empty,
                BusinessDescription = user.BusinessDescription ?? string.Empty,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        // This POST action processes the submitted changes
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                // Handle optional new profile picture upload
                if (model.ProfilePicture != null)
                {
                    var uploadResult = await _cloudinaryService.UploadProfileImageAsync(model.ProfilePicture);
                    if (uploadResult.Error == null)
                    {
                        user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                // Update user properties
                user.BusinessName = model.BusinessName;
                user.BusinessPhoneNumber = model.BusinessPhoneNumber;
                user.BusinessAddress = model.BusinessAddress;
                user.BusinessDescription = model.BusinessDescription;

                // Save changes to the database
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Redirect back to the profile view page
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If the form is not valid, show it again with errors
            return View(model);
        }
    }
}