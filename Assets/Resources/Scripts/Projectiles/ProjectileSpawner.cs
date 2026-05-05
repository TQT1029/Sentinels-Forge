using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    private IObjectPool<Projectile> projectilePool;

    private void Start()
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
    }

    private Projectile CreateProjectile()
    {
        GameObject projectile = Instantiate();
    }

    private void OnGetProjectileFromPool(Projectile projectile)
    {
        projectile.transform.position = transform.position;
        projectile.transform.rotation = transform.rotation;
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnToPool(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
    private void OnDestroyPoolObject(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
}
