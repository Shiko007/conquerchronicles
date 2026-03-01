using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Map;

namespace ConquerChronicles.Gameplay.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyPool _pool;
        [SerializeField] private MapBoundsProvider _mapBounds;
        [SerializeField] private Transform _playerTarget;

        private readonly List<EnemyView> _activeEnemies = new(256);

        public IReadOnlyList<EnemyView> ActiveEnemies => _activeEnemies;
        public int ActiveCount => _activeEnemies.Count;

        public void Initialize(EnemyPool pool, MapBoundsProvider mapBounds, Transform playerTarget)
        {
            _pool = pool;
            _mapBounds = mapBounds;
            _playerTarget = playerTarget;
        }

        public EnemyView SpawnEnemy(EnemyData data, SpawnEdge edge)
        {
            var spawnPos = _mapBounds.GetSpawnPosition(edge);
            var enemy = _pool.Get();
            enemy.Initialize(data, spawnPos);

            var movement = enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetTarget(_playerTarget);
                movement.SetMoveSpeed(data.MoveSpeed);
            }

            _activeEnemies.Add(enemy);
            return enemy;
        }

        public void DespawnEnemy(EnemyView enemy)
        {
            _activeEnemies.Remove(enemy);
            _pool.Return(enemy);
        }

        public void DespawnAll()
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                _pool.Return(_activeEnemies[i]);
            }
            _activeEnemies.Clear();
        }

        public void RemoveDeadEnemies(System.Action<EnemyView> onEnemyDead = null)
        {
            for (int i = _activeEnemies.Count - 1; i >= 0; i--)
            {
                var enemy = _activeEnemies[i];
                if (enemy.State.IsDead)
                {
                    onEnemyDead?.Invoke(enemy);
                    _activeEnemies.RemoveAt(i);
                    _pool.Return(enemy);
                }
            }
        }
    }
}
