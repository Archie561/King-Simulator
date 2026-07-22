using System;
using System.Collections.Generic;

namespace Game.Shared.Services.TradeGoods
{
    public class TradeGoodsService : ITradeGoodsService
    {
        private readonly Dictionary<ResourceType, int> _resources = new();
        private readonly Dictionary<ResourceType, int> _capacities = new();
        private readonly TradeGoodsConfigSO _config;

        public event Action<ResourceType, int> OnResourceChanged;
        public event Action<ResourceType, int> OnCapacityChanged;

        public TradeGoodsService(TradeGoodsConfigSO config)
        {
            _config = config;

            // Initialize defaults based on the injected configuration
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                _resources[type] = 0;
                _capacities[type] = _config != null ? _config.DefaultCapacity : 100;
            }
        }

        public int GetResourceAmount(ResourceType type)
        {
            return _resources.GetValueOrDefault(type, 0);
        }

        public int GetResourceCapacity(ResourceType type)
        {
            return _capacities.GetValueOrDefault(type, 0);
        }

        public void AddResource(ResourceType type, int amount)
        {
            if (amount <= 0) return;
            
            int capacity = GetResourceCapacity(type);
            int current = GetResourceAmount(type);
            
            _resources[type] = Math.Min(current + amount, capacity);
            OnResourceChanged?.Invoke(type, _resources[type]);
        }

        public bool TrySpendResource(ResourceType type, int amount)
        {
            if (amount <= 0) return false;
            
            int current = GetResourceAmount(type);
            if (current < amount) return false;
            
            _resources[type] = current - amount;
            OnResourceChanged?.Invoke(type, _resources[type]);
            return true;
        }

        public void SetCapacity(ResourceType type, int newCapacity)
        {
            if (newCapacity < 0) return;
            _capacities[type] = newCapacity;
            OnCapacityChanged?.Invoke(type, newCapacity);

            // If capacity shrinks below current amount, cap the resource
            int current = GetResourceAmount(type);
            if (current > newCapacity)
            {
                _resources[type] = newCapacity;
                OnResourceChanged?.Invoke(type, newCapacity);
            }
        }
    }
}
