namespace DailyReward.GameModule.DailyReward.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("DailyRewardMiscParam")]
    public class DailyRewardMiscParamBlueprint : GenericBlueprintReaderByCol
    {
        public bool PopupOnBegin { get; set; }
        public int TimeLoop { get; set; }
    }
}