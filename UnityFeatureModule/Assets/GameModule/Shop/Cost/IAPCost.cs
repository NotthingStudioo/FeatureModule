namespace GameModule.Shop.Cost
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Services;

    public class IAPCost : BaseCost
    {
        private readonly FeatureIapServices featureIapServices;
        public override  string             Id => "IAP";

        public IAPCost(FeatureIapServices featureIapServices) { this.featureIapServices = featureIapServices; }

        public override bool CanAfford(ICostRecord record) { return true; }

        public override UniTask<bool> Purchase(ICostRecord record)
        {
            this.featureIapServices.BuyProduct(null, record.CostValue,
                _ => this.PurchaseFail(record),
                _ => this.PurchaseSuccess(record)
            );

            return UniTask.FromResult(true);
        }
    }
}