using VContainer.Unity;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Map;

namespace ConquerChronicles.Gameplay.Bootstrap
{
    public class GameplayInitializer : IStartable, ITickable
    {
        private readonly IsometricCamera _camera;
        private readonly CharacterView _characterView;
        private readonly EnemyPool _enemyPool;
        private readonly EnemySpawner _enemySpawner;
        private readonly MapBoundsProvider _mapBoundsProvider;

        private float _spawnTimer;
        private const float SpawnInterval = 2f;
        private const int MaxEnemies = 30;

        // Test enemy data for Phase 2
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

        public GameplayInitializer(
            IsometricCamera camera,
            CharacterView characterView,
            EnemyPool enemyPool,
            EnemySpawner enemySpawner,
            MapBoundsProvider mapBoundsProvider)
        {
            _camera = camera;
            _characterView = characterView;
            _enemyPool = enemyPool;
            _enemySpawner = enemySpawner;
            _mapBoundsProvider = mapBoundsProvider;
        }

        public void Start()
        {
            // Initialize player as Trojan for testing
            _characterView.Initialize(CharacterClass.Trojan);

            // Camera follows the player
            _camera.SetFollowTarget(_characterView.transform);

            // Initialize subsystems
            _mapBoundsProvider.Initialize(_camera);
            _enemySpawner.Initialize(_enemyPool, _mapBoundsProvider, _characterView.transform);

            // Warm up the enemy pool
            _enemyPool.Warmup();

            _spawnTimer = 0f;
        }

        public void Tick()
        {
            _spawnTimer += UnityEngine.Time.deltaTime;

            if (_spawnTimer >= SpawnInterval && _enemySpawner.ActiveCount < MaxEnemies)
            {
                _spawnTimer = 0f;
                _enemySpawner.SpawnEnemy(_testEnemy, SpawnEdge.Random);
            }
        }
    }
}
