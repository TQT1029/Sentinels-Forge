using UnityEngine;
public class WeaponControl : MonoBehaviour
{
    public GameObject firePoint;
    [field: SerializeField] public ProjectileSpawner projectileSpawner { get; private set; }
    [field: SerializeField] public WeaponData weaponData;
    public ProjectileData CurrentProjectileData => projectileSpawner != null ? projectileSpawner.projectileData : weaponData.listOfAvailableProjectiles[0];

    private IFireBehavior fireBehavior;
    private float lastFireTime = 0;

    protected void Awake()
    {
        if (firePoint == null)
        {
            firePoint= transform.Find("FirePoint")?.gameObject;
        }

        if (projectileSpawner == null && firePoint != null)
        {
            projectileSpawner = firePoint.GetComponent<ProjectileSpawner>();
        }

    }
    public void EquipWeapon(WeaponData newWeaponData, ProjectileData newProjectileData)
    {
        if (newWeaponData == null || newProjectileData == null)
        {
            Debug.LogError("[WeaponControl] Data truyền vào bị Null!");
            return;
        }

        weaponData = newWeaponData;

        // 1. Cập nhật lại Chiến thuật bắn (Strategy Pattern)
        InitializeFireBehavior();

        // 2. Cập nhật Đạn cho Spawner
        ChangeProjectileData(newProjectileData);

        Debug.Log($"[WeaponControl] Đã trang bị thành công: {weaponData.name} với đạn {newProjectileData.name}");
    }

    private void InitializeFireBehavior()
    {

        switch (weaponData.weaponType)
        {
            case WeaponType.Single:
                fireBehavior = new StandardFireBehavior();
                break;
            case WeaponType.Shotgun:
                fireBehavior = new StandardFireBehavior();
                break;
            case WeaponType.Burst:
                fireBehavior = new BurstFireBehavior();
                break;
            case WeaponType.Auto:
                fireBehavior = new AutoFireBehavior();
                break;
            case WeaponType.Laser:
                fireBehavior = new LaserFireBehavior();
                break;
        }
    }

    /// <summary>
    /// Player/Input System sẽ gọi hàm này khi người chơi muốn xả đạn (Nhả chuột kéo thả)
    /// </summary>
    public bool TryFire(Vector2 aimDirection, float chargePower = 1f)
    {
        if (Time.time - lastFireTime < weaponData.fireCooldown) return false; // Chưa hồi xong

        lastFireTime = Time.time;

        // Giao việc xả đạn cho Behavior xử lý
        fireBehavior.ExecuteFire(this, aimDirection, chargePower);

        return true;
    }

    /// <summary>
    /// Hàm API để các FireBehavior gọi ra sinh đạn. Tương thích hoàn hảo với ProjectileSystem hiện tại.
    /// </summary>
    public void SpawnAndFireProjectile(Vector2 direction, float velocity)
    {
        if (projectileSpawner == null) return;

        Projectile projectile = projectileSpawner.CurrentPool.Get();

        if (projectile.rb != null)
        {
            projectile.rb.linearVelocity = direction.normalized * velocity;
        }
    }


    public void ChangeProjectile(int index)
    {
        int listIndex = index - 1;

        if (weaponData.listOfAvailableProjectiles != null && listIndex >= 0 && listIndex < weaponData.listOfAvailableProjectiles.Count)
        {
            ChangeProjectileData(weaponData.listOfAvailableProjectiles[listIndex]);
        }
        else
        {
            Debug.LogWarning($"[WeaponControl] Không có loại đạn nào ở slot {index}.");
        }
    }

    private void ChangeProjectileData(ProjectileData newData)
    {
        if (projectileSpawner != null)
        {
            projectileSpawner.SetupData(newData);
            Debug.Log($"[WeaponControl] Projectile data changed to: {projectileSpawner.projectileData}");
        }
    }
}