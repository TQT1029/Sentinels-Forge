using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Game/Projectile/Projectile Stats")]
public class ProjectileData : ScriptableObject
{
    public GameObject prefab; // Prefab của đạn

    [Space(10)]
    public float baseDamage = 1;
    public float damageVariation = 0.5f; // Damage dao động trong khoảng baseDamage ± damageVariation

    [Space(10)] 
    public float weight = 1;//Trọng lượng của đạn để tính khoảng cách rơi
    public float stunDuration = 0f; // Thời gian làm choáng (nếu có)
    public float lifeTime = 5f; 
}
