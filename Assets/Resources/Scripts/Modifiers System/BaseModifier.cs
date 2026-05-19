using UnityEngine;
using System;
public class HitActionContext
{
    public bool TerminateProjectile = true; // Mặc định chạm là huỷ viên đạn
    public bool CancelDamage = false;       // Có gây dame không?

    public bool IsHandled = false;  

    public Action PostHitActions; // Hành động bổ sung sau khi xử lý hit xong.
    public bool HasSplit = false;
    public bool HasHomed = false;
}
public abstract class BaseModifier : ScriptableObject
{
    [Header("Base Settings")]
    public string modifierName;
    public string description;

    protected ProjectileSpawner projectileSpawner;

    // Khởi tạo RuntimeState cho đạn khi vừa bắn ra
    public virtual void OnFire(Projectile projectile, ProjectileRuntimeState state) { }

    public virtual void OnUpdate(Projectile projectile, ProjectileRuntimeState state) { }

    // Xử lý va chạm
    public virtual void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext) { }
}