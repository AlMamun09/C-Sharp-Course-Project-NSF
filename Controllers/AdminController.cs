using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NeighborhoodServiceFinder.Services;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using NeighborhoodServiceFinder.Models;
using Microsoft.AspNetCore.Identity;
using NeighborhoodServiceFinder.Data;
using NeighborhoodServiceFinder.ViewModels;
using System.Linq;

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

        // Updated UserManagement method
        public async Task<IActionResult> UserManagement(string activeTab = "providers")
        {
            var currentAdminId = _userManager.GetUserId(User);
            var allUsers = _userManager.Users.Where(u => u.Id != currentAdminId).ToList();
            var viewModel = new UserManagementViewModel();

            foreach (var user in allUsers)
            {
                if (await _userManager.IsInRoleAsync(user, "ServiceProvider"))
                {
                    viewModel.ServiceProviders.Add(user);
                }
                else
                {
                    viewModel.RegularUsers.Add(user);
                }
            }

            // Pass the active tab name to the view
            ViewBag.ActiveTab = activeTab;

            return View(viewModel);
        }

        // This action shows the details of a specific user
        // Updated UserDetails method
        public async Task<IActionResult> UserDetails(string id, string activeTab)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Pass the active tab name to the view
            ViewBag.ActiveTab = activeTab;

            return View(user);
        }

        // Updated DeleteUser method
        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id, string activeTab)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
            }

            // Redirect back to the UserManagement page, passing the active tab name along
            return RedirectToAction("UserManagement", new { activeTab = activeTab });
        }
    }
}