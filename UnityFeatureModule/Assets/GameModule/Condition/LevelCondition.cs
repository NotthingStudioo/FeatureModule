namespace GameModule.GameModule.Condition
{
    using FeatureTemplate.Scripts.Models.Controllers;
    using global::GameModule.GameModule.Mission.Blueprints;
    using UnityEngine;
    using Zenject;

    public class LevelCondition : ICondition
    {
        public string ConditionId => "Level";

        [Inject] private FeatureLevelDataControllerData featureLevelDataControllerData;

        //NOTE: param format [targetLevel]
        public bool IsMet(string param)
        {
            var targetLevel  = int.Parse(param);
            var currentLevel = this.featureLevelDataControllerData.CurrentLevel;

            return currentLevel >= targetLevel;
        }

        public float GetProgress(IConditionRecord conditionRecord)
        {
            var targetLevel  = int.Parse(conditionRecord.ConditionParam);
            var currentLevel = this.featureLevelDataControllerData.CurrentLevel;

            return Mathf.Clamp(currentLevel / targetLevel, 0, 1);
        }
    }
}