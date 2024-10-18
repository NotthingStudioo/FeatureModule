namespace GameModule.GameModule.DailyReward.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("DailyRewardMiscParam")]
    public class DailyRewardMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        /// <summary>
        /// Name of the presenter to show the popup
        /// </summary>
        public string StartOnScreen { get; set; }
        public int TimeLoop { get; set; }
    }
}