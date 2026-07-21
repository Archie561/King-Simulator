using System;

namespace Game.Shared.Services.Trade
{
    public interface ITradeGoodsService
    {
        int GetResourceAmount(ResourceType type);
        int GetResourceCapacity(ResourceType type);

        event Action<ResourceType, int> OnResourceChanged;
        event Action<ResourceType, int> OnCapacityChanged;

        void AddResource(ResourceType type, int amount);
        bool TrySpendResource(ResourceType type, int amount);
        void SetCapacity(ResourceType type, int newCapacity);
    }
}
