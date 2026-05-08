using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class BowControl : WeaponControl
{
    private float angleAfterVibration; // Góc bắn cơ bản trước khi thêm biến động
    private float launchVelocityAfterVibration; // Vận tốc bắn cơ bản trước khi thêm biến động

    private LineRenderer lineRenderer;

    [Header("Trajectory Settings")]
    [SerializeField] private int trajectoryStepCount = 30; // Số điểm gãy để tạo thành đường cong
    [SerializeField] private float trajectoryTimeStep = 0.05f; // Khoảng cách thời gian giữa các điểm (càng nhỏ càng mịn nhưng ngắn)
    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }
    protected override void VibrateWeapon()
    {
        currentBaseAngle = CalculateRotationAngle();
        launchBaseVelocity = CalculateLaunchVelocity() * weaponData.launchVelocity;

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentBaseAngle * Mathf.Deg2Rad), Mathf.Sin(currentBaseAngle * Mathf.Deg2Rad), 0) * launchBaseVelocity, Color.blueViolet, 0.1f);

        angleAfterVibration = currentBaseAngle + Random.Range(-weaponData.angleVibration, weaponData.angleVibration); // Thêm một chút biến động vào góc bắn để tạo hiệu ứng bắn không quá đều đặn
        launchVelocityAfterVibration = launchBaseVelocity + Random.Range(-weaponData.launchVelocity * 0.1f, weaponData.launchVelocity * 0.1f); // Thêm một chút biến động vào vận tốc bắn để tạo hiệu ứng bắn không quá đều đặn

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(angleAfterVibration * Mathf.Deg2Rad), Mathf.Sin(angleAfterVibration * Mathf.Deg2Rad), 0) * launchVelocityAfterVibration, Color.red, 0.1f);
    }

    protected override void Shoot()
    {
        // Kiểm tra Cooldown từ WeaponData để tránh xả đạn liên tục
        if (projectileSpawner != null && Time.time - lastLaunchTime >= weaponData.attackCooldown)
        {
            lastLaunchTime = Time.time;

            Projectile projectile = projectileSpawner.projectilePool.Get();
            if (projectile.rb != null)
            {
                Vector2 launchDirection = new Vector2(Mathf.Cos(angleAfterVibration * Mathf.Deg2Rad), Mathf.Sin(angleAfterVibration * Mathf.Deg2Rad));
                projectile.rb.linearVelocity = launchDirection * launchVelocityAfterVibration;
            }
        }
    }

    // Bắt buộc override từ class cha
    protected override void DrawTrajectory()
    {
        lineRenderer.positionCount = trajectoryStepCount;

        // Vị trí bắt đầu tính quỹ đạo
        Vector2 startPos = launcherPoint != null ? launcherPoint.transform.position : transform.position;

        // Vận tốc ban đầu dạng Vector2
        Vector2 startVelocity = new Vector2(Mathf.Cos(currentBaseAngle * Mathf.Deg2Rad), Mathf.Sin(currentBaseAngle * Mathf.Deg2Rad)) * launchBaseVelocity;

        // Lấy hệ số trọng lực. Nếu đạn có gravityScale khác 1, nhân thêm vào đây (VD: lấy từ projectileData.weight)
        float gravity = Physics2D.gravity.y;

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
