using UnityEngine;

enum FlyingType
{
    Line,
    Sine,
    PerlinNoise
}
public class FlyingEnemyAI : EnemyAI
{
    [Header("Flying Settings")]
    [SerializeField] private FlyingType approachStyle = FlyingType.Line;
    [SerializeField, Tooltip("Sử dụng bắn xa")] private bool usesRangedAttack = false;
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Safety Settings")]
    private float bufferRange = 1.5f;
    [SerializeField, Min(5)] private float minAltitude = 10f; // Độ cao tối thiểu so với mặt đất
    [SerializeField, Min(5)] private float altitudeCorrectionForce = 5f; // Tốc độ đẩy lên khi quá sát đất
    [SerializeField] private float overshootBrakeDistance = 1f; // Khoảng cách bắt đầu phanh gấp nếu lỡ bay quá tháp

    [Header("Sine Wave Settings")]
    [SerializeField] private float sineAmplitude = 2f; // Biên độ (Độ cao tối đa nảy lên/xuống)
    [SerializeField] private float sineFrequency = 3f; // Tần số (Tốc độ lượn sóng)

    [Header("Perlin Noise Settings")]
    [SerializeField] private float perlinAmplitude = 2f; // Biên độ xê dịch ngẫu nhiên
    [SerializeField] private float perlinFrequency = 1f; // Tốc độ xê dịch ngẫu nhiên

    private float nextAttackTime = 0f;
    private float handleTime = 0f;

    private bool isInRangeAttack = false;
    private float actualAttackRange;
    // Thay đổi sang Vector2 để SmoothDamp theo cả trục X và Y
    private Vector2 velocityRef;

    // Offset ngẫu nhiên để các con quái dùng Perlin Noise không bay lượn đồng bộ với nhau
    private float randomNoiseOffset;

    protected override void Awake()
    {
        base.Awake();

        if (firePoint == null) firePoint = transform;
    }

    public override void ResetStats()
    {
        base.ResetStats();

        actualAttackRange = enemyData.attackRange + Random.Range(-0.5f, 0.5f);
        actualAttackRange = Mathf.Max(1.5f, actualAttackRange);

        bufferRange = actualAttackRange * 0.8f;

        randomNoiseOffset = Random.Range(-1000f, 1000f);
    }

    protected override void ProcessAI()
    {
        if (Time.time >= handleTime)
        {
            float distanceToTower = Vector2.Distance(transform.position, actualTargetPosition);
            Vector2 directionToTower = (actualTargetPosition - transform.position).normalized;
            isInRangeAttack = IsInRangeAttack(distanceToTower);

            ApproachingTower(distanceToTower, directionToTower, approachStyle);

            handleTime = Time.time + 0.05f;
        }

        if (isInRangeAttack && Time.time >= nextAttackTime)
        {
            Attack(usesRangedAttack);
            nextAttackTime = Time.time + enemyData.attackCooldown;
        }
    }

    private void Attack(bool useRangedAttack)
    {
        float finalDamage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        finalDamage *= damageMultiplier;

        if (useRangedAttack)
        {
            EnemyProjectileManager.Instance.SpawnProjectile(
                projectilePrefab,
                firePoint.position,
                actualTargetPosition,
                finalDamage
            );
        }
        else
        {
            // Cận chiến
            towerController.TakeDamage(new DamageInfo { damage = finalDamage, isCritical = false });
        }
        //Debug.Log($"[FlyingEnemyAI] Đã tấn công: {finalDamage}");
    }

    private bool IsInRangeAttack(float distanceToTower)
    {
        return distanceToTower < actualAttackRange;
    }
    private void ApproachingTower(float distanceToTower, Vector2 directionToTower, FlyingType style)
    {
        Vector2 targetVelocity = Vector2.zero;

        // Xử lý di chuyển trục X
        if (!isInRangeAttack)
        {
            if (distanceToTower > actualAttackRange)
            {
                // Đi thẳng tới mục tiêu theo Vector hướng
                targetVelocity = directionToTower * (enemyData.moveSpeed * speedMultiplier);
            }
            else if (distanceToTower < bufferRange)
            {
                // Bị mục tiêu áp sát -> Đi lùi ra xa (Đảo ngược hướng)
                targetVelocity = -directionToTower * (enemyData.moveSpeed * speedMultiplier);
            }
        }

        bool isBehindTower = transform.position.x < towerTransform.position.x + overshootBrakeDistance;

        if (isBehindTower)
        {
            // Nếu đã bay quá, ép vận tốc hướng ngược lại ngay lập tức để quay về vùng tấn công
            targetVelocity = Vector2.right * (enemyData.moveSpeed * speedMultiplier);
        }

        targetVelocity.x += RandomUtils.GetPerlinHeight(0, Time.time * perlinFrequency + randomNoiseOffset, transform.position.x, 0.1f, -1f, 1f, 0);

        // Xử lý di chuyển trục Y
        float driftVelocityY = 0f;


        switch (style)
        {
            case FlyingType.Line:
                // Bay thẳng: Không thêm gì cả
                driftVelocityY = 0f;
                break;

            case FlyingType.Sine:
                // Bay lượn sóng: Cố định quy luật lên xuống
                driftVelocityY = Mathf.Cos(Time.time * sineFrequency) * sineAmplitude;
                break;

            case FlyingType.PerlinNoise:
                // Bay hỗn loạn: Mathf.PerlinNoise trả về 0 -> 1. 
                // Ta nhân 2 trừ 1 để chuyển đổi nó thành -1 -> 1 (lên và xuống)
                float noiseValue = Mathf.PerlinNoise(Time.time * perlinFrequency + randomNoiseOffset, 0f) * 2f - 1f;
                driftVelocityY = noiseValue * perlinAmplitude;
                break;
        }



        // KẾT HỢP VẬN TỐC ĐỊNH HƯỚNG VÀ DAO ĐỘNG
        targetVelocity.y += driftVelocityY;
        targetVelocity.y = ApplyAltitudeSafety(targetVelocity.y);

        // Giảm smoothTime (từ 0.2s xuống 0.1s) khi ở gần tháp hoặc khi đã lỡ bay quá để phanh gấp hơn
        float currentSmoothTime = (isInRangeAttack || isBehindTower) ? 0.1f : 0.2f;
        rb.linearVelocity = Vector2.SmoothDamp(rb.linearVelocity, targetVelocity, ref velocityRef, currentSmoothTime);
    }

    private float ApplyAltitudeSafety(float currentTargetVelocityY)
    {
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, minAltitude, GameConstants.MASK_BORDER);
        //Debug.DrawRay(transform.position, Vector2.down * minAltitude, Color.aquamarine);

        if (groundHit.collider != null && groundHit.collider.gameObject.CompareTag(GameConstants.GROUND_TAG))
        {
            float proximityFactor = 1f - (groundHit.distance / minAltitude);

            // Tính toán vận tốc mới sau khi bù trừ lực đẩy lên
            float safeVelocityY = currentTargetVelocityY + (proximityFactor * altitudeCorrectionForce);

            // Nếu quái rơi xuống mức < 30% độ cao an toàn, tuyệt đối không cho phép có vận tốc đi xuống.
            if (groundHit.distance < minAltitude * 0.3f)
            {
                safeVelocityY = Mathf.Max(safeVelocityY, altitudeCorrectionForce * 0.5f);
            }

            return safeVelocityY;
        }

        // Nếu ở cao an toàn, cứ giữ nguyên vận tốc bay lượn bình thường
        return currentTargetVelocityY;
    }


}