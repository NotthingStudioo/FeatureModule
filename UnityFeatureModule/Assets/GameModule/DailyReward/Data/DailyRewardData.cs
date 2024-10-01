namespace DailyReward.GameModule.DailyReward.Data
{
    using System;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using GameFoundation.Scripts.Interfaces;
    using Zenject;

    public class DailyRewardData : ILocalData, IFeatureLocalData
    {
        public DateTime FirstTimeLogin { get; private set; }
        public DateTime LastSavedDay   { get; set; }
        public int      Today          { get; set; }

        public void Init()
        {
            this.FirstTimeLogin = DateTime.Now;
            this.Today          = 1;
        }

        public Type ControllerType => typeof(DailyRewardDataController);
    }

    public class DailyRewardDataController : IFeatureControllerData, IInitializable
    {
        private readonly DailyRewardData DailyRewardData;

        public DailyRewardDataController(DailyRewardData dailyRewardData) { this.DailyRewardData = dailyRewardData; }

        public void Initialize()
        {
            var currentDay = DateTime.Now;
            this.DailyRewardData.Today = DateTime.Now.Day - this.DailyRewardData.FirstTimeLogin.Day + 1;

            if (this.DailyRewardData.LastSavedDay == default || this.DailyRewardData.LastSavedDay < this.DailyRewardData.FirstTimeLogin)
                this.DailyRewardData.LastSavedDay = currentDay;
        }

        public int Today => this.DailyRewardData.Today == 0 ? 1 : this.DailyRewardData.Today;
        public DateTime LastSavedDay {get => this.DailyRewardData.LastSavedDay; set => this.DailyRewardData.LastSavedDay = value; }
    }
}