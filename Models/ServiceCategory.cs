using Google.Cloud.Firestore; // We will add this library to our project next

namespace NeighborhoodServiceFinder.Models
{
    [FirestoreData] // Marks this class as a model for a Firestore collection
    public class ServiceCategory
    {
        [FirestoreDocumentId] // Tells Firestore this is the unique document ID
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("name")] // Maps this property to the "name" field in the database
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty("description")]
        public string Description { get; set; } = string.Empty;

        [FirestoreProperty("iconUrl")]
        public string IconUrl { get; set; } = string.Empty;

        [FirestoreProperty("createdAt")]
        [ServerTimestamp] // This will be set automatically by the Firestore server on creation
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        [ServerTimestamp] // This will be set automatically on every update
        public Timestamp UpdatedAt { get; set; }

        [FirestoreProperty("isActive")]
        public bool IsActive { get; set; }

        [FirestoreProperty("priorityOrder")]
        public int PriorityOrder { get; set; }
    }
}