using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class PlayerWeaponInput : MonoBehaviour
{
    [Header("Target Weapon")]
    [Tooltip("Vũ khí mà người chơi đang cầm trên tay")]
    public WeaponControl currentWeapon;
    private int activeWeaponIndex = 0; // Tương tác với PlayerLoadoutManager

    [Header("Input Settings")]
    [Tooltip("Khoảng cách rê chuột tối đa để đạt 100% lực bắn")]
    public float maxDragDistance = 10f;
    [SerializeField] private float minimumChargePower = 0.3f; // Vận tốc tối thiểu khi bắn

    [Header("Trajectory Settings")]
    [SerializeField] private int trajectoryStepCount = 40;
    [SerializeField] private float trajectoryTimeStep = 0.05f;

    private Camera mainCam;
    private LineRenderer trajectoryLine;

    // Trạng thái (State)
    private bool isDragging = false;
    private Vector2 startDragPoint;

    private void Awake()
    {
        mainCam = ReferenceManager.Instance.MainCamera;
        if (currentWeapon == null) currentWeapon = gameObject.GetComponentInChildren<WeaponControl>(false);

        trajectoryLine = GetComponent<LineRenderer>();
        trajectoryLine.positionCount = 0;
    }

    private void Start()
    {
        if (currentWeapon == null) return;

        // BẮT BUỘC NẠP DỮ LIỆU TỪ KHI BẮT ĐẦU
        if (PlayerLoadoutManager.Instance != null && PlayerLoadoutManager.Instance.ActiveLoadouts.Count > 0)
        {
            var firstLoadout = PlayerLoadoutManager.Instance.ActiveLoadouts[0];

            // Ép WeaponControl khởi tạo toàn bộ logic dựa trên Data này
            if (firstLoadout.selectedProjectiles.Count > 0)
            {
                currentWeapon.EquipWeapon(firstLoadout.weaponData, firstLoadout.selectedProjectiles[0]);
            }
        }
        else
        {
            Debug.LogWarning("[PlayerWeaponInput] Không có dữ liệu Loadout từ MainMenu!");
        }
    }

    private void OnEnable()
    {
        EquipmentEvents.OnEquipmentChanged += HandleEquipmentSwitching;
    }

    private void OnDisable()
    {
        EquipmentEvents.OnEquipmentChanged -= HandleEquipmentSwitching;
    }

    private void Update()
    {
        if (currentWeapon == null || currentWeapon.weaponData == null) return;

        HandleUtilities();
        HandleWeaponSwitching();
        HandleProjectileSwitching();
        HandleAimingAndFiring();
    }

    private void HandleUtilities()
    {
        if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.B)) InventoryManager.Instance.ShowInventory();

        if (Input.GetKeyDown(KeyCode.C)) CraftingManager.Instance.CraftItem("Piercing Modifier");
    }

    private void HandleEquipmentSwitching(WeaponData weaponData, ProjectileData projectileData)
    {
        if (currentWeapon != null)
        {
            // Cập nhật súng và đạn mới vào WeaponControl
            currentWeapon.EquipWeapon(weaponData, projectileData);
        }
    }

    private void HandleWeaponSwitching()
    {
        // Khuyến nghị: Trong tương lai nên gọi hàm đổi súng thông qua UI thay vì phím cứng
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (activeWeaponIndex)
            {
                case 0: activeWeaponIndex = 1; break;
                case 1: activeWeaponIndex = 0; break;
            }

            currentWeapon.EquipWeapon(PlayerLoadoutManager.Instance.ActiveLoadouts[activeWeaponIndex].weaponData, PlayerLoadoutManager.Instance.ActiveLoadouts[activeWeaponIndex].selectedProjectiles[0]);
        }
    }

    /// <summary>
    /// Xử lý bấm phím 1,2,3,4 để đổi đạn
    /// </summary>
    private void HandleProjectileSwitching()
    {
        // Khuyến nghị: Trong tương lai nên gọi hàm đổi đạn thông qua UI thay vì phím cứng
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) currentWeapon.ChangeProjectile(1);
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) currentWeapon.ChangeProjectile(2);
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) currentWeapon.ChangeProjectile(3);
    }

    /// <summary>
    /// Xử lý Kéo thả chuột (Drag & Drop) và vẽ quỹ đạo
    /// </summary>
    private void HandleAimingAndFiring()
    {
        // Ngăn không cho người chơi bắn đạn khi họ đang click vào UI (Nút bấm, Menu...)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Bắt đầu kéo
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            startDragPoint = GetMouseWorldPosition();
        }

        // Đang kéo (GIỮ CHUỘT)
        if (isDragging)
        {
            Vector2 currentMousePos = GetMouseWorldPosition();
            Vector2 weaponPos = currentWeapon.firePoint != null ? currentWeapon.firePoint.transform.position : currentWeapon.transform.position;

            // Tính hướng ngắm (Từ nòng súng chỉa thẳng ra con trỏ chuột)
            Vector2 aimDirection = (currentMousePos - weaponPos).normalized;

            // Giới hạn hướng ngắm(Ép nó nằm trong góc cho phép)
            aimDirection = ClampDirect(aimDirection);

            // Tính lực bắn dựa trên quãng đường kéo chuột
            float dragDistance = Vector2.Distance(startDragPoint, currentMousePos);
            float chargePower = Mathf.Max(minimumChargePower, Mathf.Clamp01(dragDistance / maxDragDistance)); // Lực tối thiểu

            // Xoay súng theo hướng chuột
            RotateWeaponToMouse(aimDirection);

            // Vẽ quỹ đạo dự kiến
            DrawTrajectory(aimDirection, chargePower, weaponPos);

            // TÁCH LOGIC BÓP CÒ TÙY THEO LOẠI SÚNG
            WeaponType type = currentWeapon.weaponData.weaponType;

            // Xả đạn Auto/Laser khi đang GIỮ chuột
            if (type == WeaponType.Auto || type == WeaponType.Laser)
            {
                currentWeapon.TryFire(aimDirection, chargePower);
            }

            // Xả đạn Single/Burst/Shotgun và tắt line khi THẢ chuột
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                HideTrajectory();

                // Chỉ TryFire khi THẢ chuột đối với các súng không phải Auto/Laser
                if (type != WeaponType.Auto && type != WeaponType.Laser)
                {
                    currentWeapon.TryFire(aimDirection, chargePower);
                }
            }
        }
    }

    private Vector2 ClampDirect(Vector2 rawDirect)
    {
        if (currentWeapon == null || currentWeapon.weaponData == null) return rawDirect;

        WeaponData data = currentWeapon.weaponData;

        // Tính góc hiện tại của chuột (trả về từ -180 đến 180 độ)
        float angle = Mathf.Atan2(rawDirect.y, rawDirect.x) * Mathf.Rad2Deg;

        // Ép góc nằm trong giới hạn của vũ khí
        float clampedAngle = Mathf.Clamp(angle, data.leftAngle, data.rightAngle);

        // Đổi ngược từ góc (độ) về lại Vector2 chỉ hướng
        float radian = clampedAngle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    private void RotateWeaponToMouse(Vector2 aimDirection)
    {
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        currentWeapon.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void DrawTrajectory(Vector2 aimDirection, float chargePower, Vector2 startPos)
    {
        trajectoryLine.positionCount = trajectoryStepCount;

        // Tính lực bắn thực tế
        float actualFireVelocity = currentWeapon.weaponData.fireVelocity * chargePower;
        Vector2 startVelocity = aimDirection * actualFireVelocity;

        // Lấy trọng lực của viên đạn đang được chọn (để vẽ đường cong cho chuẩn)
        float gravity = Physics2D.gravity.y * currentWeapon.CurrentProjectileData.gravityScale;

        for (int i = 0; i < trajectoryStepCount; i++)
        {
            float t = i * trajectoryTimeStep;

            // Phương trình chuyển động ném xiên: S = V0*t + 0.5*a*t^2
            Vector2 pointPos = startPos + (startVelocity * t);
            pointPos.y += 0.5f * gravity * t * t;

            trajectoryLine.SetPosition(i, pointPos);
        }
    }

    private void HideTrajectory()
    {
        trajectoryLine.positionCount = 0;
    }

    private Vector2 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // Đẩy ra xa camera để tránh lỗi tọa độ Z
        return mainCam.ScreenToWorldPoint(mousePos);
    }

}