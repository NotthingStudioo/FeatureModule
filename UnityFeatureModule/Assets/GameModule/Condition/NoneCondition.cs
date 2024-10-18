namespace GameModule.Condition
{
    public class NoneCondition : ICondition
    {
        public string ConditionId         => "";
        public bool   IsMet(string param) { return true; }

        public float GetProgress(IConditionRecord conditionRecord) { return 1; }
    }
}