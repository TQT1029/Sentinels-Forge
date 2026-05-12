using UnityEngine;

[CreateAssetMenu(fileName = "BounceMod", menuName = "Game/Modifiers/Bounce")]
public class BounceModifier : ModifierBase
{
    public int additionalBounces = 2;
    public float speedRetention = 0.7f; // Tỉ lệ giữ lại tốc độ sau mỗi lần nảy
    private const string BOUNCE_COUNT = "BounceCount";
    

    public override void OnFire(Projectile projectile, ProjectileRuntimeState state)
    {
        state.AddStat(BOUNCE_COUNT, additionalBounces);
    }

    public override void OnHit(Projectile projectile, ProjectileRuntimeState state, HitData hitData, HitActionContext hitContext)
    {
        if (hitData.Enemy != null) return;
        int bouncesLeft = state.GetStat(BOUNCE_COUNT);

        if (bouncesLeft > 0)
        {
            state.SetStat(BOUNCE_COUNT, bouncesLeft - 1);

            // Logic phản xạ tia (Toán học)
            Vector2 currentVel = projectile.rb.linearVelocity;
            Vector2 reflected = Vector2.Reflect(currentVel.normalized, hitData.Normal);
            projectile.rb.linearVelocity = reflected * currentVel.magnitude * speedRetention;

            // Đẩy nhẹ đạn ra khỏi tường để chống kẹt (chống Double-hit vật lý)
            projectile.transform.position = hitData.Point + hitData.Normal * 0.05f;

            // CỐT LÕI: Yêu cầu đạn KHÔNG tự hủy (Để nó bay tiếp theo hướng mới)
            hitContext.TerminateProjectile = false;

            // Xoá danh sách quái đã trúng để đạn nảy lại có thể đánh trúng con cũ
             projectile.ClearHitTargets(); // Nếu bạn public hàm này bên Projectile
        }
    }
}