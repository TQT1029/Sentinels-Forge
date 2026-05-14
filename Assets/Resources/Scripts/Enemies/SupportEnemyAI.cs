using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private float bufferDistance = 1.5f;

    private float nextBuffTime = 0f;
    private float findAllyTime = 0f;

    private float actualBuffRange;
    private float velocityXRef;
    [SerializeField] private Transform nearestAlly;

    protected override void Awake()
    {
        base.Awake();

        actualBuffRange = enemyData.buffRange + Random.Range(-0.5f, 0.5f);
        ValidateBufferDistance();

        nearestAlly = FindNearestAlly();

    }

    public override void ResetStats()
    {
        base.ResetStats();

        actualBuffRange = enemyData.attackRange + Random.Range(-0.5f, 0.5f);

        actualBuffRange = Mathf.Max(0.5f, actualBuffRange);
        ValidateBufferDistance();

        nearestAlly = FindNearestAlly();

    }

    protected override void ProcessAI()
    {

        if (Time.time >= findAllyTime)
        {

            nearestAlly = FindNearestAlly();

            findAllyTime = Time.time + 0.1f;
        }
        float distanceToNearesAlly = Mathf.Infinity;

        if (nearestAlly != null)
        {
            distanceToNearesAlly = Vector2.Distance(transform.position, nearestAlly.position);
        }
        ApproachingAlly(nearestAlly, distanceToNearesAlly);


        if (distanceToNearesAlly <= actualBuffRange)
        {
            if (Time.time >= nextBuffTime)
            {
                UsingBuff(isSingleTargeted, type);
                nextBuffTime = Time.time + enemyData.attackCooldown;
            }
        }
    }

    private void UsingBuff(bool isSingleTargeted, SupportType type)
    {
        EnemyAI ally = nearestAlly.GetComponent<EnemyAI>();
        Debug.Log($"[SupportEnemyAI] Đang buff cho {ally.name}, vị trí {nearestAlly.position}");
    }

    private void ApproachingAlly(Transform nearestAlly, float distance)
    {
        if (nearestAlly == null) { ApproachingTower(base.towerTransform); return; }

        float targetVelocityX = 0f;
        Vector2 direction = (nearestAlly.position - transform.position).normalized;

        if (distance > enemyData.buffRange)
            targetVelocityX = direction.x * enemyData.moveSpeed;
        else if (distance >= enemyData.buffRange - bufferDistance)
            targetVelocityX = 0f;
        else targetVelocityX = -direction.x * enemyData.moveSpeed;

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

    }

    private void ApproachingTower(Transform towerTrans)
    {
        float targetVelocityX = 0f;

        float distance = Vector2.Distance(towerTrans.position, transform.position);
        Vector2 direction = (towerTrans.position - transform.position).normalized;

        if (distance > enemyData.buffRange)
            targetVelocityX = direction.x * enemyData.moveSpeed;
        else if (distance >= enemyData.buffRange - bufferDistance)
            targetVelocityX = 0f;
        else targetVelocityX = -direction.x * enemyData.moveSpeed;

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

    }
    private Transform FindNearestAlly()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        List<EnemyAI> listOfEnemyAI = FindAllAllies();

        if (listOfEnemyAI != null)
            foreach (var ally in listOfEnemyAI)
            {
                float distance = (transform.position - ally.transform.position).sqrMagnitude;
                if (distance < closestDistanceSqr)
                {
                    closestDistanceSqr = distance;
                    bestTarget = ally.transform;
                }
            }

        return bestTarget;
    }
    private List<EnemyAI> FindAllAllies()
    {
        Collider2D[] hitAllies = Physics2D.OverlapCircleAll(transform.position, enemyData.buffRange, GameConstants.MASK_ENEMY);

        List<EnemyAI> listOfEnemyAI = new List<EnemyAI>();

        if (listOfEnemyAI != null)
            foreach (Collider2D ally in hitAllies)
            {
                if (!ally.gameObject.CompareTag(GameConstants.SUPPORT_ENEMY_TAG))
                    listOfEnemyAI.Add(ally.GetComponent<EnemyAI>());
            }
        return listOfEnemyAI;
    }



    /// <summary>
    /// Safety Check: Đảm bảo vùng đệm (buffer) không bao giờ lớn hơn hoặc bằng tầm đánh (range).
    /// </summary>
    private void ValidateBufferDistance()
    {
        if (bufferDistance >= actualBuffRange)
        {
            float oldBuffer = bufferDistance;

            // Ép bufferDistance tối đa chỉ bằng 80% tầm đánh thực tế.
            // Ví dụ: Tầm đánh là 2, thì buffer lớn nhất chỉ được là 1.6 (quái sẽ đứng ở khoảng cách từ 1.6 đến 2.0 để bắn)
            bufferDistance = actualBuffRange * 0.8f;

            // Bắn LogWarning để Designer/Tester biết mà sửa lại file Scriptable Object
            Debug.LogWarning($"[Enemy System - Safety Check] Quái {gameObject.name} có Buffer Distance ({oldBuffer}) >= UsingBuff Range ({actualBuffRange}). Đã tự động giảm Buffer xuống {bufferDistance} để tránh lỗi hành vi.");
        }
    }

}
