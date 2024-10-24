namespace GameModule.Shop.Cost
{
    using Cysharp.Threading.Tasks;

    public interface ICost
    {
        string Id { get; }

        bool CanAfford(ICostRecord record);

        /// <summary>
        /// Call when player tap purchase button
        /// </summary>
        /// <param name="record"></param>
        /// <returns>a Task return the result whether purchasable or not</returns>
        UniTask<bool> Purchase(ICostRecord record);

        /// <summary>
        /// On Purchase Fail get Invoke when the transaction is not enough. If the transaction require ads,
        /// this method will be called to show ads.
        /// </summary>
        /// <param name="record"></param>
        void PurchaseFail(ICostRecord record);

        void PurchaseSuccess(ICostRecord record);
    }
}