namespace QuestModule.Provider
{
    using System.Collections.Generic;
    using System.Linq;
    using Blueprints;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Services;
    using GameModule.QuestModule.Blueprints;
    using GameModule.QuestModule.Model;
    using QuestModule.Context;
    using Zenject;

    public interface IQuestProvider
    {
        QuestProviderType QuestProviderType { get; }
        QuestRecord       GetQuestRecord(string questId, string providerId);
        void              GiveNewQuest(string questId, string providerId, QuestProviderType questProviderType);
        void              CheckToStartQuest(string questId, string providerId);
    }

    public abstract class BaseQuestProvider : IQuestProvider, IInitializable
    {
        protected readonly  QuestManager                      QuestManager;
        private readonly   QuestContextBlueprint             questContextBlueprint;
        public abstract    QuestProviderType                 QuestProviderType { get; }
        private            Dictionary<string, IQuestContext> questContexts;
        [Inject] protected FeatureDataState                  FeatureDataState;

        protected BaseQuestProvider(QuestManager questManager, List<IQuestContext> questContexts, QuestContextBlueprint questContextBlueprint)
        {
            this.QuestManager          = questManager;
            this.questContextBlueprint = questContextBlueprint;
            this.questContexts         = questContexts.ToDictionary(x => x.ContextType);
        }

        public abstract QuestRecord GetQuestRecord(string questId, string providerId);

        public void GiveNewQuest(string questId, string providerId, QuestProviderType questProviderType)
        {
            if (this.QuestProviderType != questProviderType)
                return;

            var questRecord = this.GetQuestRecord(questId, providerId);
            this.QuestManager.CheckToAddNewQuest(questId, providerId, questProviderType, questRecord);
            this.QuestManager.UpdateQuestStatus(providerId, questId, QuestStatus.NotStarted);
        }

        public virtual void CheckToStartQuest(string questId, string providerId)
        {
            var questInfo = this.QuestManager.CheckToAddNewQuest(questId, providerId, this.QuestProviderType, this.GetQuestRecord(questId, providerId));

            if (questInfo.QuestStatus == QuestStatus.NotStarted)
            {
                this.QuestManager.UpdateQuestStatus(providerId, questId, QuestStatus.InProgress);
                this.SetupContext(questInfo.TaskProgress.First());
            }
        }

        /// <summary>
        /// Setup Context at Each Task
        /// </summary>
        /// <param name="taskLog"></param>
        public virtual void SetupContext(TaskLog taskLog)
        {
            if (taskLog.TaskRecord.TaskSates.TryGetValue(taskLog.TaskStatus, out var questContext))
            {
                foreach (var contextId in questContext.QuestContextIds)
                {
                    var contextBp = this.questContextBlueprint[contextId];

                    if (this.questContexts.TryGetValue(contextBp.QuestContextType, out var context))
                    {
                        context.SetupContext(this.questContextBlueprint[contextId]);
                    }
                }
            }
        }

        public async void Initialize()
        {
            await this.InitInternal();
            this.QuestManager.LoadRecord(this);
        }

        protected virtual async UniTask InitInternal() { }
    }
}