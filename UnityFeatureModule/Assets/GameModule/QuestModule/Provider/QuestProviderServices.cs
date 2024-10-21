namespace QuestModule.Provider
{
    using System.Collections.Generic;
    using System.Linq;
    using GameModule.QuestModule.Model;

    public class QuestProviderServices
    {
        private Dictionary<QuestProviderType, IQuestProvider> questProviders;
        public QuestProviderServices(List<IQuestProvider> questProviders) { this.questProviders = questProviders.ToDictionary(x => x.QuestProviderType); }
        
        public void GiveQuestToUser(string questId, string providerId, QuestProviderType questProviderType)
        {
            if (this.questProviders.TryGetValue(questProviderType, out var questProvider))
            {
                questProvider.GiveNewQuest(questId, providerId, questProviderType);
            }
        }

        public void StartQuest(QuestProviderType questProviderType, string questId, string providerId)
        {
            this.questProviders[questProviderType].CheckToStartQuest(questId, providerId);
        }
    }
}