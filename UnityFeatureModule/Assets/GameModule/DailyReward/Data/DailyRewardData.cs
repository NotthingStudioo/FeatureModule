namespace DailyReward.GameModule.DailyReward.Data
{
    using System;
    using FeatureTemplate.Scripts.InterfacesAndEnumCommon;
    using GameFoundation.Scripts.Interfaces;
    using global::DailyReward.GameModule.DailyReward.Blueprints;
    using Newtonsoft.Json;
    using Zenject;

    public class DailyRewardData : ILocalData, IFeatureLocalData
    {
        public  DateTime FirstTimeLogin => this.DeserializeDateTime(this.SerializeFtl) == null ? DateTime.Now : (DateTime)this.DeserializeDateTime(this.SerializeFtl);
        private DateTime firstTimeLogin;
        public  DateTime LastSavedDay;
        public  string   SerializeFtl;
        public  int     DayOffSet;

        public void Init()
        {
            this.firstTimeLogin = DateTime.Now;
            this.SerializeFtl   = this.SerializeDateTime(this.firstTimeLogin);
            this.DayOffSet      = 1;
        }
        
        private string SerializeDateTime(DateTime dateTime)
        {
            // Serialize DateTime object to a JSON string
            return JsonConvert.SerializeObject(dateTime);
        }
        
        private DateTime? DeserializeDateTime(string dateTimeString)
        {
            // if(dateTimeString == null) return null;
            // Deserialize JSON string to a DateTime object
            return JsonConvert.DeserializeObject<DateTime>(dateTimeString);
        }

        public Type ControllerType => typeof(DailyRewardDataController);
    }

    public class DailyRewardDataController : IFeatureControllerData, IInitializable
    {
        private readonly DailyRewardData               DailyRewardData;
        private readonly DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint;

        public DailyRewardDataController(DailyRewardData dailyRewardData, DailyRewardMiscParamBlueprint dailyRewardMiscParamBlueprint)
        {
            this.DailyRewardData               = dailyRewardData;
            this.dailyRewardMiscParamBlueprint = dailyRewardMiscParamBlueprint;
        }

        public void Initialize()
        {
            var currentDay = DateTime.Now;

            if (this.DailyRewardData.LastSavedDay == default || this.DailyRewardData.LastSavedDay < this.DailyRewardData.FirstTimeLogin)
                this.DailyRewardData.LastSavedDay = currentDay;
        }
        
        public int DayOffSet
        {
            get => this.DailyRewardData.DayOffSet;
            set => this.DailyRewardData.DayOffSet = value;
        }

        public int      Today        => this.dailyRewardMiscParamBlueprint.TimeLoop > 0 ? // If TimeLoop is set > 0, return the remainder of the division
            (DateTime.Now.Day - this.DailyRewardData.FirstTimeLogin.Day + 1) % 
            this.dailyRewardMiscParamBlueprint.TimeLoop : DateTime.Now.Day - this.DailyRewardData.FirstTimeLogin.Day + this.DailyRewardData.DayOffSet;
        public DateTime LastSavedDay { get => this.DailyRewardData.LastSavedDay; set => this.DailyRewardData.LastSavedDay = value; }
    }
}