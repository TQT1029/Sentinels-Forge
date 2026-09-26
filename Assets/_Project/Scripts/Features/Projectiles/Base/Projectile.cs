using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System;
using SentinelForge.Core.Interfaces;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour, IPoolable
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

    #region IPoolable Implementation

    public virtual void OnGetFromPool()
    {
        // Chuẩn bị trạng thái sẵn sàng khi lấy ra từ Pool
    }

    public virtual void OnReturnToPool()
    {
        ResetIgnoredCollisions();
        ClearHitTargets();
        CancelInvoke();
    }

    #endregion

    protected virtual void ResetPhysic()
    {
        // Quyết định D-006: Sử dụng Kinematic RB2D thay cho Dynamic để triệt tiêu tải trọng solver khi có 500+ đạn
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f; // Tự tính toán trọng lực mô phỏng trong FixedUpdate
        rb.simulated = true;
    }

    public virtual void Init(float lifeTime)
    {
        fireVelocity = weaponControl != null && weaponControl.weaponData != null ? weaponControl.weaponData.fireVelocity : 10f;
        RuntimeState.Reset(projectileData != null ? projectileData.baseDamage : 10f);
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

    protected virtual void FixedUpdate()
    {
        // Áp dụng gia tốc trọng lực mô phỏng cho đạn Kinematic nếu có gravityScale > 0
        if (projectileData != null && projectileData.gravityScale > 0f && rb.simulated)
        {
            rb.linearVelocity += Physics2D.gravity * projectileData.gravityScale * Time.fixedDeltaTime;
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
        OnReturnToPool();

        if (gameObject.activeInHierarchy)
        {
            if (managedPool != null)
            {
                managedPool.Release(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
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