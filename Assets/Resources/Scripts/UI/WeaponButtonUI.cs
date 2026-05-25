using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public class WeaponButtonUI : MonoBehaviour
{
    private WeaponData myWeapon;
    private List<ProjectileData> myProjectiles;

    private QuickSwapUI quickSwapManager;
    private RectTransform myRect;

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    public void Setup(WeaponData weaponData, List<ProjectileData> projectiles, QuickSwapUI manager)
    {
        myWeapon = weaponData;
        myProjectiles = projectiles;
        quickSwapManager = manager;

        // TODO: Gán sprite icon cho súng
        // imageIcon.sprite = myWeapon.weaponIcon;

        // TODO: Cập nhật text hiển thị (số lượng đạn còn lại, tên súng...)
        // weaponNameText.text = myWeapon.weaponName;
    }

    private void HandleClick()
    {
        // Truyền kèm danh sách Đạn sang Manager để nó vẽ Panel Xanh
        quickSwapManager.OnWeaponSelected(myWeapon, myProjectiles, myRect);
    }
}