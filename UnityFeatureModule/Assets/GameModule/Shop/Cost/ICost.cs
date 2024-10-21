namespace GameModule.Shop.Cost
{
    public interface ICost
    {
        string Id { get; }

        bool CanAfford(ICostRecord record);

        /// <summary>
        /// Call when player tap purchase button
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        bool Purchase(ICostRecord record);

        /// <summary>
        /// On Purchase Fail get Invoke when the transaction is not enough. If the transaction require ads,
        /// this method will be called to show ads.
        /// </summary>
        /// <param name="record"></param>
        void PurchaseFail(ICostRecord record);

        void PurchaseSuccess(ICostRecord record);
    }
}