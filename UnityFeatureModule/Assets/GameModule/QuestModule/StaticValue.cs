namespace GameModule.QuestModule
{
    public partial class StaticValue
    {
         public class RequirementStaticValue
        {
            public static string KillEnemy             { get; set; } = "kill_enemy";
            public static string EarnCurrency          { get; set; } = "earn_currency";
            public static string CollectItem           { get; set; } = "collect_item";
            public static string SpendCurrency         { get; set; } = "spend_currency";
            public static string UnlockNewBuilding     { get; set; } = "unlock_new_building";
            public static string EquipInventoryItem    { get; set; } = "equip_inventory_item";
            public static string BuildBuilding         { get; set; } = "build_building";
            public static string UpgradeBuilding       { get; set; } = "upgrade_building";
            public static string OwnBuilding           { get; set; } = "own_building";
            public static string UnlockRegion          { get; set; } = "unlock_region";
            public static string SpendConsumable       { get; set; } = "spend_consumable";
            public static string SaveWorker            { get; set; } = "save_worker";
            public static string OwnWorker             { get; set; } = "own_worker";
            public static string EquipWorkerOnMining   { get; set; } = "equip_worker_on_mining";
            public static string MarkResourceForWorker { get; set; } = "mark_resource_for_worker";
            public static string BuildTownCenter       { get; set; } = "build_town_center";
            public static string UpgradeTowerCenter    { get; set; } = "upgrade_town_center";
            public static string CompleteASideQuest    { get; set; } = "complete_side_quest";
            public static string OwnerItem             { get; set; } = "owner_item";
            public static string CompleteDungeon       { get; set; } = "complete_dungeon";
            public static string AssignJobForWorker    { get; set; } = "assign_job_for_worker";
            public static string UpgradeItem           { get; set; } = "upgrade_item";
        }
    }
}