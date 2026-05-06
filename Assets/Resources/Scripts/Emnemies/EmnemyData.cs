using UnityEngine;


public enum EmnemyType
{
    Melee,
    Ranged,
    Flying,
    Boss
}

[CreateAssetMenu(fileName = "EmnemyData", menuName = "Game/Emnemy/EmnemyData")]
public class EmnemyData : ScriptableObject
{
    public string emnemyName="";
    public EmnemyType emnemyType= EmnemyType.Melee;

    public float maxHealth=100f;
    public float moveSpeed=2f;

    public float attackRange=1f;
    public float attackDamage=10f;
    public float damageVariation=2f;
    public float attackCooldown=1f;
}
