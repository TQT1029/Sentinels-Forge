using UnityEngine;

// Lưu trữ thông số thiết lập từ Inspector
public abstract class EffectData : ScriptableObject
{
    public string effectName = "New Effect";
    public string description = "Effect Description";
    public float baseDuration = 3f;
    public int maxStacks = 1;
    // Gọi để sinh ra một bản sao (Instance) gắn lên quái vật
    public abstract RuntimeEffect CreateRuntimeEffect(EnemyAI target);
}

// Lưu trữ trạng thái động của hiệu ứng trên 1 con quái cụ thể
public abstract class RuntimeEffect
{
    public EffectData Data { get; private set; }
    public EnemyAI Target { get; private set; }
    public float TimeRemaining { get; set; } // Dùng để đếm ngược
    public int CurrentStacks { get; protected set; } = 1;

    public bool IsFinished => TimeRemaining <= 0;

    public RuntimeEffect(EffectData data, EnemyAI target)
    {
        Data = data;
        Target = target;
        TimeRemaining = data.baseDuration;
    }

    // Chạy 1 lần khi hiệu ứng vừa áp dụng lên quái
    public virtual void OnApply() { }

    // Chạy mỗi frame (Dùng cho dính Độc/Cháy trừ máu liên tục)
    public virtual void OnTick(float deltaTime) { }

    // Chạy 1 lần khi hết thời gian hoặc quái chết
    public virtual void OnRemove() { }

    // Xử lý khi Effect bị apply đè lên
    public virtual void OnStack()
    {
        if (CurrentStacks < Data.maxStacks)
        {
            CurrentStacks++;
        }

        // Default behavior: Refresh duration. Các effect con có thể override để cộng dồn.
        TimeRemaining = Data.baseDuration;
    }
}