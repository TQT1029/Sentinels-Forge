using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyAI : MonoBehaviour
{
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    protected Rigidbody2D rb;
    protected Transform towerTransform;

    private IObjectPool<EnemyAI> managedPool;

    protected float currentHealth;
    protected float percentHealth => currentHealth / enemyData.maxHealth;


    protected float checkingFrequency = 10f; // Số lần check trong 1 giây
    protected float checkingTime = 0f;

    protected float previousAttackTime = 0f;

    protected bool isInvincible = false;
    public bool IsDead => currentHealth <= 0;


    //protected float percentHealth => (float)enemyData.CurrentHealth / enemyData.MaxHealth;
    protected virtual void Awake()
    {
        towerTransform = WaveManager.Instance.TowerTransform;
        rb = GetComponent<Rigidbody2D>();        
    }

    public void SetPool(IObjectPool<EnemyAI> pool)
    {
        managedPool = pool;
    }


    public virtual void ResetStats()
    {
        currentHealth = enemyData.maxHealth;
        isInvincible = false;
        checkingTime = 0f;
    }

    protected virtual void Update()
    {
        checkingTime += Time.deltaTime;

        if (checkingTime >= 1f / checkingFrequency)
        {
            checkingTime = 0f;

            if (IsInAttackRange())
            {
                if (Time.time - previousAttackTime >= enemyData.attackCooldown)
                {
                    Attack();
                    previousAttackTime = Time.time;
                }

                rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            }
            else
            {
                Move();
            }
        }
    }

    protected virtual void FixedUpdate() { }

    protected virtual void Move()
    {
        rb.linearVelocity = new Vector2(-enemyData.moveSpeed, rb.linearVelocityY);
    }


    protected virtual bool IsInAttackRange()
    {
        Vector2 direction = towerTransform.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, enemyData.attackRange, LayerMask.GetMask("Tower"));

        Debug.DrawRay(transform.position, direction.normalized * enemyData.attackRange, Color.green);

        if (hit == default) return false;

        return true;

    }
    /// <summary>
    /// Hàm này xử lý việc kẻ địch tấn công tháp. Trong lớp cơ sở, nó chỉ ghi log một thông điệp. Các lớp con có thể override hàm này để thực hiện các hành động tấn công
    /// </summary>
    protected virtual void Attack()
    {
        Debug.Log($"[EnemyAI] {enemyData.emnemyName} Attacking tower!");
    }

    /// <summary>
    /// Hàm chính để gọi từ bên ngoài
    /// </summary>
    /// <param name="amount">Số lượng sát thương nhận vào</param>
    /// <param name="invincibilityTime">Thời gian bất tử sau khi bị hit</param>
    public virtual void TakeDamage(float amount, float invincibilityTime = 0f)
    {
        if (isInvincible || currentHealth <= 0)
            return ;

        currentHealth -= amount;

        OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invincibilityTime > 0)
        {
            StartCoroutine(InvincibilityRoutine(invincibilityTime));
        }

    }

    /// <summary>
    /// Coroutine xử lý thời gian vô địch
    /// </summary>
    protected virtual IEnumerator InvincibilityRoutine(float time)
    {
        isInvincible = true;

        // Bạn có thể chèn logic nhấp nháy SpriteRenderer ở đây (sẽ hướng dẫn bên dưới nếu bạn cần)

        // Chờ hết thời gian
        yield return new WaitForSeconds(time);

        isInvincible = false;
    }

    /// <summary>
    /// Các class con có thể override hàm này để thêm âm thanh / hiệu ứng riêng khi bị đánh trúng
    /// </summary>
    protected virtual void OnHit()
    {
        Debug.Log("[EnemyAI] "+gameObject.name + " took damage! Remaining HP: " + currentHealth);
    }

    /// <summary>
    /// Hàm xử lý cái chết (Override để rớt đồ, cộng điểm, hoặc trả về Object Pool)
    /// </summary>
    protected virtual void Die()
    {
        Debug.Log("[EnemyAI]"+gameObject.name + " has died.");
         gameObject.SetActive(false); // Thay bằng lệnh Release của Object Pool nếu bạn đang dùng

        WaveManager.Instance.EnemyKilled();

        // Xử lý thu hồi về Pool
        if (gameObject.activeInHierarchy && managedPool != null)
        {
            managedPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}