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

    private void Update()
    {
        if (!isStuck)
        {
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

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = projectileData.gravityScale;
        rb.simulated = true;
    }

    protected override void ReturnToPool()
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

    private bool CheckCollision()
    {
        Vector2 direction = rb.linearVelocity;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, CollisionCheckInterval);
        Debug.DrawRay(transform.position, direction.normalized * CollisionCheckInterval, Color.green, 0.1f);

        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                bool shouldKeepFlying = ProcessEnvironmentHit(hit);

                if (!shouldKeepFlying)
                {
                    if (!isStuck) StuckingArrow(hit); // Không nảy thì găm vào tường
                }
                return true;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();

                if (enemy != null)
                {

                    bool shouldKeepFlying = ProcessHitEnemy(enemy);


                    // Nếu không xuyên và không kẹt, thì mới dính vào quái vật
                    if (!shouldKeepFlying)
                    {
                        if (enemy.IsDead)
                        {
                            ReturnToPool();
                            ReleaseArrowsOnTarget(hit.transform);
                            return true;
                        }

                        if (!isStuck) StuckingArrow(hit);
                    }
                }
                return true;
            }
        }
        return false;
    }

    private void StuckingArrow(RaycastHit2D hit)
    {
        rb.linearVelocity = Vector3.zero;
        rb.gravityScale = 0f;
        rb.simulated = false;

        stuckTarget = hit.collider.transform;

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