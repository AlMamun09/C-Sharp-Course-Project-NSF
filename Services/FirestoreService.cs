using Google.Cloud.Firestore;
using NeighborhoodServiceFinder.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborhoodServiceFinder.Services
{
    public class FirestoreService
    {
        private readonly string _projectId;
        private readonly FirestoreDb _db;
        private const string ServiceCategoriesCollection = "serviceCategories";
        private const string ProviderServicesCollection = "providerServices"; // New collection name
        private const string CategoryRequestsCollection = "serviceCategoryRequests";
        private const string NotificationsCollection = "notifications";

        public FirestoreService()
        {
            _projectId = "c-sharp-project-nsf";
            _db = FirestoreDb.Create(_projectId);
        }

        // --- Service Category Methods (No changes here) ---
        public async Task<List<ServiceCategory>> GetAllCategoriesAsync()
        {
            CollectionReference collectionRef = _db.Collection(ServiceCategoriesCollection);
            Query query = collectionRef.OrderByDescending("isActive").OrderBy("priorityOrder").OrderBy("name");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ServiceCategory> categories = new List<ServiceCategory>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                ServiceCategory category = document.ConvertTo<ServiceCategory>();
                categories.Add(category);
            }
            return categories;
        }

        public async Task<ServiceCategory?> GetCategoryByIdAsync(string id)
        {
            DocumentReference docRef = _db.Collection(ServiceCategoriesCollection).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<ServiceCategory>();
            }
            return null;
        }

        public async Task AddCategoryAsync(ServiceCategory category)
        {
            CollectionReference collectionRef = _db.Collection(ServiceCategoriesCollection);
            await collectionRef.AddAsync(category);
        }

        public async Task UpdateCategoryAsync(ServiceCategory category)
        {
            DocumentReference docRef = _db.Collection(ServiceCategoriesCollection).Document(category.Id);
            await docRef.SetAsync(category, SetOptions.MergeAll);
        }

        public async Task ToggleCategoryStatusAsync(string id)
        {
            DocumentReference docRef = _db.Collection(ServiceCategoriesCollection).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                bool currentStatus = snapshot.GetValue<bool>("isActive");
                await docRef.UpdateAsync("isActive", !currentStatus);
            }
        }

        public async Task DeleteCategoryAsync(string id)
        {
            DocumentReference docRef = _db.Collection(ServiceCategoriesCollection).Document(id);
            await docRef.DeleteAsync();
        }

        // --- NEW Provider Service Methods ---

        // Gets all services created by a specific provider ID
        public async Task<List<ProviderService>> GetServicesByProviderIdAsync(string providerId)
        {
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);
            // Create a query to find all services where the 'providerId' field matches
            Query query = collectionRef.WhereEqualTo("providerId", providerId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ProviderService> services = new List<ProviderService>();
            foreach (var document in snapshot.Documents)
            {
                services.Add(document.ConvertTo<ProviderService>());
            }
            return services;
        }

        // Adds a new service to the database
        public async Task AddNewServiceAsync(ProviderService service)
        {
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);
            await collectionRef.AddAsync(service);
        }

        // Adds a new service category request to the database
        public async Task AddCategoryRequestAsync(ServiceCategoryRequest request)
        {
            CollectionReference collectionRef = _db.Collection(CategoryRequestsCollection);
            await collectionRef.AddAsync(request);
        }

        // Gets all pending service category requests
        public async Task<List<ServiceCategoryRequest>> GetPendingCategoryRequestsAsync()
        {
            CollectionReference collectionRef = _db.Collection(CategoryRequestsCollection);
            // Create a query to find all requests where the 'status' is "Pending"
            Query query = collectionRef.WhereEqualTo("status", "Pending");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ServiceCategoryRequest> requests = new List<ServiceCategoryRequest>();
            foreach (var document in snapshot.Documents)
            {
                requests.Add(document.ConvertTo<ServiceCategoryRequest>());
            }
            return requests;
        }

        // Gets a single service category request by its ID
        public async Task<ServiceCategoryRequest?> GetCategoryRequestByIdAsync(string id)
        {
            DocumentReference docRef = _db.Collection(CategoryRequestsCollection).Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<ServiceCategoryRequest>();
            }
            return null;
        }

        // Updates the status of a specific category request
        public async Task UpdateCategoryRequestStatusAsync(string id, string status)
        {
            DocumentReference docRef = _db.Collection(CategoryRequestsCollection).Document(id);
            // Update only the 'status' field
            await docRef.UpdateAsync("status", status);
        }

        // --- NEW Notification Methods ---

        // Creates a new notification for a specific user
        public async Task CreateNotificationAsync(Notification notification)
        {
            CollectionReference collectionRef = _db.Collection(NotificationsCollection);
            await collectionRef.AddAsync(notification);
        }

        // Gets all unread notifications for a specific user
        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            CollectionReference collectionRef = _db.Collection(NotificationsCollection);
            // Create a query to find all notifications for this user that are not yet read
            Query query = collectionRef.WhereEqualTo("userId", userId).WhereEqualTo("isRead", false);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<Notification> notifications = new List<Notification>();
            foreach (var document in snapshot.Documents)
            {
                notifications.Add(document.ConvertTo<Notification>());
            }
            return notifications;
        }

        // Marks a specific notification as read so it doesn't show again
        public async Task MarkNotificationAsReadAsync(string notificationId)
        {
            DocumentReference docRef = _db.Collection(NotificationsCollection).Document(notificationId);
            await docRef.UpdateAsync("isRead", true);
        }

        // Gets a single provider service by its unique ID
        public async Task<ProviderService?> GetProviderServiceByIdAsync(string serviceId)
        {
            DocumentReference docRef = _db.Collection(ProviderServicesCollection).Document(serviceId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<ProviderService>();
            }
            return null;
        }

        // Updates an existing provider service in the database
        public async Task UpdateProviderServiceAsync(ProviderService service)
        {
            DocumentReference docRef = _db.Collection(ProviderServicesCollection).Document(service.Id);
            // Use SetAsync with MergeAll to update the document without overwriting fields
            await docRef.SetAsync(service, SetOptions.MergeAll);
        }

        // Deletes a specific provider service from the database
        public async Task DeleteProviderServiceAsync(string serviceId)
        {
            DocumentReference docRef = _db.Collection(ProviderServicesCollection).Document(serviceId);
            await docRef.DeleteAsync();
        }

        // Gets a limited number of random services for the homepage
        public async Task<List<ProviderService>> GetRandomServicesAsync(int limit)
        {
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);

            // Firestore doesn't have a built-in "random". A common technique is to
            // order by a random field or, for simplicity here, just take the newest ones.
            // For a true random, a more complex solution would be needed.
            Query query = collectionRef.Limit(limit);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ProviderService> services = new List<ProviderService>();
            foreach (var document in snapshot.Documents)
            {
                services.Add(document.ConvertTo<ProviderService>());
            }
            return services;
        }
    }
}
