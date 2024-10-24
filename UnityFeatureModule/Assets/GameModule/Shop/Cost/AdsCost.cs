namespace a
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Models.Controllers;
    using FeatureTemplate.Scripts.Services.Ads;
    using GameModule.Shop;
    using GameModule.Shop.Cost;
    using Zenject;

    public class AdsCost : BaseCost
    {
        public override string Id => "Ads";
        [Inject] private FeatureTransactionDataController featureTransactionDataController;
        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;
        [Inject] private FeatureAdsServices featureAdsServices;

        public override bool CanAfford(ICostRecord record) { return true; }

        public override async UniTask<bool> Purchase(ICostRecord record)
        {
            var result     = false;
            var isComplete = false;
            this.featureAdsServices.ShowRewardedAd("UnlockSkin", () =>
            {
                this.PurchaseSuccess(record);
                isComplete = true;
                result     = true;
            }, () =>
            {
                this.PurchaseFail(record);
                result     = false;
                isComplete = true;
            });

            await UniTask.WaitUntil(() => isComplete);
            return result;
        }

        public override void PurchaseFail(ICostRecord record)
        {
            base.PurchaseFail(record);
            this.featureTransactionDataController.AddTransaction(record);
        }
    }
}