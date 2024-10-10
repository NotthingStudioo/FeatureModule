namespace GameModule.Leaderboard.Editor
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Firebase;
    using Firebase.Firestore;
    using GameModule.Leaderboard.Scripts;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Serialization;
    using UnityEditor;
    using UnityEditor.AddressableAssets;
    using UnityEditor.AddressableAssets.Settings;
    using UnityEditor.AddressableAssets.Settings.GroupSchemas;
    using UnityEngine;

    public class LeaderboardEditor : OdinEditorWindow
    {
        private enum LeaderboardOption
        {
            None,
            Firebase,
            InjectData,
            Playfab
        }

        [Title("Leaderboard Settings")] [ShowInInspector, OdinSerialize]
        private LeaderboardOption selectedOption;

        [Title("Leaderboard Prefab")] [Required, AssetSelector] [InlineEditor(InlineEditorModes.SmallPreview)]
        public GameObject leaderboardPrefab;

        [TabGroup("Firebase", "Firestore")] 
        [ShowIf("@this.selectedOption == LeaderboardOption.Firebase")] 
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        [SerializeField] 
        private LeaderboardDataConfig leaderboardDataConfig;
        public List<LeaderboardData> leaderboardData => this.leaderboardDataConfig.leaderboardData;

        [TabGroup("Playfab", "Settings")] [ShowIf("@this.selectedOption == LeaderboardOption.Playfab")] [LabelText("Playfab Title ID")]
        public string playfabTitleId;

        [MenuItem("Tools/Feature/Leaderboard")]
        public static void ShowWindow() { GetWindow<LeaderboardEditor>("Leaderboard").Show(); }

        [Button("Apply Settings")]
        private void ApplySettings()
        {
            switch (this.selectedOption)
            {
                case LeaderboardOption.Firebase:
                    this.ApplyFirebaseSettings();

                    break;
                case LeaderboardOption.InjectData:
                    this.ApplyInjectDataSettings();

                    break;
                case LeaderboardOption.Playfab:
                    this.ApplyPlayfabSettings();

                    break;
                case LeaderboardOption.None:
                    this.ApplyNoneSettings();

                    break;
            }
        }

        private void OnLeaderboardRemove()
        {
            if (leaderboardPrefab == null)
            {
                throw new Exception("No Leaderboard prefab assigned.");
            }

            // Check if the Addressable Asset Settings exist
            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");

                return;
            }

            // Check if the prefab is already addressable
            var assetPath = AssetDatabase.GetAssetPath(leaderboardPrefab);
            var entry     = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));

            if (entry != null)
            {
                // If addressable, remove it
                settings.RemoveAssetEntry(entry.guid);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryRemoved, entry, true);
                Debug.Log($"Removed {leaderboardPrefab.name} from Addressables.");
            }
            else
            {
                Debug.Log($"{leaderboardPrefab.name} is not found in Addressables.");
            }
        }

        private void ApplyFirebaseSettings()
        {
            this.OnLeaderboardApply();

            this.SetScriptingDefineSymbols(
                new List<string> { "LEADERBOARD_FIRESTORE" },
                new List<string> { "LEADERBOARD_DATA", "LEADERBOARD_PLAYFAB" }
            );
        }

        #region Firestore Initialization

        private FirebaseFirestore db;

        [Button("Initialize Firestore")]
        [ShowIf("@this.selectedOption == LeaderboardOption.Firebase")]
        private async void InitializeFirestore()
        {
            Debug.Log("Initializing Firestore...");

            try
            {
                // Display a progress bar
                EditorUtility.DisplayProgressBar("Firestore Initialization", "Initializing Firebase...", 0f);

                // Initialize Firestore and write leaderboard data with progress updates
                await InitializeFireStore(this.leaderboardData);

                // Hide the progress bar after completion
                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                // Hide progress bar in case of error
                EditorUtility.ClearProgressBar();
                Debug.LogError($"Firestore initialization failed: {e.Message}");
            }
        }

        private async UniTask<bool> InitializeFireStore(List<LeaderboardData> data)
        {
            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();

                    if (dependencyStatus != DependencyStatus.Available)
                    {
                        Debug.LogError("Could not resolve all Firebase dependencies.");

                        return false; // Firebase initialization failed
                    }
                }

                // Get Firestore database instance
                db = FirebaseFirestore.DefaultInstance;

                // Write leaderboard data to Firestore using UniTask
                bool writeSuccess = await WriteLeaderboardDataToFirestore(data);

                return writeSuccess; // Return true if all data was written successfully
            }
            catch (Exception e)
            {
                Debug.LogError($"Firebase initialization failed: {e.Message}");

                return false; // Initialization failed
            }
        }

        private async UniTask<bool> WriteLeaderboardDataToFirestore(List<LeaderboardData> lbdata)
        {
            try
            {
                // The collection name in Firestore, e.g., 'leaderboard'
                var leaderboardRef = this.db.Collection("leaderboard");

                // Create a document using deviceId as document ID
                var docRef = leaderboardRef.Document(SystemInfo.deviceUniqueIdentifier); // Using deviceId as document ID

                // Prepare data entry for Firestore
                var dataEntry = new Dictionary<string, object>
                {
                    { "deviceId", SystemInfo.deviceUniqueIdentifier }, // Unique identifier for the player's device
                };

                // Prepare data in a Dictionary format for Firestore
                foreach (var data in lbdata)
                {
                    dataEntry.Add(
                        data.columnName, 
                        data.defaultValue
                        );
                }

                // Add or update Firestore document
                await docRef.SetAsync(dataEntry).AsUniTask(); // Using AsUniTask() for async Firestore call

                Debug.Log("Firestore data initialization completed.");

                return true; // Success
            }
            catch (Exception e)
            {
                Debug.LogError($"Error adding/updating Firestore document: {e.Message}");

                return false; // Failure
            }
        }

        private async UniTask DropFirestoreCollection(string collectionName)
        {
            this.db = FirebaseFirestore.DefaultInstance;

            var collectionRef = db.Collection(collectionName);

            // Fetch all documents in the collection
            var snapshot = await collectionRef.GetSnapshotAsync().AsUniTask();

            // Loop through each document and delete it
            foreach (var document in snapshot.Documents)
            {
                await document.Reference.DeleteAsync().AsUniTask();
                Debug.Log($"Document {document.Id} deleted from {collectionName} collection.");
            }

            Debug.Log($"All documents deleted from {collectionName} collection.");
        }

        [Button("Delete Leaderboard Data")]
        [ShowIf("@this.selectedOption == LeaderboardOption.Firebase")]
        private async void DeleteLeaderboardData() { await DropFirestoreCollection("leaderboard"); }

        #endregion

        private void ApplyInjectDataSettings()
        {
            this.OnLeaderboardApply();

            SetScriptingDefineSymbols(
                new List<string> { "LEADERBOARD_DATA" },
                new List<string> { "LEADERBOARD_FIRESTORE", "LEADERBOARD_PLAYFAB" }
            );
        }

        private void ApplyPlayfabSettings()
        {
            this.OnLeaderboardApply();

            SetScriptingDefineSymbols(
                new List<string> { "LEADERBOARD_PLAYFAB" },
                new List<string> { "LEADERBOARD_FIRESTORE", "LEADERBOARD_DATA" }
            );
        }

        private void ApplyNoneSettings()
        {
            this.OnLeaderboardRemove();

            SetScriptingDefineSymbols(
                new List<string>(),
                new List<string> { "LEADERBOARD_FIRESTORE", "LEADERBOARD_DATA", "LEADERBOARD_PLAYFAB" }
            );
        }

        private void OnLeaderboardApply()
        {
            if (!leaderboardPrefab)
            {
                throw new Exception("No Leaderboard prefab assigned.");
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            if (settings == null)
            {
                Debug.LogError("Addressable Asset Settings not found.");

                return;
            }

            var group = settings.FindGroup("Default Local Group") ??
                        settings.CreateGroup("Default Local Group", false, false, false, null, typeof(BundledAssetGroupSchema));

            string assetPath = AssetDatabase.GetAssetPath(leaderboardPrefab);
            var    entry     = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(assetPath));

            if (entry == null)
            {
                entry         = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(assetPath), group);
                entry.address = leaderboardPrefab.name;
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                Debug.Log($"Added {leaderboardPrefab.name} to Addressables.");
            }
            else
            {
                Debug.Log($"{leaderboardPrefab.name} is already in Addressables.");
            }
        }

        private void SetScriptingDefineSymbols(List<string> addSymbols, List<string> removeSymbols)
        {
            var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            foreach (var symbol in addSymbols)
            {
                if (!currentSymbols.Contains(symbol))
                {
                    currentSymbols = $"{symbol};{currentSymbols}";
                }
            }

            foreach (var symbol in removeSymbols)
            {
                currentSymbols = currentSymbols.Replace(symbol, "");
            }

            currentSymbols = currentSymbols.Replace(";;", ";").Trim(';');
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currentSymbols);
            Debug.Log($"Applied scripting define symbols: {string.Join(", ", addSymbols)}");
        }
    }
}