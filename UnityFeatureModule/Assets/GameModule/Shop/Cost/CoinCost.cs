namespace Game.Scripts.Shop.Cost
{
    using FeatureTemplate.Scripts.Models.Controllers;
    using Game.Scripts.Blueprints;
    using Zenject;

    public class CoinCost : BaseCost
    {
        [Inject] private FeatureInventoryDataControllerData featureInventoryDataControllerData;
        public override  string                             Id => "Coin";

        public override bool CanAfford(ICostRecord record)
        {
            return this.featureInventoryDataControllerData.GetCurrencyValue() >= int.Parse(record.CostValue);
        }

        public override bool Purchase(ICostRecord record)
        {
            if (this.CanAfford(record))
            {
                this.featureInventoryDataControllerData.AddCurrency(-int.Parse(record.CostValue));
                this.PurchaseSuccess(record);
                return true;
            }
            this.PurchaseFail(record);
            return false;
        }
    }
}