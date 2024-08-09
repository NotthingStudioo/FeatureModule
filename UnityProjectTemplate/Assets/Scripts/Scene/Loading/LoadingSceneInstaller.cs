namespace Game.Scripts.Scene.Loading
{
    using Game.Scripts.Installer.Scene;
    using GameFoundation.Scripts.UIModule.Utilities;

    public class LoadingSceneInstaller : BaseSceneInstallerTemplate
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            this.Container.InitScreenManually<LoadingScreenPresenter>();
        }
    }
}