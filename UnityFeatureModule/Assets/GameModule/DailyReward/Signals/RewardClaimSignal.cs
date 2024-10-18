namespace GameModule.DailyReward.Signals
{
    using System.Collections.Generic;
    using GameModule.DailyReward.Blueprints;

    public class RewardClaimSignal
    {
        public List<Reward> Reward;
        public int          Day;
    }
}