using UnityEngine;

[CreateAssetMenu(fileName = "StunEffect", menuName = "Game/Effects/Stun")]
public class StunEffectData : EffectData
{
    public override RuntimeEffect CreateRuntimeEffect(EnemyAI target)
    {
        return new RuntimeStun(this, target);
    }
}

public class RuntimeStun : RuntimeEffect, IStunEffect
{
    public RuntimeStun(EffectData data, EnemyAI target) : base(data, target) { }

    public override void OnApply()
    {
        Target.isStunned = true;
        Target.rb.linearVelocity = Vector2.zero;
    }

    public override void OnRemove()
    {
        // Chỉ tắt stun nếu không còn source nào khác đang giữ cờ
        foreach (var effect in Target.activeEffects.Values)
        {
            if (effect != this && effect is IStunEffect)
                return;
        }
        Target.isStunned = false;
    }
}