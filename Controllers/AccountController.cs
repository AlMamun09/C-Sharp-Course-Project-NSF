using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalScout.Data;
using LocalScout.ViewModels;
using LocalScout.Services;
using Microsoft.AspNetCore.Authorization;

namespace LocalScout.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CloudinaryService cloudinaryService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
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
                try
                {
                    string profilePictureUrl = string.Empty;
                    if (model.ProfilePicture != null)
                    {
                        var uploadResult = await _cloudinaryService.UploadProfileImageAsync(model.ProfilePicture);
                        if (uploadResult.Error != null)
                        {
                            ModelState.AddModelError(string.Empty, "Error uploading profile picture. Please try again.");
                            return View(model);
                        }
                        profilePictureUrl = uploadResult.SecureUrl.ToString();
                    }

                    var user = new ApplicationUser
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Address = model.Address,
                        ProfilePictureUrl = profilePictureUrl,
                        CreatedAt = DateTimeOffset.UtcNow
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        TempData["SuccessMessage"] = "Registration successful! Please log in to continue.";
                        return RedirectToAction("Login");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An unexpected error occurred during registration.");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                }
            }

            return View(model);
        }

        // --- LOGIN AND LOGOUT ---
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            var model = new LoginViewModel { ReturnUrl = returnUrl };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // Ensure the returnUrl is passed back to the view if login fails
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // --- LOGIN REDIRECT LOGIC ---

                    // 1. (FIX FOR ISSUE #5): Handle the returnUrl FIRST.
                    // If the user was trying to access a protected page, send them back there.
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    // 2. If there is no returnUrl, redirect based on role.
                    var user = await _userManager.FindByEmailAsync(model.Email);

                    if (user == null) // Safety check
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    // Admin Redirect
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Admin");
                    }

                    // (THIS IS OUR NEW STEP 5): Service Provider Redirect
                    if (await _userManager.IsInRoleAsync(user, "ServiceProvider"))
                    {
                        // This now correctly redirects to our new master profile/dashboard action at
                        // ServiceProviderController.Index() (which defaults to the logged-in user)
                        return RedirectToAction("Index", "ServiceProvider");
                    }

                    // (FIX FOR ISSUE #4): Default Regular User Redirect
                    // Send regular users to their profile page, not the empty Dashboard/Index.
                    return RedirectToAction("MyProfile", "Dashboard");
                }

                // If login failed
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            // If model state is invalid
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
