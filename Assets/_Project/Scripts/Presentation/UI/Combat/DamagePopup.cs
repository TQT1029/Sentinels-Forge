using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Pool;

[RequireComponent(typeof(TextMeshPro))]
public class DamagePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private IObjectPool<DamagePopup> managedPool;

    [Header("Animation Settings")]
    [SerializeField] private float jumpPower = 1.5f; // Lực nảy lên cao
    [SerializeField] private float duration = 0.7f;  // Thời gian hiển thị
    [SerializeField] private float spreadX = 1.2f;   // Độ văng theo trục X
    [SerializeField] private float dropY = 0.5f;     // Độ rớt xuống so với điểm ban đầu

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void SetPool(IObjectPool<DamagePopup> pool) => managedPool = pool;

    public void Setup(float damageAmount, Vector3 spawnPosition, bool isCritical = false)
    {
        transform.position = spawnPosition;
        textMesh.text = $"{damageAmount}";

        // Reset trạng thái (Vì tái sử dụng từ Pool)
        transform.localScale = Vector3.one;
        textMesh.alpha = 1f;

        // Tùy chỉnh màu sắc/kích thước nếu là chí mạng (Critical)
        if (isCritical)
        {
            textMesh.color = Color.yellow;
            transform.localScale = Vector3.one * 1.5f; // To hơn bình thường
        }
        else
        {
            textMesh.color = Color.white;
        }

        AnimatePopup();
    }

    private void AnimatePopup()
    {
        // 1. Dọn dẹp Tween cũ phòng trường hợp lỗi đè Tween
        transform.DOKill();
        textMesh.DOKill();

        // 2. Tính toán điểm rơi ngẫu nhiên
        float randomDirX = Random.Range(-spreadX, spreadX);
        Vector3 targetPosition = transform.position + new Vector3(randomDirX, -dropY, 0);

        Sequence seq = DOTween.Sequence();

        // 3. Hiệu ứng Nảy (DOJump là hàm tuyệt vời nhất của DOTween để làm quỹ đạo parabol)
        seq.Append(transform.DOJump(targetPosition, jumpPower, numJumps: 1, duration));

        // 4. Hiệu ứng Mờ dần (Fade Out) - Bắt đầu mờ đi ở nửa cuối chu kỳ
        seq.Insert(duration * 0.5f, textMesh.DOFade(0f, duration * 0.5f));

        // 5. Thu hồi về Pool khi hoàn thành
        seq.OnComplete(() =>
        {
            if (gameObject.activeInHierarchy)
            {
                managedPool.Release(this);
            }
        });
    }
}