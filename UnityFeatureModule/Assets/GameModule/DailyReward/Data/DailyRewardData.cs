﻿namespace DailyReward.GameModule.DailyReward.Data
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using FeatureTemplate.Scripts.Services;
    using GameFoundation.Scripts.Interfaces;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using Newtonsoft.Json;
    using Sirenix.Utilities;
    using Zenject;

    public class DailyRewardData : ILocalData, IFeatureLocalData
    {
        public  DateTime FirstTimeLogin => this.DeserializeDateTime(this.SerializeFtl) == null ? DateTime.Now : (DateTime)this.DeserializeDateTime(this.SerializeFtl);

        public DateTime                 LastSavedDay;
        public string                   SerializeFtl;
        public int                      DayOffSet;
        public Dictionary<string, bool> DailyRewards; //Daily reward id, claimed status
        public int                      Page = 0;
        public bool                     IsInit;

        public void Init()
        {
            this.LastSavedDay   = DateTime.Now;
            this.SerializeFtl   = this.SerializeDateTime(this.LastSavedDay);
            this.DayOffSet      = 0;
            this.DailyRewards   = new();
        }

        private string SerializeDateTime(DateTime dateTime)
        {
            // Serialize DateTime object to a JSON string
            return JsonConvert.SerializeObject(dateTime);
        }

        private DateTime? DeserializeDateTime(string dateTimeString)
        {
            // Deserialize JSON string to a DateTime object
            return JsonConvert.DeserializeObject<DateTime>(dateTimeString);
        }

        public Type ControllerType => typeof(DailyRewardDataController);
    }

    public class DailyRewardDataController : IFeatureControllerData, IInitializable
    {
        private readonly DailyRewardData               dailyRewardData;
        private readonly FeatureDataState              featureDataState;
        private readonly DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint;
        private readonly FeatureDailyRewardBlueprint   featureDailyRewardBlueprint;

        public DailyRewardDataController(DailyRewardData dailyRewardData, FeatureDataState featureDataState, DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint,
            FeatureDailyRewardBlueprint featureDailyRewardBlueprint)
        {
            this.dailyRewardData               = dailyRewardData;
            this.featureDataState              = featureDataState;
            this.dailyRewardMiscParamBlueprint = dailyRewardMiscParamBlueprint;
            this.featureDailyRewardBlueprint   = featureDailyRewardBlueprint;
        }

        public void Initialize()
        {
            this.OnUserDataLoaded();
            var currentDay = DateTime.Now;

            if (this.dailyRewardData.LastSavedDay == default || this.dailyRewardData.LastSavedDay < this.dailyRewardData.FirstTimeLogin)
                this.dailyRewardData.LastSavedDay = currentDay;
        }

        private async void OnUserDataLoaded()
        {
            await UniTask.WaitUntil(() => this.featureDataState.IsBlueprintAndLocalDataLoaded);

            if (this.dailyRewardData.IsInit) return;
            this.featureDailyRewardBlueprint.ForEach(x => this.dailyRewardData.DailyRewards.Add(x.Value.Id, false));
            this.dailyRewardData.IsInit = true;
        }

        /// <summary>
        /// Day offset represent the amount of day which player have skip.
        /// </summary>
        public int DayOffSet { get => this.dailyRewardData.DayOffSet; set => this.dailyRewardData.DayOffSet = value; }

        public int Page { get => this.dailyRewardData.Page; set => this.dailyRewardData.Page = value; }

        public int Today => this.dailyRewardMiscParamBlueprint.TimeLoop > 0
            ? // If TimeLoop is set > 0, return the remainder of the division
            (DateTime.Now.Day - this.dailyRewardData.FirstTimeLogin.Day + 1) %
            this.dailyRewardMiscParamBlueprint.TimeLoop + this.dailyRewardData.DayOffSet
            : DateTime.Now.Day - this.dailyRewardData.FirstTimeLogin.Day + this.dailyRewardData.DayOffSet;

        public DateTime LastSavedDay { get => this.dailyRewardData.LastSavedDay; set => this.dailyRewardData.LastSavedDay = value; }

        public bool IsClaimed(int day) { return this.dailyRewardData.DailyRewards[this.featureDailyRewardBlueprint[day.ToString()].Id]; }

        public void ClaimReward(int day)
        {
            if (this.IsClaimed(day)) return;
            this.dailyRewardData.DailyRewards[this.featureDailyRewardBlueprint[day.ToString()].Id] = true;
        }
    }
}