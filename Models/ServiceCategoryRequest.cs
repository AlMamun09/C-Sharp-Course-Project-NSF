using Google.Cloud.Firestore;

namespace LocalScout.Models
{
    [FirestoreData]
    public class ServiceCategoryRequest
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("requestedById")]
        public string RequestedById { get; set; } = string.Empty;

        [FirestoreProperty("requestedByName")]
        public string RequestedByName { get; set; } = string.Empty;

        [FirestoreProperty("categoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [FirestoreProperty("categoryDescription")]
        public string CategoryDescription { get; set; } = string.Empty;

        [FirestoreProperty("status")]
        public string Status { get; set; } = "Pending";

        [FirestoreProperty("createdAt")]
        [ServerTimestamp]
        public Timestamp CreatedAt { get; set; }
    }
}