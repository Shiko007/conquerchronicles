using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Combat
{
    /// <summary>
    /// Object pool for ProjectileView instances. Follows the same pattern
    /// as EnemyPool, DamageNumberPool, and HitEffectPool.
    /// </summary>
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private ProjectileView _prefab;
        [SerializeField] private int _warmupCount = 32;

        private readonly Queue<ProjectileView> _available = new();

        public void Initialize(ProjectileView prefab)
        {
            _prefab = prefab;
        }

        public void Warmup()
        {
            for (int i = 0; i < _warmupCount; i++)
            {
                var instance = CreateInstance();
                instance.gameObject.SetActive(false);
                _available.Enqueue(instance);
            }
        }

        public ProjectileView Get()
        {
            var instance = _available.Count > 0
                ? _available.Dequeue()
                : CreateInstance();

            return instance;
        }

        public void Return(ProjectileView projectile)
        {
            projectile.Deactivate();
            _available.Enqueue(projectile);
        }

        private ProjectileView CreateInstance()
        {
            var instance = Instantiate(_prefab, transform);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
