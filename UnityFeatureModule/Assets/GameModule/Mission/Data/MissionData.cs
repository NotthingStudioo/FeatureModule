namespace GameModule.GameModule.Mission.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using FeatureTemplate.Scripts.RewardHandle;
    using GameFoundation.Scripts.Interfaces;
    using global::GameModule.GameModule.Mission.Blueprints;
    using R3;

    public class MissionData : ILocalData, IFeatureLocalData
    {
        // Store dynamic data for all missions
        public Dictionary<string, MissionProgressData> MissionProgress;

        // Store IDs of started missions
        public HashSet<string> StartedMissions;

        public MissionProgressData GetMissionProgress(IMissionRecord missionRecord)
        {
            if (!this.MissionProgress.ContainsKey(missionRecord.Id))
            {
                this.MissionProgress[missionRecord.Id] = new MissionProgressData();
            }

            return this.MissionProgress[missionRecord.Id];
        }

        // Method to mark a mission as started
        public void MarkMissionStarted(IMissionRecord missionRecord)
        {
            // Check instance first
            if (this.MissionProgress.TryGetValue(missionRecord.Id, out var value))
            {
                value.StartTime = DateTime.Now;
            }
            else
            {
                this.MissionProgress[missionRecord.Id] = new MissionProgressData
                {
                    IsCompleted = false,
                    StartTime   = DateTime.Now,
                    Timer       = new ReactiveProperty<int>((int)missionRecord.GetTimeLimit().Value.TotalSeconds) // Default timer value
                };
            }

            this.StartedMissions.Add(missionRecord.Id);
        }

        // Method to cancel track mission
        public void CancelTrackMission(IMissionRecord missionRecord)
        {
            // Check null first
            if (this.MissionProgress.ContainsKey(missionRecord.Id))
            {
                this.MissionProgress.Remove(missionRecord.Id);
            }
        }

        public void Init()
        {
            this.MissionProgress = new Dictionary<string, MissionProgressData>();
            this.StartedMissions = new HashSet<string>();
        }

        public Type ControllerType => typeof(MissionDataController);
    }

    public class MissionProgressData
    {
        public bool                  IsCompleted { get; set; } = false;
        public DateTime?             StartTime   { get; set; }
        public ReactiveProperty<int> Timer       { get; set; } = new(0); // Reactive timer property
        public float                 Progress    { get; set; } = 0;
    }

    public class MissionDataController : IFeatureControllerData
    {
        private readonly MissionData          missionData;
        private readonly FeatureRewardHandler featureRewardHandler;

        public MissionDataController(MissionData missionData, FeatureRewardHandler featureRewardHandler)
        {
            this.missionData          = missionData;
            this.featureRewardHandler = featureRewardHandler;
        }

        public MissionProgressData GetMissionProgress(IMissionRecord missionRecord) { return this.missionData.GetMissionProgress(missionRecord); }

        public void UpdateMissionProgress(IMissionRecord missionRecord, TimeSpan progress)
        {
            var missionProgress = this.missionData.GetMissionProgress(missionRecord);
            missionProgress.Timer.Value = (int)progress.TotalSeconds;
        }

        public void StartMissionProgress(IMissionRecord missionRecord)
        {
            this.missionData.MarkMissionStarted(missionRecord);
            var missionProgress = this.missionData.GetMissionProgress(missionRecord);
            missionProgress.StartTime = DateTime.Now;
        }

        public void CompleteMission(IMissionRecord missionRecord)
        {
            var missionProgress = this.missionData.GetMissionProgress(missionRecord);
            missionProgress.IsCompleted = true;
            missionRecord.OnMissionCompleted?.Invoke(missionRecord);
            this.FinishMission(missionRecord);
        }
        
        public void GrantMissionReward(IMissionRecord missionRecord)
        {
            this.featureRewardHandler.AddRewards(missionRecord.GetRewards(), null);

        }

        private void FinishMission(IMissionRecord missionRecord)
        {
            this.missionData.CancelTrackMission(missionRecord);
            missionRecord.OnMissionFinish?.Invoke(missionRecord);
        }

        public void FailMission(IMissionRecord missionRecord)
        {
            missionRecord.OnMissionFailed?.Invoke(missionRecord);
            this.FinishMission(missionRecord);
        }

        // Get the mission's timer (ReactiveProperty) to allow subscription
        public ReactiveProperty<int> GetMissionTimer(IMissionRecord missionRecord)
        {
            var missionProgress = this.GetMissionProgress(missionRecord);

            return missionProgress.Timer; // Returns the ReactiveProperty<int> for the timer
        }

        // New method to get started missions
        public List<IMissionRecord> GetStartedMissions(MissionBlueprint missionBlueprint) { return this.missionData.StartedMissions.Select(missionBlueprint.GetMissionById).ToList(); }

        public DateTime GetMissionEndTime(IMissionRecord missionRecord)
        {
            var missionProgress = this.missionData.GetMissionProgress(missionRecord);

            return missionProgress.StartTime.Value.Add(missionRecord.GetTimeLimit().Value);
        }

        public void ResetMission(IMissionRecord missionRecord)
        {
            var missionProgress = this.missionData.GetMissionProgress(missionRecord);
            missionProgress.IsCompleted = false;
            missionProgress.StartTime   = null;
            missionProgress.Timer.Value = 0;
        }
    }
}