namespace GameModule.DailyReward.Scripts
{
    using System;
    using FeatureTemplate.Scripts.MonoUltils;
    using GameFoundation.Scripts.UIModule.Adapter;
    using GameModule.DailyReward.Scripts.RewardSlotItem;
    using UnityEngine;
    using UnityEngine.UI;

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
                this.isLocked           = value;
                this.notOpenYet.enabled = this.isLocked;

                if (this.featureButtonView)
                    this.featureButtonView.enabled = !value;
            }
        }

        public int DayIndex { get; set; }

        public void SetLockIcon(bool enable)
        {
            if (this.featureButtonView)
                this.featureButtonView.gameObject.SetActive(enable);
        }

        public void InitButton(Action<FeatureButtonModel> action)
        {
            if (this.featureButtonView)
                this.featureButtonView.InitButtonEvent(action, new FeatureButtonModel()
                {
                    ButtonName   = "unlock_tomorrow_reward",
                    ButtonStatus = ButtonStatus.On
                });

            this.done.enabled       = false;
            this.notOpenYet.enabled = false;
        }

        public void UpdateClaim(bool enable) { this.done.enabled = enable; }
    }
}