namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.Services;
    using FeatureTemplate.Scripts.Signals;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using global::DailyReward.GameModule.DailyReward.Scripts;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class DummyDailyRewardView : DailyRewardViewTemplate
    {
        [SerializeField]
        public List<TextMeshProUGUI> rewardTexts;

        public FeatureButtonView claimButton;
    }
    [PopupInfo("BaseDailyRewardView")]
    public class DummyDailyRewardPresenter : DailyRewardPresenterTemplate
    {
        private DummyDailyRewardView View => (DummyDailyRewardView)base.View;
        public DummyDailyRewardPresenter(DailyRewardService dailyRewardService, SignalBus signalBus, ScreenManager screenManager, SceneDirector sceneDirector) : base(dailyRewardService, signalBus, screenManager, sceneDirector)
        {
        }

        public override UniTask BindData()
        {
            base.BindData();

            for (var index = 0; index < this.View.rewardTexts.Count; index++)
            {
                var dataRaw = "";
                this.dailyRewardService.ReadRewardsAt(index).ForEach(x => dataRaw += x.RewardType + " ");
                var text    = this.View.rewardTexts[index];
                text.text = "Claim " + dataRaw;
            }

            this.View.claimButton.InitButtonEvent(_ => this.ClaimReward(), new()
            {
                ButtonName = "btn_claim",
                ScreenPresenter = this,
                ScreenViewName = this.View.name,
                ButtonStatus = ButtonStatus.On
            });
            
            return UniTask.CompletedTask;
        }

        protected override void OnClaimReward(RewardClaimSignal signal)
        {
            this.LogMessage("presenter receive claimed signal ", Color.green);
        }
    }
}