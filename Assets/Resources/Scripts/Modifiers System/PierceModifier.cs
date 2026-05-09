using UnityEngine;

[CreateAssetMenu(fileName = "PierceMod", menuName = "Game/Modifiers/Pierce")]
public class PierceModifier : ModifierBase
{
    public int additionalPierces = 1;
    public float damageReductionPerHit = 0.8f; // Giảm còn 80%

    public override void OnLaunch(Projectile projectile)
    {
        // Khởi tạo state: Cấp cho viên đạn số lượt xuyên
        projectile.pierceCount += additionalPierces;
    }

    public override bool OnHitEnemy(Projectile projectile, EnemyAI enemy)
    {
        if (projectile.pierceCount > 0)
        {
            // Trừ lượt xuyên
            projectile.pierceCount--;

            // Gây sát thương và giảm sát thương cho lần xuyên sau 
            enemy.TakeDamage(projectile.currentDamage);
            projectile.currentDamage = projectile.currentDamage * damageReductionPerHit + projectile.projectileData.GetDamageAfterVariation();

            Debug.Log($"[PierceModifier] Projectile hit enemy. Remaining pierces: {projectile.pierceCount}, Current damage: {projectile.currentDamage}");

            // Yêu cầu giữ đạn sống, ngăn chạy các modifier phía sau
            return true;
        }

        // Hết lượt xuyên, nhường quyền xử lý lại cho các Modifier xếp sau nó
        return false;
    }
}