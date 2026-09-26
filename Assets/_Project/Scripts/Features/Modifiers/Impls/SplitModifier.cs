using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitMod", menuName = "Game/Modifiers/Split")]
public class SplitModifier : BaseModifier
{
    public int additionalSplits = 1;
    public int splitNumber = 2;
    public float splitAngle = 30f;

    public const string SPLIT_COUNT = "SplitCount";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(SPLIT_COUNT, additionalSplits);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitContext.IsHandled || hitData.Enemy == null || hitContext.HasSplit) return;

        int splitsLeft = state.GetStat(SPLIT_COUNT);
        if (splitsLeft <= 0) return;

        hitContext.IsHandled = true;
        hitContext.HasSplit = true;
        hitContext.TerminateProjectile = true;
        state.SetStat(SPLIT_COUNT, splitsLeft - 1);

        // Capture trước khi vào closure để tránh capture biến ngoài scope
        ProjectileSpawner spawner = projectile.projectileSpawner;
        Vector2 fireVelocity = projectile.rb.linearVelocity;
        Vector2 hitPoint = hitData.Point;
        int remainingSplits = splitsLeft - 1;
        HashSet<EnemyAI> inheritedHitTargets = new HashSet<EnemyAI>(projectile.hitTargets);
        List<BaseModifier> inheritedModifiers = projectile.modifiers;
        ProjectileRuntimeState sourceState = state;

        hitContext.PostHitActions += () =>
        {
            for (int i = 0; i < splitNumber; i++)
            {
                float angleOffset = splitAngle * (i - (splitNumber - 1) / 2f);
                Vector2 newDirection = (Quaternion.Euler(0, 0, angleOffset) * fireVelocity.normalized);

                Projectile newProj = spawner.CurrentPool.Get();

                newProj.hitTargets = new HashSet<EnemyAI>(inheritedHitTargets);
                newProj.transform.position = hitPoint + newDirection * 0.5f;
                newProj.rb.linearVelocity = newDirection * fireVelocity.magnitude;

                // Mỗi modifier tự copy phần state của nó — không hardcode tên stat của modifier khác
                if (inheritedModifiers != null)
                {
                    foreach (var mod in inheritedModifiers)
                        mod.InheritState(sourceState, newProj.RuntimeState);
                }

                // Override split count cho đạn con sau khi InheritState chạy xong
                newProj.RuntimeState.SetStat(SPLIT_COUNT, remainingSplits);
                newProj.RuntimeState.Velocity = newProj.rb.linearVelocity;

                newProj.ProcessImediate();
            }
        };
    }

    public override void InheritState(ProjectileRuntimeState source, ProjectileRuntimeState destination)
    {
        destination.SetStat(SPLIT_COUNT, source.GetStat(SPLIT_COUNT));
    }
}