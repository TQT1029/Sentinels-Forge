using UnityEngine;

public class NormalZombieAI : EnemyAI
{

    protected override void Attack()
    {
        base.Attack();

        float damage = enemyData.attackDamage + RandomUtils.RandomWithSteps(-enemyData.damageVariation, enemyData.damageVariation, 0.25f);

        Debug.Log($"[NormalZombieAI] Attacking tower with {damage} damage!");
    }
}
