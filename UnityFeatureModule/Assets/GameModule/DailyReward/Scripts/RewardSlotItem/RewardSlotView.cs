namespace DailyReward.GameModule.DailyReward.Scripts.RewardSlotItem
{
    using FeatureTemplate.Scripts.MVP;
    using GameFoundation.Scripts.AssetLibrary;
    using UnityEngine;
    using UnityEngine.UI;
    using Zenject;

    public class RewardSlotModel
    {
        public Sprite            Sprite;
        public RewardSlotAdapter RewardSlotAdapter;
    }

    public class RewardSlotView : FeatureBaseItemViewTemplate
    {
        public Image image;
    }

    public class RewardSlotPresenter : FeatureBaseItemPresenterTemplate<RewardSlotView, RewardSlotModel>
    {
        private RewardSlotModel model;

        public RewardSlotPresenter(SignalBus signalBus, IGameAssets gameAssets) : base(signalBus, gameAssets) { }

        public override void BindData(RewardSlotModel param)
        {
            this.model             = param;
            this.View.image.sprite = param.Sprite;
            base.BindData(param);
        }
    }
}