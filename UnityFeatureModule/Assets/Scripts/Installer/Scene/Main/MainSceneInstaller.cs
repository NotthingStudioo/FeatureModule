namespace Game.Scripts.Installer.Scene.Main
{
    using DailyReward.GameModule.DailyReward.Scripts;
    using Game.Scripts.Services;
    using Game.Scripts.StateMachine;

    public class MainSceneInstaller : BaseSceneInstallerTemplate
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.BindInterfacesAndSelfTo<MainScreenHandler>().AsCached().NonLazy();
            DailyRewardInstaller.Install(this.Container);
            GameStateMachineInstaller.Install(this.Container);
        }
    }
}