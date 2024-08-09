namespace Game.Scripts.StateMachine
{
    using FeatureTemplate.Scripts.StateMachine.Interface;
    using FeatureTemplate.Scripts.StateMachine.Signals;
    using Zenject;

    public class GameStateMachineInstaller:Installer<GameStateMachineInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.DeclareSignal<FeatureOnStateEnterSignal>();
            this.Container.DeclareSignal<FeatureOnStateExitSignal>();
            
            this.Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle().NonLazy();
            this.Container.Bind<IFeatureGameState>().To(convention => convention.AllNonAbstractClasses().DerivingFrom<IFeatureGameState>()).WhenInjectedInto<GameStateMachine>();
        }
    }
}