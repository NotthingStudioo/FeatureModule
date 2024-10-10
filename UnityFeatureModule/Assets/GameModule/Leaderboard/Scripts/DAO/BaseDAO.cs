namespace GameModule.Leaderboard.Scripts.DAO
{
    using System.Collections.Generic;
    using Zenject;

    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameModule.Leaderboard.Scripts.DAO.QueryBuilder;

    public abstract class BaseDAO : ILeaderboardDataReader, IInitializable
    {
        public void Initialize()
        {
            InternalInitialize(); // Ensure the internal initialization is called
        }

        // Abstract method for internal initialization
        protected abstract void InternalInitialize();

        // Read data method
        public abstract UniTask<Dictionary<string, object>> Read(string documentId);
    
        // Create a record in the database
        public abstract UniTask<bool> Create(string documentId, Dictionary<string, object> record);
    
        // Update an existing record
        public abstract UniTask<bool> Update(string documentId, Dictionary<string, object> record);
    
        // Delete a record
        public abstract UniTask<bool>         Delete(string documentId);
        public abstract IQueryBuilder<Dictionary<string,string>> GenerateQueryBuilder();

    }

}