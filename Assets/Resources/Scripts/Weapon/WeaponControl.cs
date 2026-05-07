using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 anchorPoint;
    [SerializeField] private float maxDrag = 10f;

    [SerializeField] private ProjectileSpawner spawner;
    public GameObject launcherPoint;
    public WeaponData weaponData;

    private bool isDragging = false;
    private float currentAngle = 0;
    private float launchVelocity = 0;
    private float lastLaunchTime = 0;

    private void Awake()
    {
        mainCam = WaveManager.Instance.MainCamera;

        // Sửa thứ tự: Tìm GameObject trước rồi mới lấy Component
        if (launcherPoint == null)
        {
            Transform foundPoint = transform.Find("LauncherPoint");
            if (foundPoint != null) launcherPoint = foundPoint.gameObject;
        }

        if (spawner == null && launcherPoint != null)
        {
            spawner = launcherPoint.GetComponent<ProjectileSpawner>();
        }
    }

    private void Start()
    {
        if (anchorPoint == null || anchorPoint == Vector3.zero)
        {
            anchorPoint = transform.position;
        }
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Shoot();
        }

        if (isDragging)
        {
            currentAngle = calculateRotationAngle();
            launchVelocity = calculateLaunchVelocity() * weaponData.launchVelocity;

            Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * launchVelocity, Color.blueViolet, 0.1f);

            currentAngle += +Random.Range(-weaponData.angleVariation, weaponData.angleVariation); // Thêm một chút biến động vào góc bắn để tạo hiệu ứng bắn không quá đều đặn
            launchVelocity += Random.Range(-weaponData.launchVelocity * 0.1f, weaponData.launchVelocity * 0.1f); // Thêm một chút biến động vào vận tốc bắn để tạo hiệu ứng bắn không quá đều đặn

            Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * launchVelocity, Color.red, 0.1f);
        }
    }

    private float calculateRotationAngle()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldMousePos = mainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = anchorPoint.z;

        Vector3 direction = worldMousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (weaponData.leftAngle <= angle && angle <= weaponData.rightAngle)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            return angle;
        }

        return transform.rotation.eulerAngles.z;
    }

    private float calculateLaunchVelocity()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        Vector3 worldMousePos = mainCam.ScreenToWorldPoint(mousePos);
        worldMousePos.z = anchorPoint.z;
        float distance = Vector3.Distance(worldMousePos, anchorPoint);
        return Mathf.Clamp01(distance / maxDrag);
    }

    private void Shoot()
    {
        if (spawner != null && Time.time - lastLaunchTime >= weaponData.launchCooldown)
        {
            lastLaunchTime = Time.time;
            Projectile projectile = spawner.projectilePool.Get();
            if (projectile.rb != null)
            {
                Vector2 launchDirection = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));
                projectile.rb.linearVelocity = launchDirection * launchVelocity;
            }
        }
    }
}