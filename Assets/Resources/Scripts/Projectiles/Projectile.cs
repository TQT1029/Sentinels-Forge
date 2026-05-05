using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    private IObjectPool<Projectile> managedPool;
    public Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        RotateTowardsVelocity();
    }

    public void setPool(IObjectPool<Projectile> pool)
    {
        managedPool = pool;
    }

    // Tách riêng logic khởi tạo vật lý và đếm lùi để Spawner chủ động gọi
    public void Init(float lifeTime)
    {
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        Invoke(nameof(ReturnToPool), lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
        // Xóa lệnh ReturnToPool() ở đây để tránh lỗi gọi Release 2 lần.
    }

    private void ReturnToPool()
    {
        // Kiểm tra xem object còn đang active không trước khi release (phòng ngừa lỗi)
        if (gameObject.activeInHierarchy)
        {
            managedPool.Release(this);
        }
    }

    private void RotateTowardsVelocity()
    {

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}