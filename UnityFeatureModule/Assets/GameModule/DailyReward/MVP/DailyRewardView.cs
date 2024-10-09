namespace DailyReward.GameModule.DailyReward.MVP
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.MVP;
    using FeatureTemplate.Scripts.Services;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using global::DailyReward.GameModule.DailyReward.Data;
    using global::DailyReward.GameModule.DailyReward.Scripts;
    using global::DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem;
    using global::DailyReward.GameModule.DailyReward.Signals;
    using UnityEngine;
    using Zenject;
    using Color = UnityEngine.Color;

    public class DailyRewardView : FeatureBasePopupViewTemplate
    {
        [SerializeField] public List<RewardSlotAdapter> adapters;

        public FeatureButtonView claimButton;
        public FeatureButtonView exitButton;
    }

    [PopupInfo(nameof(DailyRewardView))]
    public class DailyRewardPresenter : FeatureBasePopupPresenterTemplate<DailyRewardView>
    {
        private readonly   DailyRewardDataController     dailyRewardDataController;
        private readonly   DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint;
        private readonly   FeatureDailyRewardBlueprint   featureDailyRewardBlueprint;
        private readonly   DiContainer                   diContainer;
        protected readonly DailyRewardService            DailyRewardService;

        private FeatureButtonModel claimFeatureButtonModel;

        public DailyRewardPresenter(DailyRewardService dailyRewardService, SignalBus signalBus,
            DailyRewardDataController dailyRewardDataController, DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint,
            FeatureDailyRewardBlueprint featureDailyRewardBlueprint,
            ScreenManager screenManager, SceneDirector sceneDirector, DiContainer diContainer) : base(signalBus, screenManager, sceneDirector)
        {
            this.DailyRewardService            = dailyRewardService;
            this.dailyRewardDataController     = dailyRewardDataController;
            this.dailyRewardMiscParamBlueprint = dailyRewardMiscParamBlueprint;
            this.featureDailyRewardBlueprint   = featureDailyRewardBlueprint;
            this.diContainer                   = diContainer;
        }

        public override async UniTask BindData()
        {
            this.SignalBus.Subscribe<RewardClaimSignal>(this.OnClaimReward);

            // ReSharper disable once PossibleLossOfFraction
            if (this.DailyRewardService.IsNewDay())
                this.dailyRewardDataController.Page = Mathf.FloorToInt(this.dailyRewardDataController.Today / this.dailyRewardMiscParamBlueprint.TimeLoop);

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

        /// <summary>
        /// Claim today reward
        /// </summary>
        /// <returns></returns>
        protected void ClaimReward(int day, GameObject source) { this.DailyRewardService.ClaimReward(day, source); }

        private UniTask Reload()
        {
            // Calculate the current page start based on today's day and the TimeLoop.
            // For example, if today is day 9 and TimeLoop is 7, the page should show days 8-14.
            var startDay = Mathf.FloorToInt((this.dailyRewardDataController.Today - 1) / this.dailyRewardMiscParamBlueprint.TimeLoop)
                * this.dailyRewardMiscParamBlueprint.TimeLoop + 1;

            // Ensure we're showing a valid range of days, adjusting the Page property if necessary.
            this.dailyRewardDataController.Page = (this.dailyRewardDataController.Today - 1) / this.dailyRewardMiscParamBlueprint.TimeLoop;

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

                var iconList = this.GetRewardsAt(index + 1).Select(x =>
                    new RewardSlotModel()
                    {
                        Reward = x
                    }
                ).ToList();

                this.View.adapters[index].InitItemAdapter(iconList, this.diContainer).Forget();
            }
            
            return UniTask.CompletedTask;
        }

        private async void UnlockReward()
        {
            this.LogMessage("Unlock next day reward", Color.green);
            this.DailyRewardService.UnlockNextDayReward();
            await this.Reload();
            this.CheckButtonStatus();
        }

        private void CheckButtonStatus()
        {
            this.claimFeatureButtonModel.ButtonStatus =
                this.View.adapters.Any(item => !item.IsLocked && !this.dailyRewardDataController.IsClaimed(item.DayIndex)) ? ButtonStatus.On : ButtonStatus.Off;

            foreach (var item in this.View.adapters.Where(x => this.dailyRewardDataController.IsClaimed(x.DayIndex)))
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

        private void ClosePopup() { this.CloseViewAsync().Forget(); }

        /// <summary>
        /// Read the reward at a specific day.
        /// </summary>
        /// <param name="dayOffset">The offset to that day calculate since today.</param>
        /// <returns></returns>
        protected List<Reward> GetRewardsAt(int dayOffset) { return this.DailyRewardService.ReadRewardsAtDay(dayOffset); }

        protected void OnClaimReward(RewardClaimSignal signal) { this.LogMessage("presenter receive claimed signal at day " + signal.Day, Color.green); }

        public override void Dispose()
        {
            this.SignalBus.Unsubscribe<RewardClaimSignal>(this.OnClaimReward);
            base.Dispose();
        }
    }
}