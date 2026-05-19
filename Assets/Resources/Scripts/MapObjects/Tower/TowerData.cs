using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "Game/Tower/Tower Data")]
public class TowerData : ScriptableObject
{
    [Header("Information")]
    public string towerName = "New Tower";
    public string description = "Description of the tower.";
    [Header("Stats")]
    public float maxHealth = 1000f;
    public float attackDamage = 50f;
    public float attackRange = 5f;
    public float attackCooldown = 1f;
    [Header("Visuals")]
    public Sprite towerSprite;
    public Sprite destroyedSprite;
    public RuntimeAnimatorController animationController;
}