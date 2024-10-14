namespace Game.Scripts.Shop
{
    using Cysharp.Threading.Tasks;
    using FeatureTemplate.Scripts.Services;
    using Game.Scripts.Shop.Condition;
    using Game.Scripts.Shop.Cost;
    using GameFoundation.Scripts.Utilities.Extension;
    using Zenject;

    public class TransactionInstaller : Installer<TransactionInstaller>
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<ICost>();
            this.Container.Bind<FeatureTransactionService>().AsCached().NonLazy();
            this.OnAfterDataLoaded();
        }

        private async void OnAfterDataLoaded()
        {
            await UniTask.WaitUntil(() => this.Container.Resolve<FeatureDataState>().IsBlueprintAndLocalDataLoaded);
            this.Container.BindInterfacesAndSelfToAllTypeDriveFrom<IShopCondition>();
        }
    }
}