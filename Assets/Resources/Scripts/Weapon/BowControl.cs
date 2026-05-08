using UnityEngine;

public class BowControl : WeaponControl
{
    protected override void VibrateWeapon()
    {
        currentAngle = CalculateRotationAngle();
        launchVelocity = CalculateLaunchVelocity() * weaponData.launchVelocity;

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * launchVelocity, Color.blueViolet, 0.1f);

        currentAngle += +Random.Range(-weaponData.angleVibration, weaponData.angleVibration); // Thêm một chút biến động vào góc bắn để tạo hiệu ứng bắn không quá đều đặn
        launchVelocity += Random.Range(-weaponData.launchVelocity * 0.1f, weaponData.launchVelocity * 0.1f); // Thêm một chút biến động vào vận tốc bắn để tạo hiệu ứng bắn không quá đều đặn

        Debug.DrawRay(transform.position, new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0) * launchVelocity, Color.red, 0.1f);
    }
}
