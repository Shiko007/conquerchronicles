using System.Collections.Generic;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Loot
{
    public class EquipmentDropPool : MonoBehaviour
    {
        [SerializeField] private EquipmentDropView _prefab;
        [SerializeField] private int _warmupCount = 16;

        private readonly Queue<EquipmentDropView> _available = new();

        public void Initialize(EquipmentDropView prefab)
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

        public EquipmentDropView Get()
        {
            if (_available.Count > 0)
                return _available.Dequeue();
            return Instantiate(_prefab, transform);
        }

        public void Return(EquipmentDropView view)
        {
            view.Deactivate();
            _available.Enqueue(view);
        }
    }
}
