using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 4f;

    private float damage;
    private IObjectPool<EnemyProjectile> managedPool;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(IObjectPool<EnemyProjectile> pool) => managedPool = pool;

    private void Update()
    {
        // Xoay đầu đạn (mũi tên, quả cầu lửa) về hướng bay
        float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    public void Fire(Vector2 direction, float dmg)
    {
        damage = dmg;
        rb.linearVelocity = direction * speed;
        rb.gravityScale = Random.Range(0.8f, 1.2f); // Tạo hiệu ứng bay lượn tự nhiên hơn cho đạn

        Invoke(nameof(ReturnToPool), lifeTime);
    }

    // Ưu tiên dùng OnTriggerEnter2D kết hợp Rigidbody2D (Kinematic) cho quái để nhẹ CPU
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(GameConstants.TOWER_TAG))
        {
             WaveManager.Instance.TowerController.TakeDamage(damage);
            //Debug.Log($"[EnemyProjectile] Hit the tower for {damage} damage!");
            ReturnToPool();
        }
        else if (collision.gameObject.CompareTag(GameConstants.GROUND_TAG))
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