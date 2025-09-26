using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LocalScout.Data;
using LocalScout.Services;
using LocalScout.ViewModels;
using LocalScout.Models;
using System.Collections.Generic;

namespace LocalScout.Controllers
{
    [Authorize(Roles = "ServiceProvider, Admin")]
    public class ServiceProviderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly FirestoreService _firestoreService;

        public ServiceProviderController(
            UserManager<ApplicationUser> userManager,
            CloudinaryService cloudinaryService,
            SignInManager<ApplicationUser> signInManager,
            FirestoreService firestoreService)
        {
            _userManager = userManager;
            _cloudinaryService = cloudinaryService;
            _signInManager = signInManager;
            _firestoreService = firestoreService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            string providerId = id;
            bool isOwner = false;

            if (string.IsNullOrEmpty(providerId))
            {
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Challenge(); // Not logged in and no ID provided.
                }
                providerId = currentUserId;
                isOwner = true;
            }
            else
            {
                isOwner = (currentUserId == providerId);
            }

            var providerUser = await _userManager.FindByIdAsync(providerId);
            if (providerUser == null)
            {
                return NotFound("Provider not found.");
            }

            // --- 1. GET DATA (Simpler Version) ---
            // Get the Public Stats (This method is still needed)
            var stats = await _firestoreService.GetBookingStatsForProviderAsync(providerId);

            // --- 2. BUILD THE MASTER VIEWMODEL ---
            var viewModel = new ProviderProfileViewModel
            {
                ProviderProfile = providerUser,
                Stats = stats,
                IsOwner = isOwner
            };

            // --- 3. DETERMINE WHICH LAYOUT TO USE ---
            string layoutToUse = "_Layout"; // Default public layout
            if (User.Identity?.IsAuthenticated == true)
            {
                if (isOwner)
                {
                    layoutToUse = "_ProviderLayout";
                }
                else if (User.IsInRole("Admin"))
                {
                    layoutToUse = "_AdminLayout";
                }
                else
                {
                    layoutToUse = "_DashboardLayout";
                }
            }

            ViewData["Layout"] = layoutToUse;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                BusinessName = user.BusinessName ?? string.Empty,
                BusinessPhoneNumber = user.BusinessPhoneNumber ?? string.Empty,
                BusinessAddress = user.BusinessAddress ?? string.Empty,
                BusinessDescription = user.BusinessDescription ?? string.Empty,
                BusinessHours = user.BusinessHours ?? string.Empty,
                CurrentProfilePictureUrl = user.ProfilePictureUrl
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
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

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // --- Service Management ---
        public async Task<IActionResult> MyServices()
        {
            var providerId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(providerId))
            {
                return View(new List<ProviderService>());
            }
            var services = await _firestoreService.GetServicesByProviderIdAsync(providerId);
            return View(services);
        }

        // This GET action shows the full details of a single service
        [HttpGet]
        public async Task<IActionResult> ServiceDetails(string id)
        {
            // 1. Fetch the service from Firestore
            var service = await _firestoreService.GetProviderServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            // 2. Security Check: Ensure the service belongs to the current user
            var currentUserId = _userManager.GetUserId(User);
            if (service.ProviderId != currentUserId)
            {
                return Forbid(); // Block access if the user is not the owner
            }

            // 3. Pass the service object to the new view
            return View(service);
        }

        // This GET action prepares and shows the empty form
        [HttpGet]
        public async Task<IActionResult> AddNewService()
        {
            var categoriesFromDb = await _firestoreService.GetAllCategoriesAsync();

            // Convert the database categories to a list of SelectListItem
            var categoryList = categoriesFromDb.Select(c => new SelectListItem
            {
                Value = c.Id,
                Text = c.Name
            }).ToList(); // Use .ToList() so we can add to it

            // --- NEW LOGIC ---
            // Add a separator and our special "Request" option to the end of the list
            categoryList.Add(new SelectListItem { Disabled = true, Text = "────────────────────" });
            categoryList.Add(new SelectListItem { Value = "request_new", Text = "Request a new service category..." });

            var model = new AddNewServiceViewModel
            {
                ServiceCategories = categoryList
            };

            return View(model);
        }

        // This POST action processes the form data and saves the new service
        [HttpPost]
        public async Task<IActionResult> AddNewService(AddNewServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                // --- FIX IS HERE ---
                // 1. Get the provider's ID and perform a safety check.
                var providerId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(providerId))
                {
                    // This should not happen if the user is authorized, but it's a good safety check.
                    ModelState.AddModelError(string.Empty, "Unable to identify the current user.");
                    // Re-populate dropdown and return to the form
                    var categories = await _firestoreService.GetAllCategoriesAsync();
                    model.ServiceCategories = categories.Select(c => new SelectListItem { Value = c.Id, Text = c.Name });
                    return View(model);
                }

                // 2. Handle Gallery Image Uploads
                var imageUrls = new List<string>();
                if (model.GalleryImages != null)
                {
                    foreach (var file in model.GalleryImages)
                    {
                        var uploadResult = await _cloudinaryService.UploadGalleryImageAsync(file);
                        if (uploadResult.Error == null)
                        {
                            imageUrls.Add(uploadResult.SecureUrl.ToString());
                        }
                    }
                }

                // 3. Get details of the selected category
                var selectedCategory = await _firestoreService.GetCategoryByIdAsync(model.SelectedServiceCategoryId);
                if (selectedCategory == null)
                {
                    ModelState.AddModelError(string.Empty, "Selected service category not found.");
                    var allCategories = await _firestoreService.GetAllCategoriesAsync();
                    model.ServiceCategories = allCategories.Select(c => new SelectListItem { Value = c.Id, Text = c.Name });
                    return View(model);
                }

                // 4. Create the new ProviderService object
                var newService = new ProviderService
                {
                    ProviderId = providerId, // Now we use the validated ID
                    ServiceCategoryId = model.SelectedServiceCategoryId,
                    ServiceName = selectedCategory.Name,
                    Description = model.Description,
                    Price = model.Price,
                    PricingUnit = model.PricingUnit ?? string.Empty,
                    IsNegotiable = model.IsNegotiable,
                    ImageUrls = imageUrls
                };

                // 5. Save the new service to Firestore
                await _firestoreService.AddNewServiceAsync(newService);

                // --- SUCCESS MESSAGE ---
                TempData["SuccessMessage"] = $"Your new service '{newService.ServiceName}' has been added successfully.";

                // 6. Redirect to the list of services
                return RedirectToAction("MyServices");
            }

            // If the model is not valid, re-populate the dropdown and show the form again
            var finalCategories = await _firestoreService.GetAllCategoriesAsync();
            model.ServiceCategories = finalCategories.Select(c => new SelectListItem { Value = c.Id, Text = c.Name });
            return View(model);
        }

        // This GET action shows the empty request form
        [HttpGet]
        public IActionResult RequestNewCategory()
        {
            return View();
        }

        // This POST action processes the submitted request
        [HttpPost]
        public async Task<IActionResult> RequestNewCategory(RequestNewCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the currently logged-in user to know who made the request
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    // This should not happen if they are authorized, but it's a good safety check
                    return Challenge();
                }

                // Create a new request object with the data from the form and the user
                var request = new ServiceCategoryRequest
                {
                    RequestedById = user.Id,
                    RequestedByName = $"{user.FirstName} {user.LastName}",
                    CategoryName = model.CategoryName,
                    CategoryDescription = model.CategoryDescription,
                    Status = "Pending" // Set the initial status
                };

                // Use our Firestore service to save the new request
                await _firestoreService.AddCategoryRequestAsync(request);

                // Set a success message and redirect the user
                TempData["SuccessMessage"] = "Your category request has been submitted successfully and is pending review.";
                return RedirectToAction("MyServices");
            }

            // If the form data was not valid, show the form again with the errors
            return View(model);
        }

        // This is an API-style action called by JavaScript in the background.
        [HttpPost]
        public async Task<IActionResult> MarkNotificationAsRead(string id)
        {
            // Call our service to update the 'isRead' flag in Firestore.
            await _firestoreService.MarkNotificationAsReadAsync(id);

            // Return a simple "OK" response since this isn't a full page navigation.
            return Ok();
        }


        // --- Edit Services ---
        // This GET action shows the pre-filled edit form for a service
        [HttpGet]
        public async Task<IActionResult> EditService(string id)
        {
            var service = await _firestoreService.GetProviderServiceByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            // Security Check: Ensure the service belongs to the current user
            var currentUserId = _userManager.GetUserId(User);
            if (service.ProviderId != currentUserId)
            {
                return Forbid(); // Or RedirectToAction("AccessDenied", "Account");
            }

            var model = new EditServiceViewModel
            {
                Id = service.Id,
                ServiceName = service.ServiceName,
                Description = service.Description,
                Price = service.Price,
                PricingUnit = service.PricingUnit,
                IsNegotiable = service.IsNegotiable,
                CurrentImageUrls = service.ImageUrls
            };

            return View(model);
        }

        // This POST action processes the submitted changes for a service
        [HttpPost]
        public async Task<IActionResult> EditService(EditServiceViewModel model)
        {
            if (ModelState.IsValid)
            {
                var serviceToUpdate = await _firestoreService.GetProviderServiceByIdAsync(model.Id);
                if (serviceToUpdate == null)
                {
                    return NotFound();
                }

                var currentUserId = _userManager.GetUserId(User);
                if (serviceToUpdate.ProviderId != currentUserId)
                {
                    return Forbid();
                }

                // --- NEW LOGIC to handle image deletion ---
                if (model.ImagesToDelete != null)
                {
                    foreach (var imageUrl in model.ImagesToDelete)
                    {
                        // 1. Delete the image from Cloudinary
                        await _cloudinaryService.DeleteImageAsync(imageUrl);

                        // 2. Remove the URL from our service's list of images
                        serviceToUpdate.ImageUrls.Remove(imageUrl);
                    }
                }

                // Handle new gallery image uploads (this logic is unchanged)
                if (model.NewGalleryImages != null)
                {
                    foreach (var file in model.NewGalleryImages)
                    {
                        var uploadResult = await _cloudinaryService.UploadGalleryImageAsync(file);
                        if (uploadResult.Error == null)
                        {
                            serviceToUpdate.ImageUrls.Add(uploadResult.SecureUrl.ToString());
                        }
                    }
                }

                // Update other properties (this logic is unchanged)
                serviceToUpdate.Description = model.Description;
                serviceToUpdate.Price = model.Price;
                serviceToUpdate.PricingUnit = model.PricingUnit ?? string.Empty;
                serviceToUpdate.IsNegotiable = model.IsNegotiable;

                // Save all changes to Firestore
                await _firestoreService.UpdateProviderServiceAsync(serviceToUpdate);

                TempData["SuccessMessage"] = "Your service has been updated successfully.";
                return RedirectToAction("MyServices");
            }

            // If the model is not valid, we must re-populate the CurrentImageUrls
            // so the page can be displayed correctly.
            var service = await _firestoreService.GetProviderServiceByIdAsync(model.Id);
            if (service != null)
            {
                model.CurrentImageUrls = service.ImageUrls;
            }

            return View(model);
        }

        // This POST action handles the deletion of a service
        [HttpPost]
        public async Task<IActionResult> DeleteService(string id)
        {
            // 1. Fetch the service to be deleted from Firestore
            var serviceToDelete = await _firestoreService.GetProviderServiceByIdAsync(id);
            if (serviceToDelete == null)
            {
                return NotFound();
            }

            // 2. Security Check: Ensure the service belongs to the current user
            var currentUserId = _userManager.GetUserId(User);
            if (serviceToDelete.ProviderId != currentUserId)
            {
                return Forbid(); // Block the request if the user is not the owner
            }

            // 3. Call the service to permanently delete the document
            await _firestoreService.DeleteProviderServiceAsync(id);

            // 4. Set a success message and redirect back to the list
            TempData["SuccessMessage"] = $"Service '{serviceToDelete.ServiceName}' was successfully deleted.";
            return RedirectToAction("MyServices");
        }

        // --- NEW ACTION FOR PROVIDER BOOKING MANAGEMENT ---
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            var providerId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(providerId))
            {
                return Challenge();
            }

            var bookings = await _firestoreService.GetBookingsForProviderAsync(providerId);

            var bookingViewModels = new List<BookingDetailsViewModel>();

            foreach (var booking in bookings)
            {
                var customer = await _userManager.FindByIdAsync(booking.CustomerId);
                var service = await _firestoreService.GetProviderServiceByIdAsync(booking.ServiceId);

                if (customer != null && service != null)
                {
                    bookingViewModels.Add(new BookingDetailsViewModel
                    {
                        Booking = booking,
                        // NOTE: We are reusing the ViewModel. Here, "ProviderBusinessName" will hold the CUSTOMER's name for display purposes.
                        ProviderBusinessName = $"{customer.FirstName} {customer.LastName}",
                        ServiceName = service.ServiceName,
                        ServicePrimaryImageUrl = service.ImageUrls.FirstOrDefault(),
                        CustomerProfilePictureUrl = customer.ProfilePictureUrl
                    });
                }
            }

            return View(bookingViewModels);
        }

        // Add this new action to your ServiceProviderController.cs

        [HttpGet]
        public async Task<IActionResult> MyPurchases()
        {
            var currentUserId = _userManager.GetUserId(User);
            if (currentUserId == null)
            {
                return Challenge();
            }

            // We get bookings where the current user is the CUSTOMER
            var bookings = await _firestoreService.GetBookingsForUserAsync(currentUserId);

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
                        ServiceName = service.ServiceName,
                        ProviderBusinessName = provider.BusinessName ?? $"{provider.FirstName} {provider.LastName}",
                        ServicePrimaryImageUrl = service.ImageUrls.FirstOrDefault()
                    });
                }
            }

            return View(bookingViewModels);
        }

    }
}
