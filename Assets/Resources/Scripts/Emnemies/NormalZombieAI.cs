using UnityEngine;

public class NormalZombieAI : EmnemyAI
{

    protected override void Attack()
    {
        base.Attack();

        float damage = emnemyData.attackDamage + RandomUtils.RandomWithSteps(-emnemyData.damageVariation, emnemyData.damageVariation, 0.25f);

        Debug.Log($"[NormalZombieAI] Attacking tower with {damage} damage!");
    }
}
