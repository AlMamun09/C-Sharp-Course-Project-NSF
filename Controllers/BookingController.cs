using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LocalScout.Data;
using LocalScout.Models;
using LocalScout.Services;
using LocalScout.ViewModels;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace LocalScout.Controllers
{
    [Authorize] // All actions in this controller require the user to be logged in
    public class BookingController : Controller
    {
        private readonly FirestoreService _firestoreService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITimeZoneConverterService _timeZoneConverterService;
        private readonly IConfiguration _configuration;

        public BookingController(FirestoreService firestoreService, UserManager<ApplicationUser> userManager, ITimeZoneConverterService timeZoneConverterService, IConfiguration configuration)
        {
            _firestoreService = firestoreService;
            _userManager = userManager;
            _timeZoneConverterService = timeZoneConverterService;
            _configuration = configuration;
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

                // User input (model.BookingDate) is Local (Dhaka) time. We must convert it to UTC for storage.
                DateTime utcBookingTime = _timeZoneConverterService.ConvertFromLocalToUtc(model.BookingDate);

                // Convert the correct UTC DateTime to a Firestore Timestamp
                var bookingTimestamp = Timestamp.FromDateTime(utcBookingTime);

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
                    Message = $"You have a new booking request from {currentUser.FirstName} for '{model.ServiceName}'. Please review it.",
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                };
                await _firestoreService.CreateNotificationAsync(notification);

                TempData["SuccessMessage"] = "Your booking request has been sent to the provider for approval!";
                return RedirectToAction("MyBookings", "Dashboard");
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
                    Message = $"Good news! Your booking for '{booking.ServiceName}' has been approved. You can now proceed with payment.",
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
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
                    Message = $"Unfortunately, your booking for '{booking.ServiceName}' has been rejected by the provider.",
                    CreatedAt = Timestamp.FromDateTime(DateTime.UtcNow)
                };
                await _firestoreService.CreateNotificationAsync(notification);
            }

            TempData["SuccessMessage"] = "Booking has been rejected.";
            return RedirectToAction("MyBookings", "ServiceProvider");
        }

        // ADD THIS NEW ACTION TO YOUR BookingController.cs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveWithPrice(string bookingId, double finalPrice)
        {
            // 1. Security Check: Ensure the booking exists and belongs to the current provider
            var booking = await _firestoreService.GetBookingByIdAsync(bookingId);
            var currentUserId = _userManager.GetUserId(User);

            if (booking == null || booking.ProviderId != currentUserId)
            {
                return Forbid(); // Prevent a provider from approving another's booking
            }

            // 2. Prepare the updates for Firestore
            var updates = new Dictionary<string, object>
    {
        { "status", "Approved" },
        { "finalPrice", finalPrice } // Save the new price set by the provider
    };
            await _firestoreService.UpdateBookingAsync(bookingId, updates);

            // 3. Notify the customer that their booking is approved with the new price
            var customer = await _userManager.FindByIdAsync(booking.CustomerId);
            if (customer != null)
            {
                var notification = new Notification
                {
                    UserId = customer.Id,
                    Message = $"Good news! Your booking for '{booking.ServiceName}' has been approved with a final price of TK {finalPrice:N2}. You can now proceed with payment.",
                    CreatedAt = Google.Cloud.Firestore.Timestamp.FromDateTime(DateTime.UtcNow)
                };
                await _firestoreService.CreateNotificationAsync(notification);
            }

            TempData["SuccessMessage"] = "Booking has been approved with the final price!";
            return RedirectToAction("MyBookings", "ServiceProvider");
        }


        // NEW ACTION TO CREATE SSLCommerz CHECKOUT SESSION
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCheckoutSession(string bookingId)
        {
            // 1. Get Booking & Security Check (Unchanged)
            var booking = await _firestoreService.GetBookingByIdAsync(bookingId);
            var currentUser = await _userManager.GetUserAsync(User);

            if (booking == null || currentUser == null || booking.CustomerId != currentUser.Id)
            {
                return Forbid();
            }

            // ... Get SSLCommerz Credentials (Unchanged) ...
            var storeId = _configuration["SslCommerzSettings:StoreId"];
            var storePassword = _configuration["SslCommerzSettings:StorePassword"];
            var isSandbox = _configuration.GetValue<bool>("SslCommerzSettings:IsSandbox");
            var apiUrl = isSandbox
                ? "https://sandbox.sslcommerz.com/gwprocess/v4/api.php"
                : "https://securepay.sslcommerz.com/gwprocess/v4/api.php";


            // 3. Prepare Transaction Data
            var transactionId = $"booking_{bookingId}_{Guid.NewGuid().ToString().Substring(0, 8)}";
            var baseUrl = "https://mn17rwfn-5282.inc1.devtunnels.ms/"; // Your Forwarded URL

            // --- THIS IS THE KEY LOGIC CHANGE ---
            // Use the FinalPrice if it exists, otherwise fall back to the original TotalPrice.
            double amountToPay = booking.FinalPrice ?? booking.TotalPrice;

            var postData = new List<KeyValuePair<string, string>>
    {
        new KeyValuePair<string, string>("store_id", storeId ?? ""),
        new KeyValuePair<string, string>("store_passwd", storePassword ?? ""),
        new KeyValuePair<string, string>("total_amount", amountToPay.ToString("0.00")), // <-- USE THE CORRECT AMOUNT
        new KeyValuePair<string, string>("currency", "BDT"),
        new KeyValuePair<string, string>("tran_id", transactionId),
        new KeyValuePair<string, string>("success_url", $"{baseUrl}Booking/PaymentSuccess"),
        new KeyValuePair<string, string>("fail_url", $"{baseUrl}Booking/PaymentCanceled"),
        new KeyValuePair<string, string>("cancel_url", $"{baseUrl}Booking/PaymentCanceled"),
        new KeyValuePair<string, string>("shipping_method", "NO"),
        new KeyValuePair<string, string>("product_name", string.IsNullOrEmpty(booking.ServiceName) ? "Service Booking" : booking.ServiceName),
        new KeyValuePair<string, string>("product_category", "Service"),
        new KeyValuePair<string, string>("product_profile", "general"),
        new KeyValuePair<string, string>("cus_name", $"{currentUser.FirstName} {currentUser.LastName}"),
        new KeyValuePair<string, string>("cus_email", currentUser.Email ?? "N/A"),
        new KeyValuePair<string, string>("cus_add1", currentUser.Address ?? "N/A"),
        new KeyValuePair<string, string>("cus_city", "Dhaka"),
        new KeyValuePair<string, string>("cus_country", "Bangladesh"),
        new KeyValuePair<string, string>("cus_phone", currentUser.PhoneNumber ?? "N/A")
    };

            // 4. Send Request to SSLCommerz (Unchanged)
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(apiUrl, new FormUrlEncodedContent(postData));
                var responseString = await response.Content.ReadAsStringAsync();
                var apiResponse = System.Text.Json.JsonSerializer.Deserialize<SslCommerzApiResponse>(responseString);

                if (apiResponse?.status == "SUCCESS" && !string.IsNullOrEmpty(apiResponse.GatewayPageURL))
                {
                    // 5. Redirect user to the multi-option "EasyCheckout" page
                    return Redirect(apiResponse.GatewayPageURL);
                }
            }

            TempData["ErrorMessage"] = "Could not initiate payment. Please try again.";
            return RedirectToAction("MyBookings", "Dashboard");
        }


        // Helper class for deserializing the JSON response from SSLCommerz
        public class SslCommerzApiResponse
        {
            public string? status { get; set; }
            public string? failedreason { get; set; }
            public string? sessionkey { get; set; }
            public string? redirectGatewayURL { get; set; }
            public string? GatewayPageURL { get; set; }
        }

        // ADD THESE TWO METHODS TO YOUR BookingController.cs
        [AllowAnonymous]
        public IActionResult PaymentSuccess()
        {
            // For now, we will just show a success message.
            // In a future step, we will handle the IPN from SSLCommerz to securely
            // verify the payment and update the booking status in Firestore.

            TempData["SuccessMessage"] = "Thank you for your payment! Your booking is being confirmed.";
            return RedirectToAction("MyBookings", "Dashboard");
        }

        [AllowAnonymous]
        public IActionResult PaymentCanceled()
        {
            TempData["ErrorMessage"] = "Your payment was canceled or failed. No charges were made.";
            return RedirectToAction("MyBookings", "Dashboard");
        }

        // ADD THIS NEW ACTION TO YOUR BookingController.cs

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentIpn([FromForm] IFormCollection form)
        {
            Console.WriteLine("--- IPN ENDPOINT HIT ---");

            // Log all the data we received from SSLCommerz
            foreach (var item in form)
            {
                Console.WriteLine($"IPN DATA: {item.Key} = {item.Value}");
            }

            string? transactionId = form["tran_id"];
            if (string.IsNullOrEmpty(transactionId))
            {
                Console.WriteLine("IPN ERROR: Transaction ID is missing.");
                return BadRequest("Transaction ID is missing.");
            }

            string bookingId = transactionId.Split('_')[1];
            Console.WriteLine($"IPN INFO: Parsed Booking ID: {bookingId}");

            // --- CRITICAL: VALIDATE THE TRANSACTION ---
            var storePassword = _configuration["SslCommerzSettings:StorePassword"];
            string? validationId = form["val_id"];

            if (string.IsNullOrEmpty(validationId))
            {
                Console.WriteLine("IPN ERROR: Validation ID is missing.");
                return BadRequest("Validation ID is missing.");
            }

            var isSandbox = _configuration.GetValue<bool>("SslCommerzSettings:IsSandbox");
            var storeId = _configuration["SslCommerzSettings:StoreId"];

            var validationUrl = isSandbox
                ? $"https://sandbox.sslcommerz.com/validator/api/validationserverAPI.php?val_id={validationId}&store_id={storeId}&store_passwd={storePassword}"
                : $"https://securepay.sslcommerz.com/validator/api/validationserverAPI.php?val_id={validationId}&store_id=[YourLiveStoreID]&store_passwd=[YourLivePassword]";

            Console.WriteLine($"IPN INFO: Calling Validation URL: {validationUrl}");

            using (var httpClient = new HttpClient())
            {
                var validationResponse = await httpClient.GetAsync(validationUrl);
                var validationJson = await validationResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"IPN INFO: Validation Response JSON: {validationJson}");

                if (validationResponse.IsSuccessStatusCode && validationJson.Contains("\"status\":\"VALID\""))
                {
                    Console.WriteLine("IPN SUCCESS: Transaction is valid.");
                    // --- PAYMENT IS VALID AND VERIFIED ---
                    string? paymentStatus = form["status"];
                    Console.WriteLine($"IPN INFO: Payment Status from form is: {paymentStatus}");

                    if (paymentStatus == "VALID")
                    {
                        Console.WriteLine("IPN ACTION: Updating booking status to Confirmed.");
                        var updates = new Dictionary<string, object>
                {
                    { "status", "Confirmed" }
                };
                        await _firestoreService.UpdateBookingAsync(bookingId, updates);
                        Console.WriteLine("IPN SUCCESS: Firestore database has been updated.");
                    }
                }
                else
                {
                    Console.WriteLine("IPN ERROR: Transaction validation failed or status was not VALID.");
                }
            }

            return Ok();
        }

    }
}