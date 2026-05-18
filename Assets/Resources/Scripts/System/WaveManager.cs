using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public Camera MainCamera { get; private set; }
    public Transform TowerTransform { get; private set; }

    public Bounds TowerBounds {  get; private set; }

    public TowerController TowerController { get; private set; }

    public int CurrentWave { get; private set; } = 0;

    public int EnemyAlive { get; private set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        MainCamera = Camera.main;
        TowerTransform = GameObject.FindGameObjectWithTag(GameConstants.TOWER_TAG).transform;
        TowerBounds = TowerTransform.GetComponent<Collider2D>().bounds;
        TowerController = TowerTransform.GetComponent<TowerController>();
    }

    protected virtual void Start()
    {
        StartNextWave();
    }

    public void EnemySpawned()
    {
        EnemyAlive++;
    }

    public void EnemyKilled()
    {
        EnemyAlive--;
        if (EnemyAlive <= 0)
        {
            StartNextWave();
        }
    }

    public void StartNextWave()
    {
        CurrentWave++;
        EnemyAlive = 0;
        EnemySpawner.Instance.StartingWave(CurrentWave);
    }
}
