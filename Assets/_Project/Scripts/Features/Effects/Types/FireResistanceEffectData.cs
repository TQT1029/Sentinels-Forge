using UnityEngine;

[CreateAssetMenu(fileName = "FireResistanceEffect", menuName = "Game/Effects/Fire Resistance")]
public class FireResistanceEffectData : EffectData
{
    [Tooltip("Tỷ lệ giảm sát thương mỗi stack (0.2 = 20%)")]
    public float damageReductionPerStack = 0.2f;

    public override RuntimeEffect CreateRuntimeEffect(EnemyAI target)
    {
        return new RuntimeFireResistance(this, target);
    }
}

public class RuntimeFireResistance : RuntimeEffect
{
    private FireResistanceEffectData resistData;

    public RuntimeFireResistance(EffectData data, EnemyAI target) : base(data, target)
    {
        resistData = data as FireResistanceEffectData;
    }

    public float GetDamageReduction()
    {
        return Mathf.Clamp01(CurrentStacks * resistData.damageReductionPerStack);
    }

    public override void OnStack()
    {
        if (CurrentStacks < resistData.maxStacks)
        {
            CurrentStacks++;
        }
        // Resistance thường chỉ refresh thời gian, không cộng dồn thời gian để tránh lạm dụng
        TimeRemaining = resistData.baseDuration;
    }
}