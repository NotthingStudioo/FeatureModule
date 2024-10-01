namespace DailyReward.GameModule.DailyReward.Scripts
{
    using FeatureTemplate.Scripts.RewardHandle;
    using FeatureTemplate.Scripts.Services;
    using UnityEngine;

    public class CurrencyRewardHandler : FeatureRewardExecutorBase
    {
        public override string RewardId                                                  => "add_currency";

        public override void ReceiveReward(int value, RectTransform startPosAnimation)
        {
            this.LogMessage("Grant reward" + value, Color.green);
        }
    }
}