using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Services;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using NeighborhoodServiceFinder.Models;
using Microsoft.AspNetCore.Identity;
using NeighborhoodServiceFinder.Data;

namespace NeighborhoodServiceFinder.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(FirestoreService firestoreService, UserManager<ApplicationUser> userManager)
        {
            _firestoreService = firestoreService;
            _userManager = userManager;
        }

        // This is the main admin page that shows the list
        public async Task<IActionResult> Index()
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            return View(categories);
        }

        // This is the GET method that SHOWS the empty form
        public IActionResult Create()
        {
            return View();
        }

        // This is the POST method that PROCESSES the submitted form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceCategory category)
        {
            if (ModelState.IsValid)
            {
                category.CreatedAt = Timestamp.GetCurrentTimestamp();
                category.UpdatedAt = Timestamp.GetCurrentTimestamp();
                await _firestoreService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // This method runs when a user goes to a URL like /Admin/Edit/some-id
        public async Task<IActionResult> Edit(string id)
        {
            // First, we get the specific category from Firestore using the ID from the URL
            ServiceCategory? category = await _firestoreService.GetCategoryByIdAsync(id);

            // If no category with that ID exists, show a "Not Found" page
            if (category == null)
            {
                return NotFound();
            }

            // If we found the category, pass it to the View
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ServiceCategory category)
        {
            // A quick check to make sure the IDs match
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Update the timestamp before saving
                category.UpdatedAt = Timestamp.GetCurrentTimestamp();

                // Call the service to save the changes
                await _firestoreService.UpdateCategoryAsync(category);

                // Go back to the main list page
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> ToggleStatus(string id)
        {
            // Call our service to flip the 'isActive' status for the given ID
            await _firestoreService.ToggleCategoryStatusAsync(id);

            // Redirect the user back to the main list page
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            // Call our service to permanently erase the document
            await _firestoreService.DeleteCategoryAsync(id);

            // Redirect the user back to the main list page
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UserManagement()
        {
            // Use the UserManager to get a list of all users from the database.
            var users = _userManager.Users.ToList();

            // Pass the list of users to the view.
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // 1. Find the user in the database by their unique ID.
            var user = await _userManager.FindByIdAsync(id);

            // 2. If the user was found...
            if (user != null)
            {
                // 3. ...delete them.
                var result = await _userManager.DeleteAsync(user);
            }

            // 4. Redirect the admin back to the user list page.
            return RedirectToAction("UserManagement");
        }
    }
}