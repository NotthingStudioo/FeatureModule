using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;
using GameModule.Leaderboard.Scripts;

[CreateAssetMenu(fileName = "LeaderboardDataConfig", menuName = "ScriptableObjects/LeaderboardDataConfig")]
public class LeaderboardDataConfig : ScriptableObject
{
    [TabGroup("Firebase", "Firestore")] 
    [TableList] 
    [LabelText("Firestore Table Schema")]
    public List<LeaderboardData> leaderboardData = new List<LeaderboardData>()
    {
        new LeaderboardData() { columnName = "Name", dataType  = "string", defaultValue = "Player" },
        new LeaderboardData() { columnName = "Score", dataType = "int", defaultValue    = "100" },
        new LeaderboardData() { columnName = "Date", dataType  = "int", defaultValue    = "30000" }
    };
}