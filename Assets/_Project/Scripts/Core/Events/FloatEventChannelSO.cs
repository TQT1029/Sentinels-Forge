using System;
using UnityEngine;

namespace SentinelForge.Core.Events
{
    /// <summary>
    /// Event Channel dạng ScriptableObject mang tham số float.
    /// Dùng cho các sự kiện như OnTowerHealthChanged(float percent), OnTimerUpdated(float seconds)...
    /// </summary>
    [CreateAssetMenu(fileName = "NewFloatEventChannel", menuName = "SentinelForge/Events/Float Event Channel")]
    public class FloatEventChannelSO : ScriptableObject
    {
        [TextArea(2, 4)]
        [SerializeField] private string _description;

        public event Action<float> OnEventRaised;

        /// <summary>
        /// Kích hoạt sự kiện gửi giá trị float đến tất cả listener.
        /// </summary>
        /// <param name="value">Giá trị số thực truyền đi.</param>
        public void RaiseEvent(float value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}
