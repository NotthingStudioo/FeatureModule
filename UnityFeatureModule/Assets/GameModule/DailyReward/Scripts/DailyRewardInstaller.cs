namespace DailyReward.GameModule.DailyReward.Scripts
{
    using FeatureTemplate.Scripts.Signals;
    using Zenject;

    public class DailyRewardInstaller : Installer<DailyRewardInstaller>
    {
        public override void InstallBindings()
        {
            this.SignalDeclaration();
            this.Container.BindInterfacesAndSelfTo<DailyRewardService>().AsCached().NonLazy();
        }

        private void SignalDeclaration()
        {
            this.Container.DeclareSignal<RewardClaimSignal>();
        }
    }
}