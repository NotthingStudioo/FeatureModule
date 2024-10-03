namespace DailyReward.GameModule.DailyReward.Scripts
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Models.Controllers;
    using FeatureTemplate.Scripts.RewardHandle;
    using FeatureTemplate.Scripts.Services;
    using UnityEngine;
    using Zenject;

    public class CurrencyRewardHandler : FeatureRewardExecutorBase
    {
        public override string RewardId                                                  => "add_currency";

        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;
        public override void ReceiveReward(int value, RectTransform startPosAnimation)
        {
            this.featureInventoryDataControllerData.AddCurrency(value).Forget();
            this.LogMessage("Grant reward" + this.featureInventoryDataControllerData.GetCurrencyData(), Color.green);
        }
    }
}