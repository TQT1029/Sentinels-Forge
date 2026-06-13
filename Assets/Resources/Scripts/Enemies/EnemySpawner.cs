using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public class EnemyPoolConfig
{
    [Header("Enemy Data")]
    public EnemyAI prefab;
    [Tooltip("Số lượng mặc định sẽ xuất hiện")] public int defaultCapacity { get; private set; } = 0;
    [field: Min(20)] public int maxSize { get; private set; } = 100;

    [Header("Wave Generation Logic")]
    [Min(1)] public int cost = 10; // Mức tiêu hao ngân sách. Quái elite cost = 100, quái thường cost = 10
    [Min(1)] public int unlockAtWave = 1; // Ngăn quái mạnh xuất hiện quá sớm
    [SerializeField] private int lockAtWave = 10; // Dừng spawn ở wave được chọn
    public int LockAtWave => lockAtWave >= unlockAtWave ? lockAtWave : unlockAtWave + 5;
}

public class EnemySpawner : Singleton<EnemySpawner>
{
    [SerializeField] private int defaultBudget = 0;

    [SerializeField] private List<EnemyPoolConfig> poolConfigs;

    private Dictionary<EnemyAI, IObjectPool<EnemyAI>> enemyPools;

    private Transform enemiesStorageObj;
    private Coroutine _spawnCoroutine;
    private List<EnemyPoolConfig> affordableTypesBuffer = new List<EnemyPoolConfig>();


    [field: SerializeField] public Transform startPoint { get; private set; }
    [field: SerializeField] public Transform endPoint { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        enemyPools = new Dictionary<EnemyAI, IObjectPool<EnemyAI>>();

        // Khởi tạo pool cho từng loại quái vật được cấu hình trong Inspector
        foreach (var config in poolConfigs)
        {
            var pool = new ObjectPool<EnemyAI>(
                createFunc: () => CreateEnemy(config.prefab),
                actionOnGet: OnGetEnemy,
                actionOnRelease: OnReleaseEnemy,
                actionOnDestroy: OnDestroyEnemy,
                collectionCheck: false,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );

            enemyPools.Add(config.prefab, pool);
        }

        if (enemiesStorageObj == null)
        {
            enemiesStorageObj = GameObject.Find(GameConstants.Config.ENEMIES_STORAGE_NAME)?.transform;
            if (enemiesStorageObj == null)
            {
                enemiesStorageObj = new GameObject(GameConstants.Config.ENEMIES_STORAGE_NAME).transform;
            }
        }
    }

    private EnemyAI CreateEnemy(EnemyAI prefab)
    {
        Vector2 randomPosition = RandomUtils.RandomPosition(startPoint.position, endPoint.position);

        EnemyAI enemy = Instantiate(prefab, randomPosition, Quaternion.identity, enemiesStorageObj);
        enemy.SetPool(enemyPools[prefab]);
        return enemy;
    }

    private void OnGetEnemy(EnemyAI enemy)
    {
        enemy.gameObject.SetActive(true);
        // Bắt buộc reset HP và RuntimeState mỗi lần lấy từ Pool ra
        enemy.ResetStats(); 
    }

    private void OnReleaseEnemy(EnemyAI enemy)
    {
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroyEnemy(EnemyAI enemy)
    {
        Destroy(enemy.gameObject);
    }

    public EnemyAI SpawnEnemy(EnemyAI prefab, Vector3 spawnPosition)
    {
        if (!enemyPools.ContainsKey(prefab))
        {
            Debug.LogError($"[EnemySpawner] Không tìm thấy Pool cho {prefab.name}");
            return null;
        }

        EnemyAI enemy = enemyPools[prefab].Get();
        enemy.transform.position = spawnPosition;

        WaveManager.Instance.EnemySpawned();

        return enemy;
    }

    public void StartingWave(int currentWave)
    {
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(ProcessWaveSpawning(currentWave));
    }

    private IEnumerator ProcessWaveSpawning(int waveIndex)
    {
        int waveBudget = CalculateBudget(waveIndex);
        List<EnemyAI> spawnQueue = GenerateSpawnQueue(waveBudget, waveIndex);

        // Giảm dần thời gian chờ spawn theo wave, khóa ở mức tối thiểu 0.5s để tránh lag
        float spawnInterval = Mathf.Clamp(3f - (waveIndex * 0.1f), 0.5f, 3f);

        foreach (var enemyPrefab in spawnQueue)
        {
            SpawnEnemy(enemyPrefab, GetRandomSpawnPosition());
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int CalculateBudget(int waveIndex)
    {
        // Công thức phi tuyến tính: Wave càng cao, ngân sách tăng càng đột biến
        int budget = 50 + (waveIndex * 15) + (int)Mathf.Pow(waveIndex, 1.8f) + defaultBudget;
        Debug.Log($"[EnemySpawner] Wave Budget: {budget}");
        return budget;
    }

    private List<EnemyAI> GenerateSpawnQueue(int budget, int currentWave)
    {
        List<EnemyAI> queue = new List<EnemyAI>();

        affordableTypesBuffer.Clear();
        foreach(var c in poolConfigs)
        {
            if (c.unlockAtWave <= currentWave && currentWave <= c.LockAtWave || c.unlockAtWave <= currentWave && c.unlockAtWave == c.LockAtWave)
            {
                affordableTypesBuffer.Add(c);
            }
        }

        while (budget > 0 && affordableTypesBuffer.Count > 0)
        {
            for (int i = affordableTypesBuffer.Count - 1; i >= 0; i--)
            {
                if (affordableTypesBuffer[i].cost > budget)
                {
                    affordableTypesBuffer.RemoveAt(i);
                }
            }

            if (affordableTypesBuffer.Count == 0) break;

            EnemyPoolConfig selectedType = affordableTypesBuffer[Random.Range(0, affordableTypesBuffer.Count)];
            queue.Add(selectedType.prefab);
            budget -= selectedType.cost;
        }

        return queue;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        // Thay đổi vị trí kẻ địch trước khi triệu hồi
        return RandomUtils.RandomPosition(startPoint.position, endPoint.position);
    }
}