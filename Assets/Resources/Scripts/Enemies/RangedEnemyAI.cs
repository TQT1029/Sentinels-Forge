using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    [Header("Ranged Settings")]
    [SerializeField] private EnemyProjectile projectilePrefab; // Loại đạn nó sẽ bắn
    [SerializeField] private Transform firePoint;

    private float nextAttackTime = 0f;

    protected override void ProcessAI()
    {
        // 1. Kiểm tra khoảng cách
        float distanceToTower = Vector2.Distance(transform.position, towerTransform.position);

        if (distanceToTower <= enemyData.attackRange)
        {
            // Đã vào tầm bắn -> Dừng lại
            rb.linearVelocity = Vector2.zero;

            // 2. Logic Cooldown bắn
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + enemyData.attackCooldown;
            }
        }
        else
        {
            // Chưa tới tầm -> Đi tiếp
            rb.linearVelocity = new Vector2(-enemyData.moveSpeed * speedMultiplier, rb.linearVelocity.y);
        }
    }

    private void Attack()
    {
        // Tính toán sát thương có độ lệch (Variation)
        float finalDamage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        // HƯỚNG TỚI MANAGER ĐỂ XIN ĐẠN (Object Pooling)
        EnemyProjectileManager.Instance.SpawnProjectile(
            projectilePrefab,
            firePoint.position,
            towerTransform.position,
            finalDamage
        );
    }
}