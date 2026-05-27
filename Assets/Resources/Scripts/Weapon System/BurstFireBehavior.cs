using UnityEngine;
using System.Collections;

public class BurstFireBehavior : IFireBehavior
{
    public void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        // Snapshot spawner và data ngay tại thời điểm bóp cò
        // Coroutine sẽ dùng các giá trị này xuyên suốt burst, bất kể player đổi súng giữa chừng
        ProjectileSpawner capturedSpawner = weapon.projectileSpawner;
        WeaponData capturedData = weapon.weaponData;

        weapon.StartCoroutine(FireBurstRoutine(weapon, capturedSpawner, capturedData, aimDirection, chargePower));
    }

    private IEnumerator FireBurstRoutine(
        WeaponControl weapon,
        ProjectileSpawner spawner,
        WeaponData data,
        Vector2 aimDirection,
        float chargePower)
    {
        for (int i = 0; i < data.burstCount; i++)
        {
            float angleOffset = data.GetAngleVibration();
            Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

            float finalVelocity = data.fireVelocity * chargePower;
            finalVelocity += data.GetVelocityVibration(finalVelocity);

            weapon.SpawnAndFireProjectileFromSpawner(spawner, finalDirection, finalVelocity);

            yield return new WaitForSeconds(data.burstInterval);
        }
    }
}