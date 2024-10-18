namespace GameModule.Condition
{
    public interface IConditionRecord
    {
        string ConditionId    { get; set; }
        string ConditionType  { get; set; }
        string ConditionParam { get; set; }
    }
}