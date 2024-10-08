namespace DailyReward.GameModule.DailyReward.Signals
{
    using System.Collections.Generic;
    using global::DailyReward.GameModule.DailyReward.Blueprints;

    public class RewardClaimSignal
    {
        public List<Reward> Reward;
        public int          Day;
    }
}