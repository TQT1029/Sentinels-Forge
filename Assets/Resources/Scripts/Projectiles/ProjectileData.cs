using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Game/Projectile/Projectile Stats")]
public class ProjectileData : ScriptableObject
{
    public GameObject prefab; // Prefab của đạn

    [Space(10)]
    public float baseDamage = 1;
    [Range(0f, 1f)] public float criticalChance = 0.1f; // Tỉ lệ chí mạng (0-1)
    [Min(1f)] public float criticalMultiplier = 1.5f; // Hệ số nhân damage khi chí mạng

    [Space(10)] 
    public float gravityScale = 1;// Hệ số trọng lực ảnh hưởng đến quỹ đạo bay của đạn
    [Min(1f)] public float lifeTime = 10f; // Thời gian tồn tại của đạn (0 = không giới hạn)


}
