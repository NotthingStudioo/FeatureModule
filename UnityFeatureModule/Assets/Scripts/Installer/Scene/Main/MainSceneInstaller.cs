namespace Game.Scripts.Installer.Scene.Main
{
    using Game.Scripts.Services;
    using Game.Scripts.StateMachine;
    using GameModule.GameModule.Mission;

    public class MainSceneInstaller : BaseSceneInstallerTemplate
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.BindInterfacesAndSelfTo<MainScreenHandler>().AsCached().NonLazy();
            GameStateMachineInstaller.Install(this.Container);
            MissionInstaller.Install(this.Container);
        }
    }
}