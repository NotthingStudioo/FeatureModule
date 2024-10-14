namespace Game.Scripts.Shop.Condition
{
    using FeatureTemplate.Scripts.Models.LocalDatas;
    using Zenject;

    public class AdsCondition : IShopCondition
    {
        [Inject] private FeatureAdsData featureAdsData;
        public           string         Id                  => "Ads_reach";

        public bool IsMet(string param)
        {
            return int.Parse(param) >= this.featureAdsData.WatchedRewardedAds + this.featureAdsData.WatchedInterstitialAds;
        }
    }
}