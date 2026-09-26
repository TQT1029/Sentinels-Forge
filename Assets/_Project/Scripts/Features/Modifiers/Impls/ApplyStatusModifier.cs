using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ApplyStatusModifier", menuName = "Game/Modifiers/ApplyStatusModifier")]
public class ApplyStatusModifier : BaseModifier
{
    public List<EffectData> effectsToApply = new List<EffectData>();

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy == null) return;

        foreach (var effect in effectsToApply)
            hitData.Enemy.AddEffect(effect);
    }
}