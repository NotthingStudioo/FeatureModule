namespace GameModule.GameModule.Mission
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::GameModule.GameModule.Condition;
    using global::GameModule.GameModule.Mission.Blueprints;
    using global::GameModule.GameModule.Mission.Data;
    using R3;
    using UnityEngine;
    using Zenject;

    /// <summary>
    /// The MissionService handles the mission logic, including starting, resetting, check condition, update, and completing missions.
    /// It implements ITickable to allow for real-time updates of mission timers.
    /// </summary>
    public class MissionService : ITickable, IInitializable
    {
        private readonly MissionBlueprint      missionBlueprint;
        private readonly MissionDataController missionDataController;
        private readonly ConditionHandler      conditionHandler;
        private          List<IMissionRecord>  startedMission;
        private          bool                  isActive; // Flag to control Tick updates
        private          float                 tickTimer = 0.0f;

        public MissionService(MissionBlueprint blueprint, MissionDataController dataController, ConditionHandler conditionHandler)
        {
            this.missionBlueprint      = blueprint;
            this.missionDataController = dataController;
            this.conditionHandler      = conditionHandler;
        }

        /// <summary>
        /// Gets the next mission in sequence based on the current mission.
        /// </summary>
        /// <param name="currentMissionRecord">The current mission to determine the next mission from.</param>
        /// <returns>The next mission, or null if there are no next missions.</returns>
        public IMissionRecord GetNextMission(IMissionRecord currentMissionRecord) // Order should be a list
        {
            if (currentMissionRecord.NextMissions.Count == 0) return null;

            // Assuming the first mission in the order
            var nextMissionId = currentMissionRecord.NextMissions[0].ToString();

            return this.missionBlueprint.GetMissionById(nextMissionId);
        }

        /// <summary>
        /// Starts the mission timer for a given mission and updates the started mission list.
        /// </summary>
        /// <param name="missionRecord">The mission to start the timer for.</param>
        public void StartMissionTimer(IMissionRecord missionRecord)
        {
            this.missionDataController.StartMissionProgress(missionRecord);
            this.UpdateStartedMission();
        }

        /// <summary>
        /// Resets a mission, clearing its progress and updating the started mission list.
        /// </summary>
        /// <param name="missionRecord">The mission to reset.</param>
        public void ResetMission(IMissionRecord missionRecord)
        {
            this.missionDataController.ResetMission(missionRecord);
            this.UpdateStartedMission();
        }

        /// <summary>
        /// Called every frame to update the timers of started missions.
        /// It calculates the remaining time and fails missions if the timer expires.
        /// </summary>
        public void Tick()
        {
            if (!this.isActive) return; // Skip if not active
            this.tickTimer += Time.deltaTime;

            if (this.tickTimer < 1.0f) return; // Skip if not enough time has passed

            foreach (var mission in this.startedMission)
            {
                var endTime         = this.missionDataController.GetMissionEndTime(mission);
                var missionProgress = this.missionDataController.GetMissionProgress(mission);

                if (missionProgress.IsCompleted) return; // Skip if mission is already completed

                // Calculate remaining time
                var remainingTime = (int)(endTime - DateTime.Now).TotalSeconds;

                if (remainingTime > 0)
                {
                    this.missionDataController.UpdateMissionProgress(mission, TimeSpan.FromSeconds(remainingTime)); // Update the timer
                }
                else
                {
                    missionProgress.Timer.Value = 0; // Timer expired
                    this.missionDataController.FailMission(mission); // Complete the mission
                }
            }
        }

        /// <summary>
        /// Initializes the MissionService and updates the list of started missions.
        /// </summary>
        public void Initialize()
        {
            this.isActive = true; // Initialize Tick as active
            this.UpdateStartedMission();
        }

        /// <summary>
        /// Updates the list of started missions from the mission data controller.
        /// </summary>
        private void UpdateStartedMission() { this.startedMission = this.missionDataController.GetStartedMissions(this.missionBlueprint); }

        /// <summary>
        /// Retrieves a list of missions based on their IDs.
        /// </summary>
        /// <param name="ids">A list of mission IDs to retrieve.</param>
        /// <returns>A list of missions matching the provided IDs.</returns>
        public List<IMissionRecord> GetMissions(List<string> ids = null)
        {
            if (ids == null)
            {
                return this.missionBlueprint.GetAllMissions();
            }

            return this.missionBlueprint.GetAllMissions().Where(x => ids.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// Sets the active state of the Tick updates. Turn it off to minimize performance impact when not needed.
        /// </summary>
        /// <param name="active">True to enable Tick updates, false to disable.</param>
        public void SetActive(bool active) { this.isActive = active; }

        /// <summary>
        /// Retrieves the timer for the specified mission as a ReactiveProperty.
        /// This allows for real-time updates and subscription to the timer's value changes.
        /// </summary>
        /// <param name="missionRecord">The mission for which to retrieve the timer.</param>
        /// <returns>A ReactiveProperty<int></int> representing the remaining time for the mission.</returns>
        public ReactiveProperty<int> GetMissionTimer(IMissionRecord missionRecord) { return this.missionDataController.GetMissionTimer(missionRecord); }

        /// <summary>
        /// Check the mission whether if it completes or not.
        /// </summary>
        /// <param name="missionRecord">The target mission</param>
        /// <returns>Result it complete or not</returns>
        public bool CheckMissionCondition(IMissionRecord missionRecord)
        {
            var conditions = missionRecord.GetConditions();

            return this.conditionHandler.CheckConditions(conditions);
        }

        /// <summary>
        /// Force complete the mission.
        /// </summary>
        /// <param name="missionRecord">The target mission</param>
        public void CompleteMission(IMissionRecord missionRecord)
        {
            this.missionDataController.CompleteMission(missionRecord);
            this.missionDataController.GrantMissionReward(missionRecord);
        }

        /// <summary>
        /// Check the progress of a specific mission condition.
        /// </summary>
        /// <param name="missionRecord">The target misison</param>
        /// <returns>Value from 0 to 1 as a percentage of progress</returns>
        public float CheckMissionProgress(IConditionRecord missionRecord)
        {
            return this.conditionHandler.GetProgress(missionRecord);
        }
    }
}