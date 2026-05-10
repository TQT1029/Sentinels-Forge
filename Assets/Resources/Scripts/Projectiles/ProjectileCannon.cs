using UnityEngine;

public class ProjectileCannon : Projectile
{
    private Vector2 currentVelocity;
    private Collider2D projectileCollider;

    private void Start()
    {
        projectileCollider = GetComponent<Collider2D>();
    }

    protected override void ReturnToPool()
    {
        base.ReturnToPool();

        projectileCollider.excludeLayers = 0;

    }
    private void Update()
    {
        currentVelocity = rb.linearVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                ProcessHitEnemy(enemy, currentVelocity.magnitude);
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            damageMultiplier = 0f;

            OnTouchGround();
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("SpawnerZone"))
        {
            ReturnToPool();
        }

    }


    private void OnTouchGround()
    {
        if (projectileCollider == null) return;

        projectileCollider.excludeLayers = LayerMask.GetMask("Enemy", "Projectile");
    }
}
