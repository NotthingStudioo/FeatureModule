namespace GameModule.Leaderboard.Scripts
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameModule.Leaderboard.Scripts.DAO.QueryBuilder;

    public interface ILeaderboardDataReader
    {
        /// <summary>
        /// Reads a record from the database using the specified document ID.
        /// </summary>
        /// <param name="documentId">The ID of the document to read.</param>
        /// <returns>A task that represents the asynchronous operation, containing the data as a dictionary.</returns>
        UniTask<Dictionary<string, object>> Read(string documentId);

        /// <summary>
        /// Creates a new record in the database with the specified document ID and data.
        /// </summary>
        /// <param name="documentId">The ID of the document to create.</param>
        /// <param name="record">The data to create.</param>
        /// <returns>A task that represents the asynchronous operation, indicating success or failure.</returns>
        UniTask<bool> Create(string documentId, Dictionary<string, object> record);

        /// <summary>
        /// Updates an existing record in the database with the specified document ID and data.
        /// </summary>
        /// <param name="documentId">The ID of the document to update.</param>
        /// <param name="record">The data to update.</param>
        /// <returns>A task that represents the asynchronous operation, indicating success or failure.</returns>
        UniTask<bool> Update(string documentId, Dictionary<string, object> record);

        /// <summary>
        /// Deletes a record from the database using the specified document ID.
        /// </summary>
        /// <param name="documentId">The ID of the document to delete.</param>
        /// <returns>A task that represents the asynchronous operation, indicating success or failure.</returns>
        UniTask<bool> Delete(string documentId);

        IQueryBuilder<Dictionary<string,string>> GenerateQueryBuilder();
    }
}