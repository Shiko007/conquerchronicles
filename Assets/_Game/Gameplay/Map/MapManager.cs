using System;
using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Core.Map;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Audio;
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
        private AreaData _currentArea;
        private Dictionary<string, EnemyData> _enemyCatalog;
        private System.Random _spawnRng;
        private int _dropSeed;
        private List<string> _droppedItems;

        private AudioManager _audioManager;

        private float _metaGoldMultiplier = 1.0f;
        private float _metaXPMultiplier = 1.0f;
        private float _metaDropRateBonus = 0f;

        // Delay before starting revive timer so the death animation can finish.
        // Death anim plays at 8 fps; 1.5 s covers up to 12 frames comfortably.
        private const float DeathAnimDuration = 1.5f;
        private float _deathAnimDelay = -1f;
        private bool _loggedFirstUpdate;

        // Revive timer — player stays dead for this many seconds before auto-reviving.
        private const float ReviveDuration = 20f;
        private float _reviveTimer = -1f;

        public AreaState AreaState => _areaState;

        public event Action<string> OnAreaAnnouncement;
        public event Action<AreaResult> OnAreaSessionEnd;
        public event Action<string, int> OnItemDropped;
        public event Action<int> OnEnemyKilledInArea;
        public event Action<GoldDropInfo> OnGoldDropped;
        public event Action<LootDropInfo> OnEquipmentDropped;
        public event Action OnAreaSessionEnding;
        public event Action<float> OnReviveTimerTick;
        public event Action OnPlayerRevived;

        public void SetMetaMultipliers(float goldMult, float xpMult, float dropBonus)
        {
            _metaGoldMultiplier = goldMult;
            _metaXPMultiplier = xpMult;
            _metaDropRateBonus = dropBonus;
        }

        public void SetAudioManager(AudioManager audioManager) { _audioManager = audioManager; }

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
            // Debug.Log($"[MapManager] EnterArea: {area.Name}, MaxEnemies={area.MaxConcurrentEnemies}, SpawnInterval={area.SpawnInterval}");
            _currentMap = map;
            _currentArea = area;
            _areaState = new AreaState(area);
            _spawnRng = new System.Random(Environment.TickCount);
            _dropSeed = Environment.TickCount;
            _droppedItems.Clear();
            _enemySpawner.DespawnAll();

            // Reset player HP to full for new session
            if (_player != null && _player.State != null)
            {
                var stats = _player.GetComputedStats();
                _player.State.CurrentHP = stats.HP;
                _player.State.CurrentMP = stats.MP;
                _player.PlayIdle();
            }

            // Debug.Log($"[MapManager] AreaState.IsActive={_areaState.IsActive}, EnemyPool={area.EnemyPool?.Length ?? 0} entries");
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
            if (_areaState == null) return;

            // Death animation delay must tick even when area is no longer "active"
            // (PlayerDied sets IsActive to false).
            if (_deathAnimDelay >= 0f)
            {
                _deathAnimDelay -= Time.deltaTime;
                if (_deathAnimDelay < 0f)
                {
                    // Death animation finished — despawn all enemies and start revive countdown.
                    _enemySpawner.DespawnAll();
                    _reviveTimer = ReviveDuration;
                    // Debug.Log($"[MapManager] Death anim finished. Starting {ReviveDuration}s revive timer.");
                }
                return;
            }

            // Revive countdown — player stays dead, waiting to auto-revive.
            if (_reviveTimer >= 0f)
            {
                _reviveTimer -= Time.deltaTime;
                OnReviveTimerTick?.Invoke(Mathf.Max(0f, _reviveTimer));
                if (_reviveTimer < 0f)
                {
                    RevivePlayer();
                }
                return;
            }

            if (!_areaState.IsActive)
            {
                if (!_loggedFirstUpdate) { _loggedFirstUpdate = true; }
                return;
            }

            if (!_loggedFirstUpdate)
            {
                // Debug.Log($"[MapManager] First Update: IsActive=true, SpawnTimer={_areaState.SpawnTimer:F2}, EnemiesAlive={_areaState.EnemiesAlive}, ShouldSpawn={_areaState.ShouldSpawn}");
                _loggedFirstUpdate = true;
            }

            _areaState.ElapsedTime += Time.deltaTime;

            // Check player death — start the animation delay.
            if (_player != null && _player.State != null && _player.State.IsDead)
            {
                _areaState.PlayerDied = true;
                _deathAnimDelay = DeathAnimDuration;
                _combatManager.enabled = false;
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
            if (!_enemyCatalog.TryGetValue(enemyID, out var enemyData))
            {
                Debug.LogWarning($"[MapManager] Enemy ID '{enemyID}' not found in catalog!");
                return;
            }

            var edge = (SpawnEdge)_spawnRng.Next(0, 4);
            // Debug.Log($"[MapManager] Spawning {enemyID} from {edge}");
            _enemySpawner.SpawnEnemy(enemyData, edge);
        }

        public void OnEnemyDied(EnemyView enemy)
        {
            if (_areaState == null) return;

            _areaState.EnemiesKilled++;
            OnEnemyKilledInArea?.Invoke(_areaState.EnemiesKilled);

            var deathPos = enemy.transform.position;

            // Track gold with area multiplier and meta multiplier
            int gold = (int)(enemy.State.Data.GoldReward * _areaState.Data.GoldMultiplier * _metaGoldMultiplier);
            _areaState.TotalGoldEarned += gold;
            OnGoldDropped?.Invoke(new GoldDropInfo(gold, deathPos.x, deathPos.y));
            if (_audioManager?.Library != null) _audioManager.PlaySFX(_audioManager.Library.GoldCollect);

            // Track XP with area multiplier and meta multiplier (bonus portion — base already granted by CombatManager)
            long baseXP = enemy.State.Data.XPReward;
            float totalXPMult = _areaState.Data.XPMultiplier * _metaXPMultiplier;
            long bonusXP = (long)(baseXP * (totalXPMult - 1.0f));
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
#if UNITY_EDITOR
                Debug.Log($"[Loot] Area drop: {itemID} x{qty}");
#endif
            }

            // Roll drops — enemy-specific drop table
            var enemyDrops = DropTable.Roll(enemy.State.Data.DropTable, _dropSeed + 1);
            foreach (var (itemID, qty) in enemyDrops)
            {
                OnEquipmentDropped?.Invoke(new LootDropInfo(itemID, qty, deathPos.x, deathPos.y));
#if UNITY_EDITOR
                Debug.Log($"[Loot] Enemy drop: {itemID} x{qty}");
#endif
            }
        }

        public void CollectItem(string itemID, int quantity)
        {
            for (int i = 0; i < quantity; i++)
                _droppedItems.Add(itemID);
            OnItemDropped?.Invoke(itemID, quantity);
#if UNITY_EDITOR
            Debug.Log($"[Loot] Collected: {itemID} x{quantity}");
#endif
        }

        private void RevivePlayer()
        {
            _reviveTimer = -1f;
            _deathAnimDelay = -1f;

            // Restore player HP/MP to full and clear death state.
            if (_player != null && _player.State != null)
            {
                var stats = _player.GetComputedStats();
                _player.State.CurrentHP = stats.HP;
                _player.State.CurrentMP = stats.MP;
                _player.ResetActionState();
                _player.PlayIdle();
            }

            // Re-enable combat.
            if (_combatManager != null) _combatManager.enabled = true;

            // Re-enter the same area to restart enemy spawning.
            Debug.Log($"[MapManager] Player revived. Re-entering area: {_currentArea.Name}");
            EnterArea(_currentMap, _currentArea);

            OnPlayerRevived?.Invoke();
        }

        private void EndSession()
        {
            // Reset death delay and re-enable combat for the next session.
            _deathAnimDelay = -1f;
            if (_combatManager != null) _combatManager.enabled = true;

            OnAreaSessionEnding?.Invoke();
            var result = AreaResult.Calculate(_areaState, _currentMap.ID, _droppedItems.ToArray());
            OnAreaSessionEnd?.Invoke(result);
            _enemySpawner.DespawnAll();
#if UNITY_EDITOR
            Debug.Log($"[MapManager] Session ended: {result.EnemiesKilled} kills, {result.GoldEarned} gold, {result.XPEarned} XP, {result.ItemsDropped.Length} items");
#endif
        }
    }
}
