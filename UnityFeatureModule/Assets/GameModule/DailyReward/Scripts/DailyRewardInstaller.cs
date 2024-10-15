namespace DailyReward.GameModule.DailyReward.Scripts
{
    using System.Drawing.Text;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Services;
    using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Signals;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using global::DailyReward.GameModule.DailyReward.MVP;
    using global::DailyReward.GameModule.DailyReward.Signals;
    using Zenject;

    // Input T as popup screen
    public class DailyRewardInstaller<T> : Installer<DailyRewardInstaller<T>> where T : IScreenPresenter
    {
        private SignalBus     signalBus;
        private ScreenManager screenManager;
        private DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint;
        private FeatureDataState featureDataState;
        private DailyRewardService dailyRewardService;
        public override void InstallBindings()
        {
            this.SignalDeclaration();
            this.Container.BindInterfacesAndSelfTo<DailyRewardService>().AsCached().NonLazy();
            this.signalBus = this.Container.Resolve<SignalBus>();
            this.screenManager = this.Container.Resolve<ScreenManager>();
            this.dailyRewardMiscParamBlueprint = this.Container.Resolve<DailyRewardMiscParamBlueprint>();
            this.featureDataState = this.Container.Resolve<FeatureDataState>();
            this.dailyRewardService = this.Container.Resolve<DailyRewardService>();
            this.AutoOpenScreen();
        }

        private void AutoOpenScreen()
        {
            this.signalBus.Subscribe<ScreenShowSignal>(this.OnScreenShowSignal);
        }

        protected async   void OnScreenShowSignal(ScreenShowSignal screenShowSignal)
        {
            await UniTask.WaitUntil(() => this.featureDataState.IsBlueprintAndLocalDataLoaded);
            if (string.IsNullOrEmpty(this.dailyRewardMiscParamBlueprint.StartOnScreen)||!this.featureDataState.IsBlueprintAndLocalDataLoaded)
            {
                return;
            }

            if (screenShowSignal.ScreenPresenter.GetType().Name != this.dailyRewardMiscParamBlueprint.StartOnScreen) return;

            if (!this.dailyRewardService.IsNewDay()) return;
            this.screenManager.OpenScreen<DailyRewardPresenter>().Forget();
        }

        private void SignalDeclaration()
        {
            this.Container.DeclareSignal<RewardClaimSignal>();
        }
    }
}