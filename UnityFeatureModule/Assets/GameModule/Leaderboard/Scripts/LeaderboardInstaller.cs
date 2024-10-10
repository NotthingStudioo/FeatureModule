namespace GameModule.Leaderboard.Scripts
{
    using Zenject;

    public class LeaderboardInstaller : Installer<LeaderboardInstaller>
    {
        public override void InstallBindings()
        {
            this.SignalDeclaration();
            this.Container.BindInterfacesAndSelfTo<LeaderboardService>().AsCached().NonLazy();
#if LEADERBOARD_FIRESTORE
            this.Container.BindInterfacesAndSelfTo<FirebaseDAO>().AsCached().NonLazy();
#elif LEADERBOARD_PLAYFAB
            this.Container.BindInterfacesAndSelfTo<PlayfabDAO>().AsCached().NonLazy();
#elif LEADERBOARD_DATA
            this.Container.BindInterfacesAndSelfTo<DataDAO>().AsCached().NonLazy();
#endif
        }

        private void SignalDeclaration() { }
    }
}