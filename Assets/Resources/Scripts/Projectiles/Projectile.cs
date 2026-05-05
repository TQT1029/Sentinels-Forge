using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering.VirtualTexturing;

public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData {  get; private set; }

    private IObjectPool<Projectile> managedPool;

    public void setPool(IObjectPool<Projectile> pool)
    {
        managedPool = pool;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnToPool), projectileData.lifeTime);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void ReturnToPool()
    {
        managedPool.Release(this);
    }
}
