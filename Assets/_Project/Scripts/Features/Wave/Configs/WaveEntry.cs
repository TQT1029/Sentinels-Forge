using System;
using UnityEngine;

namespace SentinelForge.Features.Wave.Configs
{
    /// <summary>
    /// Định nghĩa một nhóm quái vật xuất hiện trong một Wave cụ thể.
    /// </summary>
    [Serializable]
    public class WaveEntry
    {
        [Tooltip("Dữ liệu chỉ số của loại quái này")]
        public EnemyData enemyData;

        [Tooltip("Prefab quái sẽ được spawn")]
        public EnemyAI enemyPrefab;

        [Min(1), Tooltip("Số lượng quái trong nhóm")]
        public int count = 5;

        [Min(0.1f), Tooltip("Khoảng thời gian giãn cách giữa mỗi con quái spawn ra (giây)")]
        public float spawnInterval = 1f;

        [Min(0f), Tooltip("Thời gian chờ trước khi spawn nhóm tiếp theo (giây)")]
        public float delayAfterGroup = 2f;
    }
}
