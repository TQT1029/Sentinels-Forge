using System.Collections.Generic;
using UnityEngine;

enum SupportType
{
    Healer,
    Buffers,
}
public class SupportEnemyAI : EnemyAI
{
    [SerializeField] private SupportType type = SupportType.Healer;

    [SerializeField, Tooltip("Dạng buff AoE hay Đơn")] private bool isSingleTargeted = false;
    [SerializeField] private List<EnemyAI> availableAllies = new List<EnemyAI>();

    private float bufferRange = 1.5f;

    private float nextBuffTime = 0f;
    private float findAllyTime = 0f;

    private float actualBuffRange;
    private float distanceToNearesAlly = Mathf.Infinity;
    private float velocityXRef;
    private float noiseOffset;

    [SerializeField] private Transform primaryTarget;

    [Header("Optimize")]
    [SerializeField] private int maxTargetsToFind = 25; // Giới hạn tối đa quét 25 mục tiêu cùng lúc
    private Collider2D[] overlapResults;
    private List<EnemyAI> cachedAlliesList = new List<EnemyAI>();
    [SerializeField] private ContactFilter2D enemyFilter;

    public override void ResetStats()
    {
        base.ResetStats();

        if (overlapResults == null)
        {
            overlapResults = new Collider2D[maxTargetsToFind];
        }

        actualBuffRange = enemyData.buffRange + Random.Range(-0.5f, 0.5f);
        actualBuffRange = Mathf.Max(3f, actualBuffRange);

        bufferRange = actualBuffRange * 0.8f;

        primaryTarget = FindPrimaryTarget();
        availableAllies.Clear();

        noiseOffset = Random.Range(-1000f, 1000f);
    }

    protected override void ProcessAI()
    {

        if (Time.time >= findAllyTime)
        {

            primaryTarget = FindPrimaryTarget();

            findAllyTime = Time.time + 0.1f;
        }


        if (primaryTarget != null)
        {
            distanceToNearesAlly = Vector2.Distance(transform.position, primaryTarget.position);
        }

        ApproachingAlly(isSingleTargeted, primaryTarget, distanceToNearesAlly);

        if (distanceToNearesAlly <= actualBuffRange)
        {
            if (Time.time >= nextBuffTime)
            {
                UsingBuff(isSingleTargeted, type);
                nextBuffTime = Time.time + enemyData.buffCooldown;
            }
        }
    }

    private void UsingBuff(bool isSingleTargeted, SupportType type)
    {
        switch (type)
        {
            case SupportType.Buffers:
                EffectData choiceBuff = ChoiceRandomBuff();
                if (isSingleTargeted)
                {
                    EnemyAI ally = primaryTarget?.GetComponentInParent<EnemyAI>();
                    if (ally == null || ally.IsDead) return;

                    ally.AddEffect(choiceBuff);
                    //Debug.Log($"[SupportEnemyAI] Đang buff cho {ally.name}, vị trí {primaryTarget.position}");
                }
                else
                {
                    foreach (var ally in availableAllies)
                    {
                        ally.AddEffect(choiceBuff);
                    }

                    //Debug.Log($"[SupportEnemyAI] Đang buff cho tất cả {availableAllies.Count} xung quanh");

                }

                break;
            case SupportType.Healer:
                // Tận dụng chỉ số attackDamage làm lượng máu được hồi
                float healAmount = enemyData.attackDamage * damageMultiplier;

                if (isSingleTargeted)
                {
                    EnemyAI ally = primaryTarget?.GetComponentInParent<EnemyAI>();
                    if (ally != null && !ally.IsDead)
                    {
                        ally.Heal(healAmount);
                    }
                }
                else
                {
                    foreach (var ally in availableAllies) ally.Heal(healAmount);
                }
                break;
        }

    }

    private EffectData ChoiceRandomBuff()
    {
        if (enemyData == null || enemyData.listOfAvailableBuffs == null || enemyData.listOfAvailableBuffs.Count == 0) return null;
        return enemyData.listOfAvailableBuffs[Random.Range(0, enemyData.listOfAvailableBuffs.Count)];
    }

    //TODO: Cần nâng cấp thêm nếu đang ở dạng buff AoE thì cần tìm vị trí trung tâm tất cả các quái để buff tối ưu nhất
    private void ApproachingAlly(bool singleTargeted, Transform nearestAlly, float distance)
    {
        if (nearestAlly == null)
        {
            ApproachingTower();
            return;
        }

        if (singleTargeted)
            ApproachingSingleTarget(nearestAlly, distance);
        else
            ApproachingOptimize();
    }

    private void ApproachingTower()
    {
        float targetVelocityX = 0f;

        float distance = Vector2.Distance(actualTargetPosition, transform.position);
        Vector2 direction = (actualTargetPosition - transform.position).normalized;

        if (distance > enemyData.buffRange)
            targetVelocityX = direction.x * enemyData.moveSpeed;
        else if (distance < bufferRange) targetVelocityX = -direction.x * enemyData.moveSpeed;

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

    }

    /// <summary>
    /// Logic tìm vị trí tối ưu cho AoE (Tính toán Center of Mass - Trọng tâm)
    /// Hạn chế di chuyển dư thừa: Điểm đến là trung bình cộng tọa độ X của tất cả đồng minh.
    /// </summary>
    private void ApproachingOptimize()
    {
        if (availableAllies.Count == 0) return;

        // Tính tọa độ X trung bình của cả bầy
        float averageX = 0f;
        foreach (var ally in availableAllies)
        {
            averageX += ally.transform.position.x;
        }
        averageX /= availableAllies.Count;

        float targetVelocityX = 0f;
        float distanceToCenter = Mathf.Abs(averageX - transform.position.x);
        float directionX = Mathf.Sign(averageX - transform.position.x);

        // Chỉ nhúc nhích nếu bầy đàn đã đi ra khỏi vùng an toàn (Chống rung giật)
        if (distanceToCenter > bufferRange * 0.5f)
        {
            targetVelocityX = directionX * enemyData.moveSpeed;
        }

        targetVelocityX += RandomUtils.GetPerlinHeight(noiseOffset, transform.position.x, -0.5f, 0.5f, 0f); // Thêm một chút biến động ngẫu nhiên để tránh chuyển động quá cứng nhắc


        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);
        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);
    }

    private void ApproachingSingleTarget(Transform nearestAlly, float distance)
    {
        float targetVelocityX = 0f;
        Vector2 direction = (nearestAlly.position - transform.position).normalized;

        if (nearestAlly.position.x <= transform.position.x)
        {
            if (distance >= enemyData.buffRange)
                targetVelocityX = direction.x * enemyData.moveSpeed;
            else if (distance < bufferRange)
                targetVelocityX = -direction.x * enemyData.moveSpeed;
        }
        else
        {
            targetVelocityX = enemyData.moveSpeed;
        }

        targetVelocityX += RandomUtils.GetPerlinHeight(noiseOffset, transform.position.x, -0.5f, 0.5f, 0f); // Thêm một chút biến động ngẫu nhiên để tránh chuyển động quá cứng nhắc

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

    }

    /// <summary>
    /// Logic phân loại độ ưu tiên (Priority)
    /// </summary>
    private Transform FindPrimaryTarget()
    {
        availableAllies = FindAllAllies();
        if (availableAllies.Count == 0) return null;

        Transform bestTarget = null;
        float bestScore = Mathf.Infinity; // Điểm càng thấp càng ưu tiên

        foreach (var ally in availableAllies)
        {

            // Nếu là Healer Đơn Mục Tiêu -> Ưu tiên Máu (HP) thấp nhất
            if (type == SupportType.Healer && isSingleTargeted)
            {
                // Bỏ qua quái đầy máu
                if (ally.PercentHealth >= 0.99f) continue;

                // Điểm ưu tiên = Tỷ lệ máu (Thằng nào máu ít -> Điểm thấp -> Được chọn)
                if (ally.PercentHealth < bestScore)
                {
                    bestScore = ally.PercentHealth;
                    bestTarget = ally.transform;
                }
            }
            // Nếu là Buffer hoặc AoE -> Ưu tiên vị trí gần nhất
            else
            {
                float distanceSqr = (transform.position - ally.transform.position).sqrMagnitude;

                if (distanceSqr < bestScore)
                {
                    bestScore = distanceSqr;
                    bestTarget = ally.transform;
                }
            }
        }

        return bestTarget;
    }
    private List<EnemyAI> FindAllAllies()
    {
        cachedAlliesList.Clear();

        int hitCount = Physics2D.OverlapCircle(
                    transform.position,
                    enemyData.buffRange * 2f,
                    enemyFilter,       // Truyền bộ lọc vào đây
                    overlapResults     // Nhét kết quả vào rổ này
                );

        for (int i = 0; i < hitCount; i++)
        {
            Collider2D allyCol = overlapResults[i];

            if (!allyCol.gameObject.CompareTag(GameConstants.Tags.SUPPORT_ENEMY) && allyCol.gameObject != this.gameObject)
            {
                EnemyAI allyAI = allyCol.GetComponentInParent<EnemyAI>();
                if (allyAI != null && !allyAI.IsDead)
                {
                    cachedAlliesList.Add(allyAI);
                }
            }
        }

        return cachedAlliesList;
    }
}
