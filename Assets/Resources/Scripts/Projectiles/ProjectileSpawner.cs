using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    // Dictionary lưu trữ Pool riêng cho từng loại đạn
    private Dictionary<ProjectileData, IObjectPool<Projectile>> poolsMap;

    [field: SerializeField] public GameObject projectilesStoreObj { get; private set; }
    public ProjectileData projectileData; // Loại đạn đang được chọn

    // Property này tự động trả về Pool của loại đạn đang chọn. 
    // Nếu loại đạn này chưa có Pool, nó sẽ tự động tạo mới (Lazy Initialization)
    public IObjectPool<Projectile> CurrentPool
    {
        get
        {
            if (poolsMap == null) poolsMap = new Dictionary<ProjectileData, IObjectPool<Projectile>>();

            if (!poolsMap.ContainsKey(projectileData))
            {
                // Capture data hiện tại vào biến local để các hàm delegate không bị nhầm lẫn khi đổi đạn
                ProjectileData dataToSpawn = projectileData;

                poolsMap[dataToSpawn] = new ObjectPool<Projectile>(
                    createFunc: () => CreateProjectile(dataToSpawn),
                    actionOnGet: p => OnGetProjectileFromPool(p, dataToSpawn),
                    actionOnRelease: OnReturnToPool,
                    actionOnDestroy: OnDestroyPoolObject,
                    collectionCheck: false,
                    defaultCapacity: 10,
                    maxSize: 100
                );
            }
            return poolsMap[projectileData];
        }
    }

    private void Awake()
    {
        poolsMap = new Dictionary<ProjectileData, IObjectPool<Projectile>>();

        if (projectilesStoreObj == null)
        {
            projectilesStoreObj = GameObject.Find("ProjectilesStoreObj");
            if (projectilesStoreObj == null)
            {
                projectilesStoreObj = new GameObject("ProjectilesStoreObj");
            }
        }
    }
    private Projectile CreateProjectile(ProjectileData data)
    {
        GameObject projectileObj = Instantiate(data.prefab, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90), projectilesStoreObj.transform);
        Projectile projectileComponent = projectileObj.GetComponent<Projectile>();

        projectileComponent.SetData(data);
        projectileComponent.SetSpawner(this);
        projectileComponent.SetPool(poolsMap[data]);

        projectileComponent.SetupPhysic();
        return projectileComponent;
    }

    public void OnGetProjectileFromPool(Projectile projectile, ProjectileData data)
    {
        projectile.transform.position = transform.position;

        // Sửa lỗi Quaternion ở đây: Phải dùng eulerAngles.z thay vì .z
        projectile.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - 90);
        projectile.gameObject.SetActive(true);

        projectile.Init(data.lifeTime);
    }

    public void OnReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    public void OnDestroyPoolObject(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}