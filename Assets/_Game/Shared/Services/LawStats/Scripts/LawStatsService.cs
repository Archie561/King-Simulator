using System;
using System.Collections.Generic;
using Game.Shared.Services.SaveSystem;
using UnityEngine;

namespace Game.Shared.Services.LawStats
{
    public class LawStatsService : ILawStatsService, ISavable, IDisposable
    {
        private readonly Dictionary<LawStatType, int> _levels = new();
        private readonly Dictionary<LawStatType, int> _points = new();
        
        private readonly LawStatsConfigSO _config;
        private readonly ISaveCoordinator _saveCoordinator;

        public event Action<LawStatType, int, int> OnStatChanged;

        public LawStatsService(LawStatsConfigSO config, ISaveCoordinator saveCoordinator)
        {
            _config = config;
            _saveCoordinator = saveCoordinator;

            int startingLevel = _config != null ? _config.DefaultStartingLevel : 1;

            foreach (LawStatType type in Enum.GetValues(typeof(LawStatType)))
            {
                _levels[type] = startingLevel;
                _points[type] = 0;
            }

            _saveCoordinator.RegisterSavable(this);
        }

        public void Dispose()
        {
            _saveCoordinator?.UnregisterSavable(this);
        }

        // --- ISavable Implementation ---

        public string SaveKey => "LawStatsService";

        [Serializable]
        private struct StatEntry
        {
            public LawStatType Type;
            public int Level;
            public int Points;
        }

        [Serializable]
        private struct LawStatsSaveData
        {
            public StatEntry[] Stats;
        }

        public string GetSaveState()
        {
            var entries = new List<StatEntry>();
            foreach (var kvp in _levels)
            {
                entries.Add(new StatEntry
                {
                    Type = kvp.Key,
                    Level = kvp.Value,
                    Points = _points.GetValueOrDefault(kvp.Key, 0)
                });
            }

            var data = new LawStatsSaveData { Stats = entries.ToArray() };
            return JsonUtility.ToJson(data);
        }

        public void LoadFromState(string jsonState)
        {
            if (string.IsNullOrEmpty(jsonState)) return;

            var data = JsonUtility.FromJson<LawStatsSaveData>(jsonState);
            if (data.Stats == null) return;

            foreach (var entry in data.Stats)
            {
                _levels[entry.Type] = entry.Level;
                _points[entry.Type] = entry.Points;
                
                // Fire event so UI updates immediately upon loading
                OnStatChanged?.Invoke(entry.Type, entry.Level, entry.Points);
            }
        }

        // --- Core Logic ---

        public int GetStatLevel(LawStatType type)
        {
            return _levels.GetValueOrDefault(type, 1);
        }

        public int GetStatPoints(LawStatType type)
        {
            return _points.GetValueOrDefault(type, 0);
        }

        public void AddPoints(LawStatType type, int amount)
        {
            _points[type] += amount;

            // Basic leveling logic: If points exceed threshold, increase level
            // Note: GDD says levels CANNOT be downgraded even if points go negative.
            int threshold = GetPointsRequiredForNextLevel(_levels[type]);

            while (_points[type] >= threshold)
            {
                _points[type] -= threshold;
                _levels[type]++;
                threshold = GetPointsRequiredForNextLevel(_levels[type]);
            }

            OnStatChanged?.Invoke(type, _levels[type], _points[type]);
        }

        private int GetPointsRequiredForNextLevel(int currentLevel)
        {
            int basePoints = _config != null ? _config.BasePointsForNextLevel : 100;
            // Exponential or linear scaling can be added here
            return basePoints * currentLevel; 
        }
    }
}
