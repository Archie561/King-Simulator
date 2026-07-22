using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.Shared.Services.Time;
using Game.Shared.Services.Gold;
using Game.Shared.Services.Crystal;
using Game.Shared.Services.TradeGoods;
using Game.Shared.Services.LawStats;
using Game.Shared.Services.GameState;
using Game.Shared.Services.Localization;
using Game.Shared.Services.Audio;
using Game.Shared.Services.UI;
using Game.Shared.Services.SaveSystem;

namespace Game.Root
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [Header("Configuration ScriptableObjects")]
        [SerializeField] private TradeGoodsConfigSO _tradeGoodsConfig;
        [SerializeField] private LawStatsConfigSO _lawStatsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_tradeGoodsConfig != null) builder.RegisterInstance(_tradeGoodsConfig);
            if (_lawStatsConfig != null) builder.RegisterInstance(_lawStatsConfig);

            // Save System
            builder.Register<LocalJsonStorageProvider>(Lifetime.Singleton).As<IStorageProvider>();
            builder.Register<SaveCoordinator>(Lifetime.Singleton).As<ISaveCoordinator>();

            // Global Services
            builder.Register<TimeService>(Lifetime.Singleton).As<ITimeService>().As<ITickable>();
            builder.Register<GoldService>(Lifetime.Singleton).As<IGoldService>();
            builder.Register<CrystalService>(Lifetime.Singleton).As<ICrystalService>();
            builder.Register<TradeGoodsService>(Lifetime.Singleton).As<ITradeGoodsService>();
            builder.Register<LawStatsService>(Lifetime.Singleton).As<ILawStatsService>();
            builder.Register<GameStateService>(Lifetime.Singleton).As<IGameStateService>();
            builder.Register<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.Register<AudioService>(Lifetime.Singleton).As<IAudioService>();
            builder.Register<UIManager>(Lifetime.Singleton).As<IUIManager>();

            // Bootstrapper (IAsyncStartable) - Replaces Monobehaviour Start()
            // (VContainer automatically registers the current LifetimeScope, so manual registration causes a conflict)
            builder.RegisterEntryPoint<GameBootstrapService>();
        }

        // Reliable saving on mobile backgrounding. Fire and forget the Task.
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && Container != null)
            {
                var saveCoordinator = Container.Resolve<ISaveCoordinator>();
                _ = saveCoordinator.SaveGameAsync();
            }
        }

        // On OS termination, we do not await. We rely on the atomic save (.tmp) fallback
        // to prevent file corruption if the OS kills the thread mid-write.
        private void OnApplicationQuit()
        {
            if (Container != null)
            {
                var saveCoordinator = Container.Resolve<ISaveCoordinator>();
                _ = saveCoordinator.SaveGameAsync();
            }
        }
    }
}
