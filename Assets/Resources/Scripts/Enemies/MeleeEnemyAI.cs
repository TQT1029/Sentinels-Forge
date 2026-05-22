using UnityEngine;

public class MeleeEnemyAI : EnemyAI
{
    [Header("Melee Settings")]
    private float nextAttackTime = 0f;
    [SerializeField] private float bufferDistance = 0f;
    private float velocityXRef; // Biến tham chiếu nội bộ dùng cho hàm SmoothDamp
    private float noiseOffset;

    public override void ResetStats()
    {
        base.ResetStats();

        noiseOffset = Random.Range(-1000f, 1000f); // Tạo một offset ngẫu nhiên cho Perlin Noise để mỗi kẻ địch có chuyển động khác nhau
    }
    protected override void ProcessAI()
    {
        // Kiểm tra khoảng cách
        float distanceToTower = Vector2.Distance(transform.position, towerTransform.position);

        ApproachingTower(distanceToTower);

        if (distanceToTower <= enemyData.attackRange)
        {
            // Đã vào tầm đánh -> Dừng lại
            rb.linearVelocity = Vector2.zero;

            // Logic Cooldown tấn công
            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + enemyData.attackCooldown;
            }
        }


    }

    private void Attack()
    {
        // Tính toán sát thương có độ lệch (Variation)
        float finalDamage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        finalDamage *= damageMultiplier;

        towerController.TakeDamage(new DamageInfo { damage = finalDamage, isCritical = false });

        //Debug.Log($"[MeleeEnemyAI] {gameObject.name} attacks the tower for {finalDamage} damage!");
    }

    private void ApproachingTower(float distanceToTower)
    {
        float targetVelocityX = 0f;

        // Tính toán vận tốc mong muốn (Target Velocity)
        if (distanceToTower > enemyData.attackRange)
        {
            // Chưa tới tầm -> Tốc độ mong muốn là đi thẳng tới
            targetVelocityX = -enemyData.moveSpeed * speedMultiplier;
        }
        else if (distanceToTower >= enemyData.attackRange - bufferDistance)
        {
            // Vừa đúng tầm bắn -> Tốc độ mong muốn là 0 (Đứng lại)
            targetVelocityX = 0f;
        }
        else
        {
            // Quá gần (Bị áp sát) -> Tốc độ mong muốn là đi lùi lại
            targetVelocityX = enemyData.moveSpeed * speedMultiplier;
        }

        targetVelocityX += RandomUtils.GetPerlinHeight(noiseOffset, transform.position.x, -0.5f, 0.5f, 0f); // Thêm một chút biến động ngẫu nhiên để tránh chuyển động quá cứng nhắc

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);
    }
}