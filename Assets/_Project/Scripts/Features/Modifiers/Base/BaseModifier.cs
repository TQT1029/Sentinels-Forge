using UnityEngine;
using System;

public class HitActionContext
{
    public bool TerminateProjectile = true;
    public bool CancelDamage = false;
    public bool IsHandled = false;
    public bool HasSplit = false;
    public bool HasHomed = false;

    public Action PostHitActions;

    public void Reset()
    {
        TerminateProjectile = true;
        CancelDamage = false;
        IsHandled = false;
        HasSplit = false;
        HasHomed = false;
        PostHitActions = null;
    }
}

public abstract class BaseModifier : ScriptableObject
{
    [Header("Base Settings")]
    public string modifierName;
    public string description;

    public virtual void OnFire(Projectile projectile, ProjectileRuntimeState state) { }
    public virtual void OnUpdate(Projectile projectile, ProjectileRuntimeState state) { }
    public virtual void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext) { }

    /// <summary>
    /// Gọi khi đạn con được tạo từ Split, để mỗi modifier tự copy state của mình sang đạn mới.
    /// Override trong subclass nếu modifier đó có runtime state cần kế thừa.
    /// </summary>
    public virtual void InheritState(ProjectileRuntimeState source, ProjectileRuntimeState destination) { }
}