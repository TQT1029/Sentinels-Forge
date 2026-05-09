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

    [SerializeField] public List<ModifierBase> modifiers; // Danh sách các modifier đang có hiệu lực trên viên đạn này (có thể có 0 modifier)

    protected HashSet<EnemyAI> hitTargets = new HashSet<EnemyAI>(); // Danh sách quái đã gây damage

    [Header("Runtime States (Dữ liệu động)")]
    [HideInInspector] public float currentDamage;
    [HideInInspector] public int pierceCount; // Số lần xuyên còn lại
    [HideInInspector] public int bounceCount; // Số lần nảy còn lại
    [HideInInspector] public int splitCount; // Số lần chia tách còn lại

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetData(ProjectileData data) => projectileData = data;
    public void SetSpawner(ProjectileSpawner spawner) => projectileSpawner = spawner;
    public void SetPool(IObjectPool<Projectile> pool) => managedPool = pool;

    private void ResetPhysic()
    {
        rb.gravityScale = projectileData.gravityScale;
    }

    /// <summary>
    /// Hàm này chạy khi projectile được lấy ra khỏi pool, có thể override để thêm logic khởi tạo riêng cho từng loại projectile
    /// </summary>
    /// <param name="lifeTime">Thời gian tồn tại trước khi bị tự động thu hồi về pool</param>
    public virtual void Init(float lifeTime)
    {
        currentDamage = projectileData.baseDamage + projectileData.GetDamageAfterVariation();
        pierceCount = 0;
        bounceCount = 0;

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

    protected virtual bool ProcessHitEnemy(EnemyAI enemy)
    {
        if (enemy == null || hitTargets.Contains(enemy)) return true; // Đã đánh trúng con này rồi thì không tính damage nữa, nhưng đạn vẫn tiếp tục bay và có thể đánh trúng con khác hoặc tương tác với môi trường

        hitTargets.Add(enemy);

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