using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ConquerChronicles.Core.Map;
using ConquerChronicles.Gameplay.Combat;

namespace ConquerChronicles.Gameplay.Loot
{
    public class LootVisualManager : MonoBehaviour
    {
        [SerializeField] private GoldCoinPool _goldCoinPool;
        [SerializeField] private EquipmentDropPool _equipmentDropPool;
        [SerializeField] private DamageNumberPool _damageNumberPool;
        [SerializeField] private Transform _playerTransform;

        private readonly List<EquipmentDropView> _activeDrops = new();
        private UnityEngine.Camera _mainCamera;

        private static readonly Color GoldTextColor = new(1f, 0.85f, 0.2f, 1f);
        private static readonly Color FullTextColor = new(1f, 0.3f, 0.3f, 1f);

        private Func<bool> _isBagFull;
        private Func<bool> _isPlayerDead;

        public event Action<string, int> OnEquipmentCollected;

        public void Initialize(GoldCoinPool goldPool, EquipmentDropPool equipPool,
            DamageNumberPool damageNumberPool, Transform playerTransform)
        {
            _goldCoinPool = goldPool;
            _equipmentDropPool = equipPool;
            _damageNumberPool = damageNumberPool;
            _playerTransform = playerTransform;
            _mainCamera = UnityEngine.Camera.main;
        }

        public void SetBagFullChecker(Func<bool> checker) => _isBagFull = checker;
        public void SetDeadChecker(Func<bool> checker) => _isPlayerDead = checker;

        private void Update()
        {
            if (_activeDrops.Count == 0) return;

            var pointer = Pointer.current;
            if (pointer == null || !pointer.press.wasPressedThisFrame) return;

            if (_mainCamera == null)
                _mainCamera = UnityEngine.Camera.main;
            if (_mainCamera == null) return;

            var screenPos = pointer.position.ReadValue();
            var worldPos = _mainCamera.ScreenToWorldPoint(screenPos);
            var hit = Physics2D.OverlapPoint(new Vector2(worldPos.x, worldPos.y));

            if (hit != null)
            {
                var drop = hit.GetComponent<EquipmentDropView>();
                if (drop != null && !drop.IsCollecting)
                    CollectDrop(drop);
            }
        }

        public void SpawnGoldCoin(GoldDropInfo info)
        {
            var coin = _goldCoinPool.Get();
            var spawnPos = new Vector3(info.WorldX, info.WorldY, 0f);
            var playerPos = _playerTransform != null ? _playerTransform.position : spawnPos;

            coin.OnReachedPlayer = () =>
            {
                ShowGoldText(info.Amount);
                _goldCoinPool.Return(coin);
            };

            coin.Play(spawnPos, playerPos, info.Amount);
        }

        private void ShowGoldText(int amount)
        {
            if (_damageNumberPool == null) return;
            var playerPos = _playerTransform != null ? _playerTransform.position : Vector3.zero;
            var textView = _damageNumberPool.Get();
            textView.PlayText($"+{amount}", 5f, GoldTextColor, playerPos);
        }

        private void ShowInventoryFullText()
        {
            if (_damageNumberPool == null) return;
            var playerPos = _playerTransform != null ? _playerTransform.position : Vector3.zero;
            var textView = _damageNumberPool.Get();
            textView.PlayText("Inventory is full", 4f, FullTextColor, playerPos);
        }

        private void ShowDeadText()
        {
            if (_damageNumberPool == null) return;
            var playerPos = _playerTransform != null ? _playerTransform.position : Vector3.zero;
            var textView = _damageNumberPool.Get();
            textView.PlayText("Can't loot while dead", 4f, FullTextColor, playerPos);
        }

        public void SpawnEquipmentDrop(LootDropInfo info)
        {
            var drop = _equipmentDropPool.Get();
            var scatter = new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                UnityEngine.Random.Range(-0.3f, 0.3f),
                0f);
            var pos = new Vector3(info.WorldX, info.WorldY, 0f) + scatter;
            drop.Initialize(info.ItemID, info.Quantity, pos);
            _activeDrops.Add(drop);
            Debug.Log($"[Loot] Equipment drop spawned: {info.ItemID} x{info.Quantity} at ({pos.x:F1}, {pos.y:F1})");
        }

        private void CollectDrop(EquipmentDropView drop)
        {
            if (_isPlayerDead != null && _isPlayerDead())
            {
                ShowDeadText();
                return;
            }
            if (_isBagFull != null && _isBagFull())
            {
                ShowInventoryFullText();
                return;
            }

            var playerPos = _playerTransform != null ? _playerTransform.position : drop.transform.position;
            var itemID = drop.ItemID;
            var qty = drop.Quantity;

            drop.OnCollectComplete = () =>
            {
                Debug.Log($"[Loot] Collected: {itemID} x{qty}");
                OnEquipmentCollected?.Invoke(itemID, qty);
                _activeDrops.Remove(drop);
                _equipmentDropPool.Return(drop);
            };

            drop.Collect(playerPos);
        }

        /// <summary>Discards all active drops without collecting them.</summary>
        public void DiscardAllDrops()
        {
            for (int i = _activeDrops.Count - 1; i >= 0; i--)
                _equipmentDropPool.Return(_activeDrops[i]);
            _activeDrops.Clear();
        }
    }
}
