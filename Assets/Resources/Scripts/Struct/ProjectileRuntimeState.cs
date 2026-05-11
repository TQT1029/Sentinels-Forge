using System.Collections.Generic;
using UnityEngine;

public class ProjectileRuntimeState
{
    // Dữ liệu cơ bản
    public float CurrentDamage;
    public float DamageMultiplier = 1f;
    public Vector2 Velocity; // Vận tốc hiện tại trước khi va chạm. (Được cập nhật liên tục qua Update)
    public Transform HomingTarget;

    // BLACKBOARD: Lưu trữ mọi state của các Modifier (Không GC sau warmup)
    private Dictionary<string, int> customIntStats = new Dictionary<string, int>();

    public void Reset(float baseDamage)
    {
        CurrentDamage = baseDamage;
        DamageMultiplier = 1f;
        Velocity = Vector2.zero;
        HomingTarget = null;
        customIntStats.Clear(); // Tránh GC, chỉ clear data cũ
    }

    // Modifier dùng hàm này để tương tác với RuntimeState
    public void SetStat(string key, int value) => customIntStats[key] = value;
    public int GetStat(string key, int defaultValue = 0)
    {
        return customIntStats.TryGetValue(key, out int val) ? val : defaultValue;
    }
    public void AddStat(string key, int amount) => SetStat(key, GetStat(key) + amount);
}