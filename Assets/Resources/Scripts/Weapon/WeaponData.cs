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
    public float rangeRadius = 10; // Khoảng cách khi đạn ra khỏi bán kính này sẽ tự biến mất
    public float launchVelocity = 1;
    public float leftAngle = -45; // Góc trái của vùng tấn công
    public float rightAngle = 45; // Góc phải của vùng tấn công

}
