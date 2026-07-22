using System;

namespace Game.Shared.Services.Gold
{
    public interface IGoldService
    {
        int Gold { get; }
        event Action<int> OnGoldChanged;

        void AddGold(int amount);
        bool TrySpendGold(int amount);
    }
}
