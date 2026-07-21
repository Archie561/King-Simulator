using System;
using System.Collections.Generic;

namespace Game.Shared.Services.Laws
{
    public class LawStatsService : ILawStatsService
    {
        private readonly Dictionary<LawStatType, int> _levels = new();
        private readonly Dictionary<LawStatType, int> _points = new();
        private readonly LawStatsConfigSO _config;

        public event Action<LawStatType, int, int> OnStatChanged;

        public LawStatsService(LawStatsConfigSO config)
        {
            _config = config;

            int startingLevel = _config != null ? _config.DefaultStartingLevel : 1;

            foreach (LawStatType type in Enum.GetValues(typeof(LawStatType)))
            {
                _levels[type] = startingLevel;
                _points[type] = 0;
            }
        }

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
