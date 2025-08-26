using Google.Cloud.Firestore;
using System.Collections.Generic;

namespace LocalScout.Models
{
    [FirestoreData]
    public class ServiceCategory
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("name")]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty("description")]
        public string Description { get; set; } = string.Empty;

        // --- ADD THIS NEW PROPERTY ---
        [FirestoreProperty("imageUrl")]
        public string ImageUrl { get; set; } = string.Empty;

        [FirestoreProperty("createdAt")]
        [ServerTimestamp]
        public Timestamp CreatedAt { get; set; }

        [FirestoreProperty("updatedAt")]
        [ServerTimestamp]
        public Timestamp UpdatedAt { get; set; }

        [FirestoreProperty("isActive")]
        public bool IsActive { get; set; }

        [FirestoreProperty("priorityOrder")]
        public int PriorityOrder { get; set; }
    }
}