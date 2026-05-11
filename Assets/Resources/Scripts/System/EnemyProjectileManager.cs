using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyProjectileManager : Singleton<EnemyProjectileManager>
{
    // Map Prefab -> Pool để xài chung đạn cho mọi loại quái Ranged
    private Dictionary<EnemyProjectile, IObjectPool<EnemyProjectile>> projectilePools = new Dictionary<EnemyProjectile, IObjectPool<EnemyProjectile>>();
    private Transform enemyProjectilesStoreObj;

    protected override void Awake()
    {
        base.Awake();
        // Tạo một GameObject để chứa tất cả đạn của quái, giúp Hierarchy gọn hơn
        enemyProjectilesStoreObj = new GameObject("EnemyProjectilesStoreObj").transform;
    }   
    public void SpawnProjectile(EnemyProjectile prefab, Vector3 spawnPos, Vector3 targetPos, float damage)
    {
        if (!projectilePools.ContainsKey(prefab))
        {
            // Lazy initialization: Chỉ tạo Pool khi có con quái đầu tiên cần loại đạn này
            projectilePools[prefab] = new ObjectPool<EnemyProjectile>(
                createFunc: () => {
                    EnemyProjectile proj = Instantiate(prefab, enemyProjectilesStoreObj);
                    proj.SetPool(projectilePools[prefab]);
                    return proj;
                },
                actionOnGet: proj => proj.gameObject.SetActive(true),
                actionOnRelease: proj => proj.gameObject.SetActive(false),
                actionOnDestroy: proj => Destroy(proj.gameObject),
                defaultCapacity: 20,
                maxSize: 100
            );
        }

        // Lấy đạn ra và bắn
        EnemyProjectile newProj = projectilePools[prefab].Get();
        newProj.transform.position = spawnPos;

        Vector2 direction = (targetPos - spawnPos).normalized;
        newProj.Fire(direction, damage);
    }
}