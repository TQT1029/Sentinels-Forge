using System;
using UnityEngine;

public class WaveManager : Singleton<WaveManager>
{
    public int CurrentWave { get; private set; } = 0;

    public float WaveMultiplier => 1f + (CurrentWave - 1) * 0.1f;

    public int EnemyAlive { get; private set; } = 0;

    public static event Action<int> OnWaveStarted;

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
        EnemyAlive = Mathf.Max(0, EnemyAlive - 1);
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

        OnWaveStarted?.Invoke(CurrentWave);
    }
}
