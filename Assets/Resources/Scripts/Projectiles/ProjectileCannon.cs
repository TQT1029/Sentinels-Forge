using UnityEngine;

public class ProjectileCannon : Projectile
{

    private float delayCollisionIgnore = 0.1f; // Thời gian delay trước khi tắt va chạm với quái
    private float torqueAmount = -25f; // Lực xoay khi va chạm với mặt đất

    [Header("Grounded Detection")]
    [SerializeField] private float minSqrVelocity = 0.1f;
    [SerializeField] private float timeToGrounded = 0.15f;

    private float lowVelocityTimer = 0f;
    private bool isGrounded = false;

    public override void Init(float lifeTime)
    {
        base.Init(lifeTime);

        lowVelocityTimer = 0f;
        isGrounded = false;

        projCollider.excludeLayers = GameConstants.MASK_TOWER | GameConstants.MASK_PROJECTILE;
        rb.AddTorque(torqueAmount, ForceMode2D.Impulse);
    }

    protected override void Update()
    {
        base.Update();

        if (isGrounded) return;

        if (rb.linearVelocity.sqrMagnitude < minSqrVelocity)
        {
            lowVelocityTimer += Time.deltaTime;
            Debug.Log($"[ProjectileCannon] {rb.linearVelocity.sqrMagnitude}");
            // Yêu cầu vận tốc thấp phải duy trì liên tục qua ngưỡng thời gian
            if (lowVelocityTimer >= timeToGrounded)
            {
                SetGrounded();
            }
        }
        else
        {
            // Reset timer ngay lập tức nếu đạn nảy lên hoặc bay nhanh lại
            lowVelocityTimer = 0f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isGrounded) return;

        if (collision.gameObject.layer == GameConstants.INDEX_BORDER_LAYER && collision.gameObject.CompareTag(GameConstants.GROUND_TAG))
        {
            HandleGroundCollision(collision);
            //Debug.Log($"[ProjectileCannon] {gameObject.name} đã va chạm với mặt đất ở vị trí {transform.position}");
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isGrounded) return;

        if (collider.gameObject.layer == GameConstants.INDEX_ENEMY_HITBOX_LAYER)
        {
            EnemyAI enemy = collider.GetComponentInParent<EnemyAI>();
            if (enemy == null) return;

            HandleEnemyCollision(collider, enemy);
            //Debug.Log($"[ProjectileCannon] {gameObject.name} đã va chạm với quái {enemy.gameObject.name} ở vị trí {transform.position}");
        }
        else if (collider.gameObject.layer == GameConstants.INDEX_SPAWNER_ZONE_LAYER)
        {
            ReturnToPool();
        }
    }

    private void HandleGroundCollision(Collision2D collision)
    {
        HitData hitData = new HitData(collision, null);
        bool shouldKeepFlying = ProcessHit(hitData);

        if (!shouldKeepFlying) SetGrounded();
    }

    private void HandleEnemyCollision(Collider2D collider, EnemyAI enemy)
    {
        // Vì Trigger không tạo ra Collision2D, ta phải giả lập dữ liệu HitData
        // - Point: Điểm gần nhất trên Hitbox của quái so với viên đạn
        // - Normal: Hướng ngược lại của hướng đạn bay (Dùng tạm vì Pierce không xài Normal)
        Vector2 hitPoint = collider.ClosestPoint(transform.position);
        Vector2 hitNormal = -rb.linearVelocity.normalized;

        Collider2D solidCollider = enemy.GetComponent<Collider2D>();

        HitData hitData = new HitData(hitPoint, hitNormal, solidCollider, enemy);

        // Gọi logic xào nấu đạn như bình thường
        ProcessHit(hitData);
    }
    public override void ProcessImediate()
    {
        // TỐI ƯU: Chỉ dò đúng layer của mặt đất (Border), dùng bán kính siêu nhỏ
        Collider2D groundOverlap = Physics2D.OverlapCircle(transform.position, 0.1f, GameConstants.MASK_BORDER);

        if (groundOverlap != null && groundOverlap.CompareTag(GameConstants.GROUND_TAG))
        {
            ProcessImmediateGroundOverlap(groundOverlap);
        }
    }

    private void ProcessImmediateGroundOverlap(Collider2D groundCollider)
    {
        if (isGrounded) return;

        // Trích xuất toán học chính xác điểm kẹt và pháp tuyến (Normal) để BounceModifier phản xạ đúng góc
        ColliderDistance2D distanceInfo = projCollider.Distance(groundCollider);

        HitData hitData = new HitData(distanceInfo.pointB, distanceInfo.normal, null);
        bool shouldKeepFlying = ProcessHit(hitData);

        if (!shouldKeepFlying) SetGrounded();
    }
    private void SetGrounded()
    {
        isGrounded = true;
        Invoke(nameof(OnTouchGround), delayCollisionIgnore);
    }
    private void OnTouchGround()
    {
        if (projCollider == null || !gameObject.activeInHierarchy) return;

        RuntimeState.DamageMultiplier = 0;

        //Dùng toán tử |= để CỘNG THÊM Mask thay vì ghi đè
        projCollider.excludeLayers = GameConstants.MASK_TOWER | GameConstants.MASK_PROJECTILE | GameConstants.MASK_ENEMY_BODY;
    }

    public override void ReturnToPool()
    {
        // Nếu viên đạn chạm đất -> bắt đầu đếm 0.1s -> nhưng 0.05s sau nó hết lifeTime và bị thu về Pool.
        // Nếu không Cancel, hàm OnTouchGround vẫn sẽ nổ ra khi đạn đang nằm trong Pool (hoặc khi vừa được lấy ra cho lần bắn tiếp theo), gây lỗi đạn xuyên quái ngay từ lúc bắn.
        CancelInvoke(nameof(OnTouchGround));

        base.ReturnToPool();
    }
}
