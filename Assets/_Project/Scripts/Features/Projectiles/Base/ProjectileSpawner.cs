using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    public WeaponControl weaponControl;

    // Dictionary lưu trữ Pool riêng cho từng loại đạn
    private Dictionary<ProjectileData, IObjectPool<Projectile>> poolsMap;

    public Transform projectilesStorageObj { get; private set; }
    [HideInInspector] public ProjectileData projectileData; // Loại đạn đang được chọn

    // Property này tự động trả về Pool của loại đạn đang chọn. 
    // Nếu loại đạn này chưa có Pool, nó sẽ tự động tạo mới (Lazy Initialization)
    public IObjectPool<Projectile> CurrentPool
    {
        get
        {
            if (poolsMap == null) poolsMap = new Dictionary<ProjectileData, IObjectPool<Projectile>>();

            if (projectileData == null)
            {
                Debug.LogError("[ProjectileSpawner] projectileData chưa được setup!");
                return null;
            }

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
                    maxSize: 800
                );
            }
            return poolsMap[projectileData];
        }
    }

    private void Awake()
    {
        poolsMap = new Dictionary<ProjectileData, IObjectPool<Projectile>>();

        if (projectilesStorageObj == null)
        {
            projectilesStorageObj = GameObject.Find(GameConstants.Config.PROJECTILE_STORAGE_NAME)?.transform;
            if (projectilesStorageObj == null)
            {
                projectilesStorageObj = new GameObject(GameConstants.Config.PROJECTILE_STORAGE_NAME).transform;
            }
        }

        weaponControl = GetComponentInParent<WeaponControl>();
    }

    public void SetupData(ProjectileData data)
    {
        projectileData = data;
    }
    private Projectile CreateProjectile(ProjectileData data)
    {
        GameObject projectileObj = Instantiate(data.prefab, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90), projectilesStorageObj);
        Projectile projectileComponent = projectileObj.GetComponent<Projectile>();

        projectileComponent.SetProjectileData(data);
        projectileComponent.SetSpawner(this);
        projectileComponent.SetWeaponControl(weaponControl);
        projectileComponent.SetPool(poolsMap[data]);

        return projectileComponent;
    }

    public void OnGetProjectileFromPool(Projectile projectile, ProjectileData data)
    {
        projectile.transform.position = transform.position;

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