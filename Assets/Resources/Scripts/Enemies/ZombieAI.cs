using UnityEngine;

public class ZombieAI : EnemyAI
{

    protected override void Attack()
    {
        base.Attack();

        float damage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        Debug.Log($"[ZombieAI] Attacking tower with {damage} damage!");
    }
}
