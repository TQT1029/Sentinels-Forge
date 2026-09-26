using System.Collections.Generic;
using UnityEngine;

public class ProjectileRuntimeState
{
    public float CurrentDamage;
    public float DamageMultiplier = 1f;
    public Vector2 Velocity;
    public Transform HomingTarget;

    private readonly Dictionary<string, int> _stats = new Dictionary<string, int>();

    private static readonly Stack<HitActionContext> _contextPool = new Stack<HitActionContext>(32);

    public void Reset(float baseDamage)
    {
        CurrentDamage = baseDamage;
        DamageMultiplier = 1f;
        Velocity = Vector2.zero;
        HomingTarget = null;
        _stats.Clear();
    }

    public void SetStat(string key, int value) => _stats[key] = value;

    public int GetStat(string key, int defaultValue = 0) =>
        _stats.TryGetValue(key, out int val) ? val : defaultValue;

    public void AddStat(string key, int amount) => SetStat(key, GetStat(key) + amount);

    public bool HasStat(string key) => _stats.ContainsKey(key);

    // --- HitActionContext pooling ---

    public static HitActionContext RentContext()
    {
        if (_contextPool.Count > 0)
        {
            HitActionContext ctx = _contextPool.Pop();
            ctx.Reset();
            return ctx;
        }
        return new HitActionContext();
    }

    public static void ReturnContext(HitActionContext ctx)
    {
        ctx.PostHitActions = null;
        _contextPool.Push(ctx);
    }
}