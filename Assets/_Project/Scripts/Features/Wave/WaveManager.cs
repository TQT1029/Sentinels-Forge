using System;
using UnityEngine;
using SentinelForge.Core.Events;
using SentinelForge.Features.Wave.Configs;

public class WaveManager : Singleton<WaveManager>
{
    [Header("Data-Driven Configuration")]
    [SerializeField] private StageConfigSO _currentStageConfig;

    [Header("Event Channels")]
    [SerializeField] private IntEventChannelSO _onWaveStartedChannel;
    [SerializeField] private VoidEventChannelSO _onWaveEndedChannel;

    public int CurrentWave { get; private set; } = 0;
    public float WaveMultiplier => 1f + (CurrentWave - 1) * 0.1f;
    public int EnemyAlive { get; private set; } = 0;

    // Static events để duy trì khả năng tương thích ngược
    public static event Action<int> OnWaveStarted;
    public static event Action OnWaveEnded;

    public StageConfigSO CurrentStageConfig => _currentStageConfig;
    public WaveConfigSO CurrentWaveConfig => _currentStageConfig != null ? _currentStageConfig.GetWaveConfig(CurrentWave) : null;

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
            EndCurrentWave();
        }
    }

    private void EndCurrentWave()
    {
        // 1. Kích hoạt Event Wave Ended (bắt đầu thời gian nghỉ Intermission hoặc mở menu chế tạo)
        if (_onWaveEndedChannel != null)
        {
            _onWaveEndedChannel.RaiseEvent();
        }
        OnWaveEnded?.Invoke();

        // 2. Chuyển sang Wave kế tiếp
        StartNextWave();
    }

    public void StartNextWave()
    {
        CurrentWave++;
        EnemyAlive = 0;

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.StartingWave(CurrentWave);
        }

        // Kích hoạt Event Channel và static event
        if (_onWaveStartedChannel != null)
        {
            _onWaveStartedChannel.RaiseEvent(CurrentWave);
        }
        OnWaveStarted?.Invoke(CurrentWave);
    }
}
