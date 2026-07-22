using System;
using VContainer.Unity;
using Game.Modules.Laws.Models;
using Game.Modules.Laws.Views;
using Game.Modules.Laws.Managers;
using Game.Shared.Services.Localization;

namespace Game.Modules.Laws.Presenters
{
    /// <summary>
    /// Ultra-thin UI mediator.
    /// Passes user inputs to the LawsManager and listens to the HandModel for UI updates.
    /// </summary>
    public class LawCardPresenter : IInitializable, IDisposable
    {
        private readonly LawHandModel _handModel;
        private readonly LawsManager _manager;
        private readonly LawCardView _view;
        private readonly LawsConfigSO _config;
        private readonly ILocalizationService _localizationService;

        public LawCardPresenter(
            LawHandModel handModel,
            LawsManager manager,
            LawCardView view,
            LawsConfigSO config,
            ILocalizationService localizationService)
        {
            _handModel = handModel;
            _manager = manager;
            _view = view;
            _config = config;
            _localizationService = localizationService;
        }

        public void Initialize()
        {
            _view.OnAcceptClicked += HandleAccept;
            _view.OnRejectClicked += HandleReject;
            _handModel.OnHandChanged += RefreshView;

            RefreshView(); // Initial state setup
        }

        public void Dispose()
        {
            _view.OnAcceptClicked -= HandleAccept;
            _view.OnRejectClicked -= HandleReject;
            _handModel.OnHandChanged -= RefreshView;
        }

        private void HandleAccept()
        {
            if (!_handModel.HasActiveCard) return;

            // View immediately starts exit animation and handles its own button states
            _view.AnimateCardExit(true);

            // Domain mutates instantly!
            _manager.SwipeCard(true);
        }

        private void HandleReject()
        {
            if (!_handModel.HasActiveCard) return;

            // View immediately starts exit animation and handles its own button states
            _view.AnimateCardExit(false);

            // Domain mutates instantly!
            _manager.SwipeCard(false);
        }

        private void RefreshView()
        {
            if (_handModel.HasActiveCard)
            {
                var activeCard = _config.GetCardData(_handModel.CurrentActiveCardIndex);
                if (activeCard == null)
                {
                    _view.ShowEmptyState();
                    return;
                }
                
                string title = _localizationService.GetLocalizedString("Laws", activeCard.TitleKey);
                string desc = _localizationService.GetLocalizedString("Laws", activeCard.DescriptionKey);
                var art = _config.GetSpriteForArtType(activeCard.ArtType);

                _view.ShowNewCard(title, desc, art);
            }
            else
            {
                _view.ShowEmptyState();
            }
        }
    }
}
