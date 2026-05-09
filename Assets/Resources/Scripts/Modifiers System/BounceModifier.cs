using UnityEngine;
/// <summary>
/// Tự động nảy lên khi chạm đất hoặc tường.
/// </summary>
[CreateAssetMenu(fileName = "BounceMod", menuName = "Game/Modifiers/Bounce")]
public class BounceModifier : ModifierBase
{
    [Header("Bounce Settings")]
    public int additionalBounces = 3;

    [Tooltip("Hệ số giữ lại vận tốc sau khi nảy (1 = không giảm tốc, 0.5 = giảm một nửa)")]
    [Range(0f, 1f)] public float bounciness = 1f;

    public override void OnLaunch(Projectile projectile)
    {
        // Cấp thêm lượt nảy cho viên đạn
        projectile.bounceCount += additionalBounces;
    }

    public override bool OnEnvironmentHit(Projectile projectile, RaycastHit2D hit)
    {
        if (projectile.bounceCount > 0)
        {
            projectile.bounceCount--;

            Vector2 currentVelocity = projectile.rb.linearVelocity;

            Vector2 reflectedDirection = Vector2.Reflect(currentVelocity.normalized, hit.normal);

            projectile.rb.linearVelocity = reflectedDirection * currentVelocity.magnitude * bounciness;

            projectile.transform.position = hit.point + hit.normal * 0.05f;


            Debug.Log($"[BounceModifier] Projectile bounced. Remaining bounces: {projectile.bounceCount}");
            return true; // Yêu cầu đạn tiếp tục bay, không được găm vào tường
        }

        return false; // Hết số lượt nảy, nhường quyền cho đạn găm vào tường
    }
}
