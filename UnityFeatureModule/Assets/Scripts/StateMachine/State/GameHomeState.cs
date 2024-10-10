namespace Game.Scripts.StateMachine.State
{
    using FeatureTemplate.Scripts.Services;
    using FeatureTemplate.Scripts.StateMachine.Interface;
    using GameFoundation.Scripts.AssetLibrary;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameModule.Leaderboard.Scripts.MVP;
    using Zenject;

    public class GameHomeState : IFeatureGameState
    {
        private readonly ScreenManager screenManager;
        private readonly DiContainer   diContainer;

        public GameHomeState(ScreenManager screenManager, DiContainer diContainer)
        {
            this.screenManager = screenManager;
            this.diContainer   = diContainer;
        }
        public void Enter() { this.START(); }

        public void Exit() { }

        private async void START()
        {
            await this.screenManager.OpenScreen<LeaderboardPopupPresenter>();
        }
    }
}