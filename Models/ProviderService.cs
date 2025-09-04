using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace LocalScout.Models
{
    [FirestoreData]
    public class ProviderService
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("providerId")]
        public string ProviderId { get; set; } = string.Empty; // Links to the ApplicationUser ID

        [FirestoreProperty("serviceCategoryId")]
        public string ServiceCategoryId { get; set; } = string.Empty; // Links to the admin-defined ServiceCategory ID

        [FirestoreProperty("serviceName")]
        public string ServiceName { get; set; } = string.Empty; // The name from the ServiceCategory

        [FirestoreProperty("description")]
        public string Description { get; set; } = string.Empty; // Provider's custom description

        [FirestoreProperty("price")]
        public double Price { get; set; }

        [FirestoreProperty("pricingUnit")]
        public string PricingUnit { get; set; } = string.Empty; // e.g., "per hour", "per day", "per project"

        // --- ADD THIS NEW PROPERTY ---
        [FirestoreProperty("isNegotiable")]
        public bool IsNegotiable { get; set; }

        [FirestoreProperty("imageUrls")]
        public List<string> ImageUrls { get; set; } = new List<string>(); // List of Cloudinary URLs

        [FirestoreProperty("createdAt")]
        [ServerTimestamp]
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        [ServerTimestamp]
        public Timestamp UpdatedAt { get; set; }
    }
}
