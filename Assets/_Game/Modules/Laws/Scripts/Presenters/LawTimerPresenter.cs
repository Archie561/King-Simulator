using System;
using VContainer.Unity;
using Game.Modules.Laws.Models;
using Game.Modules.Laws.Views;
using Game.Modules.Laws.Managers;

namespace Game.Modules.Laws.Presenters
{
    /// <summary>
    /// Ultra-thin UI mediator.
    /// Passes user inputs to the LawsManager and listens to models to update the timer UI.
    /// </summary>
    public class LawTimerPresenter : IInitializable, IDisposable
    {
        private readonly LawTimerModel _timerModel;
        private readonly LawHandModel _handModel;
        private readonly LawsManager _manager;
        private readonly LawTimerView _view;
        private readonly LawsConfigSO _config;

        public LawTimerPresenter(
            LawTimerModel timerModel,
            LawHandModel handModel,
            LawsManager manager,
            LawTimerView view,
            LawsConfigSO config)
        {
            _timerModel = timerModel;
            _handModel = handModel;
            _manager = manager;
            _view = view;
            _config = config;
        }

        public void Initialize()
        {
            _view.OnBuyRefillClicked += HandleBuyRefill;
            
            _timerModel.OnTimerChanged += HandleTimerTick;
            _handModel.OnHandChanged += HandleCardStateChanged;
            
            // Initial view setup
            HandleTimerTick(_timerModel.CurrentTimer);
            HandleCardStateChanged();
        }

        public void Dispose()
        {
            _view.OnBuyRefillClicked -= HandleBuyRefill;
            
            _timerModel.OnTimerChanged -= HandleTimerTick;
            _handModel.OnHandChanged -= HandleCardStateChanged;
        }

        private void HandleTimerTick(float timerValue)
        {
            _view.UpdateTimerDisplay(timerValue);
        }

        private void HandleCardStateChanged()
        {
            _view.UpdateCardCount(_handModel.AvailableCardCount, _config.MaxCards);
        }

        private void HandleBuyRefill()
        {
            // The manager handles logic, cost checking, and domain rules
            _manager.TryBuyRefill();
        }
    }
}
