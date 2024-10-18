namespace GameModule.Condition
{
    public interface ICondition
    {
        string ConditionId { get; }
        bool   IsMet(string param);

        /// <summary>
        /// Return the progress of the condition
        /// </summary>
        /// <returns>Make sure the return between 0 and 1</returns>
        float GetProgress(IConditionRecord conditionRecord);
    }
}