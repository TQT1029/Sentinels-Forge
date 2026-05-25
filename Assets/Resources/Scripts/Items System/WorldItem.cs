using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Pool;

public static class RewardEvents
{
    // Bắn ra khi nhặt được 1 item trên map
    public static Action<BaseItemSO, int> OnRewardCollected;

    // Bắn ra khi Boss chết hoặc hết giờ (kích hoạt mở bảng Result)
    public static Action OnLevelEnding;
}
[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D), typeof(Rigidbody2D))]
public class WorldItem : MonoBehaviour
{
    public InventorySlot CurrentSlot { get; private set; }

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private IObjectPool<WorldItem> managedPool;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(IObjectPool<WorldItem> pool) => managedPool = pool;
    public void Init(InventorySlot slot)
    {
        CurrentSlot = slot;
        sr.sprite = slot.itemData.icon;

        // Hiệu ứng văng ngẫu nhiên (Juice)
        Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
        float popForce = UnityEngine.Random.Range(2f, 4f);
        rb.AddForce(randomDir * popForce, ForceMode2D.Impulse);
    }

    // Cơ chế tự nhặt
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra va chạm
        if (collision.collider.gameObject.layer == GameConstants.INDEX_BORDER_LAYER && collision.collider.CompareTag(GameConstants.GROUND_TAG))
        {
            Invoke(nameof(Pickup), 1f);
        }
    }

    // Hàm public để có thể gọi từ bên ngoài (VD: Dùng chuột click để nhặt, hoặc tự hút về căn cứ)
    public void Pickup()
    {
        if (InventoryManager.Instance != null)
        {
            if (CurrentSlot.itemData != null)
            {
                RewardEvents.OnRewardCollected?.Invoke(CurrentSlot.itemData, CurrentSlot.amount);
            }

            OnPickup();
        }


    }

    // TODO: Play âm thanh "Ting!" nhặt đồ và animation biến mất
    private void OnPickup()
    {
        // Hiệu ứng 1: Nảy nhẹ lên trên 0.5 đơn vị
        transform.DOMoveY(transform.position.y + 0.5f, 0.3f)
            .SetEase(Ease.OutQuad);

        // Hiệu ứng 2: Thu nhỏ về 0, khi hoàn thành (OnComplete) thì thu hồi object
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one; // Reset scale để lần sau dùng lại không bị lỗi
                // Trả về Pool thay vì Destroy
                ReturnToPool();
            });
    }

    private void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            managedPool.Release(this);
        }
    }
}