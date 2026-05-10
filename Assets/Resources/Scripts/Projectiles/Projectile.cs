using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    protected IObjectPool<Projectile> managedPool;
    public Rigidbody2D rb { get; private set; }

    public ProjectileData projectileData { get; private set; }
    protected ProjectileSpawner projectileSpawner;
    protected WeaponControl weaponControl;

    [SerializeField] public List<ModifierBase> modifiers; // Danh sách các modifier đang có hiệu lực 

    protected HashSet<EnemyAI> hitTargets = new HashSet<EnemyAI>(); // Danh sách quái đã gây damage

    [Header("Runtime States (Dữ liệu động)")]
    [HideInInspector] public float currentDamage;
    [HideInInspector] public int pierceCount; // Số lần xuyên còn lại
    [HideInInspector] public int bounceCount; // Số lần nảy còn lại
    [HideInInspector] public int splitCount; // Số lần chia tách còn lại

    [HideInInspector] public float damageMultiplier = 1f; // Hệ số nhân sát thương, có thể bị modifier thay đổi tạm thời
    protected float launchVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetProjectileData(ProjectileData data) => projectileData = data;
    public void SetSpawner(ProjectileSpawner spawner) => projectileSpawner = spawner;
    public void SetWeaponControl(WeaponControl control) => weaponControl = control;
    public void SetPool(IObjectPool<Projectile> pool) => managedPool = pool;

    protected virtual void ResetPhysic()
    {
        launchVelocity = weaponControl.weaponData.launchVelocity;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = projectileData.gravityScale;
        rb.simulated = true;
    }

    /// <summary>
    /// Hàm này chạy khi projectile được lấy ra khỏi pool, có thể override để thêm logic khởi tạo riêng cho từng loại projectile
    /// </summary>
    /// <param name="lifeTime">Thời gian tồn tại trước khi bị tự động thu hồi về pool</param>
    public virtual void Init(float lifeTime)
    {
        pierceCount = 0;
        bounceCount = 0;
        splitCount = 0;
        damageMultiplier = 1f;

        // Xóa danh sách hit targets trước khi tái sử dụng
        hitTargets.Clear();
        ResetPhysic();

        if (modifiers != null)
        {
            foreach (var mod in modifiers)
            {
                mod.OnLaunch(this);
            }
        }

        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void Update()
    {

        if (modifiers != null)
        {
            foreach (var mod in modifiers)
            {
                mod.OnUpdate(this);
            }
        }
    }

    protected virtual bool ProcessHitEnemy(EnemyAI enemy, float impactVelocity)
    {
        if (enemy == null || hitTargets.Contains(enemy)) return true; // Đã đánh trúng con này rồi thì không tính damage nữa, nhưng đạn vẫn tiếp tục bay và có thể đánh trúng con khác hoặc tương tác với môi trường

        hitTargets.Add(enemy);

        float speedRatio = impactVelocity / launchVelocity;
        currentDamage = projectileData.baseDamage * speedRatio * damageMultiplier;

        if (modifiers != null)
        {
            foreach (var mod in modifiers)
            {
                if (mod.OnHitEnemy(this, enemy))
                {
                    return true; // Đạn đã bắn trúng quái tiếp tục bay và chuyển sang xử lý modifier
                }
            }
        }

        enemy.TakeDamage(currentDamage); // Nếu không có modifier nào xử lý, áp dụng sát thương bình thường

        return false; // Đạn dừng lại không bay nữa (Xử lý cuối để cho class con)

    }

    public virtual bool ProcessEnvironmentHit(RaycastHit2D hit)
    {
        if (modifiers != null)
        {
            foreach (var mod in modifiers)
            {
                if (mod.OnEnvironmentHit(this, hit))
                {
                    return true; // Đã tương tác với môi trường và modifier xử lý thành công, đạn sẽ tiếp tục bay và chuyển sang xử lý modifier
                }
            }
        }
        return false; // Không có modifier nào cản lại, đạn sẽ dính vào tường hoặc nổ
    }


    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// Hàm này chạy khi projectile cần được trả về pool, có thể override để thêm logic dọn dẹp riêng cho từng loại projectile
    /// </summary>
    protected virtual void ReturnToPool()
    {
        // Kiểm tra xem object còn đang active không trước khi release (phòng ngừa lỗi)
        if (gameObject.activeInHierarchy)
        {
            managedPool.Release(this);
        }
    }


}