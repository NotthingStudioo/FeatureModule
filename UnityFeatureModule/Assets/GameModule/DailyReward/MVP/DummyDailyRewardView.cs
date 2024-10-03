namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.Services;
    using FeatureTemplate.Scripts.Signals;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using global::DailyReward.GameModule.DailyReward.Scripts;
    using global::DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem;
    using UnityEngine;
    using Zenject;

    public class DummyDailyRewardView : DailyRewardViewTemplate
    {
        [SerializeField] public List<RewardSlotAdapter> adapters;

        public FeatureButtonView claimButton;
        public FeatureButtonView claimButtonWithAds;
    }

    [PopupInfo("BaseDailyRewardView")]
    public class DummyDailyRewardPresenter : DailyRewardPresenterTemplate
    {
        private readonly LoadImageHelper      loadImageHelper;
        private readonly DiContainer          diContainer;
        private          DummyDailyRewardView View => (DummyDailyRewardView)base.View;

        public DummyDailyRewardPresenter(LoadImageHelper loadImageHelper, DailyRewardService dailyRewardService, SignalBus signalBus, 
            ScreenManager screenManager, SceneDirector sceneDirector, DiContainer diContainer) : base(
            dailyRewardService, signalBus, screenManager, sceneDirector)
        {
            this.loadImageHelper = loadImageHelper;
            this.diContainer     = diContainer;
        }

        public override async UniTask BindData()
        {
            base.BindData();

            for (var index = 0; index < this.View.adapters.Count; index++)
            {
                var iconList   = await this.dailyRewardService.ReadRewardsAt(index).Select(async x => 
                    new RewardSlotModel()
                    {
                        Sprite = await this.loadImageHelper.LoadLocalSprite(x.IconPath)
                    }
                    ).ToList();
                

                await this.View.adapters[index].InitItemAdapter(iconList.ToList(), this.diContainer);
            }

            this.View.claimButton.InitButtonEvent(_ => this.ClaimReward(), new()
            {
                ButtonName      = "btn_claim",
                ScreenPresenter = this,
                ScreenViewName  = this.View.name,
                ButtonStatus    = ButtonStatus.On
            });
            
            this.View.claimButtonWithAds.InitButtonEvent(_ => this.ClaimRewardWithAds(), new()
            {
                ButtonName      = "btn_claim_with_ads",
                ScreenPresenter = this,
                ScreenViewName  = this.View.name,
                ButtonStatus    = ButtonStatus.On
            });
        }

        private void ClaimRewardWithAds()
        {
            this.dailyRewardService.MoveToNextDayAndClaimReward();
        }

        protected override void OnClaimReward(RewardClaimSignal signal) { this.LogMessage("presenter receive claimed signal ", Color.green); }
    }
}