namespace GameModule.Leaderboard.Scripts.DAO.QueryBuilder
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Firebase.Firestore;

    public interface IQueryBuilder<T>
    {
        // Method to add a where condition
        IQueryBuilder<T> Where(string field, string operatorType, object value);

        // Method to add an order by condition
        IQueryBuilder<T> OrderBy(string field, bool ascending = true);

        // Method to limit the number of results
        IQueryBuilder<T> Limit(int limit);

        // Method to start the query after a specific document (for pagination)
        IQueryBuilder<T> StartAfter(DocumentSnapshot snapshot);

        // Method to execute the query and return results
        Task<List<T>> ExecuteAsync();

        // Optional: Method to retrieve the last document snapshot (for pagination)
        DocumentSnapshot GetLastDocumentSnapshot();
    }
}