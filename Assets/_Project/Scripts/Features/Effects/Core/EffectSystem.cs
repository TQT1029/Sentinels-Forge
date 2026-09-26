using UnityEngine;

public abstract class EffectData : ScriptableObject
{
    public string effectName = "New Effect";
    public string description = "Effect Description";
    public float baseDuration = 3f;
    public int maxStacks = 1;

    public abstract RuntimeEffect CreateRuntimeEffect(EnemyAI target);
}

public abstract class RuntimeEffect
{
    public EffectData Data { get; private set; }
    public EnemyAI Target { get; private set; }
    public float TimeRemaining { get; set; }
    public int CurrentStacks { get; protected set; } = 1;

    public bool IsFinished => TimeRemaining <= 0;

    public RuntimeEffect(EffectData data, EnemyAI target)
    {
        Data = data;
        Target = target;
        TimeRemaining = data.baseDuration;
    }

    public virtual void OnApply() { }
    public virtual void OnTick(float deltaTime) { }
    public virtual void OnRemove() { }

    public virtual void OnStack()
    {
        if (CurrentStacks < Data.maxStacks)
            CurrentStacks++;

        TimeRemaining = Data.baseDuration;
    }
}

// Marker interface: dùng để query "còn stun nào đang active không"
public interface IStunEffect { }