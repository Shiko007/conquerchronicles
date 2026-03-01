using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Core.Stage;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Map;
using ConquerChronicles.Gameplay.Stage;
using ConquerChronicles.Gameplay.UI.HUD;

namespace ConquerChronicles.Gameplay.Bootstrap
{
    /// <summary>
    /// Test harness for Phase 4 stage-based combat.
    /// Drop this on a GameObject in the scene — wires everything together.
    /// </summary>
    public class GameplayTestSetup : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private IsometricCamera _isometricCamera;
        [SerializeField] private CharacterView _characterView;
        [SerializeField] private EnemyPool _enemyPool;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private MapBoundsProvider _mapBoundsProvider;

        [Header("Combat")]
        [SerializeField] private CombatManager _combatManager;
        [SerializeField] private DamageNumberPool _damageNumberPool;
        [SerializeField] private HitEffectPool _hitEffectPool;

        [Header("Stage")]
        [SerializeField] private StageManager _stageManager;
        [SerializeField] private WaveAnnouncerUI _waveAnnouncer;
        [SerializeField] private RunSummaryUI _runSummary;

        [Header("UI")]
        [SerializeField] private PlayerHUD _playerHUD;

        [Header("Settings")]
        [SerializeField] private CharacterClass _testClass = CharacterClass.Trojan;
        [SerializeField] private int _testStageIndex = 0;

        private void Start()
        {
            // Initialize player
            _characterView.Initialize(_testClass);

            // Camera
            _isometricCamera.SetFollowTarget(_characterView.transform);

            // Map
            _mapBoundsProvider.Initialize(_isometricCamera);

            // Enemy spawning
            _enemySpawner.Initialize(_enemyPool, _mapBoundsProvider, _characterView.transform);
            _enemyPool.Warmup();

            // Combat pools
            if (_damageNumberPool != null) _damageNumberPool.Warmup();
            if (_hitEffectPool != null) _hitEffectPool.Warmup();

            // Set up Trojan skills
            var skills = new List<SkillState>();
            foreach (var skillData in TrojanSkills.GetAll())
            {
                skills.Add(new SkillState(skillData));
            }

            // Combat manager
            if (_combatManager != null)
            {
                _combatManager.Initialize(
                    _characterView,
                    _enemySpawner,
                    _damageNumberPool,
                    _hitEffectPool,
                    skills);
            }

            // HUD
            if (_playerHUD != null)
            {
                _playerHUD.Initialize(_characterView, _combatManager);
            }

            // Build enemy catalog from test data
            var enemyCatalog = new Dictionary<string, EnemyData>();
            foreach (var enemy in TestStages.AllEnemies)
            {
                enemyCatalog[enemy.ID] = enemy;
            }

            // Stage manager
            if (_stageManager != null)
            {
                _stageManager.Initialize(_enemySpawner, _combatManager, _characterView, enemyCatalog);

                // Wire wave announcer
                if (_waveAnnouncer != null)
                {
                    _stageManager.OnWaveAnnouncement += _waveAnnouncer.ShowAnnouncement;
                }

                // Wire run summary
                if (_runSummary != null)
                {
                    _runSummary.Initialize();
                    _stageManager.OnStageComplete += _runSummary.Show;
                    _runSummary.OnContinue += OnStageContinue;
                }

                // Start the selected test stage
                var stages = TestStages.AllStages;
                int idx = Mathf.Clamp(_testStageIndex, 0, stages.Length - 1);
                _stageManager.StartStage(stages[idx]);
            }
        }

        private void OnStageContinue()
        {
            // Restart the same stage for testing
            var stages = TestStages.AllStages;
            int idx = Mathf.Clamp(_testStageIndex, 0, stages.Length - 1);
            _stageManager.StartStage(stages[idx]);
        }

        private void OnDestroy()
        {
            if (_stageManager != null)
            {
                if (_waveAnnouncer != null)
                    _stageManager.OnWaveAnnouncement -= _waveAnnouncer.ShowAnnouncement;
                if (_runSummary != null)
                    _stageManager.OnStageComplete -= _runSummary.Show;
            }
            if (_runSummary != null)
                _runSummary.OnContinue -= OnStageContinue;
        }
    }
}
