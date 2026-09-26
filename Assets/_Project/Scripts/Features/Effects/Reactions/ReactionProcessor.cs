using System;
using System.Collections.Generic;
using UnityEngine;

namespace SentinelForge.Features.Effects.Reactions
{
    /// <summary>
    /// Bộ xử lý phản ứng nguyên tố. Lắng nghe và kiểm tra khi có hiệu ứng mới được thêm vào kẻ địch.
    /// </summary>
    public static class ReactionProcessor
    {
        private static readonly List<ElementalReactionSO> _registeredReactions = new();

        public static event Action<ElementalReactionSO, EnemyAI> OnReactionTriggered;

        public static void RegisterReaction(ElementalReactionSO reaction)
        {
            if (reaction != null && !_registeredReactions.Contains(reaction))
            {
                _registeredReactions.Add(reaction);
            }
        }

        public static void ClearReactions()
        {
            _registeredReactions.Clear();
        }

        /// <summary>
        /// Kiểm tra xem việc áp dụng newEffect lên target có kích hoạt phản ứng với bất kỳ effect nào đang có sẵn không.
        /// </summary>
        /// <param name="target">Kẻ địch bị áp hiệu ứng.</param>
        /// <param name="newEffect">Hiệu ứng vừa được áp dụng.</param>
        /// <returns>True nếu có phản ứng xảy ra.</returns>
        public static bool CheckAndTriggerReaction(EnemyAI target, EffectData newEffect)
        {
            if (target == null || newEffect == null || target.activeEffects == null) return false;

            foreach (var activeKvp in target.activeEffects)
            {
                EffectData activeEffectData = activeKvp.Key;
                if (activeEffectData == newEffect) continue;

                foreach (var reaction in _registeredReactions)
                {
                    if (reaction.Matches(activeEffectData, newEffect))
                    {
                        TriggerReaction(reaction, target, activeEffectData, newEffect);
                        return true;
                    }
                }
            }

            return false;
        }

        private static void TriggerReaction(ElementalReactionSO reaction, EnemyAI target, EffectData existingEffect, EffectData incomingEffect)
        {
            Debug.Log($"[ElementalReaction] '{reaction.reactionName}' kích hoạt trên '{target.name}'!");

            // 1. Gây thêm sát thương phản ứng nếu có
            if (reaction.bonusDamageMultiplier > 1f)
            {
                target.TakeDamage(new DamageInfo
                {
                    damage = 20f * reaction.bonusDamageMultiplier,
                    isCritical = true
                });
            }

            // 2. Tạo VFX nếu có
            if (reaction.vfxPrefab != null)
            {
                UnityEngine.Object.Instantiate(reaction.vfxPrefab, target.transform.position, Quaternion.identity);
            }

            // 3. Tiêu hao (hủy) các effect nếu phản ứng yêu cầu
            if (reaction.consumesBothEffects)
            {
                if (target.activeEffects.TryGetValue(existingEffect, out var activeRuntime))
                {
                    activeRuntime.TimeRemaining = 0f; // Sẽ tự dọn ở HandleEffects
                }
            }

            OnReactionTriggered?.Invoke(reaction, target);
        }
    }
}
