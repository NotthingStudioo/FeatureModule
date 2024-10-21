namespace Blueprints
{
    using System;
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("Achievement")]
    public class AchievementBlueprint : GenericBlueprintReaderByRow<string, AchievementRecord>
    {
    }

    [CsvHeaderKey("Id")]
    [Serializable]
    public class AchievementRecord : QuestRecord
    {
    }
}