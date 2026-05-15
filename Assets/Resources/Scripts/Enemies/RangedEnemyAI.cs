using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [Header("Ranged Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Khoảng cách an toàn để quái không đi quá sát (Có thể chỉnh trong Inspector)
    private float bufferRange = 1.5f;

    private float nextAttackTime = 0f;

    private float actualAttackRange;
    private float velocityXRef;

    protected override void Awake()
    {
        base.Awake();
        if (firePoint == null) firePoint = transform;
    }

    public override void ResetStats()
    {
        base.ResetStats();

        actualAttackRange = enemyData.attackRange + Random.Range(-0.5f, 0.5f);
        actualAttackRange = Mathf.Max(3f, actualAttackRange);

        bufferRange = actualAttackRange * 0.8f;
    }

    protected override void ProcessAI()
    {
        float distanceToTower = Vector2.Distance(transform.position, actualTargetPosition);

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
            actualTargetPosition,
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
        else if (distanceToTower < bufferRange)
        {
            // Quá gần (Bị áp sát) -> Tốc độ mong muốn là đi lùi lại
            targetVelocityX = enemyData.moveSpeed * speedMultiplier;
        }

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);
    }


}