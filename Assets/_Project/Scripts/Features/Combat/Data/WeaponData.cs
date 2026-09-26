using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WeaponType
{
    Single,     // Cung, Sniper (Bắn 1 viên/lần)
    Burst,      // Súng trường (Bắn 3 viên liên tục rồi nghỉ)
    Auto,       // Súng máy (Đè chuột là xả đạn liên tục)
    Shotgun,    // Đại bác chùm (Bắn 1 lần ra X viên theo hình nón)
    Laser       // Bắn tia Beam thẳng (Raycast liên tục)
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/Weapon/Weapon Stats")]
public class WeaponData : ScriptableObject
{
    [Header("Core Settings")]
    public WeaponType weaponType = WeaponType.Single;
    public List<ProjectileData> listOfAvailableProjectiles = new List<ProjectileData>();

    public int leftAngle = -45;
    public int rightAngle = 45;

    public float fireVelocity = 15f;
    public float fireCooldown = 0.5f; // Thời gian nghỉ giữa các lần bóp cò

    [Header("Inaccuracy / Spread")]
    [Range(0f, 45f)] public float angleVibration = 5f; // Độ lệch góc
    [Range(0f, 1f)] public float velocityVibration = 0.1f; // Độ lệch lực bắn

    [Header("Shotgun / Multi-shot Settings")]
    public int projectilesPerShot = 1; // Số đạn bắn ra cùng lúc (Shotgun = 5)
    public float spreadAngle = 30f; // Góc tỏa của Shotgun

    [Header("Burst Settings")]
    public int burstCount = 3; // Số viên trong 1 loạt đạn
    public float burstInterval = 0.1f; // Độ trễ giữa các viên trong cùng 1 loạt

    public float GetAngleVibration()
    {
        return Random.Range(-angleVibration, angleVibration);
    }

    public float GetVelocityVibration( float baseVelocity)
    {
        return Random.Range(-baseVelocity * velocityVibration, baseVelocity * velocityVibration);
    }

}
