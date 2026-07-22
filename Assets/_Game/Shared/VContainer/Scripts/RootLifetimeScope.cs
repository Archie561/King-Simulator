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
using UnityEngine.SceneManagement;

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

            builder.Register<TimeService>(Lifetime.Singleton).As<ITimeService>().As<ITickable>();
            builder.Register<GoldService>(Lifetime.Singleton).As<IGoldService>();
            builder.Register<CrystalService>(Lifetime.Singleton).As<ICrystalService>();
            builder.Register<TradeGoodsService>(Lifetime.Singleton).As<ITradeGoodsService>();
            builder.Register<LawStatsService>(Lifetime.Singleton).As<ILawStatsService>();
            builder.Register<GameStateService>(Lifetime.Singleton).As<IGameStateService>();
            builder.Register<LocalizationService>(Lifetime.Singleton).As<ILocalizationService>();
            builder.Register<AudioService>(Lifetime.Singleton).As<IAudioService>();
            builder.Register<UIManager>(Lifetime.Singleton).As<IUIManager>();
        }

        // Acts like a bootstrapper for the game
        private void Start()
        {
            // EnqueueParent ensures that any LifetimeScope in the next loaded scene 
            // will automatically be parented to our master scope, avoiding FindObjectOfType hacks.
            using (LifetimeScope.EnqueueParent(this))
            {
                // Load additively so the Bootstrap Master Scene is never destroyed
                SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
            }
        }
    }
}
