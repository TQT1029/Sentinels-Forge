using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ProjectileButtonUI : MonoBehaviour
{
    private ProjectileData myProjectile;
    private QuickSwapUI uiManager;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(HandleClick);
    }

    public void Setup(ProjectileData projectileData, QuickSwapUI manager)
    {
        myProjectile = projectileData;
        uiManager = manager;

        // TODO: Gán sprite icon cho đạn
        // imageIcon.sprite = myProjectile.projectileIcon;

        // TODO: Nếu đạn có số lượng, cập nhật Text hiển thị ở đây
    }

    private void HandleClick()
    {
        uiManager.OnProjectileSelected(myProjectile);
    }
}