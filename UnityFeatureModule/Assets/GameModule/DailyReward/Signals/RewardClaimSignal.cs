namespace GameModule.GameModule.DailyReward.Signals
{
    using System.Collections.Generic;
    using global::GameModule.GameModule.DailyReward.Blueprints;

    public class RewardClaimSignal
    {
        public List<Reward> Reward;
        public int          Day;
    }
}