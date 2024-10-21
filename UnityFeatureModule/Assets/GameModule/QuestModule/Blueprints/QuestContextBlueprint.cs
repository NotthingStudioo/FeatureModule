namespace GameModule.QuestModule.Blueprints
{
    using BlueprintFlow.BlueprintReader;

    [BlueprintReader("QuestContext")]
    public class QuestContextBlueprint : GenericBlueprintReaderByRow<string, QuestContextRecord>
    {
    }

    [CsvHeaderKey("Id")]
    public class QuestContextRecord
    {
        public string Id;

        public string QuestContextType;
        public string Data;
    }
}