using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 4f;

    private float damage;
    private IObjectPool<EnemyProjectile> managedPool;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(IObjectPool<EnemyProjectile> pool) => managedPool = pool;

    public void Fire(Vector2 direction, float dmg)
    {
        damage = dmg;
        rb.linearVelocity = direction * speed;

        // Xoay đầu đạn (mũi tên, quả cầu lửa) về hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Invoke(nameof(ReturnToPool), lifeTime);
    }

    // Ưu tiên dùng OnTriggerEnter2D kết hợp Rigidbody2D (Kinematic) cho quái để nhẹ CPU
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Tower")) // Giả sử Căn cứ có tag là Tower
        {
            // Base/Tower.Instance.TakeDamage(damage); 
            ReturnToPool();
        }
        else if (collision.CompareTag("Ground"))
        {
            ReturnToPool();
        }
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
            managedPool.Release(this);
    }
}