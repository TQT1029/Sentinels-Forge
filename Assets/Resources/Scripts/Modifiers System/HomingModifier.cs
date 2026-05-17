using UnityEngine;

[CreateAssetMenu(fileName = "HomingMod", menuName = "Game/Modifiers/Homing")]
public class HomingModifier : ModifierBase
{
    public int additionalHoming = 1; // Số lần homing có thể kích hoạt
    public float homingStrength = Mathf.PI; // Tốc độ bẻ lái (Turn rate - Radian/sec)
    [Tooltip("Góc quét phía trước mũi đạn. VD: 90 nghĩa là quét 45 độ sang mỗi bên")]
    public float homingSpreadAngle = 90f;

    [SerializeField] private float homingRange = 10f; // Tầm tìm kiếm mục tiêu để homing

    private const string HOMING_COUNT = "HomingCount";
    private const string FRAME_TIMER = "HomingFrameTimer";
    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(HOMING_COUNT, additionalHoming);
        state.SetStat(FRAME_TIMER, 0); // Đồng hồ đếm frame để tối ưu quét mục tiêu
        state.HomingTarget = null;
    }

    public override void OnUpdate(Projectile projectile, ProjectileRuntimeState state)
    {
        int homingLeft = state.GetStat(HOMING_COUNT);
        if (homingLeft <= 0) return;

        // Tìm mục tiêu mới nếu chưa có hoặc mục tiêu cũ đã chết/không còn hoạt động
        if (state.HomingTarget == null || !state.HomingTarget.gameObject.activeInHierarchy)
        {
            int frameTimer = state.GetStat(FRAME_TIMER);
            if (frameTimer >= 5) // Cứ 5 frame mới cho phép quét 1 lần để tiết kiệm CPU
            {
                state.HomingTarget = FindNearestTargetInCone(projectile);
                state.SetStat(FRAME_TIMER, 0);
            }
            else
            {
                state.SetStat(FRAME_TIMER, frameTimer + 1);
            }
        }

        // Bẻ lái (Steering) - Chạy mỗi frame để đạn bay mượt
        if (state.HomingTarget != null)
        {
            Vector2 currentVel = state.Velocity;
            Vector2 directionToTarget = ((Vector2)state.HomingTarget.position - (Vector2)projectile.transform.position).normalized;

            // Xoay vector vận tốc hiện tại hướng về mục tiêu một cách mượt mà
            Vector2 newDirection = Vector3.RotateTowards(
                currentVel.normalized,
                directionToTarget,
                homingStrength * Time.deltaTime,
                0f
            );

            projectile.rb.linearVelocity = newDirection * currentVel.magnitude;

            //Debug.Log($"[Homing] Projectile is homing towards {state.HomingTarget.name} with new direction {newDirection}");
        }
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy == null) return;

        // Tránh trùng lặp nếu có 2 thẻ Homing gắn vào cùng 1 đạn
        if (hitContext.HasHomed) return;

        int homingLeft = state.GetStat(HOMING_COUNT);

        if (homingLeft > 0)
        {
            hitContext.HasHomed = true;

            // Xử lý Hậu va chạm (Post-Hit): Bỏ mục tiêu cũ để đi tìm mục tiêu mới
            hitContext.PostHitActions += () =>
            {
                state.SetStat(HOMING_COUNT, homingLeft - 1);
                state.HomingTarget = null; // Xóa cache để OnUpdate tự tìm con quái tiếp theo
            };
        
            // Đạn tự huỷ khi va chạm
            hitContext.TerminateProjectile = true; 
        }
    }

    /// <summary>
    /// Thuật toán tìm mục tiêu tối ưu, có kết hợp Field of View (Cone) và loại trừ quái đã trúng đạn
    /// </summary>
    private Transform FindNearestTargetInCone(Projectile projectile)
    {
        // OverlapCircleAll tốn ít tài nguyên hơn Raycast, phù hợp tìm trong bán kính
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(projectile.transform.position, homingRange, GameConstants.MASK_ENEMY);

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity; // Dùng SqrMagnitude để tối ưu toán học (bỏ qua hàm Căn Bậc 2)
        Vector2 currentDir = projectile.rb.linearVelocity.normalized;

        foreach (var col in hitColliders)
        {
            // Kiểm tra xem đây có phải con quái đạn vừa xuyên qua không? (Nếu có thì lờ nó đi)
            EnemyAI enemy = col.GetComponent<EnemyAI>();
            if (enemy != null && projectile.hitTargets.Contains(enemy)) continue;

            Vector2 dirToEnemy = (col.transform.position - projectile.transform.position).normalized;

            // Kiểm tra xem quái có nằm trong "tầm nhìn" (Cone) của viên đạn không
            if (Vector2.Angle(currentDir, dirToEnemy) <= homingSpreadAngle / 2f)
            {
                float distanceSqr = (col.transform.position - projectile.transform.position).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    bestTarget = col.transform;
                }
            }
        }
        
        if (bestTarget != null)
        {
            //Debug.Log($"[HomingModifier] Found new target: {bestTarget.name}, Position: {bestTarget.position}");
        }

        return bestTarget;
    }
}


