using UnityEngine;

public class ProjectileCannon : Projectile
{
    private Collider2D projectileCollider;
    private float delayDisableCollider = 0.1f; // Thời gian delay trước khi tắt va chạm với quái
    private float torqueAmount = -25f; // Lực xoay khi va chạm với mặt đất

    protected override void Awake()
    {
        base.Awake();
        projectileCollider = GetComponent<Collider2D>();
    }

    public override void Init(float lifeTime)
    {
        base.Init(lifeTime);
        
        projectileCollider.excludeLayers = 0;
        rb.AddTorque(torqueAmount,ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

            HitData hitData = new HitData(collision, enemy);
            bool shouldKeepFlying = ProcessHit(hitData);
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {

            HitData hitData = new HitData(collision, null);
            bool shouldKeepFlying=ProcessHit(hitData);

            if (!shouldKeepFlying)
            {
                Invoke(nameof(OnTouchGround), delayDisableCollider);
            }
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("SpawnerZone"))
        {
            ReturnToPool();
        }

    }


    private void OnTouchGround()
    {
        if (projectileCollider == null) return;
        RuntimeState.DamageMultiplier = 0;
        projectileCollider.excludeLayers = LayerMask.GetMask("Enemy", "Projectile");
    }
}
