using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public Camera MainCamera { get; private set; }
    public Transform TowerTransform { get; private set; }

    public int CurrentWave { get; private set; } = 0;

    public int EnemyAlive { get; private set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        MainCamera = Camera.main;
        TowerTransform = GameObject.FindGameObjectWithTag("Tower").transform;   
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
        //EnemySpawner.Instance.StartSpawning(CurrentWave);
    }
}
