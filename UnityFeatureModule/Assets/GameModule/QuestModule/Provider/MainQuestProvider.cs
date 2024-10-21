namespace QuestModule.Provider
{
    using System.Collections.Generic;
    using Blueprints;
    using GameModule.QuestModule.Blueprints;
    using GameModule.QuestModule.Model;
    using QuestModule.Context;

    public class MainQuestProvider : BaseQuestProvider
    {
        private readonly MainQuestBlueprint mainQuestBlueprint;
        public MainQuestProvider(QuestManager questManager, MainQuestBlueprint mainQuestBlueprint,List<IQuestContext> questContexts,QuestContextBlueprint questContextBlueprint) : base(questManager,questContexts,questContextBlueprint) { this.mainQuestBlueprint = mainQuestBlueprint; }

        public override QuestProviderType QuestProviderType => QuestProviderType.Main;

        public override QuestRecord GetQuestRecord(string questId, string providerId) { return this.mainQuestBlueprint[questId]; }
    }
}