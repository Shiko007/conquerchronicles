using System;
using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Core.Map;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;

namespace ConquerChronicles.Gameplay.Map
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private CombatManager _combatManager;
        [SerializeField] private CharacterView _player;

        private AreaState _areaState;
        private MapData _currentMap;
        private Dictionary<string, EnemyData> _enemyCatalog;
        private System.Random _spawnRng;
        private int _dropSeed;
        private List<string> _droppedItems;

        public AreaState AreaState => _areaState;

        public event Action<string> OnAreaAnnouncement;
        public event Action<AreaResult> OnAreaSessionEnd;
        public event Action<string, int> OnItemDropped;
        public event Action<int> OnEnemyKilledInArea;
        public event Action<GoldDropInfo> OnGoldDropped;
        public event Action<LootDropInfo> OnEquipmentDropped;
        public event Action OnAreaSessionEnding;

        public void Initialize(
            EnemySpawner spawner,
            CombatManager combatManager,
            CharacterView player,
            Dictionary<string, EnemyData> enemyCatalog)
        {
            _enemySpawner = spawner;
            _combatManager = combatManager;
            _player = player;
            _enemyCatalog = enemyCatalog;
            _droppedItems = new List<string>();
        }

        public void EnterArea(MapData map, AreaData area)
        {
            _currentMap = map;
            _areaState = new AreaState(area);
            _spawnRng = new System.Random(Environment.TickCount);
            _dropSeed = Environment.TickCount;
            _droppedItems.Clear();
            _enemySpawner.DespawnAll();

            OnAreaAnnouncement?.Invoke($"Entering: {area.Name}");
        }

        public void LeaveArea()
        {
            if (_areaState == null) return;
            _areaState.PlayerLeft = true;
            EndSession();
        }

        private void Update()
        {
            if (_areaState == null || !_areaState.IsActive) return;

            _areaState.ElapsedTime += Time.deltaTime;

            // Check player death
            if (_player != null && _player.State != null && _player.State.IsDead)
            {
                _areaState.PlayerDied = true;
                EndSession();
                return;
            }

            // Update alive count
            _areaState.EnemiesAlive = _enemySpawner.ActiveCount;

            // Spawn timer
            _areaState.SpawnTimer -= Time.deltaTime;
            if (_areaState.ShouldSpawn)
            {
                SpawnRandomEnemy();
                _areaState.SpawnTimer = _areaState.Data.SpawnInterval;
            }
        }

        private void SpawnRandomEnemy()
        {
            string enemyID = TestMaps.PickRandomEnemy(_areaState.Data.EnemyPool, _spawnRng);
            if (!_enemyCatalog.TryGetValue(enemyID, out var enemyData)) return;

            var edge = (SpawnEdge)_spawnRng.Next(0, 4);
            _enemySpawner.SpawnEnemy(enemyData, edge);
        }

        public void OnEnemyDied(EnemyView enemy)
        {
            if (_areaState == null) return;

            _areaState.EnemiesKilled++;
            OnEnemyKilledInArea?.Invoke(_areaState.EnemiesKilled);

            var deathPos = enemy.transform.position;

            // Track gold with area multiplier
            int gold = (int)(enemy.State.Data.GoldReward * _areaState.Data.GoldMultiplier);
            _areaState.TotalGoldEarned += gold;
            OnGoldDropped?.Invoke(new GoldDropInfo(gold, deathPos.x, deathPos.y));

            // Track XP with area multiplier (bonus portion — base already granted by CombatManager)
            int baseXP = enemy.State.Data.XPReward;
            int bonusXP = (int)(baseXP * (_areaState.Data.XPMultiplier - 1.0f));
            _areaState.TotalXPEarned += baseXP + bonusXP;
            if (bonusXP > 0 && _player != null)
            {
                _player.GainXP(bonusXP);
            }

            // Roll drops — area drop table
            _dropSeed++;
            var areaDrops = DropTable.Roll(_areaState.Data.AreaDropTable, _dropSeed);
            foreach (var (itemID, qty) in areaDrops)
            {
                OnEquipmentDropped?.Invoke(new LootDropInfo(itemID, qty, deathPos.x, deathPos.y));
                Debug.Log($"[Loot] Area drop: {itemID} x{qty}");
            }

            // Roll drops — enemy-specific drop table
            var enemyDrops = DropTable.Roll(enemy.State.Data.DropTable, _dropSeed + 1);
            foreach (var (itemID, qty) in enemyDrops)
            {
                OnEquipmentDropped?.Invoke(new LootDropInfo(itemID, qty, deathPos.x, deathPos.y));
                Debug.Log($"[Loot] Enemy drop: {itemID} x{qty}");
            }
        }

        public void CollectItem(string itemID, int quantity)
        {
            for (int i = 0; i < quantity; i++)
                _droppedItems.Add(itemID);
            OnItemDropped?.Invoke(itemID, quantity);
            Debug.Log($"[Loot] Collected: {itemID} x{quantity}");
        }

        private void EndSession()
        {
            OnAreaSessionEnding?.Invoke();
            var result = AreaResult.Calculate(_areaState, _currentMap.ID, _droppedItems.ToArray());
            OnAreaSessionEnd?.Invoke(result);
            _enemySpawner.DespawnAll();
            Debug.Log($"[MapManager] Session ended: {result.EnemiesKilled} kills, {result.GoldEarned} gold, {result.XPEarned} XP, {result.ItemsDropped.Length} items");
        }
    }
}
