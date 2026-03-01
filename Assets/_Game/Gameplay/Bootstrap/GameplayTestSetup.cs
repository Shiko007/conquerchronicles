using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Map;
using ConquerChronicles.Gameplay.UI.HUD;

namespace ConquerChronicles.Gameplay.Bootstrap
{
    /// <summary>
    /// Test harness for Phase 3 combat testing.
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

        [Header("UI")]
        [SerializeField] private PlayerHUD _playerHUD;

        [Header("Settings")]
        [SerializeField] private CharacterClass _testClass = CharacterClass.Trojan;
        [SerializeField] private float _spawnInterval = 1.5f;
        [SerializeField] private int _maxEnemies = 40;

        private float _spawnTimer;

        private readonly EnemyData _testEnemy = new()
        {
            ID = "slime_01",
            Name = "Green Slime",
            Stats = new CharacterStats
            {
                HP = 30,
                ATK = 5,
                DEF = 2,
                MATK = 0,
                MDEF = 1,
                AGI = 3
            },
            MoveSpeed = 1.5f,
            AttackRange = 0.5f,
            AttackCooldown = 1.5f,
            XPReward = 10,
            GoldReward = 5,
            IsBoss = false
        };

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
        }

        private void Update()
        {
            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= _spawnInterval && _enemySpawner.ActiveCount < _maxEnemies)
            {
                _spawnTimer = 0f;
                _enemySpawner.SpawnEnemy(_testEnemy, SpawnEdge.Random);
            }
        }
    }
}
