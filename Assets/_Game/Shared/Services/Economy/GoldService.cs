using System;

namespace Game.Shared.Services.Economy
{
    public class GoldService : IGoldService
    {
        public int Gold { get; private set; }
        public event Action<int> OnGoldChanged;

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            Gold += amount;
            OnGoldChanged?.Invoke(Gold);
        }

        public bool TrySpendGold(int amount)
        {
            if (amount <= 0 || Gold < amount) return false;
            Gold -= amount;
            OnGoldChanged?.Invoke(Gold);
            return true;
        }
    }
}
