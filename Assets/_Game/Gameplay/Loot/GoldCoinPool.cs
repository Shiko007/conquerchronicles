using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Loot
{
    public class GoldCoinPool : MonoBehaviour
    {
        [SerializeField] private GoldCoinView _prefab;
        [SerializeField] private int _warmupCount = 32;

        private readonly Queue<GoldCoinView> _available = new();

        public void Initialize(GoldCoinView prefab)
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

        public GoldCoinView Get()
        {
            if (_available.Count > 0)
                return _available.Dequeue();
            return Instantiate(_prefab, transform);
        }

        public void Return(GoldCoinView view)
        {
            view.gameObject.SetActive(false);
            _available.Enqueue(view);
        }
    }
}
