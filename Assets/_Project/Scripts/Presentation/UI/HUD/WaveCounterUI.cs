using UnityEngine;
using TMPro; // Nếu bạn dùng TextMeshPro
using DG.Tweening; // Import DOTween

[RequireComponent(typeof(CanvasGroup))]
public class WaveCounterUI : MonoBehaviour
{
    private TextMeshProUGUI waveText;
    private CanvasGroup canvasGroup;

    [Header("DOTween Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float displayDuration = 3.0f;
    [SerializeField] private Vector3 punchScaleAmount = new Vector3(0.2f, 0.2f, 0f);

    private Tween fadeTween;

    private void Awake()
    {
        waveText = GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Ban đầu ẩn chữ đi bằng cách đặt Alpha của CanvasGroup về 0
        canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        // Giả sử WaveManager của bạn bắn sự kiện kèm số Wave hiện tại khi bắt đầu Wave mới
        if (WaveManager.Instance != null)
        {
             WaveManager.OnWaveStarted += ShowWaveNotification;
        }
    }

    private void OnDisable()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.OnWaveStarted -= ShowWaveNotification;
        }

        // SENIOR TIP: Luôn Kill Tween khi Object bị tắt để tránh lỗi rò rỉ bộ nhớ (Memory Leak)
        fadeTween?.Kill();
    }

    // Hàm public này có thể gọi từ Sự kiện của WaveManager, hoặc thông qua UIManager nếu muốn
    public void ShowWaveNotification(int waveNumber)
    {
        waveText.text = $"WAVE - {waveNumber}";

        // 1. AN TOÀN: Kill cái tween cũ nếu người chơi qua màn quá nhanh, tránh hiệu ứng đè nhau
        fadeTween?.Kill();
        transform.DOKill(); // Xóa các tween liên quan đến scale/position nếu có

        // Reset lại trạng thái ban đầu
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one;

        // 2. TẠO CHUỖI HIỆU ỨNG (Sequence)
        Sequence seq = DOTween.Sequence();

        // Hiệu ứng 1: Hiện chữ lên (Fade In)
        seq.Append(canvasGroup.DOFade(1f, fadeDuration));

        // Hiệu ứng phụ họa (Juice): Giật nhẹ kích thước khi vừa xuất hiện để tạo điểm nhấn
        seq.Join(transform.DOPunchScale(punchScaleAmount, fadeDuration, 5, 0.5f));

        // Hiệu ứng 2: Đứng im chờ trong X giây (Display Duration)
        seq.AppendInterval(displayDuration);

        // Hiệu ứng 3: Mờ dần rồi biến mất (Fade Out)
        seq.Append(canvasGroup.DOFade(0f, fadeDuration));

        // Lưu lại tham chiếu để quản lý
        fadeTween = seq;
    }
}