using UnityEngine;

[CreateAssetMenu(fileName = "BounceMod", menuName = "Game/Modifiers/Bounce")]
public class BounceModifier : BaseModifier
{
    public int additionalBounces = 2;
    public float speedRetention = 0.7f;

    public const string BOUNCE_COUNT = "BounceCount";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(BOUNCE_COUNT, additionalBounces);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy != null) return;
        if (hitContext.IsHandled) return;

        int bouncesLeft = state.GetStat(BOUNCE_COUNT);
        if (bouncesLeft <= 0) return;

        state.SetStat(BOUNCE_COUNT, bouncesLeft - 1);
        hitContext.IsHandled = true;
        hitContext.TerminateProjectile = false;

        Vector2 currentVel = state.Velocity;
        Vector2 reflected = Vector2.Reflect(currentVel.normalized, hitData.Normal);
        projectile.rb.linearVelocity = reflected * currentVel.magnitude * speedRetention;

        // Đẩy đạn ra khỏi bề mặt để tránh double-hit vật lý ngay frame tiếp theo
        projectile.transform.position = hitData.Point + hitData.Normal * 0.05f;

        projectile.ClearHitTargets();
    }

    public override void InheritState(ProjectileRuntimeState source, ProjectileRuntimeState destination)
    {
        destination.SetStat(BOUNCE_COUNT, source.GetStat(BOUNCE_COUNT));
    }
}