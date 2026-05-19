using UnityEngine;
using UnityEngine.Pool;

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
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float popForce = Random.Range(3f, 6f);
        rb.AddForce(randomDir * popForce, ForceMode2D.Impulse);

        Debug.Log($"[WorldItem] {gameObject.name} initialized with {slot.amount} {slot.itemData.name}");
    }

    // Cơ chế Nhặt bằng va chạm (Player đi qua)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem ai chạm vào (Căn cứ, Player hoặc Vùng thu thập)
        if (collision.collider.gameObject.layer == GameConstants.INDEX_BORDER_LAYER)
        {
            Pickup();
        }
    }

    // Hàm public để có thể gọi từ bên ngoài (VD: Dùng chuột click để nhặt, hoặc tự hút về căn cứ)
    public void Pickup()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(CurrentSlot.itemData, CurrentSlot.amount);
            // TODO: Play âm thanh "Ting!" nhặt đồ ở đây
        }

        // Trả về Pool thay vì Destroy
        if (gameObject.activeInHierarchy)
        {
            managedPool.Release(this);
        }
    }
}