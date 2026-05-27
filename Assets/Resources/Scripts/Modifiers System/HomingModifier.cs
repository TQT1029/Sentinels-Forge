using UnityEngine;

[CreateAssetMenu(fileName = "HomingMod", menuName = "Game/Modifiers/Homing")]
public class HomingModifier : BaseModifier
{
    public int additionalHoming = 1;
    public float homingStrength = Mathf.PI;
    [Tooltip("Góc quét phía trước mũi đạn. VD: 90 nghĩa là quét 45 độ sang mỗi bên")]
    public float homingSpreadAngle = 90f;
    [SerializeField] private float homingRange = 10f;

    public const string HOMING_COUNT = "HomingCount";
    private const string FRAME_TIMER = "HomingFrameTimer";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(HOMING_COUNT, additionalHoming);
        state.SetStat(FRAME_TIMER, 0);
        state.HomingTarget = null;
    }

    public override void OnUpdate(Projectile projectile, ProjectileRuntimeState state)
    {
        if (state.GetStat(HOMING_COUNT) <= 0) return;

        if (state.HomingTarget == null || !state.HomingTarget.gameObject.activeInHierarchy)
        {
            int frameTimer = state.GetStat(FRAME_TIMER);
            if (frameTimer >= 5)
            {
                state.HomingTarget = FindNearestTargetInCone(projectile);
                state.SetStat(FRAME_TIMER, 0);
            }
            else
            {
                state.SetStat(FRAME_TIMER, frameTimer + 1);
            }
        }

        if (state.HomingTarget != null)
        {
            Vector2 currentVel = state.Velocity;
            Vector2 directionToTarget = ((Vector2)state.HomingTarget.position - (Vector2)projectile.transform.position).normalized;

            Vector2 newDirection = Vector3.RotateTowards(
                currentVel.normalized,
                directionToTarget,
                homingStrength * Time.deltaTime,
                0f
            );

            projectile.rb.linearVelocity = newDirection * currentVel.magnitude;
        }
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy == null) return;
        if (hitContext.HasHomed) return;

        int homingLeft = state.GetStat(HOMING_COUNT);
        if (homingLeft <= 0) return;

        hitContext.HasHomed = true;
        hitContext.TerminateProjectile = true;

        int capturedHoming = homingLeft;
        hitContext.PostHitActions += () =>
        {
            state.SetStat(HOMING_COUNT, capturedHoming - 1);
            state.HomingTarget = null;
        };
    }

    public override void InheritState(ProjectileRuntimeState source, ProjectileRuntimeState destination)
    {
        destination.SetStat(HOMING_COUNT, source.GetStat(HOMING_COUNT));
        // FRAME_TIMER không kế thừa — đạn con tự tìm mục tiêu ngay từ đầu
    }

    private Transform FindNearestTargetInCone(Projectile projectile)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
            projectile.transform.position, homingRange, GameConstants.MASK_ENEMY_HITBOX);

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector2 currentDir = projectile.rb.linearVelocity.normalized;

        foreach (var col in hitColliders)
        {
            EnemyAI enemy = col.GetComponentInParent<EnemyAI>();
            if (enemy != null && projectile.hitTargets.Contains(enemy)) continue;

            Vector2 dirToEnemy = (col.transform.position - projectile.transform.position).normalized;

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

        return bestTarget;
    }
}