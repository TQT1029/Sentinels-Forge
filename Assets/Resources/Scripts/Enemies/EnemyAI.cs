using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class EnemyAI : MonoBehaviour
{
    [Header("Enemy Data")]
    [field: SerializeField] public EnemyData enemyData { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Collider2D solidBody { get; private set; }
    public Collider2D triggerHitbox { get; private set; }

    protected Transform towerTransform;
    private Bounds towerBounds;
    protected TowerController towerController;

    [SerializeField, Tooltip("Vị trí lệch so với tâm của tower (%)")] protected Vector3 towerTargetOffset;
    protected Vector3 actualTargetPosition;

    private IObjectPool<EnemyAI> managedPool;


    public Dictionary<EffectData, RuntimeEffect> activeEffects = new Dictionary<EffectData, RuntimeEffect>();
    private List<EffectData> effectsToRemove = new List<EffectData>(); // Cache để tránh lỗi xóa trong vòng lặp
    [Header("Loot System")]
    [SerializeField] private LootTableSO lootTable;// Bảng xác suất rớt đồ, có thể gán riêng cho từng loại quái trong Inspector

    [Header("Utilities")]
    protected float currentHealth;
    public float PercentHealth => currentHealth / (enemyData.maxHealth * healthMultiplier);

    protected float checkingFrequency = 10f; // Số lần check trong 1 giây
    protected float checkingTime = 0f;

    protected float previousAttackTime = 0f;

    protected bool isInvincible = false;
    public bool IsDead => currentHealth <= 0;

    [HideInInspector] public bool isStunned = false;
    [HideInInspector] public float speedMultiplier = 1f; // Dùng cho hiệu ứng Slow (Làm chậm)
    [HideInInspector] public float damageMultiplier = 1f; // Dùng cho cơ chế tăng damage theo wave hoặc buff
    [HideInInspector] public float healthMultiplier = 1f; // Dùng cho cơ chế tăng damage theo wave hoặc buff


    protected virtual void Awake()
    {
        towerTransform = WaveManager.Instance.TowerTransform;
        towerBounds = WaveManager.Instance.TowerBounds;
        towerController = WaveManager.Instance.TowerController;

        rb = GetComponent<Rigidbody2D>();
        solidBody = GetComponent<Collider2D>();
        triggerHitbox = transform.Find("SoulEnemy")?.GetComponent<Collider2D>();

        ResetStats();

        // Tọa độ tâm thực tế của hình ảnh/collider
        Vector3 center = towerBounds.center;

        // Kích thước chiều rộng và chiều cao
        Vector3 size = towerBounds.size;

        // Tính % dựa trên Kích thước (Size)
        actualTargetPosition.x = center.x + (size.x * towerTargetOffset.x);
        actualTargetPosition.y = center.y + (size.y * towerTargetOffset.y);
        actualTargetPosition.z = 0f;
    }

    public void SetPool(IObjectPool<EnemyAI> pool)
    {
        managedPool = pool;
    }

    //Khi lấy quái ra từ pool thì tự đăng kí vào EnemyPhysicsRegistry để các hệ thống khác có thể dễ dàng dò tìm
    protected virtual void OnEnable()
    {
        EnemyPhysicsRegistry.Register(triggerHitbox, this, solidBody);
    }

    //Khi quái chết hoặc vào Pool thì tự hủy đăng kí khỏi EnemyPhysicsRegistry để tránh rò rỉ bộ nhớ và lỗi dò tìm
    protected virtual void OnDisable()
    {
        if (triggerHitbox != null)
        {
            EnemyPhysicsRegistry.Unregister(triggerHitbox);
        }
    }

    // Quản lý thêm hiệu ứng
    public void AddEffect(EffectData effectData)
    {
        if (IsDead) return;

        if (activeEffects.ContainsKey(effectData))
        {
            // Giao quyền định đoạt logic stack cho chính RuntimeEffect
            activeEffects[effectData].OnStack();
        }
        else
        {
            // Chưa có thì tạo mới và áp dụng
            RuntimeEffect newEffect = effectData.CreateRuntimeEffect(this);
            activeEffects.Add(effectData, newEffect);
            newEffect.OnApply();
        }
    }
    // Tiện ích để các effect dò tìm nhau trên cùng 1 target
    public RuntimeEffect GetEffect(EffectData effectData)
    {
        if (activeEffects.TryGetValue(effectData, out RuntimeEffect effect))
            return effect;
        return null;
    }
    public virtual void ResetStats()
    {
        currentHealth = enemyData.maxHealth * healthMultiplier;
        isInvincible = false;
        checkingTime = 0f;

        // Bắt buộc Reset tất cả trạng thái trước khi lấy khỏi Pool
        isStunned = false;
        speedMultiplier = 1f;
        damageMultiplier = WaveManager.Instance.WaveMultiplier;
        healthMultiplier = WaveManager.Instance.WaveMultiplier;


        // Xóa sạch hiệu ứng cũ (Không gọi OnRemove để tránh logic chạy đè)
        activeEffects.Clear();
    }
    protected virtual void Update()
    {
        HandleEffects(); // Chạy bộ đếm thời gian của hiệu ứng

        if (isStunned) return; // Đang choáng thì bỏ qua toàn bộ Update bên dưới (Không đi, không đánh)

        checkingTime += Time.deltaTime;

        ProcessAI();
    }
    protected abstract void ProcessAI();

    private void HandleEffects()
    {
        if (activeEffects.Count == 0) return;

        effectsToRemove.Clear();

        foreach (var kvp in activeEffects)
        {
            RuntimeEffect effect = kvp.Value;

            effect.OnTick(Time.deltaTime); // Chạy logic tick (VD: Độc trừ máu)
            if (IsDead) break;
            effect.TimeRemaining -= Time.deltaTime; // Giảm thời gian

            if (effect.IsFinished)
            {
                effect.OnRemove(); // Gọi logic kết thúc (VD: Mở khóa Stun)
                effectsToRemove.Add(kvp.Key);
            }
        }
        // Nếu quái chết, danh sách đã được dọn sạch trong hàm Die(), ta thoát luôn
        if (IsDead) return;
        // Dọn dẹp các hiệu ứng đã hết hạn
        foreach (var key in effectsToRemove)
        {
            activeEffects.Remove(key);
        }
    }

    public virtual void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, enemyData.maxHealth);
        Debug.Log($"[EnemyAI] {gameObject.name} healed {amount} HP! Current HP: {currentHealth}");
    }

    /// <summary>
    /// Hàm chính để gọi từ bên ngoài
    /// </summary>
    /// <param name="amount">Số lượng sát thương nhận vào</param>
    /// <param name="invincibilityTime">Thời gian bất tử sau khi bị hit</param>
    public virtual void TakeDamage(float amount, float invincibilityTime = 0f)
    {
        if (isInvincible || currentHealth <= 0)
            return;

        currentHealth -= amount;

        OnHit();
        //Debug.Log($"[EnemyAI] {gameObject.name} took {amount} damage! Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (invincibilityTime > 0)
        {
            StartCoroutine(InvincibilityRoutine(invincibilityTime));
        }

    }

    /// <summary>
    /// Coroutine xử lý thời gian vô địch
    /// </summary>
    protected virtual IEnumerator InvincibilityRoutine(float time)
    {
        isInvincible = true;

        // Bạn có thể chèn logic nhấp nháy SpriteRenderer ở đây (sẽ hướng dẫn bên dưới nếu bạn cần)

        // Chờ hết thời gian
        yield return new WaitForSeconds(time);

        isInvincible = false;
    }

    /// <summary>
    /// Các class con có thể override hàm này để thêm âm thanh / hiệu ứng riêng khi bị đánh trúng
    /// </summary>
    protected virtual void OnHit()
    {
        //Debug.Log($"[EnemyAI] {gameObject.name} took damage! Remaining HP: {currentHealth}");
    }

    /// <summary>
    /// Hàm xử lý cái chết (Override để rớt đồ, cộng điểm, hoặc trả về Object Pool)
    /// </summary>
    protected virtual void Die()
    {
        foreach (var effect in activeEffects.Values)
        {
            effect.OnRemove();
        }
        activeEffects.Clear();

        //Debug.Log($"[EnemyAI] {gameObject.name} has died.");

        WaveManager.Instance.EnemyKilled();

        DropLoot();

        // Xử lý thu hồi về Pool
        if (gameObject.activeInHierarchy && managedPool != null)
        {
            managedPool.Release(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void DropLoot()
    {
        if (lootTable == null || ItemDropManager.Instance == null) return;

        List<InventorySlot> drops = lootTable.GetRandomDrops();
        foreach (var slot in drops)
        {
            // Yêu cầu Manager sinh ra item vật lý tại vị trí quái chết
           ItemDropManager.Instance.SpawnWorldItem(transform.position, slot);
        }
    }
}