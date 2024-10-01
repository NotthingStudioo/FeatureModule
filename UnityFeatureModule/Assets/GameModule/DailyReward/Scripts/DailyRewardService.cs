namespace DailyReward.GameModule.DailyReward.Scripts
{
    using System;
    using System.Collections.Generic;
    using FeatureTemplate.Scripts.Blueprints;
    using FeatureTemplate.Scripts.RewardHandle;
    using FeatureTemplate.Scripts.Services;
    using FeatureTemplate.Scripts.Signals;
    using global::DailyReward.GameModule.DailyReward.Data;
    using UnityEngine;
    using Zenject;

    public class DailyRewardService
    {
        private readonly DailyRewardDataController        dailyRewardDataController;
        private readonly FeatureDailyRewardBlueprint      featureDailyRewardBlueprint;
        private readonly List<IFeatureRewardExecutorBase> rewardHandlers;
        private readonly SignalBus                        signalBus;

        // Constructor to initialize
        public DailyRewardService(DailyRewardDataController dailyRewardDataController,
            FeatureDailyRewardBlueprint featureDailyRewardBlueprint, List<IFeatureRewardExecutorBase> rewardHandlers, SignalBus signalBus)
        {
            this.dailyRewardDataController   = dailyRewardDataController;
            this.featureDailyRewardBlueprint = featureDailyRewardBlueprint;
            this.rewardHandlers              = rewardHandlers;
            this.signalBus                   = signalBus;
        }

        // Check if reward can be claimed (based on date, not exact hours)
        private bool CanClaimReward() { return IsNewDay(); }

        // Claim reward if eligible
        public bool ClaimReward()
        {
            if (CanClaimReward())
            {
                // Trigger the onRewardClaim event, passing the currentDay as argument
                var currentRewards = featureDailyRewardBlueprint.GetRewards(this.dailyRewardDataController.Today.ToString());

                this.signalBus.Fire(new RewardClaimSignal()
                {
                    Reward = currentRewards
                });

                foreach (var item in this.rewardHandlers)
                {
                    foreach (var reward in currentRewards)
                    {
                        if (item.RewardId.Equals(reward.RewardId))
                            item.ReceiveReward(int.Parse(reward.RewardValue), null);
                    }
                }

                // Update claim time and save it
                this.dailyRewardDataController.LastSavedDay = DateTime.UtcNow;

                return true;
            }
            else
            {
                this.LogMessage("Reward cannot be claimed yet.", Color.red);

                return false;
            }
        }

        public List<Reward> ReadRewardsAt(int dayOffset)
        {
            var today = this.dailyRewardDataController.Today;

            return this.featureDailyRewardBlueprint.GetRewards((today + dayOffset).ToString());
        }

        // Check if it's a new day based on last claim
        private bool IsNewDay()
        {
            var currentUtcTime = DateTime.UtcNow;
            var lastClaimDate  = this.dailyRewardDataController.LastSavedDay.Date;
            var currentDate    = currentUtcTime.Date;

            // If the current day (at UTC) is different from the last claim day, it's a new day
            return currentDate > lastClaimDate;
        }
    }
}