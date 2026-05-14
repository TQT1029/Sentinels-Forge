using System.Collections.Generic;
using UnityEngine;


public enum EnemyType
{
    Melee,
    Ranged,
    Flying,
    Support,
    Boss
}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Introduce")]
    public string enemyName = "";
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Base Stats")]
    [Space(10)]
    [Min(1)] public float maxHealth = 100f;
    [Min(0.005f)] public float moveSpeed = 2f;

    [Space(10)]
    [Min(0.005f)] public float attackRange = 1.5f;
    [Min(0f)] public float attackDamage = 10f;
    [Min(0f)] public float damageVariation = 2f;
    [Min(0f)] public float attackCooldown = 1f;

    [Header("Support Class Stats")]
    public List<EffectData> listAvailableBuffs = new List<EffectData>();
    [Min(3f)] public float buffRange = 1.5f;
    [Min(0f)] public float buffCooldown = 1f;
}
