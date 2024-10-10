namespace Game.Scripts.Installer.Scene.Main
{
#if DAILY_REWARD
    using DailyReward.GameModule.DailyReward.Scripts;
#endif
    using Game.Scripts.Services;
    using Game.Scripts.StateMachine;

    public class MainSceneInstaller : BaseSceneInstallerTemplate
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.BindInterfacesAndSelfTo<MainScreenHandler>().AsCached().NonLazy();
            GameStateMachineInstaller.Install(this.Container);
        }
    }
}