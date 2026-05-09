using UnityEngine;

public class ProjectileCannon : Projectile
{

    public override void Init(float lifeTime)
    {
        base.Init(lifeTime);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = projectileData.gravityScale;
        rb.simulated = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            currentDamage = 0;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                ProcessHitEnemy(enemy);
            }
        }
        else
        {
            // Tương tác với môi trường, có thể thêm hiệu ứng va chạm hoặc logic khác ở đây
            ReturnToPool();
        }

    }
}
