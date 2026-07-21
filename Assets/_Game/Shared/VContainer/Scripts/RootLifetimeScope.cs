using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.Shared.Services.Time;
using Game.Shared.Services.Economy;
using Game.Shared.Services.Premium;
using Game.Shared.Services.Trade;
using Game.Shared.Services.Laws;
using Game.Shared.Services.GameState;
using Game.Shared.Services.Localization;
using Game.Shared.Services.Audio;
using Game.Shared.Services.UI;

namespace Game.Root
{
    /// <summary>
    /// Global entry point for VContainer DI.
    /// Registers core services.
    /// </summary>
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [Header("Configuration ScriptableObjects")]
        [SerializeField] private TradeGoodsConfigSO _tradeGoodsConfig;
        [SerializeField] private LawStatsConfigSO _lawStatsConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            // Register ScriptableObject Configs as singletons
            // Using RegisterInstance so they are injected directly where needed
            if (_tradeGoodsConfig != null)
            {
                builder.RegisterInstance(_tradeGoodsConfig);
            }
            if (_lawStatsConfig != null)
            {
                builder.RegisterInstance(_lawStatsConfig);
            }

            // Register TimeService as both its interface and ITickable
            builder.Register<TimeService>(Lifetime.Singleton)
                   .As<ITimeService>()
                   .As<ITickable>();

            // Register distinct Resource Services
            builder.Register<GoldService>(Lifetime.Singleton).As<IGoldService>();
            builder.Register<CrystalService>(Lifetime.Singleton).As<ICrystalService>();
            builder.Register<TradeGoodsService>(Lifetime.Singleton).As<ITradeGoodsService>();
            builder.Register<LawStatsService>(Lifetime.Singleton).As<ILawStatsService>();

            // Register remaining global services
            builder.Register<GameStateService>(Lifetime.Singleton).As<IGameStateService>();
            builder.Register<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.Register<AudioService>(Lifetime.Singleton).As<IAudioService>();
            builder.Register<UIManager>(Lifetime.Singleton).As<IUIManager>();
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
