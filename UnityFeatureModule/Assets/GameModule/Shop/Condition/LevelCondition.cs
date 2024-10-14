namespace GameModule.Shop.Condition
{
    using Game.Scripts.Shop.Condition;
    using Zenject;
/// <summary>
/// This script is just an example for Condition, please ignore it if you don't need it.
/// </summary>
    public class LevelCondition : IShopCondition
    {
        public string Id => "Level";
        
        [Inject]
        // private GameProgressDataController gameProgressDataController;
        
        //NOTE: param format [targetLevel]
        public bool IsMet(string param)
        {
            var targetLevel  = int.Parse(param);
            // var currentLevel = int.Parse(this.gameProgressDataController.Data.CurrentLevel);
            // return currentLevel >= targetLevel;
            return true;
        }
    }
}