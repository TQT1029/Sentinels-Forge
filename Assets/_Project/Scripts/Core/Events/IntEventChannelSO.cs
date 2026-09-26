using System;
using UnityEngine;

namespace SentinelForge.Core.Events
{
    /// <summary>
    /// Event Channel dạng ScriptableObject mang tham số int.
    /// Dùng cho các sự kiện như OnWaveStarted(int waveIndex), OnGoldChanged(int amount)...
    /// </summary>
    [CreateAssetMenu(fileName = "NewIntEventChannel", menuName = "SentinelForge/Events/Int Event Channel")]
    public class IntEventChannelSO : ScriptableObject
    {
        [TextArea(2, 4)]
        [SerializeField] private string _description;

        public event Action<int> OnEventRaised;

        /// <summary>
        /// Kích hoạt sự kiện gửi giá trị int đến tất cả listener.
        /// </summary>
        /// <param name="value">Giá trị số nguyên truyền đi.</param>
        public void RaiseEvent(int value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}
