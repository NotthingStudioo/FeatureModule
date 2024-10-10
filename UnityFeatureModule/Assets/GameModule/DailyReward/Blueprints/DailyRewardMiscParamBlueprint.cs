namespace DailyReward.GameModule.DailyReward.Blueprints
{
    using BlueprintFlow.BlueprintReader;
#if DAILY_REWARD
    [BlueprintReader("DailyRewardMiscParam")]
    public class DailyRewardMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        /// <summary>
        /// Name of the presenter to show the popup
        /// </summary>
        public string StartOnScreen { get; set; }
        public int TimeLoop { get; set; }
    }
#endif
}