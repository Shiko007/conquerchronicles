using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Map;

namespace ConquerChronicles.Gameplay.Bootstrap
{
    /// <summary>
    /// Temporary test harness for Phase 2.
    /// Drop this on a GameObject in the scene to test enemy spawning.
    /// Will be replaced by proper VContainer wiring later.
    /// </summary>
    public class GameplayTestSetup : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private IsometricCamera _isometricCamera;
        [SerializeField] private CharacterView _characterView;
        [SerializeField] private EnemyPool _enemyPool;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private MapBoundsProvider _mapBoundsProvider;

        [Header("Settings")]
        [SerializeField] private CharacterClass _testClass = CharacterClass.Trojan;
        [SerializeField] private float _spawnInterval = 2f;
        [SerializeField] private int _maxEnemies = 30;

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
            _characterView.Initialize(_testClass);
            _isometricCamera.SetFollowTarget(_characterView.transform);
            _mapBoundsProvider.Initialize(_isometricCamera);
            _enemySpawner.Initialize(_enemyPool, _mapBoundsProvider, _characterView.transform);
            _enemyPool.Warmup();
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
