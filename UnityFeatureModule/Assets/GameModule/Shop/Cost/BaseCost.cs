namespace Game.Scripts.Shop.Cost
{
    using System;
    using Game.Scripts.Blueprints;

    public abstract class BaseCost : ICost
    {
        public event Action<ICostRecord> OnPurchaseSuccess;
        public event Action<ICostRecord> OnPurchaseFail;

        public abstract string Id { get; }
        public abstract bool   CanAfford(ICostRecord record);
        public abstract bool   Purchase(ICostRecord record);
        public virtual  void   PurchaseFail(ICostRecord record)    { this.OnPurchaseFail?.Invoke(record); }
        public virtual  void   PurchaseSuccess(ICostRecord record) { this.OnPurchaseSuccess?.Invoke(record); }
    }
}