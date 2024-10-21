namespace GameModule.QuestModule.NotificationBag
{
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.MVP;
    using UnityEngine;

    public class NotificationBagItemModel
    {
        public bool HasNotice { get; set; }
    }

    public class NotificationBagItemView : TViewMono
    {
        public GameObject objIcon;
    }

    public class NotificationBagItemPresenter : BaseUIItemPresenter<NotificationBagItemView, NotificationBagItemModel>
    {
        public NotificationBagItemPresenter(IGameAssets gameAssets) : base(gameAssets) { }

        public override void BindData(NotificationBagItemModel param) { this.View.objIcon.gameObject.SetActive(param.HasNotice); }
    }
}