namespace Game.Scripts.Shop.Cost
{
    using FeatureTemplate.Scripts.Services;
    using Game.Scripts.Blueprints;

    public class IAPCost : BaseCost
    {
        private readonly FeatureIapServices featureIapServices;
        public override  string             Id => "IAP";

        public IAPCost(FeatureIapServices featureIapServices) { this.featureIapServices = featureIapServices; }

        public override bool CanAfford(ICostRecord record) { return true; }

        public override bool Purchase(ICostRecord record)
        {
            this.featureIapServices.BuyProduct(null, record.CostValue,
                _ => this.PurchaseFail(record),
                _ => this.PurchaseSuccess(record)
            );

            return true;
        }
    }
}