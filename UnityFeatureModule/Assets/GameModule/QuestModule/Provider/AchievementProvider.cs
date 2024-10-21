namespace GameModule.QuestModule.Provider
{
    using System.Collections.Generic;
    using Blueprints;
    using Cysharp.Threading.Tasks;
    using GameModule.QuestModule.Model;
    using global::Blueprints;
    using global::QuestModule.Context;
    using global::QuestModule.Provider;

    public class AchievementProvider : BaseQuestProvider
    {
        private readonly AchievementBlueprint achievementBlueprint;

        public AchievementProvider(QuestManager questManager, AchievementBlueprint achievementBlueprint, List<IQuestContext> questContexts, QuestContextBlueprint questContextBlueprint) :
            base(questManager, questContexts, questContextBlueprint)
        {
            this.achievementBlueprint = achievementBlueprint;
        }

        public override QuestProviderType QuestProviderType => QuestProviderType.Achievement;

        public override QuestRecord GetQuestRecord(string questId, string providerId) { return this.achievementBlueprint[questId]; }

        protected override async UniTask InitInternal()
        {
            await UniTask.WaitUntil(() => this.FeatureDataState.IsBlueprintAndLocalDataLoaded);

            foreach (var item in this.achievementBlueprint)
            {
                this.GiveNewQuest(item.Key, QuestProviderType.Achievement.ToString(), this.QuestProviderType);
                this.CheckToStartQuest(item.Key, QuestProviderType.Achievement.ToString());
            }
        }

        public override void SetupContext(TaskLog taskLog) { }
    }
}