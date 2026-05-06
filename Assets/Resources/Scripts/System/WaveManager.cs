using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public Camera MainCamera { get; private set; }
    public Transform TowerTransform { get; private set; }

    public int CurrentWave { get; private set; } = 0;

    public int EmnemyAlive { get; private set; } = 0;

    protected override void Awake()
    {
        base.Awake();
        MainCamera = Camera.main;
        TowerTransform = GameObject.FindGameObjectWithTag("Tower").transform;   
    }

    public void EnemySpawned()
    {
        EmnemyAlive++;
    }

    public void EnemyKilled()
    {
        EmnemyAlive--;
        if (EmnemyAlive <= 0)
        {
            StartNextWave();
        }
    }

    public void StartNextWave()
    {
        CurrentWave++;
        EmnemyAlive = 0;
        //EnemySpawner.Instance.StartSpawning(CurrentWave);
    }
}
