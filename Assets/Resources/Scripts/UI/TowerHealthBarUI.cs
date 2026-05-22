using UnityEngine;
using UnityEngine.UI; // Hoặc TMPro, tùy bạn dùng UI nào

public class TowerHealthBarUI : MonoBehaviour
{
    [SerializeField] private TowerController towerController; // Kéo thả GameObject phần Tháp vào đây

    private void OnEnable()
    {
        // Khi bật lên, đăng ký lắng nghe sự kiện
        if (towerController != null)
        {
            towerController.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        // QUAN TRỌNG: Phải hủy lắng nghe khi tắt đi (quái chết/vào pool) để tránh tràn RAM
        if (towerController != null)
        {
            towerController.OnHealthChanged -= UpdateHealthBar;
        }
    }

    // Hàm này CHỈ CHẠY khi quái vật thực sự nhận sát thương
    private void UpdateHealthBar(float percentHealth)
    {
        if (UIManager.Instance != null && UIManager.Instance.globalTowerHealthBar != null)
        {
            UIManager.Instance.globalTowerHealthBar.fillAmount = percentHealth;
        }
    }
}