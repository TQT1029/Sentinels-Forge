using UnityEngine;
using System.Collections;

public class BurstFireBehavior : IFireBehavior
{
    public void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        // Vì hành vi này cần thời gian trễ (Interval), ta mượn Coroutine của WeaponControl
        weapon.StartCoroutine(FireBurstRoutine(weapon, aimDirection, chargePower));
    }

    private IEnumerator FireBurstRoutine(WeaponControl weapon, Vector2 aimDirection, float chargePower)
    {
        WeaponData data = weapon.weaponData;

        for (int i = 0; i < data.burstCount; i++)
        {
            float angleOffset = Random.Range(-data.angleVibration, data.angleVibration);
            Vector2 finalDirection = Quaternion.Euler(0, 0, angleOffset) * aimDirection;

            float finalVelocity = data.fireVelocity * chargePower;
            finalVelocity += data.GetVelocityVibration(finalVelocity);

            weapon.SpawnAndFireProjectile(finalDirection, finalVelocity);

            yield return new WaitForSeconds(data.burstInterval);
        }
    }
}