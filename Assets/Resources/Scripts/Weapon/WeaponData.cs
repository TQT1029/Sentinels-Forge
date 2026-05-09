using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Bow,
    Cannon,
    Laser
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon/Weapon Stats")]
public class WeaponData : ScriptableObject
{
    public WeaponType weaponType= WeaponType.Bow;
    public List<ProjectileData> listOfAvailableProjectiles = new List<ProjectileData>();

    [Space(10)]
    public float rangeRadius = 10; // Khoảng cách khi đạn ra khỏi bán kính này sẽ tự biến mất
    public float launchVelocity = 1;
    [Range(0f,1f)] public float vibrationVelocityStrength = 0.1f; // Độ mạnh của biến động vận tốc
    public float attackCooldown = 0.2f; 

    [Space(10)]
    public float leftAngle = -45; // Góc trái của vùng tấn công
    public float rightAngle = 45; // Góc phải của vùng tấn công
    [Range(0f,45f)] public float angleVibration = 5; // Độ lệch ngẫu nhiên của góc bắn, giúp tạo hiệu ứng bắn không quá đều đặn

    public float GetVibrationAngle()
    {
        return Random.Range(-angleVibration, angleVibration);
    }

    public float GetVibrationLaunchVelocity()
    {
        return Random.Range(-launchVelocity * vibrationVelocityStrength, launchVelocity * vibrationVelocityStrength);
    }

}
