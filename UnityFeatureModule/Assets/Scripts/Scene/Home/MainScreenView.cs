namespace Game.Scripts.Scene.Home
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.MonoUltils;
    using Game.Scripts.MVP;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using GameFoundation.Scripts.Utilities.LogService;
    using RewardBar.GameModule.RewardBar.Scripts;
    using Zenject;

    public class MainScreenView : BaseScreenViewTemplate
    {
        public FeatureButtonView btnShowRewardBar;
    }

    [ScreenInfo(nameof(MainScreenView))]
    public class MainScreenPresenter : BaseScreenPresenterTemplate<MainScreenView>
    {
        public MainScreenPresenter(SignalBus signalBus, GameQueueActionContext gameQueueActionContext, ILogService logger, ScreenManager screenManager, SceneDirector sceneDirector) : base(signalBus,
            gameQueueActionContext, logger, screenManager, sceneDirector)
        {
        }

        private int                rewardIn;
        private int                rewardOut;
        private FeatureButtonModel btnShowRewardBar;

        public override UniTask BindData()
        {
            this.btnShowRewardBar = new FeatureButtonModel()
            {
                ButtonName      = "Show Reward Bar",
                ScreenPresenter = this,
                ScreenViewName  = this.View.name,
            };

            this.View.btnShowRewardBar.InitButtonEvent(this.ShowRewardBar, this.btnShowRewardBar);

            return UniTask.CompletedTask;
        }

        private void ShowRewardBar(FeatureButtonModel obj)
        {
            this.rewardIn = 10;

            this.ScreenManager.OpenScreen<RewardBarPopupPresenter, RewardBarPopupModel>(new RewardBarPopupModel()
            {
                RewardIn = this.rewardIn,
            }).Forget();
        }
    }
}