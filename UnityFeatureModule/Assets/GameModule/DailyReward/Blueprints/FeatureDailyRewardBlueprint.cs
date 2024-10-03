namespace FeatureTemplate.Scripts.Blueprints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("DailyRewardData")]
    public class FeatureDailyRewardBlueprint : GenericBlueprintReaderByRow<string, DailyRewardRecord>
    {
        public List<Reward> GetRewards(string day) { return this[day].Rewards.Values.ToList(); }
    }

    [CsvHeaderKey("Id")]
    public class DailyRewardRecord
    {
        public string Id             { get; set; }
        public string DayIndex       { get; set; }
        public string Title          { get; set; }
        public string Description    { get; set; }
        public bool   ShowAdsNextDay { get; set; }
        public string RewardId       { get; set; }
        public string RewardType     { get; set; }
        public string RewardValue    { get; set; }

        public BlueprintByRow<string, Reward> Rewards { get; set; }
    }

    [CsvHeaderKey("RId")]
    [Serializable]
    public class Reward
    {
        public string RId         { get; set; }
        public string RewardId    { get; set; }
        public string RewardType  { get; set; }
        public string RewardValue { get; set; }
        public string IconPath    { get; set; }
    }
}