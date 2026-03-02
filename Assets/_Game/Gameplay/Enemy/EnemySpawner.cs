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

            var movement = enemy.Movement ?? enemy.GetComponent<EnemyMovement>();
            if (movement != null)
            {
                movement.SetTarget(_playerTarget);
                movement.SetMoveSpeed(data.MoveSpeed);
                movement.SetAttackData(data.AttackRange, data.AttackCooldown);
            }

            _activeEnemies.Add(enemy);
            return enemy;
        }

        public void DespawnEnemy(EnemyView enemy)
        {
            int idx = _activeEnemies.IndexOf(enemy);
            if (idx >= 0)
            {
                int last = _activeEnemies.Count - 1;
                _activeEnemies[idx] = _activeEnemies[last];
                _activeEnemies.RemoveAt(last);
            }
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
            int i = 0;
            while (i < _activeEnemies.Count)
            {
                var enemy = _activeEnemies[i];
                if (enemy.State.IsDead)
                {
                    onEnemyDead?.Invoke(enemy);
                    int last = _activeEnemies.Count - 1;
                    _activeEnemies[i] = _activeEnemies[last];
                    _activeEnemies.RemoveAt(last);
                    _pool.Return(enemy);
                    // Don't increment i — check the swapped element
                }
                else
                {
                    i++;
                }
            }
        }
    }
}
