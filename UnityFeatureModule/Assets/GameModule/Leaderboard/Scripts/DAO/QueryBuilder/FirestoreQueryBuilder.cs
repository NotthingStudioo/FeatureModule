using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;

namespace GameModule.Leaderboard.Scripts.DAO.QueryBuilder
{
    using System.Linq;

    public class FirestoreQueryBuilder<T> : IQueryBuilder<T> where T : class
    {
        private CollectionReference collectionRef;
        private Query               query;
        private DocumentSnapshot     lastDocumentSnapshot; // Field to track the last document

        // Constructor to initialize the collection reference
        public FirestoreQueryBuilder(CollectionReference collectionRef)
        {
            this.collectionRef = collectionRef;
            this.query         = collectionRef; // Start with the base collection
            this.lastDocumentSnapshot = null; // Initialize to null
        }

        // Method to add where conditions
        public IQueryBuilder<T> Where(string field, string op, object value)
        {
            this.query = this.query.WhereEqualTo(field, value); // Example for equality
            return this;
        }

        // Method to add order by conditions
        public IQueryBuilder<T> OrderBy(string field, bool ascending = true)
        {
            this.query = ascending ? this.query.OrderBy(field) : this.query.OrderByDescending(field);
            return this;
        }

        // Method to limit the number of results
        public IQueryBuilder<T> Limit(int limit)
        {
            this.query = this.query.Limit(limit);
            return this;
        }

        // Method to start the query after a specific document (for pagination)
        public IQueryBuilder<T> StartAfter(DocumentSnapshot snapshot)
        {
            this.query = this.query.StartAfter(snapshot);
            this.lastDocumentSnapshot = snapshot; // Keep track of the last snapshot
            return this;
        }

        // Method to execute the query and return results
        public async Task<List<T>> ExecuteAsync()
        {
            var snapshot = await this.query.GetSnapshotAsync();
            List<T> results = new List<T>();

            foreach (var document in snapshot.Documents)
            {
                // Assuming you have a method to convert a Firestore document to your model type T
                var item = document.ConvertTo<T>();
                results.Add(item);
            }

            // Update the lastDocumentSnapshot with the last document from the results
            if (results.Count > 0)
            {
                lastDocumentSnapshot = snapshot.Documents.Last();
            }

            return results;
        }

        // Method to retrieve the last document snapshot (for pagination)
        public DocumentSnapshot GetLastDocumentSnapshot()
        {
            return this.lastDocumentSnapshot; // Return the last document snapshot
        }
    }
}
