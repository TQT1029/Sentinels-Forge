using UnityEngine;

[CreateAssetMenu(fileName = "PierceMod", menuName = "Game/Modifiers/Pierce")]
public class PierceModifier : ModifierBase
{
    public int additionalPierces = 1;
    public float damageReduction = 0.8f;

    // Dùng string hằng số làm Key truy xuất RuntimeState
    private const string PIERCE_COUNT = "PierceCount";

    public override void OnLaunch(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(PIERCE_COUNT, additionalPierces);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        // Pierce chỉ hoạt động nếu đụng quái vật
        if (hitData.Enemy == null) return;

        int piercesLeft = state.GetStat(PIERCE_COUNT);

        if (piercesLeft > 0)
        {
            state.SetStat(PIERCE_COUNT, piercesLeft - 1); // Trừ số lượt xuyên
            hitContext.PostHitActions += () =>
            {
                state.DamageMultiplier *= damageReduction;
            };
            // CỐT LÕI: Yêu cầu đạn KHÔNG tự hủy (Bay tiếp)
            hitContext.TerminateProjectile = false;
        }
    }
}