using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Data;       // Needed for ApplicationUser
using NeighborhoodServiceFinder.ViewModels; // Needed for RegisterViewModel
using System.Threading.Tasks;             // Needed for async methods

namespace NeighborhoodServiceFinder.Controllers
{
    public class AccountController : Controller
    {
        // Updated to use our custom ApplicationUser
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // Updated constructor
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // This action shows the empty registration form
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // This action processes the data submitted from the form
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Create a new user object with the data from the form
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address
                };

                // 2. Use the UserManager to try and save the user to the database
                var result = await _userManager.CreateAsync(user, model.Password);

                // 3. If saving was successful...
                if (result.Succeeded)
                {
                    // 4. ...sign the new user in...
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // 5. ...and send them to the homepage.
                    return RedirectToAction("Index", "Home");
                }

                // 6. If saving failed, show the errors on the page
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If the form data was not valid, show the form again
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Use the SignInManager to sign the current user out
            await _signInManager.SignOutAsync();

            // Redirect the user back to the homepage
            return RedirectToAction("Index", "Home");
        }

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
                // Use the SignInManager to check the user's password and sign them in.
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // If the password was correct, redirect to the homepage.
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If the password was incorrect, show an error message.
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If the form data was not valid, show the form again.
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}