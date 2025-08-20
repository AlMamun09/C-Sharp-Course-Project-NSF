using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.ViewModels;
using System;
using System.Threading.Tasks;
using NeighborhoodServiceFinder.Services;

namespace NeighborhoodServiceFinder.Controllers
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
                        ProfilePictureUrl = profilePictureUrl
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
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    // Check if the user is an Admin.
                    if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // If so, redirect to the Admin panel.
                        return RedirectToAction("Index", "Admin");
                    }

                    // For ALL other users (Regular and ServiceProviders), redirect to the main dashboard.
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
