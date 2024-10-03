namespace Game.Scripts.StateMachine.State
{
    using DailyReward.GameModule.DailyReward.MVP;
    using FeatureTemplate.Scripts.StateMachine.Interface;
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;

    public class GameHomeState : IFeatureGameState
    {
        private readonly ScreenManager screenManager;

        public GameHomeState(ScreenManager screenManager) { this.screenManager = screenManager; }
        public void Enter() { this.OpenDailyRewardPopup(); }

        private void OpenDailyRewardPopup()
        {
            
        }

        public void Exit() { }
    }
}