using System;
using Cysharp.Threading.Tasks;
using FeatureTemplate.Scripts.MVP;
using GameFoundation.Scripts.UIModule.ScreenFlow.BaseScreen.Presenter;
using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
using Zenject;

namespace GameModule.Leaderboard.Scripts.MVP
{
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.Services;
    using Firebase.Firestore;
    using UnityEngine;

    public class LeaderboardPopupView : FeatureBasePopupViewTemplate
    {
        public event Action OnCreate; // Event for Create operation
        public event Action OnRead;   // Event for Read operation
        public event Action OnUpdate; // Event for Update operation
        public event Action OnDelete; // Event for Delete operation

        private void Update()
        {
            // Check for CRUD input
            if (Input.GetKeyDown(KeyCode.C)) // Create
            {
                OnCreate?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.R)) // Read
            {
                OnRead?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.U)) // Update
            {
                OnUpdate?.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.D)) // Delete
            {
                OnDelete?.Invoke();
            }
        }
    }

    [PopupInfo(nameof(LeaderboardPopupView))]
    public class LeaderboardPopupPresenter : FeatureBasePopupPresenterTemplate<LeaderboardPopupView>
    {
        private readonly LeaderboardService leaderboardService;

        public LeaderboardPopupPresenter(SignalBus signalBus, ScreenManager screenManager, SceneDirector sceneDirector, LeaderboardService leaderboardService) : base(signalBus, screenManager, sceneDirector)
        {
            this.leaderboardService = leaderboardService;
        }

        public override UniTask BindData()
        {
            // Subscribe to the events
            View.OnCreate += HandleCreate;
            View.OnRead += HandleRead;
            View.OnUpdate += HandleUpdate;
            View.OnDelete += HandleDelete;

            return UniTask.CompletedTask;
        }

        private void HandleCreate()
        {
            this.LogMessage("Create operation invoked.");
            this.leaderboardService.AddRecord(UnityEngine.Random.Range(0, 10000000).ToString(), new()
            {
                { "deviceId", SystemInfo.deviceUniqueIdentifier }, // Unique identifier for the player's device
                { "Score", UnityEngine.Random.Range(0, 1000).ToString() }, // Random score value
                { "Name", "AAAAAAAAAA" },
                { "Date", "465890648" }
            });
        }

        private async void HandleRead()
        {
            foreach (var item in await this.leaderboardService.GetRangeFromTop(10))
            {
                foreach(var entry in item)
                {
                    Debug.Log($"Read: {entry.Key}: {entry.Value}");
                }
            }
        }

        private void HandleUpdate()
        {
            // Handle update logic here
            // For example: Update a leaderboard entry
            this.leaderboardService.UpdateRecord("5065506306a9161bcf213b8c514ed300fd0d11fc", new Dictionary<string, object>()
            {
                { "deviceId", SystemInfo.deviceUniqueIdentifier }, // Unique identifier for the player's device
                { "Score", UnityEngine.Random.Range(0, 1000).ToString() }, // Random score value
                { "Name", "First" },
                { "Date", "1111111" }
            });
        }

        private void HandleDelete()
        {
            this.leaderboardService.DeleteRecord("5065506306a9161bcf213b8c514ed300fd0d11fc");
        }
    }
}
