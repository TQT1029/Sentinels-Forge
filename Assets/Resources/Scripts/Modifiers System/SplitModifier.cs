using UnityEngine;

[CreateAssetMenu(fileName = "SplitModifier", menuName = "Game/Modifiers/Split")]
public class SplitModifier : ModifierBase
{
    private int additionalProjectiles = 2;
    [Range(0f, 180f)] private float splitAngle = 30f;
    [Range(0f, 1f)] private float splitDamageMultiplier = 0.8f;
    public override void OnLaunch(Projectile projectile)
    {
        projectile.splitCount += additionalProjectiles;
    }
    public override bool OnHitEnemy(Projectile projectile, EnemyAI enemy)
    {
        if (projectile.splitCount > 0)
        {
            projectile.splitCount--;
            
            enemy.TakeDamage(projectile.currentDamage);
            
            
            return true; // Đạn tiếp tục bay sau khi chia tách
        }
        return false; // Không còn viên đạn để chia tách, đạn dừng lại
    }
}
