using UnityEngine;
using UnityEngine.UI; // Hoặc TMPro, tùy bạn dùng UI nào

public class EnemyHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image healthFillImage; // Kéo thả Image thanh máu vào đây
    [SerializeField] private EnemyAI enemyAI;       // Kéo thả GameObject phần Xác vào đây

    private void OnEnable()
    {
        // Khi bật lên, đăng ký lắng nghe sự kiện
        if (enemyAI != null)
        {
            enemyAI.OnHealthChanged += UpdateHealthBar;
        }
    }

    private void OnDisable()
    {
        // QUAN TRỌNG: Phải hủy lắng nghe khi tắt đi (quái chết/vào pool) để tránh tràn RAM
        if (enemyAI != null)
        {
            enemyAI.OnHealthChanged -= UpdateHealthBar;
        }
    }

    // Hàm này CHỈ CHẠY khi quái vật thực sự nhận sát thương
    private void UpdateHealthBar(float percentHealth)
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = percentHealth;
        }
    }
}