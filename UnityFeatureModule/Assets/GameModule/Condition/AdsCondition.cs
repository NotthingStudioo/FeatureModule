namespace GameModule.GameModule.Condition
{
    using FeatureTemplate.Scripts.Models.LocalDatas;
    using global::GameModule.GameModule.Mission.Blueprints;
    using UnityEngine;
    using Zenject;

    public class AdsCondition : ICondition
    {
        [Inject] private FeatureAdsData featureAdsData;
        public           string         ConditionId                  => "Ads_reach";

        public bool IsMet(string param)
        {
            return int.Parse(param) >= this.featureAdsData.WatchedRewardedAds + this.featureAdsData.WatchedInterstitialAds;
        }

        public float GetProgress(IConditionRecord conditionRecord)
        {
            var finalValue = int.Parse(conditionRecord.ConditionParam);
            var currentValue = this.featureAdsData.WatchedRewardedAds + this.featureAdsData.WatchedInterstitialAds;
            return Mathf.Clamp(currentValue/finalValue, 0, 1);
        }
    }
}