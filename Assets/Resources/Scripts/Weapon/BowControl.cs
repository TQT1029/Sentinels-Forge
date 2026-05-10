using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class BowControl : WeaponControl
{
    private float angleAfterVibration;
    private float launchVelocityAfterVibration;

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

}
