using System;

namespace Game.Modules.Laws.Models
{
    /// <summary>
    /// Pure Domain Model representing the cards currently held by the player.
    /// Manages the queue count and the currently active card.
    /// </summary>
    public class LawHandModel
    {
        public int AvailableCardCount { get; private set; }
        public int CurrentActiveCardIndex { get; private set; } = -1;

        public event Action OnHandChanged;

        public void LoadState(int availableCount, int activeCardIndex)
        {
            AvailableCardCount = availableCount;
            CurrentActiveCardIndex = activeCardIndex;
        }

        public void AddCardCount()
        {
            AvailableCardCount++;
            OnHandChanged?.Invoke();
        }

        public void SetActiveCard(int cardIndex)
        {
            CurrentActiveCardIndex = cardIndex;
            OnHandChanged?.Invoke();
        }

        public void ConsumeActiveCard()
        {
            if (AvailableCardCount > 0)
            {
                AvailableCardCount--;
            }
            CurrentActiveCardIndex = -1;
            OnHandChanged?.Invoke();
        }

        public bool HasCards => AvailableCardCount > 0;
        public bool HasActiveCard => CurrentActiveCardIndex != -1;
    }
}
