using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalScout.Data;
using LocalScout.Models;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace LocalScout.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService; // Add this

        // Updated constructor
        public AdminController(FirestoreService firestoreService, UserManager<ApplicationUser> userManager, CloudinaryService cloudinaryService)
        {
            _firestoreService = firestoreService;
            _userManager = userManager;
            _cloudinaryService = cloudinaryService; // Add this
        }

        // --- Service Category Management ---
        public async Task<IActionResult> Index()
        {
            var categories = await _firestoreService.GetAllCategoriesAsync();
            return View(categories);
        }

        // Updated GET action to show the new form
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryViewModel());
        }

        // Updated POST action to handle image upload
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = string.Empty;
                if (model.Image != null)
                {
                    // Use the gallery image uploader for a 'fit' transformation
                    var uploadResult = await _cloudinaryService.UploadGalleryImageAsync(model.Image);
                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error uploading image.");
                        return View(model);
                    }
                    imageUrl = uploadResult.SecureUrl.ToString();
                }

                var newCategory = new ServiceCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    PriorityOrder = model.PriorityOrder,
                    ImageUrl = imageUrl,
                    IsActive = model.IsActive
                };

                await _firestoreService.AddCategoryAsync(newCategory);
                TempData["SuccessMessage"] = "New category created successfully.";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // ADD this new [HttpGet] Edit method
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var category = await _firestoreService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var model = new EditCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                PriorityOrder = category.PriorityOrder,
                IsActive = category.IsActive,
                CurrentImageUrl = category.ImageUrl
            };

            return View(model);
        }


        // ADD this new [HttpPost] Edit method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryToUpdate = await _firestoreService.GetCategoryByIdAsync(model.Id);
                if (categoryToUpdate == null)
                {
                    return NotFound();
                }

                // Handle optional new image upload
                if (model.NewImage != null)
                {
                    var uploadResult = await _cloudinaryService.UploadGalleryImageAsync(model.NewImage);
                    if (uploadResult.Error != null)
                    {
                        ModelState.AddModelError(string.Empty, "Error uploading new image.");
                        return View(model);
                    }
                    categoryToUpdate.ImageUrl = uploadResult.SecureUrl.ToString();
                }

                // Update properties
                categoryToUpdate.Name = model.Name;
                categoryToUpdate.Description = model.Description;
                categoryToUpdate.PriorityOrder = model.PriorityOrder;
                categoryToUpdate.IsActive = model.IsActive;
                categoryToUpdate.UpdatedAt = Timestamp.GetCurrentTimestamp();

                // Save changes to Firestore
                await _firestoreService.UpdateCategoryAsync(categoryToUpdate);

                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            // If the model is not valid, re-populate the CurrentImageUrl before showing the form again
            var originalCategory = await _firestoreService.GetCategoryByIdAsync(model.Id);
            if (originalCategory != null)
            {
                model.CurrentImageUrl = originalCategory.ImageUrl;
            }
            return View(model);
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

        // This GET action shows the list of pending category requests
        public async Task<IActionResult> CategoryRequests()
        {
            // Fetch all pending requests from Firestore
            var requests = await _firestoreService.GetPendingCategoryRequestsAsync();

            // Pass the list of requests to the new view
            return View(requests);
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

        // This POST action handles the "Approve" button click
        [HttpPost]
        public async Task<IActionResult> ApproveCategoryRequest(string requestId)
        {
            var request = await _firestoreService.GetCategoryRequestByIdAsync(requestId);

            if (request != null && request.Status == "Pending")
            {
                var newCategory = new ServiceCategory
                {
                    Name = request.CategoryName,
                    Description = request.CategoryDescription,
                    IsActive = true,
                    PriorityOrder = 99
                };
                await _firestoreService.AddCategoryAsync(newCategory);
                await _firestoreService.UpdateCategoryRequestStatusAsync(requestId, "Approved");

                // --- NEW NOTIFICATION LOGIC ---
                var notification = new Notification
                {
                    UserId = request.RequestedById,
                    Message = $"Congratulations! Your request for the new category '{request.CategoryName}' has been approved."
                };
                await _firestoreService.CreateNotificationAsync(notification);

                TempData["SuccessMessage"] = $"Category '{request.CategoryName}' has been approved and is now available.";
            }
            return RedirectToAction("CategoryRequests");
        }

        // This POST action handles the "Deny" button click
        [HttpPost]
        public async Task<IActionResult> DenyCategoryRequest(string requestId)
        {
            var request = await _firestoreService.GetCategoryRequestByIdAsync(requestId);
            if (request != null && request.Status == "Pending")
            {
                await _firestoreService.UpdateCategoryRequestStatusAsync(requestId, "Denied");

                // --- NEW NOTIFICATION LOGIC ---
                var notification = new Notification
                {
                    UserId = request.RequestedById,
                    Message = $"We're sorry, but your request for the new category '{request.CategoryName}' has been denied at this time."
                };
                await _firestoreService.CreateNotificationAsync(notification);

                TempData["SuccessMessage"] = $"Category request '{request.CategoryName}' has been denied.";
            }
            return RedirectToAction("CategoryRequests");
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