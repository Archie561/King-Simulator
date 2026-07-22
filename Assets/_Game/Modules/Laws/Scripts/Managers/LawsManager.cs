using System;
using VContainer.Unity;
using Game.Modules.Laws.Models;
using Game.Shared.Services.Time;
using Game.Shared.Services.LawStats;
using Game.Shared.Services.Crystal;
using UnityEngine;

namespace Game.Modules.Laws.Managers
{
    /// <summary>
    /// The Use Case / Domain Manager for the Laws module.
    /// Orchestrates interactions between the Pure Models (Deck, Hand, Timer)
    /// and Global Services (Time, Crystal, LawStats).
    /// </summary>
    public class LawsManager : IInitializable, IDisposable
    {
        private readonly LawDeckModel _deckModel;
        private readonly LawHandModel _handModel;
        private readonly LawTimerModel _timerModel;
        
        private readonly ITimeService _timeService;
        private readonly ILawStatsService _statsService;
        private readonly ICrystalService _crystalService;
        private readonly LawsConfigSO _config;

        public LawsManager(
            LawDeckModel deckModel,
            LawHandModel handModel,
            LawTimerModel timerModel,
            ITimeService timeService,
            ILawStatsService statsService,
            ICrystalService crystalService,
            LawsConfigSO config)
        {
            _deckModel = deckModel;
            _handModel = handModel;
            _timerModel = timerModel;
            
            _timeService = timeService;
            _statsService = statsService;
            _crystalService = crystalService;
            _config = config;
        }

        public void Initialize()
        {
            // Subscribe to global time events for the countdown
            _timeService.OnOneSecondTick += HandleGlobalTick;
            
            // Subscribe to local timer completion to add a card
            _timerModel.OnTimerComplete += HandleTimerComplete;

            // Initialize models (this would normally load from Save Data)
            _deckModel.RefillAndShuffle(_config.TotalCardCount);
            _handModel.LoadState(0, -1);
            _timerModel.LoadState(_config.RefillTimeSeconds);
        }

        public void Dispose()
        {
            _timeService.OnOneSecondTick -= HandleGlobalTick;
            _timerModel.OnTimerComplete -= HandleTimerComplete;
        }

        private void HandleGlobalTick()
        {
            if (_handModel.AvailableCardCount < _config.MaxCards)
            {
                _timerModel.Tick(1f);
            }
            else
            {
                // Ensure timer is reset if hand is full
                if (_timerModel.CurrentTimer != _config.RefillTimeSeconds)
                {
                    _timerModel.ResetTimer(_config.RefillTimeSeconds);
                }
            }
        }

        private void HandleTimerComplete()
        {
            _timerModel.ResetTimer(_config.RefillTimeSeconds);
            AddCardToHand();
        }

        private void AddCardToHand()
        {
            if (_handModel.AvailableCardCount >= _config.MaxCards) return;

            _handModel.AddCardCount();

            // If we didn't have an active card, make the next one active immediately
            if (!_handModel.HasActiveCard)
            {
                ActivateNextCard();
            }
        }

        private void ActivateNextCard()
        {
            if (_deckModel.IsEmpty)
            {
                _deckModel.RefillAndShuffle(_config.TotalCardCount);
            }

            int nextCardIndex = _deckModel.DrawCard();
            _handModel.SetActiveCard(nextCardIndex);
        }

        /// <summary>
        /// Called by the Presenter when the user swipes a card.
        /// </summary>
        public void SwipeCard(bool accepted)
        {
            if (!_handModel.HasActiveCard) return;

            // 1. Get the card data securely via encapsulated config method
            var card = _config.GetCardData(_handModel.CurrentActiveCardIndex);
            if (card == null) return;

            // 2. Apply the stats based on the choice
            var effects = accepted ? card.AcceptEffects : card.RejectEffects;
            foreach (var effect in effects)
            {
                _statsService.AddPoints(effect.StatType, effect.Amount);
            }

            // 3. Consume the card from the hand
            _handModel.ConsumeActiveCard();

            // 4. If we still have cards available, immediately queue up the next one
            if (_handModel.HasCards)
            {
                ActivateNextCard();
            }
        }

        /// <summary>
        /// Called by the Presenter when the user wants to buy a refill.
        /// </summary>
        public void TryBuyRefill()
        {
            if (_handModel.AvailableCardCount >= _config.MaxCards) return;

            // Refill cost is 2 crystals (could be moved to config)
            if (_crystalService.TrySpendCrystals(2))
            {
                AddCardToHand();
            }
            else
            {
                Debug.LogWarning("Not enough crystals to buy a card refill.");
                // A UI event bus could optionally broadcast a "NotEnoughCrystals" event here
                // if we wanted a floating text or shake animation.
            }
        }
    }
}
