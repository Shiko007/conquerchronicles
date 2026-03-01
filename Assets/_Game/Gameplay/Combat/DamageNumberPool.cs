using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Combat
{
    public class DamageNumberPool : MonoBehaviour
    {
        [SerializeField] private DamageNumberView _prefab;
        [SerializeField] private int _warmupCount = 32;

        private readonly Queue<DamageNumberView> _available = new();

        public void Initialize(DamageNumberView prefab)
        {
            _prefab = prefab;
        }

        public void Warmup()
        {
            for (int i = 0; i < _warmupCount; i++)
            {
                var instance = Instantiate(_prefab, transform);
                instance.gameObject.SetActive(false);
                _available.Enqueue(instance);
            }
        }

        public DamageNumberView Get()
        {
            if (_available.Count > 0)
                return _available.Dequeue();
            return Instantiate(_prefab, transform);
        }

        public void Return(DamageNumberView view)
        {
            view.gameObject.SetActive(false);
            _available.Enqueue(view);
        }
    }
}
