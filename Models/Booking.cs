using Google.Cloud.Firestore;

namespace LocalScout.Models
{
    [FirestoreData]
    public class Booking
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("serviceId")]
        public string ServiceId { get; set; } = string.Empty;

        [FirestoreProperty("serviceName")]
        public string ServiceName { get; set; } = string.Empty;

        [FirestoreProperty("customerId")]
        public string CustomerId { get; set; } = string.Empty;

        [FirestoreProperty("providerId")]
        public string ProviderId { get; set; } = string.Empty;

        [FirestoreProperty("bookingDate")]
        public Timestamp BookingDate { get; set; }

        [FirestoreProperty("totalPrice")]
        public double TotalPrice { get; set; }

        [FirestoreProperty("isNegotiable")] // <-- ADD THIS
        public bool IsNegotiable { get; set; }

        [FirestoreProperty("status")]
        public string Status { get; set; } = string.Empty;
        // Possible Statuses:
        // "PendingApproval"  - User has requested, waiting for provider.
        // "Approved"         - Provider has approved, waiting for user payment.
        // "Confirmed"        - User has paid, booking is confirmed.
        // "Completed"        - Service has been rendered and marked as complete.
        // "Rejected"         - Provider has rejected the request.
        // "CanceledByUser"   - Canceled by the customer.

        [FirestoreProperty("customerNotes")]
        public string? CustomerNotes { get; set; }

        [FirestoreProperty("cancellationReason")]
        public string? CancellationReason { get; set; }

        [FirestoreProperty("rejectionReason")]
        public string? RejectionReason { get; set; }

        [FirestoreProperty("stripePaymentIntentId")]
        public string? StripePaymentIntentId { get; set; }

        [FirestoreProperty("createdAt")]
        [ServerTimestamp]
        public Timestamp CreatedAt { get; set; }
    }
}