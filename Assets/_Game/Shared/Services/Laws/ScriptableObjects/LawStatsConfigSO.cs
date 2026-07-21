using UnityEngine;

namespace Game.Shared.Services.Laws
{
    [CreateAssetMenu(fileName = "LawStatsConfig", menuName = "Configs/Law Stats Config")]
    public class LawStatsConfigSO : ScriptableObject
    {
        [Tooltip("The default starting level for all Kingdom Stats.")]
        public int DefaultStartingLevel = 1;
        
        [Tooltip("The points required to reach level 2. Scales up per level thereafter.")]
        public int BasePointsForNextLevel = 100;
    }
}
