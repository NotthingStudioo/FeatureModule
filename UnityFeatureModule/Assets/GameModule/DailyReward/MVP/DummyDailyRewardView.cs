namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.Services;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using global::DailyReward.GameModule.DailyReward.Data;
    using global::DailyReward.GameModule.DailyReward.Scripts;
    using global::DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem;
    using global::DailyReward.GameModule.DailyReward.Signals;
    using UnityEngine;
    using Zenject;
    using Color = UnityEngine.Color;

    public class DummyDailyRewardView : DailyRewardViewTemplate
    {
        [SerializeField] public List<RewardSlotAdapter> adapters;

        public FeatureButtonView claimButton;
        public FeatureButtonView exitButton;
    }

    [PopupInfo("BaseDailyRewardView")]
    public class DummyDailyRewardPresenter : DailyRewardPresenterTemplate
    {
        private readonly LoadImageHelper               loadImageHelper;
        private readonly DailyRewardDataController     dailyRewardDataController;
        private readonly DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint;
        private readonly FeatureDailyRewardBlueprint   featureDailyRewardBlueprint;
        private readonly DiContainer                   diContainer;
        private          int                           Page { get => this.dailyRewardDataController.Page; set => this.dailyRewardDataController.Page = value; }
        private          FeatureButtonModel            claimFeatureButtonModel;
        private          DummyDailyRewardView          View => (DummyDailyRewardView)base.View;

        public DummyDailyRewardPresenter(LoadImageHelper loadImageHelper, DailyRewardService dailyRewardService, SignalBus signalBus,
            DailyRewardDataController dailyRewardDataController, DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint,
            FeatureDailyRewardBlueprint featureDailyRewardBlueprint,
            ScreenManager screenManager, SceneDirector sceneDirector, DiContainer diContainer) : base(
            dailyRewardService, signalBus, screenManager, sceneDirector)
        {
            this.loadImageHelper               = loadImageHelper;
            this.dailyRewardDataController     = dailyRewardDataController;
            this.dailyRewardMiscParamBlueprint = dailyRewardMiscParamBlueprint;
            this.featureDailyRewardBlueprint   = featureDailyRewardBlueprint;
            this.diContainer                   = diContainer;
        }

        public override async UniTask BindData()
        {
            base.BindData();

            // ReSharper disable once PossibleLossOfFraction
            if (this.dailyRewardService.IsNewDay())
                this.Page = Mathf.FloorToInt(this.dailyRewardDataController.Today / this.dailyRewardMiscParamBlueprint.TimeLoop);

            this.claimFeatureButtonModel = new FeatureButtonModel
            {
                ButtonName      = "btn_claim",
                ScreenPresenter = this,
                ScreenViewName  = this.View.name,
                ButtonStatus    = ButtonStatus.On
            };

            await this.Reload();

            this.View.claimButton.InitButtonEvent(_ => this.ClaimButtonClick(), this.claimFeatureButtonModel);

            this.View.exitButton.InitButtonEvent(_ => this.ClosePopup(), new FeatureButtonModel()
            {
                ButtonName      = "btn_exit",
                ScreenPresenter = this,
                ScreenViewName  = this.View.name,
                ButtonStatus    = ButtonStatus.On
            });

            this.CheckButtonStatus();
        }

        private async UniTask Reload()
        {
            // Calculate the current page start based on today's day and the TimeLoop.
            // For example, if today is day 9 and TimeLoop is 7, the page should show days 8-14.
            int startDay = Mathf.FloorToInt((this.dailyRewardDataController.Today - 1) / this.dailyRewardMiscParamBlueprint.TimeLoop)
                * this.dailyRewardMiscParamBlueprint.TimeLoop + 1;

            // Ensure we're showing a valid range of days, adjusting the Page property if necessary.
            this.Page = (this.dailyRewardDataController.Today - 1) / this.dailyRewardMiscParamBlueprint.TimeLoop;

            for (var index = 0; index < this.View.adapters.Count; index++)
            {
                var item = this.View.adapters[index];

                // Calculate the actual DayIndex for this slot (within the current page).
                item.DayIndex = startDay + index;

                // Skip if the calculated DayIndex is 0 (this won't happen as long as Today >= 1).
                if (item.DayIndex == 0)
                {
                    continue; // Skip invalid day.
                }

                item.InitButton(_ => this.UnlockReward());

                // Lock the reward if the DayIndex is greater than today's day.
                item.IsLocked = item.DayIndex > this.dailyRewardDataController.Today;

                // Calculate which reward to display using modulo for looping.
                var rewardDayIndex = (item.DayIndex - 1) % this.dailyRewardMiscParamBlueprint.TimeLoop + 1;

                // Set lock icon based on the current day and blueprint data.
                if (this.featureDailyRewardBlueprint.Count >= rewardDayIndex)
                {
                    item.SetLockIcon(item.DayIndex == this.dailyRewardDataController.Today + 1 &&
                                     this.featureDailyRewardBlueprint[rewardDayIndex.ToString()].ShowAdsNextDay);
                }
                else
                {
                    item.SetLockIcon(item.DayIndex == this.dailyRewardDataController.Today + 1 &&
                                     this.featureDailyRewardBlueprint[(rewardDayIndex).ToString()].ShowAdsNextDay);
                }

                // Get the rewards for the current day (actual day, but load based on looping).
                var iconList = await this.GetRewardsAt(index + 1).Select(async x =>
                    new RewardSlotModel()
                    {
                        Sprite            = await this.loadImageHelper.LoadLocalSprite(x.IconPath),
                        RewardSlotAdapter = this.View.adapters[index]
                    }
                ).ToList();

                await this.View.adapters[index].InitItemAdapter(iconList.ToList(), this.diContainer);
            }
        }
        
        private async void UnlockReward()
        {
            this.LogMessage("Unlock next day reward", Color.green);
            this.dailyRewardService.UnlockNextDayReward();
            await this.Reload();
            this.CheckButtonStatus();
        }

        private void CheckButtonStatus()
        {
            this.claimFeatureButtonModel.ButtonStatus =
                this.View.adapters.Any(item => !item.IsLocked && !this.dailyRewardDataController.IsClaimed(item.DayIndex)) ? ButtonStatus.On : ButtonStatus.Off;
            
            foreach(var item in this.View.adapters.Where(x => this.dailyRewardDataController.IsClaimed(x.DayIndex)))
            {
                item.UpdateClaim();
            }

            this.View.claimButton.RefreshButtonStatus();
        }

        private void ClaimButtonClick()
        {
            foreach (var item in this.View.adapters.Where(item => !item.IsLocked && !this.dailyRewardDataController.IsClaimed(item.DayIndex)))
            {
                this.ClaimReward(item.DayIndex, item.gameObject);
            }

            this.CheckButtonStatus();
        }

        private async void ClosePopup() { await this.CloseViewAsync(); }

        protected override void OnClaimReward(RewardClaimSignal signal) { this.LogMessage("presenter receive claimed signal at day " + signal.Day, Color.green); }
    }
}