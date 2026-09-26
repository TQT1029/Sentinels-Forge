using System.Collections;
using UnityEngine;

public interface IFireBehavior
{
    // Được gọi khi bóp cò (Hoặc thả chuột nếu dùng cơ chế Drag)
    void ExecuteFire(WeaponControl weapon, Vector2 aimDirection, float chargePower);
}