using System;

namespace Game.Shared.Services.Premium
{
    public class CrystalService : ICrystalService
    {
        public int Crystals { get; private set; }
        public event Action<int> OnCrystalsChanged;

        public void AddCrystals(int amount)
        {
            if (amount <= 0) return;
            Crystals += amount;
            OnCrystalsChanged?.Invoke(Crystals);
        }

        public bool TrySpendCrystals(int amount)
        {
            if (amount <= 0 || Crystals < amount) return false;
            Crystals -= amount;
            OnCrystalsChanged?.Invoke(Crystals);
            return true;
        }
    }
}
