using System.Collections.Generic;
using UnityEngine;

public class ProjectileArrow : Projectile
{
    [SerializeField] private const float CollisionCheckInterval = 0.5f;
    private static List<Projectile> stuckProjectiles = new List<Projectile>();
    private Transform stuckTarget;
    private bool isStuck = false;
    private bool hasDealtDamage = false;

    private Vector3 positionOffset;
    private Quaternion rotationOffset;

    private void Update()
    {
        RotateTowardsVelocity();
        CheckCollision();
    }

    private void LateUpdate()
    {
        if (stuckTarget != null)
        {
            // TransformPoint sẽ nhân khoảng cách với scale của quái, giúp đạn bám đúng trên bề mặt
            transform.position = stuckTarget.TransformPoint(positionOffset);

            // Giữ góc xoay đồng bộ với góc xoay của quái
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
        rb.gravityScale = 1f;
        rb.simulated = true;
    }

    protected override void ReturnToPool()
    {
        stuckTarget = null;
        hasDealtDamage = false;
        isStuck = false;
        base.ReturnToPool();
    }

    private bool CheckCollision()
    {
        Vector2 direction = rb.linearVelocity;
        //float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, CollisionCheckInterval);
        Debug.DrawRay(transform.position, direction.normalized * CollisionCheckInterval, Color.green, 0.1f);
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                if (!isStuck) StuckingArrow(hit);

                return true;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                EnemyAI enemy = hit.transform.GetComponent<EnemyAI>();

                if (enemy != null)
                {
                    if (!hasDealtDamage)
                    {
                        enemy.TakeDamage(projectileData.baseDamage + RandomUtils.RandomWithSteps(-projectileData.damageVariation, projectileData.damageVariation, 0.5f), 0.5f);
                        hasDealtDamage = true;

                        if (enemy.IsDead) { ReturnToPool(); }
                    }

                    if (!isStuck) StuckingArrow(hit);
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
    }

}
