namespace Game.Scripts.Scene.Home
{
    using Cysharp.Threading.Tasks;
    using Game.Scripts.MVP;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.Utilities.GameQueueAction;
    using GameFoundation.Scripts.Utilities.LogService;
    using Zenject;

    public class MainScreenView : BaseScreenViewTemplate
    {
    }

    [ScreenInfo(nameof(MainScreenView))]
    public class MainScreenPresenter : BaseScreenPresenterTemplate<MainScreenView>
    {
        public MainScreenPresenter(SignalBus signalBus, GameQueueActionContext gameQueueActionContext, ILogService logger, ScreenManager screenManager, SceneDirector sceneDirector) : base(signalBus,
            gameQueueActionContext, logger, screenManager, sceneDirector)
        {
        }

        public override UniTask BindData() { return UniTask.CompletedTask; }
    }
}