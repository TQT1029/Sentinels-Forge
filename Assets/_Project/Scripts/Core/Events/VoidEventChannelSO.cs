using System;
using UnityEngine;

namespace SentinelForge.Core.Events
{
    /// <summary>
    /// Event Channel dạng ScriptableObject không tham số (Void signal).
    /// Dùng cho các sự kiện như OnWaveEnded, OnGamePaused, OnGameOver...
    /// </summary>
    [CreateAssetMenu(fileName = "NewVoidEventChannel", menuName = "SentinelForge/Events/Void Event Channel")]
    public class VoidEventChannelSO : ScriptableObject
    {
        [TextArea(2, 4)]
        [SerializeField] private string _description;

        public event Action OnEventRaised;

        /// <summary>
        /// Kích hoạt sự kiện gửi đến tất cả listener đang lắng nghe.
        /// </summary>
        public void RaiseEvent()
        {
            if (OnEventRaised == null)
            {
                #if UNITY_EDITOR
                Debug.Log($"[VoidEventChannelSO] '{name}' được kích hoạt nhưng không có listener nào lắng nghe.");
                #endif
                return;
            }

            OnEventRaised.Invoke();
        }
    }
}
