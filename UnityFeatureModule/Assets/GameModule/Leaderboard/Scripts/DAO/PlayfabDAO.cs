namespace GameModule.Leaderboard.Scripts.DAO
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using GameModule.Leaderboard.Scripts.DAO.QueryBuilder;

    public class PlayfabDAO : BaseDAO
    {
        protected override void InternalInitialize() { throw new System.NotImplementedException(); }

        public override UniTask<Dictionary<string, object>> Read(string documentId) { throw new System.NotImplementedException(); }

        public override UniTask<bool> Create(string documentId, Dictionary<string, object> record) { throw new System.NotImplementedException(); }

        public override UniTask<bool> Update(string documentId, Dictionary<string, object> record) { throw new System.NotImplementedException(); }

        public override UniTask<bool>                            Delete(string documentId) { throw new System.NotImplementedException(); }
        public override IQueryBuilder<Dictionary<string,string>> GenerateQueryBuilder()    { throw new System.NotImplementedException(); }
    }
}