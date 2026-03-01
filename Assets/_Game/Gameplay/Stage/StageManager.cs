using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Stage;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;

namespace ConquerChronicles.Gameplay.Stage
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CombatManager _combatManager;
        [SerializeField] private CharacterView _player;

        private StageState _state;
        private Dictionary<string, EnemyData> _enemyCatalog;
        private int _totalXPFromKills;
        private bool _spawningWave;

        public StageState State => _state;

        public event Action<int, int> OnWaveChanged;         // current, total
        public event Action<string> OnWaveAnnouncement;       // message text
        public event Action<StageResult> OnStageComplete;
        public event Action OnBossSpawned;

        public void Initialize(EnemySpawner spawner, CombatManager combatManager, CharacterView player, Dictionary<string, EnemyData> enemyCatalog)
        {
            _enemySpawner = spawner;
            _combatManager = combatManager;
            _player = player;
            _enemyCatalog = enemyCatalog;
            _totalXPFromKills = 0;
        }

        public void StartStage(StageData stageData)
        {
            _state = new StageState(stageData);
            _totalXPFromKills = 0;
            _spawningWave = false;
            _enemySpawner.DespawnAll();

            // Subscribe to enemy deaths for tracking
            if (_combatManager != null)
            {
                _combatManager.OnPlayerXPGained -= OnXPGained;
                _combatManager.OnPlayerXPGained += OnXPGained;
            }

            OnWaveAnnouncement?.Invoke($"Stage: {stageData.Name}");
            StartCoroutine(SpawnNextWaveAfterDelay());
        }

        private void OnXPGained(int xp, bool isCrit)
        {
            _totalXPFromKills += xp;
        }

        private void Update()
        {
            if (_state == null || _state.IsComplete) return;

            _state.ElapsedTime += Time.deltaTime;

            // Check if player died
            if (_player != null && _player.State != null && _player.State.IsDead)
            {
                _state.PlayerDied = true;
                _state.IsComplete = true;
                CompleteStage();
                return;
            }

            // Update alive count from spawner
            _state.EnemiesAlive = _enemySpawner.ActiveCount;

            // If not currently spawning and no enemies alive, advance
            if (!_spawningWave && _state.EnemiesAlive == 0)
            {
                if (_state.BossSpawned)
                {
                    // Boss killed — stage complete!
                    _state.IsComplete = true;
                    CompleteStage();
                }
                else if (_state.AllWavesCleared)
                {
                    // All normal waves done — spawn boss
                    SpawnBoss();
                }
                else
                {
                    // Next wave
                    StartCoroutine(SpawnNextWaveAfterDelay());
                }
            }
        }

        private IEnumerator SpawnNextWaveAfterDelay()
        {
            _spawningWave = true;
            var wave = _state.GetCurrentWave();
            if (wave == null) { _spawningWave = false; yield break; }

            var waveData = wave.Value;
            int waveNum = _state.CurrentWaveIndex + 1;
            OnWaveAnnouncement?.Invoke($"Wave {waveNum}/{_state.TotalWaves}");
            OnWaveChanged?.Invoke(waveNum, _state.TotalWaves);

            yield return new WaitForSeconds(waveData.DelayBeforeWave);

            foreach (var entry in waveData.SpawnEntries)
            {
                if (!_enemyCatalog.TryGetValue(entry.EnemyID, out var enemyData)) continue;

                var edge = ConvertEdge(entry.Edge);

                switch (entry.Pattern)
                {
                    case SpawnPattern.Burst:
                        for (int i = 0; i < entry.Count; i++)
                            _enemySpawner.SpawnEnemy(enemyData, edge == Map.SpawnEdge.Random ? (Map.SpawnEdge)UnityEngine.Random.Range(0, 4) : edge);
                        break;

                    case SpawnPattern.Stream:
                        for (int i = 0; i < entry.Count; i++)
                        {
                            _enemySpawner.SpawnEnemy(enemyData, edge == Map.SpawnEdge.Random ? (Map.SpawnEdge)UnityEngine.Random.Range(0, 4) : edge);
                            yield return new WaitForSeconds(waveData.SpawnInterval);
                        }
                        break;

                    case SpawnPattern.Surround:
                        for (int i = 0; i < entry.Count; i++)
                            _enemySpawner.SpawnEnemy(enemyData, (Map.SpawnEdge)(i % 4));
                        break;
                }
            }

            _state.AdvanceWave();
            _spawningWave = false;
        }

        private void SpawnBoss()
        {
            _state.BossSpawned = true;
            OnBossSpawned?.Invoke();
            OnWaveAnnouncement?.Invoke("BOSS INCOMING!");

            var bossWave = _state.Data.BossWave;
            StartCoroutine(SpawnBossCoroutine(bossWave));
        }

        private IEnumerator SpawnBossCoroutine(WaveData bossWave)
        {
            _spawningWave = true;
            yield return new WaitForSeconds(bossWave.DelayBeforeWave);

            foreach (var entry in bossWave.SpawnEntries)
            {
                if (!_enemyCatalog.TryGetValue(entry.EnemyID, out var enemyData)) continue;
                for (int i = 0; i < entry.Count; i++)
                    _enemySpawner.SpawnEnemy(enemyData, ConvertEdge(entry.Edge));
            }

            _spawningWave = false;
        }

        private void CompleteStage()
        {
            _state.EnemiesKilled = _combatManager != null ? _combatManager.KillCount : 0;
            var result = StageResult.Calculate(_state, _totalXPFromKills);
            OnStageComplete?.Invoke(result);

            if (_combatManager != null)
                _combatManager.OnPlayerXPGained -= OnXPGained;
        }

        // Convert Core.Stage.SpawnEdge to Gameplay.Map.SpawnEdge
        private static Map.SpawnEdge ConvertEdge(ConquerChronicles.Core.Stage.SpawnEdge edge)
        {
            return edge switch
            {
                ConquerChronicles.Core.Stage.SpawnEdge.North => Map.SpawnEdge.North,
                ConquerChronicles.Core.Stage.SpawnEdge.South => Map.SpawnEdge.South,
                ConquerChronicles.Core.Stage.SpawnEdge.East => Map.SpawnEdge.East,
                ConquerChronicles.Core.Stage.SpawnEdge.West => Map.SpawnEdge.West,
                ConquerChronicles.Core.Stage.SpawnEdge.All => Map.SpawnEdge.Random,
                _ => Map.SpawnEdge.Random
            };
        }
    }
}
