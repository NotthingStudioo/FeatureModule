namespace GameModule.GameModule.Condition
{
    using System.Collections.Generic;
    using System.Linq;
    using global::GameModule.GameModule.Mission.Blueprints;

    public class ConditionHandler
    {
        private readonly List<ICondition> conditions;

        public ConditionHandler(List<ICondition> conditions) { this.conditions = conditions; }
        
        public bool CheckCondition(IConditionRecord conditionRecord)
        {
            return this.conditions.Where(condition => condition.ConditionId == conditionRecord.ConditionId).Any(condition => condition.IsMet(conditionRecord.ConditionParam));
        }

        public bool CheckConditions(List<IConditionRecord> conditionRecords)
        {
            return conditionRecords.All(this.CheckCondition);
        }
        
        public float GetProgress(IConditionRecord conditionRecord)
        {
            return this.conditions.Where(condition => condition.ConditionId == conditionRecord.ConditionId).Select(condition => condition.GetProgress(conditionRecord)).FirstOrDefault();
        }
    }
}