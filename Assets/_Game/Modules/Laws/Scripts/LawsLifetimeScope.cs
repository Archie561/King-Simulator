using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.Modules.Laws.Models;
using Game.Modules.Laws.Managers;
using Game.Modules.Laws.Presenters;
using Game.Modules.Laws.Views;

namespace Game.Modules.Laws
{
    public class LawsLifetimeScope : LifetimeScope
    {
        [Header("Configuration")]
        [SerializeField] private LawsConfigSO _config;

        [Header("View References")]
        [SerializeField] private LawCardView _cardView;
        [SerializeField] private LawTimerView _timerView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (_config != null) builder.RegisterInstance(_config);
            if (_cardView != null) builder.RegisterComponent(_cardView);
            if (_timerView != null) builder.RegisterComponent(_timerView);

            // Register Pure Domain Models
            builder.Register<LawDeckModel>(Lifetime.Scoped);
            builder.Register<LawHandModel>(Lifetime.Scoped);
            builder.Register<LawTimerModel>(Lifetime.Scoped);

            // Register Domain Manager
            builder.RegisterEntryPoint<LawsManager>().AsSelf();

            // Register Thin Presenters
            builder.RegisterEntryPoint<LawCardPresenter>();
            builder.RegisterEntryPoint<LawTimerPresenter>();
        }
    }
}
