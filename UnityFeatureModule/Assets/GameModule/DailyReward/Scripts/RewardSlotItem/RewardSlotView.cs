namespace DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem
{
    using FeatureTemplate.Scripts.MVP;
    using GameFoundation.Scripts.AssetLibrary;
    using global::DailyReward.GameModule.DailyReward.Data;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class RewardSlotModel
    {
        public Sprite Sprite;
    }
    public class RewardSlotView : FeatureBaseItemViewTemplate
    {
        public Image image;
    }

    public class RewardSlotPresenter : FeatureBaseItemPresenterTemplate<RewardSlotView,RewardSlotModel>
    {
        private readonly DailyRewardDataController dailyRewardDataController;
        private RewardSlotModel model;

        public RewardSlotPresenter(SignalBus signalBus, IGameAssets gameAssets, DailyRewardDataController dailyRewardDataController) : base(signalBus, gameAssets)
        {
            this.dailyRewardDataController = dailyRewardDataController;
        }

        public override void BindData(RewardSlotModel param)
        {
            this.model = param;
            this.View.image.sprite = param.Sprite;
            base.BindData(param);
        }
    }
}