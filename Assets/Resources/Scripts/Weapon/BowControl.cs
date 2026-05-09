using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class BowControl : WeaponControl
{
    private float angleAfterVibration;
    private float launchVelocityAfterVibration;


    [Header("Trajectory Settings")]
    [SerializeField] private int trajectoryStepCount = 30; // Số điểm gãy để tạo thành đường cong
    [SerializeField] private float trajectoryTimeStep = 0.05f; // Khoảng cách thời gian giữa các điểm (càng nhỏ càng mịn nhưng ngắn)
    private LineRenderer lineRenderer;
    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }
    protected override void VibrateWeapon()
    {
        base.VibrateWeapon();

        angleAfterVibration = currentBaseAngle + weaponData.GetVibrationAngle();
        launchVelocityAfterVibration = launchBaseVelocity + weaponData.GetVibrationLaunchVelocity();

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(angleAfterVibration * Mathf.Deg2Rad), Mathf.Sin(angleAfterVibration * Mathf.Deg2Rad), 0) * launchVelocityAfterVibration, Color.red);
    }

    protected override void Shoot()
    {
        // Kiểm tra Cooldown từ WeaponData để tránh xả đạn liên tục
        if (projectileSpawner != null && Time.time - lastLaunchTime >= weaponData.attackCooldown)
        {
            lastLaunchTime = Time.time;

            Projectile projectile = projectileSpawner.CurrentPool.Get();
            if (projectile.rb != null)
            {
                Vector2 launchDirection = new Vector2(Mathf.Cos(angleAfterVibration * Mathf.Deg2Rad), Mathf.Sin(angleAfterVibration * Mathf.Deg2Rad));
                projectile.rb.linearVelocity = launchDirection * launchVelocityAfterVibration;
            }
        }
    }

    protected override void DrawTrajectory()
    {
        lineRenderer.positionCount = trajectoryStepCount;

        Vector2 startPos = launcherPoint != null ? launcherPoint.transform.position : transform.position;

        // Vận tốc ban đầu dạng Vector2
        Vector2 startVelocity = new Vector2(Mathf.Cos(currentBaseAngle * Mathf.Deg2Rad), Mathf.Sin(currentBaseAngle * Mathf.Deg2Rad)) * launchBaseVelocity;

        float gravity = Physics2D.gravity.y * projectileSpawner.projectileData.gravityScale;

        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float time = i * trajectoryTimeStep;

            // Công thức tính toạ độ theo thời gian t
            Vector2 pointPos = startPos + (startVelocity * time);
            pointPos.y += 0.5f * gravity * time * time;

            lineRenderer.SetPosition(i, pointPos);
        }
    }

    // Bắt buộc override từ class cha
    protected override void HideTrajectory()
    {
        // Reset LineRenderer về 0 điểm để nó biến mất
        lineRenderer.positionCount = 0;
    }
}
