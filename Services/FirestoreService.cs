using Google.Cloud.Firestore;
using NeighborhoodServiceFinder.Models; // Add this using statement
using System.Collections.Generic;      // Add this using statement
using System.Threading.Tasks;        // Add this using statement

namespace NeighborhoodServiceFinder.Services
{
    public class FirestoreService
    {
        private readonly string _projectId;
        private readonly FirestoreDb _db;

        public FirestoreService()
        {
            _projectId = "c-sharp-project-nsf";
            _db = FirestoreDb.Create(_projectId);
        }

        public async Task<List<ServiceCategory>> GetAllCategoriesAsync()
        {
            //1. Get a reference to the collection
            CollectionReference collectionRef = _db.Collection("serviceCategories");

            //2. Build a query that includes the sorting rules
            Query query = collectionRef.OrderByDescending("isActive").OrderBy("priorityOrder").OrderBy("name");

            //3. Get a snapshot from the sorted query
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            // 4. Convert documents to a list of ServiceCategory objects
            List<ServiceCategory> categories = new List<ServiceCategory>();
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                ServiceCategory category = document.ConvertTo<ServiceCategory>();
                categories.Add(category);
            }

            // 5. Return the list
            return categories;
        }

        public async Task AddCategoryAsync(ServiceCategory category)
        {
            // Get a reference to the 'serviceCategories' collection
            CollectionReference collectionRef = _db.Collection("serviceCategories");

            // Add the new category object to the collection.
            // Firestore will automatically convert it and assign a new ID.
            await collectionRef.AddAsync(category);
        }

        public async Task<ServiceCategory?> GetCategoryByIdAsync(string id)
        {
            // Get a reference to the specific document in the collection
            DocumentReference docRef = _db.Collection("serviceCategories").Document(id);

            // Get a snapshot of that single document
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            // Check if the document actually exists
            if (snapshot.Exists)
            {
                // Convert it to our C# object and return it
                return snapshot.ConvertTo<ServiceCategory>();
            }

            // If no document was found with that ID, return null
            return null;
        }

        public async Task UpdateCategoryAsync(ServiceCategory category)
        {
            // Get a reference to the document we want to update
            DocumentReference docRef = _db.Collection("serviceCategories").Document(category.Id);

            // Save the changes to the document in Firestore
            await docRef.SetAsync(category, SetOptions.MergeAll);
        }

        public async Task ToggleCategoryStatusAsync(string id)
        {
            // Get a reference to the document
            DocumentReference docRef = _db.Collection("serviceCategories").Document(id);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                // Find out the current status
                bool currentStatus = snapshot.GetValue<bool>("isActive");

                // Update ONLY the 'isActive' field to the opposite value
                await docRef.UpdateAsync("isActive", !currentStatus);
            }
        }

        public async Task DeleteCategoryAsync(string id)
        {
            // Get a reference to the document we want to delete
            DocumentReference docRef = _db.Collection("serviceCategories").Document(id);

            // Permanently delete the document
            await docRef.DeleteAsync();
        }
    }
}