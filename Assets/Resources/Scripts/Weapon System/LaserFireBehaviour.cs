using UnityEngine;

public class LaserFireBehavior : IFireBehavior
{
    public void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        WeaponData data = weapon.weaponData;

        // Laser có thể rung lắc nhẹ tạo cảm giác giật điện
        float angleOffset = data.GetAngleVibration();
        Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

        // Tính góc xoay Z để quay prefab tia Laser đúng hướng
        float angle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg;

        if (weapon.projectileSpawner != null)
        {
            // Vẫn lấy đạn từ Pool để hưởng lợi ích về mặt hiệu suất và Modifier
            Projectile projectile = weapon.projectileSpawner.CurrentPool.Get();

            projectile.transform.position = weapon.firePoint != null ? weapon.firePoint.transform.position : weapon.transform.position;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (projectile.rb != null)
            {
                // Ép chết vật lý: Tia laser không bay, không bị rơi
                projectile.rb.linearVelocity = Vector2.zero;
                projectile.rb.angularVelocity = 0f;
                projectile.rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // Gọi xử lý tức thì. 
            // NOTE: Bạn cần tạo 1 class "ProjectileLaser : Projectile", 
            // ghi đè (override) hàm ProcessImediate() để nó tự thực hiện Physics2D.Raycast và tính sát thương.
            projectile.ProcessImediate();
        }
    }
}