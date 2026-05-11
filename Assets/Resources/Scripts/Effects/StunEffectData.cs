using UnityEngine;

[CreateAssetMenu(fileName = "StunEffect", menuName = "Game/Effects/Stun")]
public class StunEffectData : EffectData
{
    public override RuntimeEffect CreateRuntimeEffect(EnemyAI target)
    {
        return new RuntimeStun(this, target);
    }
}

public class RuntimeStun : RuntimeEffect
{
    public RuntimeStun(EffectData data, EnemyAI target) : base(data, target) { }

    public override void OnApply()
    {
        Target.isStunned = true; 
        Target.rb.linearVelocity = Vector2.zero; 
        Debug.Log($"[Effect] {Target.name} bị choáng!");
    }

    public override void OnRemove()
    {
        Target.isStunned = false; // Tắt cờ choáng trả lại tự do
        Debug.Log($"[Effect] {Target.name} hết choáng.");
    }
}