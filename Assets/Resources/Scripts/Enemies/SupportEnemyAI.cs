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
    [SerializeField] private List<EnemyAI> availableAllies = new List<EnemyAI>();

    private float bufferRange = 1.5f;

    private float nextBuffTime = 0f;
    private float findAllyTime = 0f;

    private float actualBuffRange;
    private float distanceToNearesAlly = Mathf.Infinity;
    private float velocityXRef;
    [SerializeField] private Transform nearestAlly;

    protected override void Awake()
    {
        base.Awake();

    }

    public override void ResetStats()
    {
        base.ResetStats();

        actualBuffRange = enemyData.buffRange + Random.Range(-0.5f, 0.5f);
        actualBuffRange = Mathf.Max(3f, actualBuffRange);

        bufferRange = actualBuffRange * 0.8f;

        nearestAlly = FindNearestAlly();
        availableAllies.Clear();
    }

    protected override void ProcessAI()
    {

        if (Time.time >= findAllyTime)
        {

            nearestAlly = FindNearestAlly();

            findAllyTime = Time.time + 0.1f;
        }


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
                nextBuffTime = Time.time + enemyData.buffCooldown;
            }
        }
    }

    private void UsingBuff(bool isSingleTargeted, SupportType type)
    {
        EffectData choiceBuff = ChoiceRandomBuff();
        if (isSingleTargeted)
        {
            EnemyAI ally = nearestAlly.GetComponent<EnemyAI>();
            ally.AddEffect(choiceBuff);
            Debug.Log($"[SupportEnemyAI] Đang buff cho {ally.name}, vị trí {nearestAlly.position}");
        }
        else
        {
            foreach (var ally in availableAllies)
            {
                ally.AddEffect(choiceBuff);
            }

            Debug.Log($"[SupportEnemyAI] Đang buff cho tất cả {availableAllies.Count} xung quanh");

        }

        availableAllies.Clear();//Buff xong xoá để tránh buff nhầm
    }

    private EffectData ChoiceRandomBuff()
    {
        if (enemyData == null && enemyData.listOfAvailableBuffs == null) { return null; }
        return enemyData.listOfAvailableBuffs[Random.Range(0, enemyData.listOfAvailableBuffs.Count)];
    }

    //TODO: Cần nâng cấp thêm nếu đang ở dạng buff AoE thì cần tìm vị trí trung tâm tất cả các quái để buff tối ưu nhất
    private void ApproachingAlly(Transform nearestAlly, float distance)
    {
        if (nearestAlly == null)
        {
            ApproachingTower();
            return;
        }

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

        float smoothX = Mathf.SmoothDamp(rb.linearVelocity.x, targetVelocityX, ref velocityXRef, 0.2f);

        rb.linearVelocity = new Vector2(smoothX, rb.linearVelocity.y);

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
    private Transform FindNearestAlly()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;

        availableAllies = FindAllAllies();

        if (availableAllies != null)
            foreach (var ally in availableAllies)
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

        List<EnemyAI> availableAllies = new List<EnemyAI>();

        if (availableAllies != null)
            foreach (Collider2D ally in hitAllies)
            {
                if (!ally.gameObject.CompareTag(GameConstants.SUPPORT_ENEMY_TAG))
                    availableAllies.Add(ally.GetComponent<EnemyAI>());
            }
        return availableAllies;
    }

}
