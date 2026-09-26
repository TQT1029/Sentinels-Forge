using UnityEngine;

namespace SentinelForge.Features.Effects.Reactions
{
    /// <summary>
    /// Định nghĩa phản ứng nguyên tố giữa hai hiệu ứng (Ví dụ: Lửa + Đóng băng -> Tan chảy / Melt gây x1.5 sát thương).
    /// </summary>
    [CreateAssetMenu(fileName = "NewElementalReaction", menuName = "SentinelForge/Combat/Elemental Reaction")]
    public class ElementalReactionSO : ScriptableObject
    {
        [Header("Effects Combination")]
        public EffectData primaryEffect;
        public EffectData secondaryEffect;

        [Header("Reaction Properties")]
        public string reactionName = "New Reaction";
        public float bonusDamageMultiplier = 1.5f;
        public GameObject vfxPrefab;
        public bool consumesBothEffects = true;
        public BaseModifier bonusModifier;

        /// <summary>
        /// Kiểm tra xem cặp hiệu ứng truyền vào có kích hoạt phản ứng này không (hỗ trợ cả 2 chiều A+B hoặc B+A).
        /// </summary>
        public bool Matches(EffectData a, EffectData b)
        {
            if (a == null || b == null || primaryEffect == null || secondaryEffect == null)
                return false;

            return (a == primaryEffect && b == secondaryEffect) ||
                   (a == secondaryEffect && b == primaryEffect);
        }
    }
}
