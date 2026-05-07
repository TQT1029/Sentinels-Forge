using UnityEngine;


public enum EmnemyType
{
    Melee,
    Ranged,
    Flying,
    Boss
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Emnemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string emnemyName="";
    public EmnemyType emnemyType= EmnemyType.Melee;

    [Space(10)]
    [Min(1)] public float maxHealth=100f;
    [Min(0.005f)] public float moveSpeed=2f;

    [Space(10)]
    [Min(0.005f)] public float attackRange=1.5f;
    [Min(0f)] public float attackDamage=10f;
    [Min(0f)] public float damageVariation=2f;
    [Min(0f)] public float attackCooldown=1f;
}
