using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponLoadout
{
    public WeaponData weaponData;
    public List<ProjectileData> selectedProjectiles;
    public WeaponLoadout(WeaponData weapon, List<ProjectileData> projectiles)
    {
        weaponData = weapon;
        selectedProjectiles = projectiles;
    }
}
public class PlayerLoadoutManager : PersistentSingleton<PlayerLoadoutManager>
{
    [field: SerializeField]
    public List<WeaponLoadout> ActiveLoadouts { get; private set; } = new List<WeaponLoadout>(2);

    public void SetLoadout(WeaponData weapon, List<ProjectileData> projectiles)
    {
        if (ActiveLoadouts.Count >= 2)
        {
            Debug.LogWarning("Đã đạt giới hạn loadout. Không thể thêm loadout mới.");
            return;
        }
        ActiveLoadouts.Add(new WeaponLoadout(weapon, projectiles));
    }

    public void ClearLoadouts()
    {
        ActiveLoadouts.Clear();
    }

}
