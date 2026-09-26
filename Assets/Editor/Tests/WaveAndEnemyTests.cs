using NUnit.Framework;
using UnityEngine;
using SentinelForge.Features.Wave.Configs;

namespace SentinelForge.Tests.EditMode
{
    // Lớp giả lập kế thừa từ EnemyAI để kiểm thử FSM trong EditMode
    public class MockEnemyAI : EnemyAI
    {
        protected override void ProcessAI() { }
    }

    [TestFixture]
    public class WaveAndEnemyTests
    {
        [Test]
        public void WaveConfigSO_TotalEnemiesCount_CalculatesCorrectSum()
        {
            var waveConfig = ScriptableObject.CreateInstance<WaveConfigSO>();
            waveConfig.entries.Add(new WaveEntry { count = 5 });
            waveConfig.entries.Add(new WaveEntry { count = 12 });
            waveConfig.entries.Add(new WaveEntry { count = 8 });

            Assert.AreEqual(25, waveConfig.TotalEnemiesCount);
        }

        [Test]
        public void StageConfigSO_GetWaveConfig_ReturnsExpectedWaves()
        {
            var stageConfig = ScriptableObject.CreateInstance<StageConfigSO>();
            var wave1 = ScriptableObject.CreateInstance<WaveConfigSO>();
            wave1.waveNumber = 1;
            var wave2 = ScriptableObject.CreateInstance<WaveConfigSO>();
            wave2.waveNumber = 2;

            stageConfig.waves.Add(wave1);
            stageConfig.waves.Add(wave2);

            Assert.AreEqual(wave1, stageConfig.GetWaveConfig(1));
            Assert.AreEqual(wave2, stageConfig.GetWaveConfig(2));
            // Kiểm tra fallback khi chỉ số vượt quá số wave thiết kế sẵn
            Assert.AreEqual(wave2, stageConfig.GetWaveConfig(99));
        }

        [Test]
        public void EnemyAI_FSM_ChangeState_UpdatesCurrentStateAndTriggersEvent()
        {
            var go = new GameObject("TestEnemy");
            var enemy = go.AddComponent<MockEnemyAI>();

            EnemyState notifiedState = EnemyState.Approaching;
            enemy.OnStateChanged += state => notifiedState = state;

            // Bắt đầu ở Approaching
            Assert.AreEqual(EnemyState.Approaching, enemy.CurrentState);

            // Chuyển sang Stunned
            enemy.ChangeState(EnemyState.Stunned);
            Assert.AreEqual(EnemyState.Stunned, enemy.CurrentState);
            Assert.AreEqual(EnemyState.Stunned, notifiedState);
            Assert.IsTrue(enemy.isStunned);

            // Chuyển sang Attacking
            enemy.ChangeState(EnemyState.Attacking);
            Assert.AreEqual(EnemyState.Attacking, enemy.CurrentState);
            Assert.AreEqual(EnemyState.Attacking, notifiedState);
            Assert.IsFalse(enemy.isStunned);

            // Chuyển sang Dying
            enemy.ChangeState(EnemyState.Dying);
            Assert.AreEqual(EnemyState.Dying, enemy.CurrentState);
            Assert.AreEqual(EnemyState.Dying, notifiedState);

            Object.DestroyImmediate(go);
        }
    }
}
