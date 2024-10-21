using System;
using GameFoundation.Scripts.AssetLibrary;
using GameFoundation.Scripts.UIModule.MVP;
using GameFoundation.Scripts.UIModule.Utilities.LoadImage;
using TMPro;
using UnityEngine.UI;

public class AskNewQuestItemModel
{
    public string QuestName;
    public string QuestDescription;
    public string QuestReward;
    public string QuestRewardValue;
    public string QuestIcon;
    public Action OnAccept, OnDecline;
}

public class AskNewQuestItemView : TViewMono
{
    public TextMeshProUGUI txtQuestName;
    public TextMeshProUGUI txtQuestDescription;
    public TextMeshProUGUI txtQuestReward;
    public TextMeshProUGUI txtQuestRewardValue;
    public Button          btnAccept, btnDecline;
    public Image           imgQuestIcon,imgRewardIcon;
}

public class AskNewQuestItemPresenter : BaseUIItemPresenter<AskNewQuestItemView, AskNewQuestItemModel>
{
    private readonly LoadImageHelper loadImageHelper;
    public AskNewQuestItemPresenter(IGameAssets gameAssets, LoadImageHelper loadImageHelper) : base(gameAssets) { this.loadImageHelper = loadImageHelper; }

    public override async void BindData(AskNewQuestItemModel param)
    {
        this.View.btnAccept.onClick.RemoveAllListeners();
        this.View.btnDecline.onClick.RemoveAllListeners();

        this.View.btnAccept.onClick.AddListener(() => param.OnAccept?.Invoke());
        this.View.btnDecline.onClick.AddListener(() => param.OnDecline?.Invoke());

        this.View.txtQuestName.text        = param.QuestName;
        this.View.txtQuestDescription.text = param.QuestDescription;
        this.View.txtQuestReward.text      = param.QuestReward;
        this.View.txtQuestRewardValue.text = param.QuestRewardValue;

        if (param.QuestIcon != null)
        {
            // this.View.imgQuestIcon.sprite  = await this.loadImageHelper.LoadLocalSprite(param.QuestIcon);
            // this.View.imgRewardIcon.sprite = await this.loadImageHelper.LoadLocalSprite(param.QuestReward);
        }
    }
}