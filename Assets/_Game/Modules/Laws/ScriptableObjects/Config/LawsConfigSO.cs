using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Modules.Laws.Models
{
    [Serializable]
    public struct LawArtMapping
    {
        public LawArtType ArtType;
        public Sprite Sprite;
    }

    [CreateAssetMenu(fileName = "LawsConfig", menuName = "Laws/Laws Config")]
    public class LawsConfigSO : ScriptableObject
    {
        [Header("Game Rules")]
        public float RefillTimeSeconds = 120f;
        public int MaxCards = 8;

        [Header("Card Data")]
        [Tooltip("Map the Art enums to actual Sprites here.")]
        public List<LawArtMapping> ArtMappings = new();

        [Tooltip("The total pool of all available law cards in the game.")]
        [SerializeField] private List<LawCardSO> _allCards = new();

        // Cached dictionary for O(1) lookups
        private Dictionary<LawArtType, Sprite> _artDictionary;

        public int TotalCardCount => _allCards != null ? _allCards.Count : 0;

        // OnEnable is generally safer than Awake for ScriptableObjects because it's called 
        // every time the SO is loaded (including after domain reloads in the editor).
        private void OnEnable()
        {
            BuildDictionary();
        }

        private void BuildDictionary()
        {
            _artDictionary = new Dictionary<LawArtType, Sprite>();
            if (ArtMappings == null) return;
            
            foreach (var mapping in ArtMappings)
            {
                if (!_artDictionary.ContainsKey(mapping.ArtType))
                {
                    _artDictionary.Add(mapping.ArtType, mapping.Sprite);
                }
            }
        }

        public Sprite GetSpriteForArtType(LawArtType artType)
        {
            if (_artDictionary == null)
            {
                BuildDictionary();
            }

            if (_artDictionary != null && _artDictionary.TryGetValue(artType, out Sprite sprite))
            {
                return sprite;
            }
            return null;
        }

        public LawCardSO GetCardData(int index)
        {
            if (_allCards == null || index < 0 || index >= _allCards.Count)
            {
                Debug.LogError($"[LawsConfigSO] Invalid card index requested: {index}");
                return null;
            }
            return _allCards[index];
        }
    }
}
