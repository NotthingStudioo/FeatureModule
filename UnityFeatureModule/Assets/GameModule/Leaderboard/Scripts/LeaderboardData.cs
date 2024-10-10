namespace GameModule.Leaderboard.Scripts
{
    using System;
    using Sirenix.OdinInspector;

    [Serializable]
    public class LeaderboardData
    {
        [TableColumnWidth(150)] public string columnName;

        [TableColumnWidth(100)] public string dataType; // e.g., string, int, float
        
        [TableColumnWidth(100)] public string defaultValue;
    }
}