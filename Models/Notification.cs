using Google.Cloud.Firestore;

namespace LocalScout.Models
{
    [FirestoreData]
    public class Notification
    {
        [FirestoreDocumentId]
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("userId")]
        public string UserId { get; set; } = string.Empty; // The ID of the user who should see this

        [FirestoreProperty("message")]
        public string Message { get; set; } = string.Empty; // The notification text

        [FirestoreProperty("isRead")]
        public bool IsRead { get; set; } = false; // To track if the user has seen it

        [FirestoreProperty("createdAt")]
        [ServerTimestamp]
        public Timestamp CreatedAt { get; set; }
    }
}