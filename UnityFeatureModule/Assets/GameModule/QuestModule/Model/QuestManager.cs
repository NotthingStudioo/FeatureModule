﻿namespace GameModule.QuestModule.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using ClaimReward;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using FeatureTemplate.Scripts.RewardHandle;
    using FeatureTemplate.Scripts.Services;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameModule.QuestModule.Signals;
    using global::Blueprints;
    using global::QuestModule.Provider;
    using Zenject;
    using RewardRecord = FeatureTemplate.Scripts.RewardHandle.RewardRecord;

    public class QuestManager : IFeatureControllerData
    {
        private readonly FeatureRewardHandler                        featureRewardHandler;
        private readonly FeatureDataState                            featureDataState;
        private readonly QuestJournal                                data;
        private readonly SignalBus                                   signalBus;
        private readonly ScreenManager                               screenManager;
        public           Dictionary<string, Dictionary<string, int>> TrackingCached => this.data.TrackingCached;

        public QuestManager(FeatureRewardHandler featureRewardHandler, FeatureDataState featureDataState, QuestJournal data, SignalBus signalBus, ScreenManager screenManager)
        {
            this.featureRewardHandler = featureRewardHandler;
            this.featureDataState     = featureDataState;
            this.data                 = data;
            this.signalBus            = signalBus;
            this.screenManager        = screenManager;
        }


        public async void LoadRecord(IQuestProvider questProvider)
        {
            await UniTask.WaitUntil((() => this.featureDataState.IsBlueprintAndLocalDataLoaded));

            foreach (var q in this.data.Quests.Where(x => x.Value.QuestProviderType == questProvider.QuestProviderType))
            {
                var questRecord = questProvider.GetQuestRecord(q.Value.QuestId, q.Value.ProviderId);

                for (var index = 0; index < q.Value.TaskProgress.Count; index++)
                {
                    var taskLog = q.Value.TaskProgress[index];
                    taskLog.TaskRecord = questRecord.Tasks[index];
                }

                q.Value.QuestRecord = questRecord;
            }
        }

        public bool CheckQuestAccepted(string id, string providerId)
        {
            return this.data.Quests.Where(x => x.Value.QuestId.Equals(id)
                                               && x.Value.ProviderId.Equals(providerId)).ToList().Count > 0;
        }

        public bool CheckCompleteNotReceivingRewardQuest(string id, string providerId)
        {
            return this.data.Quests.Where(x
                => x.Value.QuestId.Equals(id)
                   && x.Value.ProviderId.Equals(providerId)
                   && x.Value.QuestStatus == QuestStatus.Completed).ToList().Count > 0;
        }

        public bool CheckQuestDone(string id, string providerId)
        {
            return this.data.Quests.Where(x
                => x.Value.QuestId.Equals(id)
                   && x.Value.ProviderId.Equals(providerId)
                   && x.Value.QuestStatus == QuestStatus.Rewarded).ToList().Count > 0;
        }

        public QuestLog GetQuest(string questId, string provideId) { return this.data.Quests.FirstOrDefault(q => q.Key.Equals(questId) && q.Value.ProviderId.Equals(provideId)).Value; }

        public QuestLog CheckToAddNewQuest(string questId, string provideId, QuestProviderType questProviderType, QuestRecord questRecord)
        {
            if (!this.data.Quests.TryGetValue(questId, out var questInfo))
            {
                var listTaskProgress = new List<TaskLog>();

                foreach (var taskRecord in questRecord.Tasks)
                {
                    var requirementProgress = new List<RequirementProgress>();

                    foreach (var requirementRecord in taskRecord.RequirementRecords)
                    {
                        requirementProgress.Add(new RequirementProgress()
                        {
                            RequirementId   = requirementRecord.RequirementId,
                            RequirementType = requirementRecord.RequirementType,
                            CurrentValue    = 0,
                            RequiredValue   = requirementRecord.RequirementValue
                        });
                    }

                    listTaskProgress.Add(new TaskLog()
                    {
                        TaskRecord = taskRecord,
                        TaskStatus = QuestStatus.NotStarted,
                        Progress   = requirementProgress
                    });
                }

                listTaskProgress[0].TaskStatus = QuestStatus.InProgress;

                questInfo = new QuestLog()
                {
                    QuestId           = questId,
                    QuestStatus       = QuestStatus.NotStarted,
                    ProviderId        = provideId,
                    TaskProgress      = listTaskProgress,
                    QuestRecord       = questRecord,
                    QuestProviderType = questProviderType,
                    QuestType         = questRecord.QuestType,
                };
            }

            this.data.Quests[questId] = questInfo;

            return questInfo;
        }

        /// <summary>
        /// Get first task progress that has not been completed
        /// </summary>
        /// <param name="questId"></param>
        /// <param name="taskId"></param>
        /// <param name="provideId"></param>
        /// <returns></returns>
        public TaskLog GetCurrentTaskProgress(string questId, string provideId)
        {
            var questInfo = this.GetQuest(questId, provideId);
            var taskId    = questInfo.TaskProgress.FirstOrDefault(task => task.Progress.All(requirement => requirement.CurrentValue < requirement.RequiredValue))?.TaskRecord.TaskId;

            return questInfo.TaskProgress.FirstOrDefault(task => task.TaskRecord.TaskId.Equals(taskId));
        }

        public void UpdateQuestStatus(string provideId, string questId, QuestStatus questStatus)
        {
            var questInfo = this.GetQuest(questId, provideId);

            questInfo.QuestStatus = questStatus;
        }

        public QuestJournal QuestJournal => this.data;

        public void UpdateTaskStatus(string questInfoProviderId, string questInfoQuestId, string taskRecordTaskId, QuestStatus questStatus)
        {
            var taskLog = this.GetQuest(questInfoQuestId, questInfoProviderId).TaskProgress.First(task => task.TaskRecord.TaskId.Equals(taskRecordTaskId));
            taskLog.TaskStatus = questStatus;
        }

        public void GiveRewardToQuest(string questInfoProviderId, string questInfoQuestId)
        {
            var quest    = this.GetQuest(questInfoQuestId, questInfoProviderId);
            var taskLogs = quest.TaskProgress.Where(t => t.TaskStatus == QuestStatus.Completed).ToList();

            foreach (var taskLog in taskLogs)
            {
                taskLog.TaskStatus = QuestStatus.Rewarded;

                var listAsset = Enumerable.Select(taskLog.TaskRecord.RewardRecords, x => new RewardRecord()
                {
                    RewardId    = x.TaskRewardId,
                    RewardValue = x.TaskRewardValue,
                    RewardType  = x.TaskRewardType
                });

                this.Payout(listAsset.ToList<IRewardRecord>(), quest.QuestProviderType == QuestProviderType.Side);
            }
        }

        public void GiveRewardToCurrentTaskQuest(string questInfoProviderId, string questInfoQuestId)
        {
            var quest   = this.GetQuest(questInfoQuestId, questInfoProviderId);
            var taskLog = quest.TaskProgress.FirstOrDefault(t => t.TaskStatus == QuestStatus.Completed);

            if (taskLog == null) return;
            taskLog.TaskStatus = QuestStatus.Rewarded;

            var listAsset = taskLog.TaskRecord.RewardRecords.Select(x => new RewardRecord()
            {
                RewardId    = x.TaskRewardId,
                RewardValue = x.TaskRewardValue,
                RewardType  = x.TaskRewardType
            });

            if (quest.TaskProgress.IndexOf(taskLog) == quest.TaskProgress.Count - 1) quest.QuestStatus = QuestStatus.Completed;

            this.Payout(listAsset.ToList<IRewardRecord>(), quest.QuestProviderType == QuestProviderType.Side);
        }

        /// <summary>
        /// GetAll task reward from quest and set task status to rewarded
        /// </summary>
        /// <param name="questLogProviderId"></param>
        /// <param name="questId"></param>
        /// <returns></returns>
        public List<IRewardRecord> GetTaskReward(string questLogProviderId, string questId)
        {
            var quest     = this.GetQuest(questId, questLogProviderId);
            var taskLogs  = quest.TaskProgress.Where(t => t.TaskStatus == QuestStatus.Completed).ToList();
            var listAsset = new List<IRewardRecord>();

            foreach (var taskLog in taskLogs)
            {
                taskLog.TaskStatus = QuestStatus.Rewarded;

                listAsset.AddRange(taskLog.TaskRecord.RewardRecords.Select(x => new RewardRecord()
                {
                    RewardId    = x.TaskRewardId,
                    RewardValue = x.TaskRewardValue,
                    RewardType  = x.TaskRewardType
                }));
            }

            return listAsset;
        }

        public List<IRewardRecord> GetCurrentTaskReward(string questLogProviderId, string questId)
        {
            var quest     = this.GetQuest(questId, questLogProviderId);
            var taskLog   = quest.TaskProgress.FirstOrDefault(t => t.TaskStatus == QuestStatus.Completed);
            var listAsset = new List<IRewardRecord>();

            if (taskLog == null) return listAsset;
            taskLog.TaskStatus = QuestStatus.Rewarded;

            listAsset.AddRange(Enumerable.Select(taskLog.TaskRecord.RewardRecords, x => new RewardRecord()
            {
                RewardId    = x.TaskRewardId,
                RewardValue = x.TaskRewardValue,
                RewardType  = x.TaskRewardType
            }));

            return listAsset;
        }

        /// <summary>
        /// GetAll reward from quest and set quest status to rewarded
        /// </summary>
        /// <param name="questLogProviderId"></param>
        /// <param name="questLogQuestId"></param>
        /// <returns></returns>
        public List<IRewardRecord> GetQuestReward(string questLogProviderId, string questLogQuestId)
        {
            var questInfo = this.GetQuest(questLogQuestId, questLogProviderId);

            var result = new List<IRewardRecord>();

            if (questInfo.QuestStatus != QuestStatus.Completed) return result;
            questInfo.QuestStatus = QuestStatus.Rewarded;

            result.AddRange(questInfo.QuestRecord.QuestRewardRecords.Select(x => new RewardRecord()
            {
                RewardId    = x.QuestRewardId,
                RewardValue = x.QuestRewardValue,
                RewardType  = x.QuestRewardType
            }));

            return result;
        }

        private void Payout(List<IRewardRecord> assets, bool isSideQuest = false) { this.featureRewardHandler.AddRewards(assets, null); }

        public void CheckToCompleteQuest(string questInfoProviderId, string questInfoQuestId)
        {
            var questInfo = this.GetQuest(questInfoQuestId, questInfoProviderId);

            //if (questInfo.TaskProgress.Any(t => t.TaskStatus != QuestStatus.Completed)) return;
            if (questInfo.QuestStatus != QuestStatus.Completed) return;
            questInfo.QuestStatus = QuestStatus.Rewarded;

            var listAsset = questInfo.QuestRecord.QuestRewardRecords.Select(x => new RewardRecord()
            {
                RewardId    = x.QuestRewardId,
                RewardValue = x.QuestRewardValue,
                RewardType  = x.QuestRewardType
            });

            var listIRewardRecord = listAsset.ToList<IRewardRecord>();

            this.Payout(listIRewardRecord, questInfo.QuestProviderType == QuestProviderType.Side);
            this.signalBus.Fire<RefreshQuestViewSignal>();

            if (questInfo.QuestProviderType == QuestProviderType.Side)
            {
                this.signalBus.Fire(new TrackingQuestSignal(StaticValue.RequirementStaticValue.CompleteASideQuest, questInfo.QuestId, 1));
            }
        }

        public List<QuestLog> GetAllQuestsType(QuestProviderType questProviderType)
        {
            return this.data.Quests.Where(x => x.Value.QuestProviderType == questProviderType).Select(x => x.Value).ToList();
        }

        public List<string> GetAllQuestsCategoryOfAQuestProvider(QuestProviderType questProviderId)
        {
            var quests = this.GetAllQuestsType(questProviderId);

            return quests.Select(q => q.QuestType).Distinct().ToList();
        }

        public List<QuestLog> GetAllQuests(QuestProviderType questProviderType, string questCategory)
        {
            var quests = this.GetAllQuestsType(questProviderType);

            return quests.Where(q => q.QuestType.Equals(questCategory)).ToList();
        }

        public QuestLog GetCurrentMainQuest()
        {
            var mainQuestInprogress = this.data.Quests.FirstOrDefault(x => x.Value.QuestProviderType == QuestProviderType.Main && x.Value.QuestStatus == QuestStatus.InProgress).Value;

            if (mainQuestInprogress != null)
            {
                return mainQuestInprogress;
            }

            var mainQuestCompleted = this.data.Quests.FirstOrDefault(x => x.Value.QuestProviderType == QuestProviderType.Main && x.Value.QuestStatus == QuestStatus.Completed).Value;

            return mainQuestCompleted ?? null;
        }

        public void SetQuestStatus(string questInfoProviderId, string questInfoQuestId, QuestStatus status)
        {
            var questInfo = this.GetQuest(questInfoQuestId, questInfoProviderId);
            questInfo.QuestStatus = status;
        }

        public void RemoveChallengeQuest(string modelProviderId, string modelQuestId)
        {
            var quest = this.GetQuest(modelQuestId, modelProviderId);
            this.data.Quests.Remove(quest.QuestId);
        }

        public void ShowPopupClaimReward(QuestLog questLog, bool isCurrentTaskOnly = false)
        {
            var listAsset = new List<IRewardRecord>();

            if (isCurrentTaskOnly) listAsset = this.GetCurrentTaskReward(questLog.ProviderId, questLog.QuestId);
            else
            {
                listAsset = this.GetTaskReward(questLog.ProviderId, questLog.QuestId);
                listAsset.AddRange(this.GetQuestReward(questLog.ProviderId, questLog.QuestId));
            }

            this.screenManager.OpenScreen<ClaimRewardPopupPresenter, ClaimRewardPopupModel>(new ClaimRewardPopupModel()
            {
                RewardResult = listAsset
            }).Forget();
        }
    }
}