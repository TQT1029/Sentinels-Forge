using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Game/Projectile/Projectile Stats")]
public class ProjectileData : ScriptableObject
{
    public GameObject prefab; // Prefab của đạn
    public float baseDamage = 1;
    public float damageVariation = 0.5f; // Damage dao động trong khoảng baseDamage ± damageVariation

    public float lifeTime = 5f; 
}
