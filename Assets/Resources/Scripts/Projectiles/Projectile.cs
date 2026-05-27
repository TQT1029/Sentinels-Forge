using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    protected IObjectPool<Projectile> managedPool;
    public Rigidbody2D rb { get; private set; }
    public Collider2D projCollider { get; private set; }
    public ProjectileData projectileData { get; private set; }
    public ProjectileSpawner projectileSpawner { get; private set; }
    protected WeaponControl weaponControl;

    [SerializeField] public List<BaseModifier> modifiers;

    public HashSet<EnemyAI> hitTargets = new HashSet<EnemyAI>();

    public ProjectileRuntimeState RuntimeState { get; private set; } = new ProjectileRuntimeState();
    public List<Collider2D> IgnoredColliders { get; } = new List<Collider2D>(10);

    protected float fireVelocity;
    protected bool isCriticalHit = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        projCollider = GetComponent<Collider2D>();
    }

    public void SetProjectileData(ProjectileData data) => projectileData = data;
    public void SetSpawner(ProjectileSpawner spawner) => projectileSpawner = spawner;
    public void SetWeaponControl(WeaponControl control) => weaponControl = control;
    public void SetPool(IObjectPool<Projectile> pool) => managedPool = pool;

    protected virtual void ResetPhysic()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = projectileData.gravityScale;
        rb.simulated = true;
    }

    public virtual void Init(float lifeTime)
    {
        fireVelocity = weaponControl.weaponData.fireVelocity;
        RuntimeState.Reset(projectileData.baseDamage);
        ClearHitTargets();
        ResetPhysic();

        if (modifiers != null)
        {
            foreach (var mod in modifiers) mod.OnFire(this, RuntimeState);
        }

        Invoke(nameof(ReturnToPool), lifeTime);
    }

    protected virtual void Update()
    {
        RuntimeState.Velocity = rb.linearVelocity;

        if (modifiers != null)
        {
            foreach (var mod in modifiers) mod.OnUpdate(this, RuntimeState);
        }
    }

    public virtual bool ProcessHit(HitData hitData)
    {
        if (hitData.Enemy != null)
        {
            if (hitTargets.Contains(hitData.Enemy)) return true;
            hitTargets.Add(hitData.Enemy);
        }

        HitActionContext hitContext = ProjectileRuntimeState.RentContext();

        try
        {
            if (modifiers != null)
            {
                foreach (var mod in modifiers)
                    mod.OnHit(this, RuntimeState, hitData, hitContext);
            }

            if (hitData.Enemy != null && !hitContext.CancelDamage)
            {
                CalculateDamage(rb.linearVelocity.magnitude);
                hitData.Enemy.TakeDamage(new DamageInfo { damage = RuntimeState.CurrentDamage, isCritical = isCriticalHit });
            }

            hitContext.PostHitActions?.Invoke();
        }
        finally
        {
            // Đảm bảo context luôn trả về pool kể cả khi exception xảy ra
            ProjectileRuntimeState.ReturnContext(hitContext);
        }

        return !hitContext.TerminateProjectile;
    }

    public virtual void ProcessImediate() { }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public virtual void ReturnToPool()
    {
        ResetIgnoredCollisions();

        if (gameObject.activeInHierarchy)
            managedPool.Release(this);
    }

    private void ResetIgnoredCollisions()
    {
        int count = IgnoredColliders.Count;
        for (int i = 0; i < count; i++)
        {
            if (IgnoredColliders[i] != null)
                Physics2D.IgnoreCollision(projCollider, IgnoredColliders[i], false);
        }
        IgnoredColliders.Clear();
    }

    protected virtual void CalculateDamage(float impactVelocity)
    {
        float speedRatio = impactVelocity / fireVelocity;
        if (speedRatio < 0.15f) speedRatio = 0;

        isCriticalHit = RandomUtils.ChancePercent(projectileData.criticalChance * 100);
        if (isCriticalHit) speedRatio *= projectileData.criticalMultiplier;

        float rawDamage = projectileData.baseDamage * speedRatio * RuntimeState.DamageMultiplier;
        RuntimeState.CurrentDamage = (float)Math.Round(rawDamage, 2);
    }

    public void ClearHitTargets() => hitTargets.Clear();
}