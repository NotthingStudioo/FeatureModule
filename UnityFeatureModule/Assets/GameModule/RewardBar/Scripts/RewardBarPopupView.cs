namespace RewardBar.GameModule.RewardBar.Scripts
{
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using FeatureTemplate.Scripts.MonoUltils;
    using FeatureTemplate.Scripts.MVP;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.Utilities.LogService;
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
        public GameObject rewardPanel;
    }

    [PopupInfo(nameof(RewardBarPopupView))]
    public class RewardBarPopupPresenter : FeatureBasePopupScreenPresenterTemplate<RewardBarPopupView, RewardBarPopupModel>, ITickable
    {
        private readonly TickableManager tickableManager;
        private const    float           CursorSpeed   = 1000f;
        private          bool            isMovingRight = true;
        private          bool            isStopped;

        // Positions cursor mark
        private readonly float[] rewardPositions = { -300f, -200f, -100f, 0f, 100f, 200f, 300f };

        // Multipliers for mark
        private readonly int[] rewardMultipliers = { 2, 3, 4, 5, 4, 3, 2 };

        public RewardBarPopupPresenter(SignalBus signalBus, ScreenManager screenManager,
            SceneDirector sceneDirector, ILogService logger, TickableManager tickableManager)
            : base(signalBus, screenManager, sceneDirector, logger)
        {
            this.tickableManager = tickableManager;
        }

        public override UniTask BindData(RewardBarPopupModel popupModel)
        {
            //Check the mark
            if (this.rewardPositions.Length != this.rewardMultipliers.Length)
            {
                Debug.LogError("rewardPositions != rewardMultipliers");
            }

            this.Effect();

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

            this.View.txtReward.text = "x" + this.rewardMultipliers[rewardIndex];
            this.Model.RewardOut     = this.Model.RewardIn * this.rewardMultipliers[rewardIndex];
        }

        private void OnClickGet(FeatureButtonModel obj)
        {
         this.StopCursor();
        }
        private async void StopCursor()
        {
            this.isStopped = true;
            this.UpdateMultiplierText();
            await Task.Delay(1000);
        }

        private void Effect()
        {
            this.View.rewardPanel.transform.DOScaleY(1, 1).SetEase(Ease.OutBack);
        }

        private void OnClickNoThank(FeatureButtonModel obj)
        {
            this.View.rewardPanel.transform.DOScaleY(0.01f, 0.2f).SetEase(Ease.OutBack).OnComplete(this.CloseView);
        }
    }
}