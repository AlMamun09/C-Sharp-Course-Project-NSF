using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.ViewModels;
using System.Threading.Tasks;
using NeighborhoodServiceFinder.Services;

namespace NeighborhoodServiceFinder.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CloudinaryService _cloudinaryService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, CloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudinaryService = cloudinaryService;
        }

        // --- REGULAR USER REGISTRATION ---
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Handle the optional profile picture upload
                string profilePictureUrl = string.Empty;
                if (model.ProfilePicture != null)
                {
                    var uploadResult = await _cloudinaryService.UploadProfileImageAsync(model.ProfilePicture);
                    if (uploadResult.Error == null)
                    {
                        profilePictureUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    ProfilePictureUrl = profilePictureUrl // Save the URL
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    // Store a success message that will be displayed on the next page
                    TempData["SuccessMessage"] = "Registration successful! Please log in to continue.";
                    // Redirect to the Login page instead of the homepage
                    return RedirectToAction("Login");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- LOGIN AND LOGOUT ---
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Login was successful, now let's find the user to check their role.
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Check if the user is in the "Admin" role.
                        if (await _userManager.IsInRoleAsync(user, "Admin"))
                        {
                            // If they are an admin, redirect to the Admin controller's Index page.
                            return RedirectToAction("Index", "Admin");
                        }
                    }

                    // For all other successful logins (Users, ServiceProviders), redirect to the user dashboard.
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
