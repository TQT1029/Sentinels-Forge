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
        projectileCollider.excludeLayers = (1 << GameConstants.INDEX_TOWER_LAYER);

        rb.AddTorque(torqueAmount, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == GameConstants.INDEX_ENEMY_LAYER)
        {
            EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();

            HitData hitData = new HitData(collision, enemy);
            bool shouldKeepFlying = ProcessHit(hitData);
        }
        if (collision.gameObject.layer == GameConstants.INDEX_BORDER_LAYER && collision.gameObject.CompareTag(GameConstants.GROUND_TAG))
        {

            HitData hitData = new HitData(collision, null);
            bool shouldKeepFlying = ProcessHit(hitData);

            if (!shouldKeepFlying)
            {
                Invoke(nameof(OnTouchGround), delayDisableCollider);
            }
        }

        if (collision.gameObject.layer == GameConstants.INDEX_SPAWNER_ZONE_LAYER)
        {
            ReturnToPool();
        }

    }

    //TODO: Chỉnh lại để khi dùng modifier split thì đạn split ra nếu chạm đất cũng sẽ bị disable tương tự hàm này tránh cản đường quái và gây thêm damage
    private void OnTouchGround()
    {
        if (projectileCollider == null || !gameObject.activeInHierarchy) return;

        RuntimeState.DamageMultiplier = 0;
        //Dùng toán tử |= để CỘNG THÊM Mask thay vì ghi đè (Tránh làm mất Mask Tower đã set ở trên)
        projectileCollider.excludeLayers |= (1 << GameConstants.INDEX_ENEMY_LAYER) | (1 << GameConstants.INDEX_PROJECTILE_LAYER);
    }

    public override void ReturnToPool()
    {
        // Nếu viên đạn chạm đất -> bắt đầu đếm 0.1s -> nhưng 0.05s sau nó hết lifeTime và bị thu về Pool.
        // Nếu không Cancel, hàm OnTouchGround vẫn sẽ nổ ra khi đạn đang nằm trong Pool (hoặc khi vừa được lấy ra cho lần bắn tiếp theo), gây lỗi đạn xuyên quái ngay từ lúc bắn.
        CancelInvoke(nameof(OnTouchGround));

        base.ReturnToPool();
    }
}
