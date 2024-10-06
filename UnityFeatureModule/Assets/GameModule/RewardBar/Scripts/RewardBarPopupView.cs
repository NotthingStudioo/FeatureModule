namespace RewardBar.GameModule.RewardBar.Scripts
{
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.MVP;
    using FeatureTemplate.Scripts.Services.Ads;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
    using GameFoundation.Scripts.Utilities.ObjectPool;
    using TMPro;
    using UnityEngine;
    using Zenject;

    public class RewardBarPopupModel
    {
        public int RewardIn  { get; set; }
        public int RewardOut { get; set; }
    }

    public class RewardBarPopupView : FeatureBasePopupViewTemplate
    {
        public RectTransform     cursor;
        public RectTransform     rewardBar;
        public FeatureButtonView btnGet;
        public FeatureButtonView btnNoThank;
        public TextMeshProUGUI   txtReward;
        public GameObject        rewardPanel;
        public Transform         rewardDestination;
        public GameObject        rewardCoin;
    }

    [PopupInfo(nameof(RewardBarPopupView))]
    public class RewardBarPopupPresenter : FeatureBasePopupScreenPresenterTemplate<RewardBarPopupView, RewardBarPopupModel>, ITickable
    {
        private readonly TickableManager    tickableManager;
        private readonly FeatureAdsServices featureAdsServices;
        private const    float              CursorSpeed   = 1000f;
        private          bool               isMovingRight = true;
        private          bool               isStopped;
        private          int                rewardMultiplier;
        private          int                coinAmount;

        // Positions cursor mark
        private readonly float[] rewardPositions = { -300f, -200f, -100f, 0f, 100f, 200f, 300f };

        // Multipliers for mark
        private readonly int[] rewardMultipliers = { 2, 3, 4, 5, 4, 3, 2 };

        public RewardBarPopupPresenter(SignalBus signalBus, ScreenManager screenManager,
            SceneDirector sceneDirector, ILogService logger, TickableManager tickableManager,
            FeatureAdsServices featureAdsServices)
            : base(signalBus, screenManager, sceneDirector, logger)
        {
            this.tickableManager    = tickableManager;
            this.featureAdsServices = featureAdsServices;
        }

        public override UniTask BindData(RewardBarPopupModel popupModel)
        {
            this.isStopped = false;

            //Check the mark
            if (this.rewardPositions.Length != this.rewardMultipliers.Length)
            {
                Debug.LogError("rewardPositions != rewardMultipliers");
            }

            this.OpenViewFX();

            this.tickableManager.Add(this);

            var btnNoThank = new FeatureButtonModel()
            {
                ScreenViewName  = this.View.name,
                ScreenPresenter = this,
                ButtonStatus    = ButtonStatus.On
            };

            var btnGet = new FeatureButtonModel()
            {
                ScreenViewName  = this.View.name,
                ScreenPresenter = this,
                ButtonStatus    = ButtonStatus.On
            };

            this.View.btnNoThank.InitButtonEvent(this.OnClickNoThank, btnNoThank);
            this.View.btnGet.InitButtonEvent(this.OnClickGet, btnGet);

            return UniTask.CompletedTask;
        }

        public void Tick()
        {
            this.MoveCursor();
            this.UpdateMultiplierText();
        }

        public override void Dispose()
        {
            this.tickableManager.Remove(this);
            base.Dispose();
        }

        private void MoveCursor()
        {
            if (this.isStopped) return;

            this.isMovingRight = this.isMovingRight switch
            {
                true when this.View.cursor.anchoredPosition.x >= this.View.rewardBar.rect.width / 2 => false,
                false when this.View.cursor.anchoredPosition.x <= -this.View.rewardBar.rect.width / 2 => true,
                _ => this.isMovingRight
            };

            var direction = this.isMovingRight ? 1f : -1f;
            this.View.cursor.anchoredPosition += new Vector2(direction * CursorSpeed * Time.deltaTime, 0);
        }

        private void UpdateMultiplierText()
        {
            var closestPosition = float.MaxValue;
            var rewardIndex     = 0;

            for (var i = 0; i < this.rewardPositions.Length; i++)
            {
                var distance = Mathf.Abs(this.View.cursor.anchoredPosition.x - this.rewardPositions[i]);

                if (!(distance < closestPosition)) continue;
                closestPosition = distance;
                rewardIndex     = i;
            }

            this.View.txtReward.text = (this.Model.RewardIn * this.rewardMultipliers[rewardIndex]).ToString();
            this.rewardMultiplier    = this.rewardMultipliers[rewardIndex];
            this.coinAmount          = this.rewardMultipliers[rewardIndex] * 5;
        }

        private void OnClickGet(FeatureButtonModel obj) { this.StopCursor(); }

        private async void StopCursor()
        {
            if (this.isStopped) return;
            this.View.btnGet.transform.DOKill();
            this.isStopped = true;
            this.UpdateMultiplierText();
            await Task.Delay(1000);
            this.featureAdsServices.ShowRewardedAd("", () => { this.Model.RewardOut = this.rewardMultiplier * this.Model.RewardIn; });
            await this.SpawnAndMoveCoin();
            UniTask.WhenAll();
            this.CloseViewFX();
        }

        private async Task SpawnAndMoveCoin()
        {
            for (var i = 0; i < this.coinAmount; i++)
            {
                var randomPos = Random.insideUnitSphere * 0.5f;
                var position  = this.View.rewardPanel.transform.position + randomPos;
                var coin      = this.View.rewardCoin.Spawn(this.View.rewardPanel.transform.position);
                coin.transform.DOMove(position, 0.1f).SetEase(Ease.OutBounce);
                await UniTask.Delay(50);
                coin.transform.DOMove(this.View.rewardDestination.position, 1.5f).SetEase(Ease.InOutQuad).OnComplete(coin.Recycle);
            }
        }

        private async void OpenViewFX()
        {
            this.View.rewardPanel.transform.DOScaleY(1, 1).SetEase(Ease.OutBack);
            this.View.btnGet.transform.DOScale(1.2f, 1.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InCirc);
            await Task.Delay(2000);
            this.View.btnNoThank.gameObject.SetActive(true);
        }

        private void OnClickNoThank(FeatureButtonModel obj) { this.CloseViewFX(); }
        private void CloseViewFX()                          { this.View.rewardPanel.transform.DOScaleY(0.01f, 0.5f).SetEase(Ease.InBack).OnComplete(this.CloseView); }
    }
}