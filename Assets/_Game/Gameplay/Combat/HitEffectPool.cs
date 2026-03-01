using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Combat
{
    public class HitEffectPool : MonoBehaviour
    {
        [SerializeField] private HitEffectView _prefab;
        [SerializeField] private int _warmupCount = 32;

        private readonly Queue<HitEffectView> _available = new();

        public void Initialize(HitEffectView prefab)
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

        public HitEffectView Get()
        {
            if (_available.Count > 0)
                return _available.Dequeue();
            return Instantiate(_prefab, transform);
        }

        public void Return(HitEffectView view)
        {
            view.gameObject.SetActive(false);
            _available.Enqueue(view);
        }
    }
}
