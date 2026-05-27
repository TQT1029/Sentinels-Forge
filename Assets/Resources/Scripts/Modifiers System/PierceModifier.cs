using UnityEngine;

[CreateAssetMenu(fileName = "PierceMod", menuName = "Game/Modifiers/Pierce")]
public class PierceModifier : BaseModifier
{
    public int additionalPierces = 1;
    public float damageReduction = 0.8f;

    public const string PIERCE_COUNT = "PierceCount";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(PIERCE_COUNT, additionalPierces);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitContext.IsHandled || hitData.Enemy == null) return;

        int piercesLeft = state.GetStat(PIERCE_COUNT);
        if (piercesLeft <= 0) return;

        state.SetStat(PIERCE_COUNT, piercesLeft - 1);
        hitContext.IsHandled = true;
        hitContext.TerminateProjectile = false;

        if (hitData.Collider != null && projectile.projCollider != null)
        {
            Physics2D.IgnoreCollision(projectile.projCollider, hitData.Collider, true);
            projectile.IgnoredColliders.Add(hitData.Collider);
        }

        hitContext.PostHitActions += () => state.DamageMultiplier *= damageReduction;
    }

    public override void InheritState(ProjectileRuntimeState source, ProjectileRuntimeState destination)
    {
        destination.SetStat(PIERCE_COUNT, source.GetStat(PIERCE_COUNT));
    }
}