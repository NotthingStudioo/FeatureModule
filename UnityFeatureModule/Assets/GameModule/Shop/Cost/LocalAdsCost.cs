﻿namespace GameModule.Shop.Cost
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Models.Controllers;
    using Zenject;

    public class LocalAdsCost : BaseCost
    {
        public override string                             Id => "Local_Ads_Count";
        [Inject] private FeatureTransactionDataController          featureTransactionDataController;
        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;

        public override bool CanAfford(ICostRecord record) { return this.featureTransactionDataController.CheckTransaction(record) >= int.Parse(record.CostValue); }

        // DO some think like try purchase, if false
        public override UniTask<bool> Purchase(ICostRecord record)
        {
            if (this.CanAfford(record))
            {
                this.PurchaseSuccess(record);

                return UniTask.FromResult(true);
            }
            else
            {
                this.PurchaseFail(record);

                return UniTask.FromResult(false);
            }
        }

        public override void PurchaseFail(ICostRecord record)
        {
            base.PurchaseFail(record);
            this.featureTransactionDataController.AddTransaction(record);
        }
    }
}