using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    public IObjectPool<Projectile> projectilePool;
    [SerializeField] private GameObject projectilesStoreObj;
    public ProjectileData projectileData;

    private void Awake()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnGetProjectileFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 100
        );

        if (projectilesStoreObj == null)
        {
            projectilesStoreObj = new GameObject("ProjectilesStoreObj");
        }
    }

    private Projectile CreateProjectile()
    {
        GameObject projectileObj = Instantiate(projectileData.prefab, transform.position, Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + 90), projectilesStoreObj.transform);
        Projectile projectileComponent = projectileObj.GetComponent<Projectile>();
        projectileComponent.setPool(projectilePool);
        //Debug.Log("[ProjectileSpawner] Created projectile: " + projectileComponent);

        return projectileComponent;
    }

    public void OnGetProjectileFromPool(Projectile projectile)
    {
        projectile.transform.position = transform.position;
        projectile.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z - 90);
        projectile.gameObject.SetActive(true);

        projectile.Init(projectileData.lifeTime);
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