namespace GameModule.Shop.Cost
{
    using FeatureTemplate.Scripts.Models.Controllers;
    using Zenject;

    public class AdsCost : BaseCost
    {
        public override string                             Id => "Ads";
        [Inject] private FeatureTransactionDataController          featureTransactionDataController;
        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;

        public override bool CanAfford(ICostRecord record) { return this.featureTransactionDataController.CheckTransaction(record) >= int.Parse(record.CostValue); }

        public override bool Purchase(ICostRecord record)
        {
            if (this.CanAfford(record))
            {
                this.PurchaseSuccess(record);

                return true;
            }
            else
            {
                this.PurchaseFail(record);

                return false;
            }
        }

        public override void PurchaseFail(ICostRecord record)
        {
            base.PurchaseFail(record);
            this.featureTransactionDataController.AddTransaction(record);
        }
    }
}