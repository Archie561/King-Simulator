using System;

namespace Game.Shared.Services.Laws
{
    public interface ILawStatsService
    {
        int GetStatLevel(LawStatType type);
        int GetStatPoints(LawStatType type);

        event Action<LawStatType, int, int> OnStatChanged; // type, newLevel, newPoints

        void AddPoints(LawStatType type, int amount);
    }
}
