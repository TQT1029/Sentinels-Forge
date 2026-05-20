using UnityEngine;
public class WeaponControl : MonoBehaviour
{
    public GameObject firePoint;
    [field: SerializeField] public ProjectileSpawner projectileSpawner { get; private set; }
    [field: SerializeField] public WeaponData weaponData { get; private set; }
    public ProjectileData CurrentProjectileData => projectileSpawner != null ? projectileSpawner.projectileData : weaponData.listOfAvailableProjectiles[0];

    private IFireBehavior fireBehavior;
    private float lastFireTime = 0;

    protected virtual void Awake()
    {
        if (firePoint == null)
        {
            Transform foundPoint = transform.Find("FirePoint");
            if (foundPoint != null) firePoint = foundPoint.gameObject;
        }

        if (projectileSpawner == null && firePoint != null)
        {
            projectileSpawner = firePoint.GetComponent<ProjectileSpawner>();
        }

        if (projectileSpawner != null && weaponData != null)
        {
            projectileSpawner.SetupData(weaponData.listOfAvailableProjectiles[0]);
        }

        InitializeFireBehavior();
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


    protected virtual void Update()
    {
        // Kiểm tra an toàn thay vì dùng try-catch để tối ưu FPS
        if (Input.GetKeyDown(KeyCode.Keypad1)) ChangeProjectile(1);
        else if (Input.GetKeyDown(KeyCode.Keypad2)) ChangeProjectile(2);
        else if (Input.GetKeyDown(KeyCode.Keypad3)) ChangeProjectile(3);
        else if (Input.GetKeyDown(KeyCode.Keypad4)) ChangeProjectile(4);

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


    public virtual void ChangeProjectile(int index)
    {
        // Trừ 1 vì list trong C# bắt đầu từ 0 (Keypad 1 = index 0)
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
            projectileSpawner.projectileData = newData;
            Debug.Log($"[WeaponControl] Projectile data changed to: {projectileSpawner.projectileData}");
        }
    }
}