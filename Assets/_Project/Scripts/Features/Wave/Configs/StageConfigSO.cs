using System.Collections.Generic;
using UnityEngine;

namespace SentinelForge.Features.Wave.Configs
{
    /// <summary>
    /// Cấu hình Stage bao gồm danh sách các Wave và hệ số nhân độ khó.
    /// </summary>
    [CreateAssetMenu(fileName = "NewStageConfig", menuName = "SentinelForge/Wave/Stage Config")]
    public class StageConfigSO : ScriptableObject
    {
        [Header("Stage Info")]
        public string stageName = "Stage 1";
        public int stageNumber = 1;
        public List<WaveConfigSO> waves = new();

        [Header("Difficulty Multipliers")]
        public float hpMultiplier = 1f;
        public float speedMultiplier = 1f;
        public float damageMultiplier = 1f;

        /// <summary>
        /// Lấy cấu hình Wave tương ứng với số thứ tự Wave (bắt đầu từ 1).
        /// </summary>
        public WaveConfigSO GetWaveConfig(int waveIndex)
        {
            if (waves == null || waves.Count == 0) return null;

            int zeroBasedIndex = waveIndex - 1;
            if (zeroBasedIndex >= 0 && zeroBasedIndex < waves.Count)
            {
                return waves[zeroBasedIndex];
            }

            // Nếu vượt quá số wave thiết kế sẵn, lặp lại wave cuối cùng với scaling
            return waves[waves.Count - 1];
        }
    }
}
