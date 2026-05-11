using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ApplyStatusModifier", menuName = "Game/Modifiers/ApplyStatusModifier")]
public class ApplyStatusModifier : ModifierBase
{
    public List<EffectData> effectToApply = new List<EffectData>(); // Kéo file StunEffectData từ Unity Editor vào đây

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy != null)
        {
            // Bơm thẳng hiệu ứng vào quái
            foreach (var effect in effectToApply)
            {
                hitData.Enemy.AddEffect(effect);
            }
        }
    }
}
