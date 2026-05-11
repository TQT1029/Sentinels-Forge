using UnityEngine;

public class SkeletonAI : EnemyAI
{
    protected override void Attack()
    {
        float damage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);
        //Debug.Log($"[SkeletonAI] Attacking tower with {damage} damage!");
    }
}
