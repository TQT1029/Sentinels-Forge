using UnityEngine;
using UnityEngine.Pool;

public class VFXManager : Singleton<VFXManager>
{
    [Header("Prefabs")]
    [SerializeField] private DamagePopup popupPrefab;
    //[SerializeField] private GameObject hitSparkPrefab;
    // [SerializeField] private GameObject bloodSplatterPrefab;

    private IObjectPool<DamagePopup> popupPool;
    //private IObjectPool<GameObject> sparkPool;

    protected override void Awake()
    {
        base.Awake();

        // Init Popup Pool
        popupPool = new ObjectPool<DamagePopup>(
            createFunc: () =>
            {
                DamagePopup popup = Instantiate(popupPrefab, transform);
                popup.SetPool(popupPool);
                return popup;
            },
            actionOnGet: (popup) => popup.gameObject.SetActive(true),
            actionOnRelease: (popup) => popup.gameObject.SetActive(false),
            actionOnDestroy: (popup) => Destroy(popup.gameObject),
            defaultCapacity: 50,
            maxSize: 300
        );
        // Khởi tạo Object Pool cho Hit Spark

/*        sparkPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                GameObject spark = Instantiate(hitSparkPrefab, transform);
                return spark;
            },
            actionOnGet: (spark) => spark.SetActive(true),
            actionOnRelease: (spark) => spark.SetActive(false),
            actionOnDestroy: (spark) => Destroy(spark.gameObject),
            defaultCapacity: 50,
            maxSize: 300
        );
*/    }

    public void CreateDamagePopup(float damageAmount, Vector3 position, bool isCrit = false)
    {
        Vector3 randomOffset = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(0f, 0.3f), 0f);
        DamagePopup popup = popupPool.Get();
        popup.Setup(damageAmount, position + randomOffset, isCrit);
    }
}
