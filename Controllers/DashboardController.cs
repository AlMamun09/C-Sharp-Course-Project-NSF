using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalScout.Data;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Threading.Tasks;
using LocalScout.Models;

namespace LocalScout.Controllers
{
    [Authorize] // Ensures only logged-in users can access this controller
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly FirestoreService _firestoreService;

        public DashboardController(UserManager<ApplicationUser> userManager, CloudinaryService cloudinaryService, SignInManager<ApplicationUser> signInManager, FirestoreService firestoreService)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _signInManager = signInManager;
            _firestoreService = firestoreService;
        }

        // --- ACTIONS for Becoming a Provider ---
        [HttpGet]
        public async Task<IActionResult> BecomeProvider()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new BecomeProviderViewModel
            {
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessPhoneNumber = user.BusinessPhoneNumber ?? string.Empty,
                BusinessAddress = user.BusinessAddress ?? string.Empty,
                BusinessDescription = user.BusinessDescription ?? string.Empty,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };
            return View(model);
        }

        // --- NEW GENERAL-PURPOSE USER PROFILE ACTION ---
        [HttpGet]
        public async Task<IActionResult> UserProfile(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var stats = await _firestoreService.GetBookingStatsForUserAsync(id);

            var viewModel = new UserProfileViewModel
            {
                UserProfile = user,
                Stats = stats
            };

            // Determine which layout to use based on the viewer's role
            if (User.IsInRole("ServiceProvider"))
            {
                ViewData["Layout"] = "_ProviderLayout";
            }
            else
            {
                ViewData["Layout"] = "_DashboardLayout";
            }

            return View(viewModel);
        }

        // This action is now for the user's OWN profile page
        [HttpGet]
        public IActionResult MyProfile()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }
            // Simply redirect to the general UserProfile action with the current user's ID
            return RedirectToAction("UserProfile", new { id = userId });
        }

        [HttpPost]
        public async Task<IActionResult> BecomeProvider(BecomeProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                if (model.ProfilePicture != null)
                {
                    var uploadResult = await _cloudinaryService.UploadProfileImageAsync(model.ProfilePicture);
                    if (uploadResult.Error == null)
                    {
                        user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                user.BusinessName = model.BusinessName;
                user.BusinessPhoneNumber = model.BusinessPhoneNumber;
                user.BusinessAddress = model.BusinessAddress;
                user.BusinessDescription = model.BusinessDescription;
                user.BusinessHours = model.BusinessHours;

                await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "ServiceProvider");
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Congratulations! You are now registered as a Service Provider.";
                return RedirectToAction("Index", "ServiceProvider");
            }
            return View(model);
        }

        // --- NEW ACTIONS for Editing a Regular User's Profile ---

        // This GET action shows the pre-filled edit form for a regular user.
        [HttpGet]
        public async Task<IActionResult> EditUserProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditUserViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };

            return View(model);
        }

        // This POST action processes the submitted changes for a regular user.
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

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
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;

                // Save changes to the database
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Redirect back to the main dashboard
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- METHOD FOR CUSTOMER BOOKING HISTORY ---
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Challenge();
            }

            var bookings = await _firestoreService.GetBookingsForUserAsync(currentUser.Id);

            var bookingViewModels = new List<BookingDetailsViewModel>();

            foreach (var booking in bookings)
            {
                var provider = await _userManager.FindByIdAsync(booking.ProviderId);
                var service = await _firestoreService.GetProviderServiceByIdAsync(booking.ServiceId);

                if (provider != null && service != null)
                {
                    bookingViewModels.Add(new BookingDetailsViewModel
                    {
                        Booking = booking,
                        ProviderBusinessName = provider.BusinessName ?? $"{provider.FirstName} {provider.LastName}",
                        ServicePrimaryImageUrl = service.ImageUrls.FirstOrDefault()
                    });
                }
            }

            return View(bookingViewModels);
        }
    }
}
