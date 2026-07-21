using UnityEngine;

namespace Game.Shared.Services.Trade
{
    [CreateAssetMenu(fileName = "TradeGoodsConfig", menuName = "Configs/Trade Goods Config")]
    public class TradeGoodsConfigSO : ScriptableObject
    {
        [Tooltip("The default starting capacity for all trade resources.")]
        public int DefaultCapacity = 100;
        
        [Tooltip("The time in seconds to passively fill resources from 0% to 100%. Defaults to 24 hours (86400 seconds).")]
        public float TimeToFillSeconds = 86400f;
    }
}
