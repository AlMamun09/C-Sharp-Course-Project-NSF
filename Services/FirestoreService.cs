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
    }
}
