namespace Game.Scripts.Shop
{
    using System;
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using Game.Scripts.Blueprints;
    using GameFoundation.Scripts.Interfaces;

    public class FeatureTransactionData : ILocalData, IFeatureLocalData
    {
        public readonly Dictionary<string, int> TransactionAdsUnlockedTime = new();
        public          void                    Init() { }

        public Type ControllerType => typeof(FeatureTransactionDataController);
    }

    public class FeatureTransactionDataController : IFeatureControllerData
    {
        public FeatureTransactionData Data { get; private set; }

        public FeatureTransactionDataController(FeatureTransactionData data) { this.Data = data; }

        public int CheckTransaction(ICostRecord shopCostRecord)
        {
            // Use ShopRecord.Id as the key for checking transactions
            if (this.Data.TransactionAdsUnlockedTime.TryGetValue(shopCostRecord.CostId, out var unlocked))
            {
                return unlocked;
            }
            else if (this.Data.TransactionAdsUnlockedTime.TryAdd(shopCostRecord.CostId, 1))
            {
                return 1; // New transaction for this ShopRecord.Id
            }

            return -1; // Transaction check failed or doesn't exist
        }

        public void AddTransaction(ICostRecord shopCostRecord)
        {
            // Use ShopRecord.Id as the key for adding/updating transactions
            if (this.Data.TransactionAdsUnlockedTime.TryGetValue(shopCostRecord.CostId, out var unlocked))
            {
                this.Data.TransactionAdsUnlockedTime[shopCostRecord.CostId] = unlocked + 1; // Increment existing transaction count
            }
            else
            {
                this.Data.TransactionAdsUnlockedTime.TryAdd(shopCostRecord.CostId, 1); // First transaction for this ShopRecord.Id
            }
        }
    }
}