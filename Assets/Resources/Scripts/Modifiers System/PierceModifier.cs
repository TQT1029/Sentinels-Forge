using UnityEngine;

[CreateAssetMenu(fileName = "PierceMod", menuName = "Game/Modifiers/Pierce")]
public class PierceModifier : ModifierBase
{
    public int additionalPierces = 1;
    public float damageReduction = 0.8f;

    // Dùng string hằng số làm Key truy xuất RuntimeState
    private const string PIERCE_COUNT = "PierceCount";

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(PIERCE_COUNT, additionalPierces);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        // Chặn luồng nếu hit này đã bị Modifier khác (đứng trước) xử lý, hoặc trúng đất
        if (hitContext.IsHandled || hitData.Enemy == null) return;
        int piercesLeft = state.GetStat(PIERCE_COUNT);

        if (piercesLeft > 0)
        {
            state.SetStat(PIERCE_COUNT, piercesLeft - 1); // Trừ số lượt xuyên

            hitContext.IsHandled = true;
            hitContext.TerminateProjectile = false; // Xuyên qua, không hủy đạn

            if (hitData.Collider!=null && projectile.projCollider != null)
            {
                Physics2D.IgnoreCollision(projectile.projCollider, hitData.Collider, true);

                projectile.IgnoredColliders.Add(hitData.Collider);
            }

            hitContext.PostHitActions += () =>
            {
                state.DamageMultiplier *= damageReduction;
            };

        }
    }
}