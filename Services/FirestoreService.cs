using Google.Cloud.Firestore;
using LocalScout.Models;
using LocalScout.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocalScout.Services
{
    public class FirestoreService
    {
        private readonly string _projectId;
        private readonly FirestoreDb _db;
        private const string ServiceCategoriesCollection = "serviceCategories";
        private const string ProviderServicesCollection = "providerServices"; // New collection name
        private const string CategoryRequestsCollection = "serviceCategoryRequests";
        private const string NotificationsCollection = "notifications";
        private const string BookingsCollection = "bookings";

        public FirestoreService()
        {
            _projectId = "c-sharp-project-nsf";
            _db = FirestoreDb.Create(_projectId);
        }

        // --- Service Category Methods (No changes here) ---
        public async Task<List<ServiceCategory>> GetAllCategoriesAsync()
        {
            CollectionReference collectionRef = _db.Collection(ServiceCategoriesCollection);
            Query query = collectionRef.WhereEqualTo("isActive", true).OrderBy("priorityOrder").OrderBy("name");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ServiceCategory> categories = new List<ServiceCategory>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                ServiceCategory category = document.ConvertTo<ServiceCategory>();
                categories.Add(category);
            }
            return categories;
        }

        // Gets ALL service categories for the admin panel, sorting them by status
        public async Task<List<ServiceCategory>> GetAllCategoriesForAdminAsync()
        {
            CollectionReference collectionRef = _db.Collection(ServiceCategoriesCollection);
            // This query gets ALL categories but sorts them so inactive ones are last.
            Query query = collectionRef.OrderByDescending("isActive").OrderBy("priorityOrder").OrderBy("name");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ServiceCategory> categories = new List<ServiceCategory>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                categories.Add(document.ConvertTo<ServiceCategory>());
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

        // Searches for services based on a query string
        public async Task<List<ProviderService>> SearchServicesAsync(string query)
        {
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);
            QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

            List<ProviderService> allServices = new List<ProviderService>();
            foreach (var document in snapshot.Documents)
            {
                allServices.Add(document.ConvertTo<ProviderService>());
            }

            // --- Simple Text Search Logic ---
            // Note: For a large-scale application, a dedicated search service like Algolia or Elasticsearch would be more efficient.
            // For our project, filtering in C# is a perfect and simple solution.
            if (string.IsNullOrWhiteSpace(query))
            {
                return allServices; // If the search is empty, return everything
            }

            string lowerCaseQuery = query.ToLower();

            var filteredServices = allServices
                .Where(s => s.ServiceName.ToLower().Contains(lowerCaseQuery) ||
                            s.Description.ToLower().Contains(lowerCaseQuery))
                .ToList();

            return filteredServices;
        }

        // Gets all services belonging to a specific category ID
        public async Task<List<ProviderService>> GetServicesByCategoryIdAsync(string categoryId)
        {
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);
            // Create a query to find all services where the 'serviceCategoryId' field matches
            Query query = collectionRef.WhereEqualTo("serviceCategoryId", categoryId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            List<ProviderService> services = new List<ProviderService>();
            foreach (var document in snapshot.Documents)
            {
                services.Add(document.ConvertTo<ProviderService>());
            }
            return services;
        }

        // --- PAGINATION METHODS ---

        // Method for category and latest services (uses cursor pagination)
        public async Task<PaginatedServiceResult> GetServicesPaginatedAsync(string? categoryId, int pageSize, string? lastDocumentId)
        {
            Query query = _db.Collection(ProviderServicesCollection);

            // Apply category filter if provided
            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.WhereEqualTo("serviceCategoryId", categoryId);
            }

            // Always order by creation date to have a consistent order for pagination
            query = query.OrderByDescending("createdAt");

            // If lastDocumentId is provided, start the new query after that document
            if (!string.IsNullOrEmpty(lastDocumentId))
            {
                DocumentSnapshot lastDocument = await _db.Collection(ProviderServicesCollection).Document(lastDocumentId).GetSnapshotAsync();
                if (lastDocument.Exists)
                {
                    query = query.StartAfter(lastDocument);
                }
            }

            // We fetch one more than the page size to check if there are more pages
            query = query.Limit(pageSize + 1);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            var services = new List<ProviderService>();
            foreach (var document in snapshot.Documents)
            {
                services.Add(document.ConvertTo<ProviderService>());
            }

            string? newLastDocumentId = null;
            bool hasMore = services.Count > pageSize;

            // If we have more services than the page size, remove the extra one
            if (hasMore)
            {
                services.RemoveAt(pageSize);
                newLastDocumentId = services.LastOrDefault()?.Id;
            }

            return new PaginatedServiceResult
            {
                Services = services,
                LastDocumentId = newLastDocumentId,
                HasMorePages = hasMore
            };
        }


        // Method for search (uses in-memory pagination)
        public async Task<PaginatedServiceResult> SearchServicesPaginatedAsync(string? query, int pageSize, int pageNumber)
        {
            // NOTE: This search fetches all documents first, then filters in C#.
            // For a large-scale app, a dedicated search service like Algolia is better.
            // For this project, this is a perfectly functional approach.
            CollectionReference collectionRef = _db.Collection(ProviderServicesCollection);
            QuerySnapshot snapshot = await collectionRef.GetSnapshotAsync();

            List<ProviderService> allServices = snapshot.Documents.Select(doc => doc.ConvertTo<ProviderService>()).ToList();

            List<ProviderService> filteredServices;
            if (string.IsNullOrWhiteSpace(query))
            {
                filteredServices = allServices;
            }
            else
            {
                string lowerCaseQuery = query.ToLower();
                filteredServices = allServices
                    .Where(s => s.ServiceName.ToLower().Contains(lowerCaseQuery) ||
                                s.Description.ToLower().Contains(lowerCaseQuery))
                    .ToList();
            }

            // Apply pagination to the filtered list
            var paginatedServices = filteredServices.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            bool hasMore = filteredServices.Count > pageNumber * pageSize;

            return new PaginatedServiceResult
            {
                Services = paginatedServices,
                HasMorePages = hasMore,
                LastDocumentId = null // Not used for this pagination type
            };
        }

        // --- BOOKING MANAGEMENT METHODS ---

        // Creates a new booking document in Firestore
        public async Task<string> CreateBookingAsync(Booking booking)
        {
            DocumentReference docRef = await _db.Collection(BookingsCollection).AddAsync(booking);
            return docRef.Id;
        }

        // Gets a single booking by its ID
        public async Task<Booking?> GetBookingByIdAsync(string bookingId)
        {
            DocumentReference docRef = _db.Collection(BookingsCollection).Document(bookingId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<Booking>();
            }
            return null;
        }

        // Gets all bookings for a specific customer
        public async Task<List<Booking>> GetBookingsForUserAsync(string customerId)
        {
            Query query = _db.Collection(BookingsCollection)
                .WhereEqualTo("customerId", customerId)
                .OrderByDescending("createdAt");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Booking>()).ToList();
        }

        // Gets all bookings for a specific provider
        public async Task<List<Booking>> GetBookingsForProviderAsync(string providerId)
        {
            Query query = _db.Collection(BookingsCollection)
                .WhereEqualTo("providerId", providerId)
                .OrderByDescending("createdAt");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Booking>()).ToList();
        }

        // A flexible method to update a booking's status and optionally add a reason
        public async Task UpdateBookingAsync(string bookingId, Dictionary<string, object> updates)
        {
            DocumentReference docRef = _db.Collection(BookingsCollection).Document(bookingId);
            await docRef.UpdateAsync(updates);
        }


        // Calculates booking statistics for a specific user
        public async Task<BookingStatsViewModel> GetBookingStatsForUserAsync(string customerId)
        {
            Query query = _db.Collection(BookingsCollection).WhereEqualTo("customerId", customerId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            var stats = new BookingStatsViewModel();
            stats.TotalRequested = snapshot.Count;

            foreach (var document in snapshot.Documents)
            {
                var booking = document.ConvertTo<Booking>();
                switch (booking.Status)
                {
                    case "Confirmed":
                        stats.TotalConfirmed++;
                        break;
                    case "Completed":
                        stats.TotalConfirmed++; // A completed booking was also confirmed
                        stats.TotalCompleted++;
                        break;
                    case "CanceledByUser":
                        stats.TotalCanceled++;
                        break;
                }
            }
            return stats;
        }

        public async Task<BookingStatsViewModel> GetBookingStatsForProviderAsync(string providerId)
        {
            var stats = new BookingStatsViewModel();
            var query = _db.Collection(BookingsCollection).WhereEqualTo("providerId", providerId);
            var snapshot = await query.GetSnapshotAsync();

            stats.TotalRequested = snapshot.Count; // Total bookings this provider has ever received

            foreach (var doc in snapshot.Documents)
            {
                // Get the "status" field from the document
                if (doc.TryGetValue("status", out string status))
                {
                    if (status == "Confirmed" || status == "Approved")
                    {
                        stats.TotalConfirmed++; // Counts jobs that are "ongoing" (approved or paid)
                    }
                    else if (status == "Completed")
                    {
                        stats.TotalCompleted++;
                    }
                    else if (status == "CanceledByUser" || status == "Rejected")
                    {
                        stats.TotalCanceled++;
                    }
                    // "PendingApproval" is counted in TotalRequested, but we don't show it publicly.
                    // We'll fetch that count separately for the provider's private view. 
                }
            }
            return stats;
        }

    }
}
