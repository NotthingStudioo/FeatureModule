namespace GameModule.QuestModule.Signals
{
    public class TrackingQuestSignal
    {
        public string RequirementType  { get; }
        public string RequirementId    { get; }
        public int    RequirementValue { get; }

        public TrackingQuestSignal(string requirementType, string requirementId, int requirementValue)
        {
            this.RequirementType  = requirementType;
            this.RequirementId    = requirementId;
            this.RequirementValue = requirementValue;
        }
    }
}