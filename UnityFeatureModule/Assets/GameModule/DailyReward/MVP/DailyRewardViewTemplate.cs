namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Blueprints;
    using FeatureTemplate.Scripts.MVP;
    using FeatureTemplate.Scripts.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using global::DailyReward.GameModule.DailyReward.Scripts;
    using global::DailyReward.GameModule.DailyReward.Signals;
    using UnityEngine;
    using Zenject;

    public abstract class DailyRewardViewTemplate : FeatureBasePopupViewTemplate
    {
    }

    public abstract class DailyRewardPresenterTemplate : FeatureBasePopupPresenterTemplate<DailyRewardViewTemplate>
    {
        protected readonly DailyRewardService dailyRewardService;

        protected DailyRewardPresenterTemplate(DailyRewardService dailyRewardService, SignalBus signalBus, ScreenManager screenManager, SceneDirector sceneDirector) : base(signalBus, screenManager,
            sceneDirector)
        {
            this.dailyRewardService = dailyRewardService;
        }

        public override UniTask BindData()
        {
            this.SignalBus.Subscribe<RewardClaimSignal>(this.OnClaimReward);

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// Claim today reward
        /// </summary>
        /// <returns></returns>
        protected void ClaimReward(int day, GameObject source) { this.dailyRewardService.ClaimReward(day, source); }

        /// <summary>
        /// Read the reward at a specific day.
        /// </summary>
        /// <param name="dayOffset">The offset to that day calculate since today.</param>
        /// <returns></returns>
        protected List<Reward> GetRewardsAt(int dayOffset) { return this.dailyRewardService.ReadRewardsAtDay(dayOffset); }

        protected abstract void OnClaimReward(RewardClaimSignal signal);

        public override void Dispose()
        {
            this.SignalBus.Unsubscribe<RewardClaimSignal>(this.OnClaimReward);
            base.Dispose();
        }
    }
}