﻿namespace GameModule.Mission
{
    using GameFoundation.Scripts.UIModule.ScreenFlow.Managers;
    using GameModule.Condition;
    using GameModule.Mission.MVP;
    using Zenject;

    public class MissionInstaller : Installer<MissionInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesTo<ICondition>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<ConditionHandler>().AsCached().NonLazy();
            this.Container.BindInterfacesAndSelfTo<MissionService>().AsCached().NonLazy();
            this.Container.Resolve<ScreenManager>().OpenScreen<MissionPresenter>();
        }
    }
}