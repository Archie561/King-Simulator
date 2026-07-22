using System;
using System.Collections.Generic;

namespace Game.Modules.Laws.Models
{
    /// <summary>
    /// Pure Domain Model representing the deck of law cards.
    /// Manages only the remaining unused cards and drawing/shuffling logic.
    /// </summary>
    public class LawDeckModel
    {
        private readonly List<int> _unusedCardIndices = new();
        
        // Single static RNG instance to prevent identical shuffling if called rapidly
        private static readonly Random _rng = new Random();

        public IReadOnlyList<int> UnusedCardIndices => _unusedCardIndices;

        public void LoadState(IEnumerable<int> savedIndices)
        {
            _unusedCardIndices.Clear();
            if (savedIndices != null)
            {
                _unusedCardIndices.AddRange(savedIndices);
            }
        }

        public void RefillAndShuffle(int totalCardsInPool)
        {
            if (totalCardsInPool <= 0) return;

            _unusedCardIndices.Clear();
            for (int i = 0; i < totalCardsInPool; i++)
            {
                _unusedCardIndices.Add(i);
            }

            Shuffle();
        }

        private void Shuffle()
        {
            int n = _unusedCardIndices.Count;
            while (n > 1)
            {
                n--;
                int k = _rng.Next(n + 1);
                int value = _unusedCardIndices[k];
                _unusedCardIndices[k] = _unusedCardIndices[n];
                _unusedCardIndices[n] = value;
            }
        }

        /// <summary>
        /// Returns the next card index, or -1 if the deck is empty.
        /// </summary>
        public int DrawCard()
        {
            if (_unusedCardIndices.Count == 0)
                return -1;

            int indexToPop = _unusedCardIndices.Count - 1;
            int cardIndex = _unusedCardIndices[indexToPop];
            _unusedCardIndices.RemoveAt(indexToPop);
            
            return cardIndex;
        }

        public bool IsEmpty => _unusedCardIndices.Count == 0;
    }
}
