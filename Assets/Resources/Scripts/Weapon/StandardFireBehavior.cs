using UnityEngine;

public class StandardFireBehavior : IFireBehavior
{
    public void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        WeaponData data = weapon.weaponData;
        int count = data.projectilesPerShot;

        // Nếu là Single thì vòng lặp chạy 1 lần. Nếu Shotgun thì chạy X lần.
        for (int i = 0; i < count; i++)
        {
            float angleOffset = 0f;

            if (count > 1) // Tính góc xòe cho Shotgun
            {
                angleOffset = Mathf.Lerp(-data.spreadAngle / 2f, data.spreadAngle / 2f, (float)i / (count - 1));
            }

            // Cộng thêm độ lệch ngẫu nhiên (Vibration)
            angleOffset += data.GetAngleVibration();

            // Xoay hướng bắn
            Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

            // Tính toán lực bắn (Có rung lắc nhẹ)
            float finalVelocity = data.fireVelocity * chargePower;
            finalVelocity += data.GetVelocityVibration(finalVelocity);

            // Bắn đạn
            weapon.SpawnAndFireProjectile(finalDirection, finalVelocity);
        }
    }
}