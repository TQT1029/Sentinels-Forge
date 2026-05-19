using UnityEngine;
using UnityEngine.Pool;

public class ItemDropManager : Singleton<ItemDropManager>
{
    [SerializeField] private WorldItem worldItemPrefab;

    private IObjectPool<WorldItem> itemPool;

    protected override void Awake()
    {
        base.Awake();

        itemPool = new ObjectPool<WorldItem>(
            createFunc: () =>
            {
                WorldItem item = Instantiate(worldItemPrefab, transform);
                item.SetPool(itemPool);
                return item;
            },
            actionOnGet: (item) => item.gameObject.SetActive(true),
            actionOnRelease: (item) => item.gameObject.SetActive(false),
            actionOnDestroy: (item) => Destroy(item.gameObject),
            defaultCapacity: 50,
            maxSize: 200
        );
    }

    public void SpawnWorldItem(Vector2 position, InventorySlot slot)
    {
        WorldItem item = itemPool.Get();
        item.transform.position = position;
        item.Init(slot);
    }
}