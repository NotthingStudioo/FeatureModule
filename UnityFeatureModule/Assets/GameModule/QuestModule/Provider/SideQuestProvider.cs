namespace GameModule.QuestModule.Provider
{
    using System.Collections.Generic;
    using Blueprints;
    using GameModule.QuestModule.Model;
    using global::Blueprints;
    using global::QuestModule.Context;
    using global::QuestModule.Provider;

    public class SideQuestProvider : BaseQuestProvider
    {
        private readonly SideQuestBlueprint sideQuestBlueprint;

        public SideQuestProvider(SideQuestBlueprint sideQuestBlueprint, QuestManager questManager, List<IQuestContext> questContexts, QuestContextBlueprint questContextBlueprint) : base(questManager,
            questContexts, questContextBlueprint)
        {
            this.sideQuestBlueprint = sideQuestBlueprint;
        }
        public override QuestProviderType QuestProviderType => QuestProviderType.Side;

        public override QuestRecord GetQuestRecord(string questId, string providerId)
        {
            var questRecord = this.sideQuestBlueprint[questId];

            return questRecord;
        }
    }
}