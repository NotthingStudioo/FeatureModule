namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Blueprints;
    using FeatureTemplate.Scripts.MVP;
    using FeatureTemplate.Scripts.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using global::DailyReward.GameModule.DailyReward.Scripts;
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
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ClaimReward() { return this.dailyRewardService.ClaimReward(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dayOffset"></param>
        /// <returns></returns>
        protected List<Reward> GetRewardsAt(int dayOffset) { return this.dailyRewardService.ReadRewardsAt(dayOffset); }

        protected abstract void OnClaimReward(RewardClaimSignal signal);

        public override void Dispose()
        {
            this.SignalBus.Unsubscribe<RewardClaimSignal>(this.OnClaimReward);
            base.Dispose();
        }
    }
}