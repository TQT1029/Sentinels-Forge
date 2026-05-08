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
    public float gravityScale = 1;// Hệ số trọng lực ảnh hưởng đến quỹ đạo bay của đạn (1 là bình thường, 0 là không chịu trọng lực, >1 là chịu trọng lực mạnh hơn)
    public float stunDuration = 0f; // Thời gian làm choáng (nếu có)
    public float lifeTime = 5f; 
}
