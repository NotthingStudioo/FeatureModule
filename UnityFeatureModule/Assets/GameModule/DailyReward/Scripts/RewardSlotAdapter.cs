namespace DailyReward.GameModule.DailyReward.Scripts
{
    using System;
    using FeatureTemplate.Scripts.MonoUltils;
    using GameFoundation.Scripts.UIModule.Adapter;
    using global::DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem;
    using UnityEngine;
    using UnityEngine.UI;
#if DAILY_REWARD
    public class RewardSlotAdapter : BasicGridAdapter<RewardSlotModel, RewardSlotView, RewardSlotPresenter>
    {
        [SerializeField] private FeatureButtonView featureButtonView;
        [SerializeField] private Image             done;
        [SerializeField] private Image             notOpenYet;

        private bool isLocked = false;

        public bool IsLocked
        {
            get => this.isLocked;
            set
            {
                this.isLocked = value;
                this.notOpenYet.enabled = this.isLocked;
                this.featureButtonView.enabled = !value;
            }
        }

        public int DayIndex { get; set; }

        public void SetLockIcon(bool enable) { this.featureButtonView.gameObject.SetActive(enable); }

        public void InitButton(Action<FeatureButtonModel> action)
        {
            this.featureButtonView.InitButtonEvent(action, new FeatureButtonModel()
            {
                ButtonName = "unlock_tomorrow_reward",
                ButtonStatus = ButtonStatus.On
            });

            this.done.enabled = false;
            this.notOpenYet.enabled = false;
        }

        public void UpdateClaim()
        {
            this.done.enabled = true;
        }
    }
#endif
}