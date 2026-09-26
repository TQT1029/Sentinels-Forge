using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class EquipmentEvents
{
    public static Action<WeaponData, ProjectileData> OnEquipmentChanged;
}

public enum SwapUIState
{
    Collapsed,
    WeaponOpened,
    ProjectileOpened
}

public class QuickSwapUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button changeWeaponBtn;
    [SerializeField] private RectTransform weaponSelectionPanel;
    [SerializeField] private RectTransform projectileSelectionPanel;

    [Header("Prefabs cho Dynamic UI")]
    [SerializeField] private WeaponButtonUI weaponButtonPrefab;
    [SerializeField] private ProjectileButtonUI projectileButtonPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.25f;
    [SerializeField] private float weaponPanelHiddenPosX = -500f;
    [SerializeField] private float projectilePanelHiddenPosX = -700f;

    private SwapUIState currentState = SwapUIState.Collapsed;
    private WeaponData currentSelectedWeapon;
    private RectTransform arrowRect;

    private void Awake()
    {
        arrowRect = changeWeaponBtn.GetComponent<RectTransform>();
    }

    private void Start()
    {
        changeWeaponBtn.onClick.AddListener(OnArrowButtonClicked);

        GenerateLoadoutUI();
        ResetUIImmediate();
    }

    private void GenerateLoadoutUI()
    {
        // TODO: Xóa data giả lập bên dưới. Thay bằng logic lấy từ PlayerLoadoutManager khi ghép code thật.
        List<WeaponLoadout> activeLoadouts = PlayerLoadoutManager.Instance.ActiveLoadouts;

        // Xóa sạch các nút cũ có trên UI (phòng trường hợp reload scene)
        foreach (Transform child in weaponSelectionPanel) Destroy(child.gameObject);

        // TODO: Tạo vòng lặp foreach duyệt qua ActiveLoadouts để instantiate. Bên dưới là code giả lập cấu trúc:

        foreach (var loadout in activeLoadouts)
        {
            WeaponButtonUI newWeaponBtn = Instantiate(weaponButtonPrefab, weaponSelectionPanel);
            newWeaponBtn.Setup(loadout.weaponData, loadout.selectedProjectiles, this);
        }

    }

    private void ResetUIImmediate()
    {
        weaponSelectionPanel.anchoredPosition = new Vector2(weaponPanelHiddenPosX, 0);
        projectileSelectionPanel.anchoredPosition = new Vector2(projectilePanelHiddenPosX, 0);
        currentState = SwapUIState.Collapsed;
    }

    private void OnArrowButtonClicked()
    {
        if (currentState == SwapUIState.Collapsed) OpenWeaponPanel();
        else CloseAllPanels();
    }

    private void OpenWeaponPanel()
    {
        currentState = SwapUIState.WeaponOpened;

        weaponSelectionPanel.DOKill();
        projectileSelectionPanel.DOKill();

        // 1. Tính toán mép phải của changeWeaponBtn ngoài Runtime
        float targetX = arrowRect.anchoredPosition.x + (arrowRect.rect.width * (1f - arrowRect.pivot.x));

        // 2. Trượt Weapon Panel đến đúng mép phải của nút mở
        weaponSelectionPanel.DOAnchorPosX(targetX, slideDuration).SetEase(Ease.OutBack);
        projectileSelectionPanel.DOAnchorPosX(projectilePanelHiddenPosX, slideDuration).SetEase(Ease.InBack);
    }

    public void CloseAllPanels()
    {
        currentState = SwapUIState.Collapsed;

        weaponSelectionPanel.DOKill();
        projectileSelectionPanel.DOKill();

        weaponSelectionPanel.DOAnchorPosX(weaponPanelHiddenPosX, slideDuration).SetEase(Ease.InBack);
        projectileSelectionPanel.DOAnchorPosX(projectilePanelHiddenPosX, slideDuration).SetEase(Ease.InBack);
    }

    public void OnWeaponSelected(WeaponData weaponData, List<ProjectileData> projectiles, RectTransform clickedButtonRect)
    {
        currentSelectedWeapon = weaponData;

        // Xóa danh sách đạn của súng cũ
        foreach (Transform child in projectileSelectionPanel) Destroy(child.gameObject);

        // Instantiate danh sách đạn của súng vừa chọn
        foreach (ProjectileData proj in projectiles)
        {
            ProjectileButtonUI projBtn = Instantiate(projectileButtonPrefab, projectileSelectionPanel);
            projBtn.Setup(proj, this);
        }

        // Ép Layout Group tính toán lại kích thước lập tức trước khi set vị trí Y để tránh lỗi lệch UI trong frame đầu
        LayoutRebuilder.ForceRebuildLayoutImmediate(projectileSelectionPanel);

        // 1. Đồng bộ trục Y của Panel Xanh theo trục Y của nút Weapon vừa nhấn
        float targetY = clickedButtonRect.anchoredPosition.y + 120;

        // 2. Tính toán mép phải của weaponSelectionPanel (Panel Đỏ) làm điểm đích cho Panel Xanh
        // Vì Panel Đỏ đã mở ra hoàn toàn, anchoredPosition.x của nó lúc này đã cố định và chính xác
        float targetX = weaponSelectionPanel.anchoredPosition.x + (weaponSelectionPanel.rect.width * (1f - weaponSelectionPanel.pivot.x));

        // 3. Đặt vị trí Y của Panel Xanh lập tức, còn vị trí X vẫn giữ ở trạng thái ẩn (ẩn dưới nền) trước khi Tween kích hoạt
        projectileSelectionPanel.anchoredPosition = new Vector2(projectilePanelHiddenPosX, targetY);

        // 4. Gọi hàm mở và truyền tọa độ X động vừa tính được vào
        OpenProjectilePanel(targetX);
    }

    private void OpenProjectilePanel(float targetX)
    {
        currentState = SwapUIState.ProjectileOpened;

        projectileSelectionPanel.DOKill();

        // Trượt đến đúng mép phải của Weapon Panel thay vì dùng số 0 cố định
        projectileSelectionPanel.DOAnchorPosX(targetX, slideDuration).SetEase(Ease.OutBack);
    }

    public void OnProjectileSelected(ProjectileData projectileData)
    {
        EquipmentEvents.OnEquipmentChanged?.Invoke(currentSelectedWeapon, projectileData);
        CloseAllPanels();
    }
}