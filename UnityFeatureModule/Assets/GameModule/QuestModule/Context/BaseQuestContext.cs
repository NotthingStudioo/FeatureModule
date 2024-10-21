namespace QuestModule.Context
{
    using GameModule.QuestModule.Blueprints;

    public interface IQuestContext
    {
        string ContextType { get; }
        void   SetupContext(QuestContextRecord data);
    }

    public abstract class BaseQuestContext : IQuestContext
    {
        public abstract string ContextType { get; }

        public virtual void SetupContext(QuestContextRecord data) { }
    }
}