using System;
using System.Linq;
using VContainer.Unity;
using Game.Modules.Laws.Models;
using Game.Shared.Services.Time;
using Game.Shared.Services.LawStats;
using Game.Shared.Services.Crystal;
using Game.Shared.Services.SaveSystem;
using UnityEngine;

namespace Game.Modules.Laws.Managers
{
    /// <summary>
    /// The Use Case / Domain Manager for the Laws module.
    /// Orchestrates interactions between the Pure Models (Deck, Hand, Timer)
    /// and Global Services (Time, Crystal, LawStats).
    /// </summary>
    public class LawsManager : IInitializable, IDisposable, ISavable
    {
        private readonly LawDeckModel _deckModel;
        private readonly LawHandModel _handModel;
        private readonly LawTimerModel _timerModel;
        
        private readonly ITimeService _timeService;
        private readonly ILawStatsService _statsService;
        private readonly ICrystalService _crystalService;
        private readonly ISaveCoordinator _saveCoordinator;
        private readonly LawsConfigSO _config;

        public LawsManager(
            LawDeckModel deckModel,
            LawHandModel handModel,
            LawTimerModel timerModel,
            ITimeService timeService,
            ILawStatsService statsService,
            ICrystalService crystalService,
            ISaveCoordinator saveCoordinator,
            LawsConfigSO config)
        {
            _deckModel = deckModel;
            _handModel = handModel;
            _timerModel = timerModel;
            
            _timeService = timeService;
            _statsService = statsService;
            _crystalService = crystalService;
            _saveCoordinator = saveCoordinator;
            _config = config;
        }

        public void Initialize()
        {
            // Subscribe to global time events for the countdown
            _timeService.OnOneSecondTick += HandleGlobalTick;
            
            // Subscribe to local timer completion to add a card
            _timerModel.OnTimerComplete += HandleTimerComplete;

            // Set default initialized state
            _deckModel.RefillAndShuffle(_config.TotalCardCount);
            _handModel.LoadState(0, -1);
            _timerModel.LoadState(_config.RefillTimeSeconds);

            // Register with Save System. If a save exists, LoadFromState will be called immediately.
            _saveCoordinator.RegisterSavable(this);

            // For testing/initialization: immediately give the player 1 card if they have none.
            if (_handModel.AvailableCardCount == 0)
            {
                AddCardToHand();
            }
        }

        public void Dispose()
        {
            _saveCoordinator.UnregisterSavable(this);
            _timeService.OnOneSecondTick -= HandleGlobalTick;
            _timerModel.OnTimerComplete -= HandleTimerComplete;
        }

        // --- ISavable Implementation ---

        public string SaveKey => "LawsModule";

        [Serializable]
        private struct LawsSaveData
        {
            public int AvailableCards;
            public int ActiveCardIndex;
            public float CurrentTimer;
            public int[] DeckIndices;
        }

        public string GetSaveState()
        {
            var data = new LawsSaveData
            {
                AvailableCards = _handModel.AvailableCardCount,
                ActiveCardIndex = _handModel.CurrentActiveCardIndex,
                CurrentTimer = _timerModel.CurrentTimer,
                DeckIndices = _deckModel.UnusedCardIndices.ToArray()
            };
            return JsonUtility.ToJson(data);
        }

        public void LoadFromState(string jsonState)
        {
            if (string.IsNullOrEmpty(jsonState)) return;
            
            var data = JsonUtility.FromJson<LawsSaveData>(jsonState);
            _handModel.LoadState(data.AvailableCards, data.ActiveCardIndex);
            _timerModel.LoadState(data.CurrentTimer);
            _deckModel.LoadState(data.DeckIndices);
        }

        // --- Core Logic ---

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
