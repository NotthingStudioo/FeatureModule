namespace GameModule.GameModule.Mission.Blueprints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;
    using FeatureTemplate.Scripts.RewardHandle;
    using global::GameModule.DailyReward.Blueprints;

    [BlueprintReader("MissionData")]
    public class MissionBlueprint : GenericBlueprintReaderByRow<string, MissionRecordRecord>
    {
        private MissionRecordViewModel ConvertToDTO(MissionRecordRecord record)
        {
            return new MissionRecordViewModel
            {
                Id           = record.Id,
                Title        = record.Title,
                Description  = record.Description,
                NextMissions = record.NextMissions,
                TimeLimit    = record.TimeLimit,
                Rewards      = record.Rewards,
                Conditions   = record.Conditions
            };
        }
        
        private List<MissionRecordViewModel> ConvertToDTOs(IEnumerable<MissionRecordRecord> records)
        {
            return records.Select(this.ConvertToDTO).ToList();
        }
        
        // Method to find a mission by its id
        public IMissionRecord GetMissionById(string id) { return this.ConvertToDTO(this[id]); }

        // You can add other methods to query by different fields if necessary
        public List<IMissionRecord> GetAllMissions() { return this.ConvertToDTOs(this.Values).Cast<IMissionRecord>().ToList(); }
    }

    #warning You should not use this class directly, use the DTO instead
    [CsvHeaderKey("Id")]
    public class MissionRecordRecord
    {
        public string                        Id           { get;         set; }
        public string                        Title        { get;         set; }
        public string                        Description  { get;         set; }
        public List<int>                     NextMissions { get;         set; } = new();
        public string                        TimeLimit    { get; set; }
        public BlueprintByRow<Reward>        Rewards      { get;         set; }
        public BlueprintByRow<MissionCondition> Conditions   { get;         set; }
    }

    public class MissionRecordViewModel : IMissionRecord
    {
        public string                        Id           { get; set; }
        public string                        Title        { get; set; }
        public string                        Description  { get; set; }
        public List<int>                     NextMissions { get; set; }
        public string                        TimeLimit    { get; set; }
        public BlueprintByRow<Reward>        Rewards      { get; set; }
        public BlueprintByRow<MissionCondition> Conditions   { get; set; }

        public TimeSpan? GetTimeLimit()
        {
            if (string.IsNullOrEmpty(this.TimeLimit)) return null;

            return TimeSpan.Parse(this.TimeLimit);
        }

        public List<IRewardRecord>    GetRewards()    { return this.Rewards.Cast<IRewardRecord>().ToList(); }
        public List<IConditionRecord> GetConditions() { return this.Conditions.Cast<IConditionRecord>().ToList(); }

        public Action<IMissionRecord> OnMissionCompleted { get; set; }
        public Action<IMissionRecord> OnMissionFailed    { get; set; }
        public Action<IMissionRecord> OnMissionFinish    { get; set; }
    }

    [CsvHeaderKey("ConditionId")]
    public class MissionCondition : IConditionRecord
    {
        public string ConditionId    { get; set; }
        public string ConditionType  { get; set; }
        public string ConditionParam { get; set; }
    }
}