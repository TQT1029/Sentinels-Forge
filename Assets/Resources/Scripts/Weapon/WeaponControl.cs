using UnityEngine;

public abstract class WeaponControl : MonoBehaviour
{
    protected Camera mainCam;
    protected Vector3 anchorPoint;
    [SerializeField] protected float maxDrag = 10f;
    protected float distanceDrag = 0;

    [SerializeField, Min(0)] protected float minLaunchVelocity = 0.3f; // Vận tốc tối thiểu khi bắn, đảm bảo đạn không bị quá yếu

    protected Vector3 startDragPoint;

    public GameObject launcherPoint;
    [SerializeField] protected ProjectileSpawner projectileSpawner;
    public WeaponData weaponData;

    protected bool isDragging = false;
    protected float currentBaseAngle = 0;// Góc bắn cơ bản chưa có biến động
    protected float launchBaseVelocity = 0;// Vận tốc bắn cơ bản chưa có biến động  
    protected float lastLaunchTime = 0;

    protected virtual void Awake()
    {
        mainCam = WaveManager.Instance.MainCamera;

        if (launcherPoint == null)
        {
            Transform foundPoint = transform.Find("LauncherPoint");
            if (foundPoint != null) launcherPoint = foundPoint.gameObject;
        }

        if (projectileSpawner == null && launcherPoint != null)
        {
            projectileSpawner = launcherPoint.GetComponent<ProjectileSpawner>();
        }
    }

    protected virtual void Start()
    {
        if (anchorPoint == Vector3.zero)
        {
            anchorPoint = transform.position;
        }
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10f;
            startDragPoint = mainCam.ScreenToWorldPoint(mousePos);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            HideTrajectory();
            Shoot();
        }

        if (isDragging)
        {
            VibrateWeapon();
            DrawTrajectory();
        }
    }

    protected virtual float CalculateRotationAngle()
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

    protected virtual void CalculateDragDistance()
    {
        Vector3 currentMousePos = Input.mousePosition;
        currentMousePos.z = 10f;
        Vector3 worldCurrentPos = mainCam.ScreenToWorldPoint(currentMousePos);
        
        distanceDrag = Vector3.Distance(startDragPoint, worldCurrentPos);
    }

    protected virtual float CalculateLaunchVelocity()
    {
        CalculateDragDistance();

        return Mathf.Max(minLaunchVelocity, Mathf.Clamp01(distanceDrag / maxDrag));
    }

    protected virtual void VibrateWeapon()
    {
        currentBaseAngle = CalculateRotationAngle();
        launchBaseVelocity = CalculateLaunchVelocity() * weaponData.launchVelocity;
        
        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentBaseAngle * Mathf.Deg2Rad), Mathf.Sin(currentBaseAngle * Mathf.Deg2Rad), 0) * launchBaseVelocity, Color.red, 0.1f);
    }

    protected virtual void Shoot()
    {
        // Kiểm tra Cooldown từ WeaponData để tránh xả đạn liên tục
        if (projectileSpawner != null && Time.time - lastLaunchTime >= weaponData.attackCooldown) 
        {
            lastLaunchTime = Time.time;
            
            Projectile projectile = projectileSpawner.projectilePool.Get(); 
            if (projectile.rb != null)
            {
                Vector2 launchDirection = new Vector2(Mathf.Cos(currentBaseAngle * Mathf.Deg2Rad), Mathf.Sin(currentBaseAngle * Mathf.Deg2Rad));
                projectile.rb.linearVelocity = launchDirection * launchBaseVelocity;
            }
        }
    }

    /// <summary>
    /// Hiển thị quỹ đạo bay dự kiến của đạn.
    /// </summary>
    protected abstract void DrawTrajectory();

    /// <summary>
    /// Xoá quỹ đạo bay khi bắn hoặc huỷ ngắm.
    /// </summary>
    protected abstract void HideTrajectory();
}