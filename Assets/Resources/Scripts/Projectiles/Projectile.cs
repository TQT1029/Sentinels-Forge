using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    protected IObjectPool<Projectile> managedPool;
    public Rigidbody2D rb;

    protected ProjectileData projectileData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetData(ProjectileData data)
    {
        projectileData = data;
    }

    public void setPool(IObjectPool<Projectile> pool)
    {
        managedPool = pool;
    }

    /// <summary>
    /// Hàm này chạy khi projectile được lấy ra khỏi pool, có thể override để thêm logic khởi tạo riêng cho từng loại projectile
    /// </summary>
    /// <param name="lifeTime"></param>
    public virtual void Init(float lifeTime)
    {
        Invoke(nameof(ReturnToPool), lifeTime);
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