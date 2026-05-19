using UnityEngine;

[CreateAssetMenu(fileName = "FireEffectData", menuName = "Game/Effects/Fire")]
public class FireEffectData : EffectData
{
    public float damagePerTick = 5f;
    public float tickInterval = 1f;
    public bool addDurationOnStack = false;

    public FireResistanceEffectData resistanceData;

    public override RuntimeEffect CreateRuntimeEffect(EnemyAI target)
    {
        return new RuntimeFire(this, target);
    }
}

public class RuntimeFire : RuntimeEffect
{
    private FireEffectData fireData;
    private float tickTimer = 0f;

    public RuntimeFire(EffectData data, EnemyAI target) : base(data, target)
    {
        fireData = data as FireEffectData;
    }

    public override void OnStack()
    {
        if (CurrentStacks < fireData.maxStacks)
        {
            CurrentStacks++;
        }

        // Quyết định cộng dồn thời gian hay chỉ refresh
        if (fireData.addDurationOnStack)
            TimeRemaining += fireData.baseDuration;
        else
            TimeRemaining = fireData.baseDuration;
    }

    public override void OnTick(float deltaTime)
    {
        tickTimer += deltaTime;
        if (tickTimer >= fireData.tickInterval)
        {
            tickTimer -= fireData.tickInterval;
            ApplyFireDamage();
        }
    }

    private void ApplyFireDamage()
    {
        float finalDamage = fireData.damagePerTick * CurrentStacks;

        // Tương tác với hệ thống kháng
        if (fireData.resistanceData != null)
        {
            var resistEffect = Target.GetEffect(fireData.resistanceData) as RuntimeFireResistance;
            if (resistEffect != null)
            {
                float reduction = resistEffect.GetDamageReduction();
                finalDamage *= (1f - reduction);
            }
        }

        if (finalDamage > 0)
        {
            Target.TakeDamage(finalDamage);
            //Debug.Log($"Applied {finalDamage} fire damage to {Target.name} (Stacks: {CurrentStacks})");
        }
    }
}