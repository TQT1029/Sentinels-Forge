using UnityEngine;

public class AutoFireBehavior : IFireBehavior
{
    public void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        WeaponData data = weapon.weaponData;

        // Tính toán độ giật (Đặc trưng của súng máy là độ lệch góc liên tục)
        float angleOffset = data.GetAngleVibration();
        Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

        float finalVelocity = data.fireVelocity * chargePower;
        finalVelocity += data.GetVelocityVibration(finalVelocity);

        weapon.SpawnAndFireProjectile(finalDirection, finalVelocity);
    }
}