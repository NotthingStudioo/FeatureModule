namespace GameModule.DailyReward.Signals
{
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.RewardHandle;
    using GameModule.DailyReward.Blueprints;

    public class RewardClaimSignal
    {
        public List<IRewardRecord> Reward;
        public int          Day;
    }
}