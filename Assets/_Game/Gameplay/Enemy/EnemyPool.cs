using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Enemy
{
    public class EnemyPool : MonoBehaviour
    {
        [SerializeField] private EnemyView _prefab;
        [SerializeField] private int _warmupCount = 64;

        private readonly Queue<EnemyView> _available = new();
        private Transform _poolParent;

        public void Initialize(EnemyView prefab)
        {
            _prefab = prefab;
        }

        private void Awake()
        {
            _poolParent = transform;
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

        public EnemyView Get()
        {
            var instance = _available.Count > 0
                ? _available.Dequeue()
                : CreateInstance();

            return instance;
        }

        public void Return(EnemyView enemy)
        {
            enemy.Deactivate();
            _available.Enqueue(enemy);
        }

        private EnemyView CreateInstance()
        {
            var instance = Instantiate(_prefab, _poolParent);
            instance.gameObject.SetActive(false);
            return instance;
        }
    }
}
