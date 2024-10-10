namespace GameModule.Leaderboard.Scripts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Firebase.Firestore;

    public class LeaderboardService
    {
        private readonly ILeaderboardDataReader iLeaderboardDataReader;

        public LeaderboardService(ILeaderboardDataReader iLeaderboardDataReader) { this.iLeaderboardDataReader = iLeaderboardDataReader; }
        
        public void AddRecord(string documentId, Dictionary<string, object> record)
        {
            this.iLeaderboardDataReader.Create(documentId, record);
        }
        
        public void UpdateRecord(string documentId, Dictionary<string, object> record)
        {
            this.iLeaderboardDataReader.Update(documentId, record);
        }
        
        public void DeleteRecord(string documentId)
        {
            this.iLeaderboardDataReader.Delete(documentId);
        }
        
        public async UniTask<Dictionary<string, object>> GetRecords(string documentId)
        {
            return await this.iLeaderboardDataReader.Read(documentId);
        }

        public async UniTask<List<Dictionary<string, string>>> GetRangeFromTop(int top)
        {
            // Create a query builder for the leaderboard collection
            var queryBuilder = this.iLeaderboardDataReader.GenerateQueryBuilder();

            // Build the query to get the specified range
            var results = await queryBuilder
                .OrderBy("Score", ascending: false) // Order by Score in descending order
                .Limit(top) // Limit to the top 'n' records
                .ExecuteAsync();

            // Filter the results to ensure we only get items in the specified range
            var filteredResults = results
                .ToList();

            return filteredResults; // Return the resulting dictionary
        }
    }
}