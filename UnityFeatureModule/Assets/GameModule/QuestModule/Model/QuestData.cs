namespace GameModule.QuestModule.Model
{
    using System;
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using GameFoundation.Scripts.Interfaces;
    using global::Blueprints;
    using Newtonsoft.Json;

    public class QuestJournal : IFeatureLocalData, ILocalData
    {
        public Dictionary<string, QuestLog>                Quests         = new();
        public Dictionary<string, Dictionary<string, int>> TrackingCached = new();

        public Type                         ControllerType => typeof(QuestManager);

        public void Init() { }
    }

    public class QuestLog
    {
        public              string            ProviderId        { get; set; }
        public              string            QuestId           { get; set; }
        public              QuestProviderType QuestProviderType { get; set; }
        public              QuestStatus       QuestStatus       { get; set; }
        public              string            QuestType         { get; set; }
        public              List<TaskLog>     TaskProgress      { get; set; } = new();
        [JsonIgnore] public QuestRecord       QuestRecord       { get; set; }
    }

    public class TaskLog
    {
        public List<RequirementProgress> Progress   { get; set; } = new();
        public QuestStatus               TaskStatus { get; set; }

        [JsonIgnore] public TaskRecord TaskRecord { get; set; }
    }

    public class RequirementProgress
    {
        public string RequirementType { get; set; }
        public string RequirementId   { get; set; }
        public int    CurrentValue    { get; set; }
        public int    RequiredValue   { get; set; }
    }

    public enum QuestProviderType
    {
        Main,
        Side,
        Achievement
    }
}