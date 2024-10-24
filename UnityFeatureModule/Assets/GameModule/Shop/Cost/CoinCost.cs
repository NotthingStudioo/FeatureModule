namespace GameModule.Shop.Cost
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Models.Controllers;
    using Zenject;

    public class CoinCost : BaseCost
    {
        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;
        public override  string                             Id => "Coin";

        public override bool CanAfford(ICostRecord record)
        {
            return this.featureInventoryDataControllerData.GetCurrencyValue() >= int.Parse(record.CostValue);
        }

        public override UniTask<bool> Purchase(ICostRecord record)
        {
            if (this.CanAfford(record))
            {
                this.featureInventoryDataControllerData.AddCurrency(-int.Parse(record.CostValue));
                this.PurchaseSuccess(record);
                return UniTask.FromResult(true);
            }
            this.PurchaseFail(record);
            return UniTask.FromResult(false);
        }
    }
}