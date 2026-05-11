using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "Game/Projectile/Projectile Stats")]
public class ProjectileData : ScriptableObject
{
    public GameObject prefab; // Prefab của đạn

    [Space(10)]
    public float baseDamage = 1;
    [Range(0f, 1f), Tooltip("Damage dao động ±damageVariation*baseDamage")] public float damageVariation = 0.05f; // Damage dao động trong khoảng baseDamage ± damageVariation*baseDamage
    [Space(10)] 
    public float gravityScale = 1;// Hệ số trọng lực ảnh hưởng đến quỹ đạo bay của đạn
    [Min(1f)] public float lifeTime = 5f; // Thời gian tồn tại của đạn (0 = không giới hạn)
    public float GetDamageAfterVariation()
    {
        return RandomUtils.RandomWithSteps(-damageVariation, damageVariation, 0.001f) * baseDamage;
    }

}
