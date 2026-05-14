using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [Header("Ranged Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Khoảng cách an toàn để quái không đi quá sát (Có thể chỉnh trong Inspector)
    [SerializeField] private float bufferDistance = 1.5f;

    private float nextAttackTime = 0f;

    private float actualAttackRange; 
    private float velocityXRef; 

    protected override void Awake()
    {
        base.Awake();
        if (firePoint == null) firePoint = transform;

        ValidateBufferDistance();

        actualAttackRange = enemyData.attackRange + Random.Range(-0.5f, 0.5f);

    }

    public override void ResetStats()
    {
        base.ResetStats();

        actualAttackRange = enemyData.attackRange + Random.Range(-0.5f, 0.5f);

        actualAttackRange = Mathf.Max(0.5f, actualAttackRange);
        ValidateBufferDistance();
    }

    protected override void ProcessAI()
    {
        float distanceToTower = Vector2.Distance(transform.position, towerTransform.position);

        ApproachingTower(distanceToTower);

        if (distanceToTower <= actualAttackRange)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + enemyData.attackCooldown;
            }
        }
    }

    private void Attack()
    {
        float finalDamage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        EnemyProjectileManager.Instance.SpawnProjectile(
            projectilePrefab,
            firePoint.position,
            towerTransform.position,
            finalDamage
        );
    }

    private void ApproachingTower(float distanceToTower)
    {
        float targetVelocityX = 0f;

        // Tính toán vận tốc mong muốn (Target Velocity)
        if (distanceToTower > actualAttackRange)
        {
            // Chưa tới tầm -> Tốc độ mong muốn là đi thẳng tới
            targetVelocityX = -enemyData.moveSpeed * speedMultiplier;
        }
        else if (distanceToTower >= actualAttackRange - bufferDistance)
        {
            // Vừa đúng tầm bắn -> Tốc độ mong muốn là 0 (Đứng lại)
            targetVelocityX = 0f;
        }
        else
        {
            // Quá gần (Bị áp sát) -> Tốc độ mong muốn là đi lùi lại
            targetVelocityX = enemyData.moveSpeed * speedMultiplier;
        }

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);
    }

    /// <summary>
    /// Safety Check: Đảm bảo vùng đệm (buffer) không bao giờ lớn hơn hoặc bằng tầm đánh (range).
    /// </summary>
    private void ValidateBufferDistance()
    {
        if (bufferDistance >= actualAttackRange)
        {
            float oldBuffer = bufferDistance;

            // Ép bufferDistance tối đa chỉ bằng 80% tầm đánh thực tế.
            // Ví dụ: Tầm đánh là 2, thì buffer lớn nhất chỉ được là 1.6 (quái sẽ đứng ở khoảng cách từ 1.6 đến 2.0 để bắn)
            bufferDistance = actualAttackRange * 0.8f;

            // Bắn LogWarning để Designer/Tester biết mà sửa lại file Scriptable Object
            Debug.LogWarning($"[Enemy System - Safety Check] Quái {gameObject.name} có Buffer Distance ({oldBuffer}) >= Attack Range ({actualAttackRange}). Đã tự động giảm Buffer xuống {bufferDistance} để tránh lỗi hành vi.");
        }
    }
}