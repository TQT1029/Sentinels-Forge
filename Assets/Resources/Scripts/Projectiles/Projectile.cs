using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    protected IObjectPool<Projectile> managedPool;
    public Rigidbody2D rb { get; private set; }

    public ProjectileData projectileData { get; private set; }
    public ProjectileSpawner projectileSpawner { get; private set; }
    protected WeaponControl weaponControl;

    [SerializeField] public List<ModifierBase> modifiers; // Danh sách các modifier đang có hiệu lực 

    public HashSet<EnemyAI> hitTargets = new HashSet<EnemyAI>(); // Danh sách quái đã gây damage

    public ProjectileRuntimeState RuntimeState { get; private set; } = new ProjectileRuntimeState();

    protected float fireVelocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetProjectileData(ProjectileData data) => projectileData = data;
    public void SetSpawner(ProjectileSpawner spawner) => projectileSpawner = spawner;
    public void SetWeaponControl(WeaponControl control) => weaponControl = control;
    public void SetPool(IObjectPool<Projectile> pool) => managedPool = pool;

    protected virtual void ResetPhysic()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = projectileData.gravityScale;
        rb.simulated = true;
    }

    /// <summary>
    /// Hàm này chạy khi projectile được lấy ra khỏi pool và tái sử dụng
    /// </summary>
    /// <param name="lifeTime">Thời gian tồn tại trước khi bị tự động thu hồi về pool</param>
    public virtual void Init(float lifeTime)
    {
        fireVelocity = weaponControl.weaponData.fireVelocity;

        RuntimeState.Reset(projectileData.baseDamage);

        ClearHitTargets();

        ResetPhysic();

        // Kích hoạt tất cả modifier
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
            foreach (var mod in modifiers)
            {
                mod.OnUpdate(this, RuntimeState);
            }
        }
    }

    /// <summary>
    /// Bất kể là đạn Raycast hay đạn Collider, khi va chạm quái đều gọi hàm này.
    /// </summary>
    /// <returns> Nếu đạn không tự huỷ thì trả về true, ngược lại false </returns>
    public virtual bool ProcessHit(HitData hitData)
    {
        if (hitData.Enemy != null)
        {
            if (hitTargets.Contains(hitData.Enemy)) return true; // Tránh hit đúp
            hitTargets.Add(hitData.Enemy);
        }

        // Tạo context cho cú hit này
        HitActionContext hitContext = new HitActionContext();

        // Chạy qua tât cả Modifier để xào nấu Context và RuntimeState
        if (modifiers != null)
        {
            foreach (var mod in modifiers)
            {
                mod.OnHit(this, RuntimeState, hitData, hitContext);
            }
        }

        // Xử lý Hậu quả (Resolution) sau khi các modifier đã chốt
        if (hitData.Enemy != null && !hitContext.CancelDamage)
        {
            CalculateDamage(rb.linearVelocity.magnitude);
            hitData.Enemy.TakeDamage(RuntimeState.CurrentDamage);
        }

        hitContext.PostHitActions?.Invoke();

        return !hitContext.TerminateProjectile;
    }


    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Hàm này chạy khi projectile cần được trả về pool, có thể override để thêm logic dọn dẹp riêng cho từng loại projectile
    /// </summary>
    public virtual void ReturnToPool()
    {
        // Kiểm tra xem object còn đang active không trước khi release (phòng ngừa lỗi)
        if (gameObject.activeInHierarchy)
        {
            managedPool.Release(this);
        }
    }

    /// <summary>
    /// Tính toán damage dựa trên vận tốc va chạm của projectile.
    /// </summary>
    /// <param name="impactVelocity">Vận tốc va chạm của projectile</param>
    protected virtual void CalculateDamage(float impactVelocity)
    {
        float speedRatio = impactVelocity / fireVelocity;
        RuntimeState.CurrentDamage = (float)Math.Round(projectileData.baseDamage * speedRatio * RuntimeState.DamageMultiplier, 2);
    }


    public void ClearHitTargets()
    {
        hitTargets.Clear();
    }
}