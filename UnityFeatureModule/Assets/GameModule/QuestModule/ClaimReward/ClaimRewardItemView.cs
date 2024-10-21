namespace GameModule.QuestModule.ClaimReward
{
    using System;
    using FeatureTemplate.Scripts.RewardHandle;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
    using TMPro;
    using UnityEngine.UI;

    public class ClaimRewardItemModel
    {
        public IRewardRecord                Asset      { get; set; }
        public Action<ClaimRewardItemModel> OnShowInfo { get; set; }
    }

    public class ClaimRewardItemView : TViewMono
    {
        public Button          btnShowInfo;
        public Image           assetIcon;
        public TextMeshProUGUI assetValue;
    }

    public class ClaimRewardItemPresenter : BaseUIItemPresenter<ClaimRewardItemView, ClaimRewardItemModel>
    {
        private readonly LoadImageHelper loadImageHelper;
        public ClaimRewardItemPresenter(IGameAssets gameAssets, LoadImageHelper loadImageHelper) : base(gameAssets) { this.loadImageHelper = loadImageHelper; }

        public override void BindData(ClaimRewardItemModel param)
        {
            this.LoadIcon(param.Asset.RewardId);
            this.View.assetValue.text = param.Asset.RewardValue.ToString();
            this.View.btnShowInfo.onClick.RemoveAllListeners();

            this.View.btnShowInfo.onClick.AddListener(() => { param.OnShowInfo?.Invoke(param); });
        }

        private async void LoadIcon(string id) { this.View.assetIcon.sprite = await this.loadImageHelper.LoadLocalSprite(id); }
    }
}