using UnityEngine;

public class BorderSafety : MonoBehaviour
{
    [SerializeField] private bool InstantKillEnemies = true;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == GameConstants.LayerIndices.PROJECTILE)
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();

            projectile.ReturnToPool();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!InstantKillEnemies) return;

        if (collision.gameObject.layer == GameConstants.LayerIndices.ENEMY_HITBOX)
        {
            EnemyAI enemy = collision.gameObject.GetComponentInParent<EnemyAI>();
            enemy.TakeDamage(new DamageInfo{ damage = Mathf.Infinity, isCritical = false });
        }
    }
}
