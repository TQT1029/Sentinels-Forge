using System.Collections.Generic;
using UnityEngine;

public class ProjectileBow : Projectile
{
    [SerializeField] private const float CollisionCheckInterval = 0.5f;

    // Gom nhóm các mũi tên theo từng mục tiêu cụ thể
    private static Dictionary<Transform, List<ProjectileBow>> stuckArrowsMap = new Dictionary<Transform, List<ProjectileBow>>();

    private Transform stuckTarget;
    private bool isStuck = false;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;
    private Vector2 currentVelocity;

    protected override void Update()
    {
        base.Update();

        if (!isStuck)
        {
            currentVelocity = rb.linearVelocity;
            RotateTowardsVelocity();
            CheckCollision();
        }
    }

    private void LateUpdate()
    {
        if (isStuck && stuckTarget != null)
        {
            // Safety check: Tự động rớt ra nếu quái vật bị tắt (trả về pool) mà quên gọi hàm giải phóng
            if (!stuckTarget.gameObject.activeInHierarchy)
            {
                ReturnToPool();
                return;
            }

            transform.position = stuckTarget.TransformPoint(positionOffset);
            transform.rotation = stuckTarget.rotation * rotationOffset;
        }
    }

    private void RotateTowardsVelocity()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public override void Init(float lifeTime)
    {
        base.Init(lifeTime);
    }

    public override void ReturnToPool()
    {
        // Gỡ mũi tên khỏi bản đồ quản lý trước khi thu hồi
        if (stuckTarget != null && stuckArrowsMap.ContainsKey(stuckTarget))
        {
            stuckArrowsMap[stuckTarget].Remove(this);
            if (stuckArrowsMap[stuckTarget].Count == 0)
            {
                stuckArrowsMap.Remove(stuckTarget);
            }
        }

        stuckTarget = null;
        isStuck = false;

        base.ReturnToPool();
    }

    private void CheckCollision()
    {
        Vector2 direction = rb.linearVelocity;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, CollisionCheckInterval);
        Debug.DrawRay(transform.position, direction.normalized * CollisionCheckInterval, Color.green, 0.1f);

        if (hit.collider != null)
        {
            bool shouldKeepFlying = false;

            if (hit.collider.gameObject.layer == GameConstants.INDEX_BORDER_LAYER && hit.collider.gameObject.CompareTag(GameConstants.GROUND_TAG))
            {
                HitData hitData = new HitData(hit, null);

                shouldKeepFlying = ProcessHit(hitData);

                if (!shouldKeepFlying)
                {
                    if (!isStuck) StuckingArrow(hit.transform);
                }

            }
            else if (hit.collider.gameObject.layer == GameConstants.INDEX_ENEMY_HITBOX_LAYER || hit.collider.gameObject.layer == GameConstants.INDEX_ENEMY_BODY_LAYER)
            {
                EnemyAI enemy = hit.transform.GetComponentInParent<EnemyAI>();

                if (enemy != null)
                {
                    HitData hitData = new HitData(hit, enemy);

                    shouldKeepFlying = ProcessHit(hitData);

                    // Nếu không xuyên và không kẹt, thì mới dính vào quái vật
                    if (!shouldKeepFlying)
                    {
                        if (enemy.IsDead)
                        {
                            ReturnToPool();
                            ReleaseArrowsOnTarget(enemy.transform);
                        }

                        if (!isStuck) StuckingArrow(enemy.transform);
                    }
                }
            }
            else if (hit.collider.gameObject.layer == GameConstants.INDEX_SPAWNER_ZONE_LAYER)
            {
                ReturnToPool();
            }
        }
    }

    private void StuckingArrow(Transform targetTrans)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector3.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;

        stuckTarget = targetTrans;

        positionOffset = stuckTarget.InverseTransformPoint(transform.position);
        rotationOffset = Quaternion.Inverse(stuckTarget.rotation) * transform.rotation;

        isStuck = true;

        if (!stuckArrowsMap.ContainsKey(stuckTarget))
        {
            stuckArrowsMap[stuckTarget] = new List<ProjectileBow>();
        }
        stuckArrowsMap[stuckTarget].Add(this);
    }

    private void ReleaseArrowsOnTarget(Transform target)
    {
        if (stuckArrowsMap.TryGetValue(target, out List<ProjectileBow> arrows))
        {
            for (int i = arrows.Count - 1; i >= 0; i--)
            {
                arrows[i].ReturnToPool();
            }
            stuckArrowsMap.Remove(target);
        }
    }
}