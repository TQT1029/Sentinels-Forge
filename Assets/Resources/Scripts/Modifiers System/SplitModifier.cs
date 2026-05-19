using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SplitMod", menuName = "Game/Modifiers/Split")]
public class SplitModifier : BaseModifier
{
    public int additionalSplits = 1;
    public int splitNumber = 2; // Số viên đạn con được tạo ra mỗi lần split
    public float splitAngle = 30f; // Góc lệch giữa các viên đạn con

    private const string SPLIT_COUNT = "SplitCount";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        projectileSpawner = projectile.projectileSpawner;
        state.AddStat(SPLIT_COUNT, additionalSplits);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitContext.IsHandled || hitData.Enemy == null || hitContext.HasSplit) return;


        int splitsLeft = state.GetStat(SPLIT_COUNT);
        if (splitsLeft > 0)
        {
            hitContext.IsHandled = true;
            hitContext.HasSplit = true;
            hitContext.TerminateProjectile = true; // Vỡ đạn gốc

            state.SetStat(SPLIT_COUNT, splitsLeft - 1);

            hitContext.PostHitActions += () =>
            {
                // Tạo các viên đạn con
                for (int i = 0; i < splitNumber; i++)
                {
                    float angleOffset = splitAngle * (i - (splitNumber - 1) / 2f); // Tính góc lệch
                    Quaternion rotation = Quaternion.Euler(0, 0, angleOffset);
                    Vector2 newDirection = rotation * projectile.rb.linearVelocity.normalized;

                    // Lấy một viên đạn mới từ pool
                    Projectile newProj = projectileSpawner.CurrentPool.Get();

                    newProj.hitTargets = newProj.hitTargets = new HashSet<EnemyAI>(projectile.hitTargets); ; // Coppy danh sách đã trúng của viên đạn gốc

                    newProj.transform.position = hitData.Point + newDirection * 0.5f;
                    newProj.rb.linearVelocity = newDirection * projectile.rb.linearVelocity.magnitude;

                    newProj.RuntimeState.SetStat(SPLIT_COUNT, splitsLeft - 1);

                    // Kế thừa các chỉ số khác để đạn con cũng được nhận modifier tương tự
                    newProj.RuntimeState.SetStat("PierceCount", state.GetStat("PierceCount"));
                    newProj.RuntimeState.SetStat("BounceCount", state.GetStat("BounceCount"));

                    newProj.RuntimeState.Velocity = newProj.rb.linearVelocity;
                    newProj.ProcessImediate();
                }

                //Debug.Log($"[SplitModifier] Projectile split into {splitNumber} new projectiles with angle offset of {splitAngle} degrees.");
            };
            // Yêu cầu đạn tự hủy (Để nó không bay tiếp)
            hitContext.TerminateProjectile = true;
        }
    }
}
