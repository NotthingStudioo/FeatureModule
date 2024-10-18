namespace GameModule.GameModule.Mission
{
    using System;
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.RewardHandle;
    using global::GameModule.GameModule.Mission.Blueprints;

    public interface IMissionRecord
    {
        string    Id           { get; set; }
        string    Title        { get; set; }
        string    Description  { get; set; }
        List<int> NextMissions { get; set; } // Missions to unlock after completion

        TimeSpan? GetTimeLimit();

        List<IRewardRecord>    GetRewards();
        List<IConditionRecord> GetConditions();

        public Action<IMissionRecord> OnMissionCompleted { get; set; }
        public Action<IMissionRecord> OnMissionFailed    { get; set; }
        public Action<IMissionRecord> OnMissionFinish    { get; set; }
    }
}