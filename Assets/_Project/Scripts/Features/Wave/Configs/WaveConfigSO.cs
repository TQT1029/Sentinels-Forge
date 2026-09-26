using System.Collections.Generic;
using UnityEngine;

namespace SentinelForge.Features.Wave.Configs
{
    /// <summary>
    /// Cấu hình Data-driven cho một Wave cụ thể (theo quyết định D-010).
    /// </summary>
    [CreateAssetMenu(fileName = "NewWaveConfig", menuName = "SentinelForge/Wave/Wave Config")]
    public class WaveConfigSO : ScriptableObject
    {
        [Header("Wave Info")]
        public int waveNumber = 1;
        public float delayBeforeWave = 3f;
        public bool isBossWave = false;

        [Header("Guaranteed Drops")]
        [Tooltip("Vật phẩm rơi chắc chắn (ví dụ: Blueprint từ Boss)")]
        public BaseItemSO guaranteedDrop;

        [Header("Wave Groups")]
        public List<WaveEntry> entries = new();

        /// <summary>
        /// Tổng số lượng quái vật cần tiêu diệt trong Wave này.
        /// </summary>
        public int TotalEnemiesCount
        {
            get
            {
                int total = 0;
                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        if (entry != null) total += entry.count;
                    }
                }
                return total;
            }
        }
    }
}
