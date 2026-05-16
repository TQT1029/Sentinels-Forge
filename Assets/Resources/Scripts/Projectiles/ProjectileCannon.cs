using UnityEngine;

public class ProjectileCannon : Projectile
{
    private Collider2D projectileCollider;
    private float delayDisableCollider = 0.1f; // Thời gian delay trước khi tắt va chạm với quái
    private float torqueAmount = -25f; // Lực xoay khi va chạm với mặt đất

    private bool isGrounded = false;
    protected override void Awake()
    {
        base.Awake();
        projectileCollider = GetComponent<Collider2D>();
    }

    public override void Init(float lifeTime)
    {
        base.Init(lifeTime);

        isGrounded = false;

        projectileCollider.excludeLayers = GameConstants.MASK_TOWER;
        rb.AddTorque(torqueAmount, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"[ProjectileCannon] Collsion {collision.gameObject.name}");
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }

    private void ProcessCollision(Collision2D collision)
    {
        // Nếu đạn đã xác nhận nằm chết trên đất, khoá hoàn toàn mọi va chạm tiếp theo
        if (isGrounded) return;

        if (collision.gameObject.layer == GameConstants.INDEX_BORDER_LAYER && collision.gameObject.CompareTag(GameConstants.GROUND_TAG))
        {
            HandleGroundCollision(collision);
        }
        else if (collision.gameObject.layer == GameConstants.INDEX_ENEMY_LAYER)
        {
            HandleEnemyCollision(collision);
        }
        else if (collision.gameObject.layer == GameConstants.INDEX_SPAWNER_ZONE_LAYER)
        {
            ReturnToPool();
        }
    }

    private void HandleGroundCollision(Collision2D collision)
    {
        if (collision.contactCount != 0)
        {
            Debug.Log(Vector2.Dot(rb.linearVelocity, collision.GetContact(0).normal));
            if (Vector2.Dot(rb.linearVelocity, collision.GetContact(0).normal) >= 0.05f)
            {
                HitData hitData = new HitData(collision, null);
                bool shouldKeepFlying = ProcessHit(hitData);

                if (!shouldKeepFlying)
                {
                    isGrounded = true;

                    Invoke(nameof(OnTouchGround), delayDisableCollider);
                }
            }
        }
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        EnemyAI enemy = collision.gameObject.GetComponent<EnemyAI>();
        HitData hitData = new HitData(collision, enemy);
        ProcessHit(hitData);
    }
    private void OnTouchGround()
    {
        if (projectileCollider == null || !gameObject.activeInHierarchy) return;

        RuntimeState.DamageMultiplier = 0;
        //Dùng toán tử |= để CỘNG THÊM Mask thay vì ghi đè (Tránh làm mất Mask Tower đã set ở trên)
        projectileCollider.excludeLayers |= GameConstants.MASK_ENEMY | GameConstants.MASK_PROJECTILE;

        Debug.Log($"[ProjectileCannon] {gameObject.name} đã hàm chạm đất ở vị trí {transform.position}");
    }

    public override void ReturnToPool()
    {
        // Nếu viên đạn chạm đất -> bắt đầu đếm 0.1s -> nhưng 0.05s sau nó hết lifeTime và bị thu về Pool.
        // Nếu không Cancel, hàm OnTouchGround vẫn sẽ nổ ra khi đạn đang nằm trong Pool (hoặc khi vừa được lấy ra cho lần bắn tiếp theo), gây lỗi đạn xuyên quái ngay từ lúc bắn.
        CancelInvoke(nameof(OnTouchGround));

        base.ReturnToPool();
    }
}
