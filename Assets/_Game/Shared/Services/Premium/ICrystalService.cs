using System;

namespace Game.Shared.Services.Premium
{
    public interface ICrystalService
    {
        int Crystals { get; }
        event Action<int> OnCrystalsChanged;

        void AddCrystals(int amount);
        bool TrySpendCrystals(int amount);
    }
}
