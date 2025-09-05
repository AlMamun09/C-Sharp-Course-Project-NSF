using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalScout.Data;
using LocalScout.Models;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace LocalScout.Controllers
{
    [Authorize] // All actions in this controller require the user to be logged in
    public class BookingController : Controller
    {
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingController(FirestoreService firestoreService, UserManager<ApplicationUser> userManager)
        {
            _firestoreService = firestoreService;
            _userManager = userManager;
        }

        // GET: /Booking/Create/{serviceId}
        // This action shows the booking form to the user.
        [HttpGet]
        public async Task<IActionResult> Create(string serviceId)
        {
            var service = await _firestoreService.GetProviderServiceByIdAsync(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            var provider = await _userManager.FindByIdAsync(service.ProviderId);
            if (provider == null)
            {
                return NotFound();
            }

            var model = new CreateBookingViewModel
            {
                ServiceId = service.Id,
                ProviderId = provider.Id,
                ServiceName = service.ServiceName,
                ProviderName = provider.BusinessName ?? $"{provider.FirstName} {provider.LastName}",
                Price = service.Price,
                IsNegotiable = service.IsNegotiable
            };

            return View(model);
        }

        // POST: /Booking/Create
        // This new action processes the submitted booking form.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Challenge(); // Should not happen if authorized
                }

                // Convert DateTime to Firestore Timestamp
                var bookingTimestamp = Timestamp.FromDateTime(model.BookingDate.ToUniversalTime());

                var newBooking = new Booking
                {
                    ServiceId = model.ServiceId,
                    ProviderId = model.ProviderId,
                    CustomerId = currentUser.Id,
                    ServiceName = model.ServiceName,
                    TotalPrice = model.Price,
                    IsNegotiable = model.IsNegotiable,
                    BookingDate = bookingTimestamp,
                    CustomerNotes = model.CustomerNotes,
                    Status = "PendingApproval" // Set the initial status
                };

                // Save the new booking to Firestore
                await _firestoreService.CreateBookingAsync(newBooking);

                // Create a notification for the service provider
                var notification = new Notification
                {
                    UserId = model.ProviderId,
                    Message = $"You have a new booking request from {currentUser.FirstName} for '{model.ServiceName}'. Please review it."
                };
                await _firestoreService.CreateNotificationAsync(notification);

                TempData["SuccessMessage"] = "Your booking request has been sent to the provider for approval!";
                return RedirectToAction("Index", "Dashboard");
            }

            // If the model is not valid, return to the form with the validation errors
            return View(model);
        }

        // ADD THESE TWO NEW ACTIONS TO YOUR BookingController

        // POST: /Booking/ApproveBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBooking(string bookingId)
        {
            var booking = await _firestoreService.GetBookingByIdAsync(bookingId);
            var currentUserId = _userManager.GetUserId(User);

            // Security check: ensure the current user is the provider for this booking
            if (booking == null || booking.ProviderId != currentUserId)
            {
                return Forbid();
            }

            // Update the booking status to "Approved"
            var updates = new Dictionary<string, object>
    {
        { "status", "Approved" }
    };
            await _firestoreService.UpdateBookingAsync(bookingId, updates);

            // Notify the customer that their booking was approved
            var customer = await _userManager.FindByIdAsync(booking.CustomerId);
            if (customer != null)
            {
                var notification = new Notification
                {
                    UserId = customer.Id,
                    Message = $"Good news! Your booking for '{booking.ServiceName}' has been approved. You can now proceed with payment."
                };
                await _firestoreService.CreateNotificationAsync(notification);
            }

            TempData["SuccessMessage"] = "Booking has been approved successfully!";
            return RedirectToAction("MyBookings", "ServiceProvider");
        }

        // POST: /Booking/RejectBooking
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBooking(string bookingId, string reason)
        {
            var booking = await _firestoreService.GetBookingByIdAsync(bookingId);
            var currentUserId = _userManager.GetUserId(User);

            // Security check
            if (booking == null || booking.ProviderId != currentUserId)
            {
                return Forbid();
            }

            // Update the booking status and save the rejection reason
            var updates = new Dictionary<string, object>
    {
        { "status", "Rejected" },
        { "rejectionReason", reason }
    };
            await _firestoreService.UpdateBookingAsync(bookingId, updates);

            // Notify the customer that their booking was rejected
            var customer = await _userManager.FindByIdAsync(booking.CustomerId);
            if (customer != null)
            {
                var notification = new Notification
                {
                    UserId = customer.Id,
                    Message = $"Unfortunately, your booking for '{booking.ServiceName}' has been rejected by the provider."
                };
                await _firestoreService.CreateNotificationAsync(notification);
            }

            TempData["SuccessMessage"] = "Booking has been rejected.";
            return RedirectToAction("MyBookings", "ServiceProvider");
        }
    }
}