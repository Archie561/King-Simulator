using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Shared.Services.LawStats; // For LawStatType

namespace Game.Modules.Laws.Models
{
    [Serializable]
    public struct StatChange
    {
        public LawStatType StatType;
        public int Amount; // Can be positive or negative
    }

    [CreateAssetMenu(fileName = "NewLawCard", menuName = "Laws/Law Card")]
    public class LawCardSO : ScriptableObject
    {
        [Tooltip("Localization key for the card's title.")]
        public string TitleKey;

        [Tooltip("Localization key for the card's description or flavor text.")]
        [TextArea]
        public string DescriptionKey;

        [Tooltip("Enum defining which art to show for this card. Mapped to actual sprites in the DeckConfig.")]
        public LawArtType ArtType;

        [Header("Effects on Accept (Swipe Right)")]
        public List<StatChange> AcceptEffects = new();

        [Header("Effects on Reject (Swipe Left)")]
        public List<StatChange> RejectEffects = new();
    }
}
