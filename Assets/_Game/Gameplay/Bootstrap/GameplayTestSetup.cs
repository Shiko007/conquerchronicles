using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Core.Map;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Loot;
using ConquerChronicles.Gameplay.Map;
using ConquerChronicles.Gameplay.Save;
using ConquerChronicles.Gameplay.Stage;
using ConquerChronicles.Gameplay.UI.HUD;
using UnityEngine.SceneManagement;

namespace ConquerChronicles.Gameplay.Bootstrap
{
    /// <summary>
    /// Test harness for MMO-style map combat.
    /// Drop this on a GameObject in the scene — wires everything together.
    /// </summary>
    public class GameplayTestSetup : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private IsometricCamera _isometricCamera;
        [SerializeField] private CharacterView _characterView;
        [SerializeField] private EnemyPool _enemyPool;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private MapBoundsProvider _mapBoundsProvider;

        [Header("Combat")]
        [SerializeField] private CombatManager _combatManager;
        [SerializeField] private DamageNumberPool _damageNumberPool;
        [SerializeField] private HitEffectPool _hitEffectPool;

        [Header("Map")]
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private WaveAnnouncerUI _waveAnnouncer;
        [SerializeField] private RunSummaryUI _runSummary;

        [Header("Loot")]
        [SerializeField] private LootVisualManager _lootVisualManager;
        [SerializeField] private GoldCoinPool _goldCoinPool;
        [SerializeField] private EquipmentDropPool _equipmentDropPool;

        [Header("UI")]
        [SerializeField] private PlayerHUD _playerHUD;

        [Header("Settings")]
        [SerializeField] private CharacterClass _testClass = CharacterClass.Trojan;
        [SerializeField] private int _testMapIndex = 0;
        [SerializeField] private int _testAreaIndex = 0;
        [SerializeField] private bool _autoSave = true;

        private SaveManager _saveManager;

        private void Start()
        {
            // Initialize player
            _characterView.Initialize(_testClass);

            // Camera
            _isometricCamera.SetFollowTarget(_characterView.transform);

            // Map bounds
            _mapBoundsProvider.Initialize(_isometricCamera);

            // Enemy spawning
            _enemySpawner.Initialize(_enemyPool, _mapBoundsProvider, _characterView.transform);
            _enemyPool.Warmup();

            // Combat pools
            if (_damageNumberPool != null) _damageNumberPool.Warmup();
            if (_hitEffectPool != null) _hitEffectPool.Warmup();

            // Loot pools
            if (_goldCoinPool != null) _goldCoinPool.Warmup();
            if (_equipmentDropPool != null) _equipmentDropPool.Warmup();
            if (_lootVisualManager != null)
                _lootVisualManager.Initialize(_goldCoinPool, _equipmentDropPool, _damageNumberPool, _characterView.transform);

            // Set up class skills
            var skills = new List<SkillState>();
            foreach (var skillData in ClassSkills.GetSkillsForClass(_testClass))
            {
                skills.Add(new SkillState(skillData));
            }

            // Combat manager
            if (_combatManager != null)
            {
                _combatManager.Initialize(
                    _characterView,
                    _enemySpawner,
                    _damageNumberPool,
                    _hitEffectPool,
                    skills);
            }

            // HUD
            if (_playerHUD != null)
            {
                _playerHUD.Initialize(_characterView, _combatManager);
                _playerHUD.OnBackPressed = () =>
                {
                    if (_mapManager != null)
                        _mapManager.LeaveArea();
                    SceneManager.LoadScene("MainMenu");
                };
            }

            // Build enemy catalog from TestMaps
            var enemyCatalog = new Dictionary<string, EnemyData>();
            foreach (var enemy in TestMaps.AllEnemies)
            {
                enemyCatalog[enemy.ID] = enemy;
            }

            // Map manager
            if (_mapManager != null)
            {
                _mapManager.Initialize(_enemySpawner, _combatManager, _characterView, enemyCatalog);

                // Wire announcer
                if (_waveAnnouncer != null)
                {
                    _mapManager.OnAreaAnnouncement += _waveAnnouncer.ShowAnnouncement;
                }

                // Wire run summary
                if (_runSummary != null)
                {
                    _runSummary.Initialize();
                    _mapManager.OnAreaSessionEnd += _runSummary.Show;
                    _runSummary.OnContinue += OnAreaContinue;
                }

                // Wire enemy death from CombatManager to MapManager
                if (_combatManager != null)
                {
                    _combatManager.OnEnemyKilled += _mapManager.OnEnemyDied;
                }

                // Wire loot visuals
                if (_lootVisualManager != null)
                {
                    _mapManager.OnGoldDropped += _lootVisualManager.SpawnGoldCoin;
                    _mapManager.OnEquipmentDropped += _lootVisualManager.SpawnEquipmentDrop;
                    _lootVisualManager.OnEquipmentCollected += _mapManager.CollectItem;
                }

                // Enter the test area
                var maps = TestMaps.AllMaps;
                int mapIdx = Mathf.Clamp(_testMapIndex, 0, maps.Length - 1);
                var map = maps[mapIdx];
                int areaIdx = Mathf.Clamp(_testAreaIndex, 0, map.Areas.Length - 1);
                _mapManager.EnterArea(map, map.Areas[areaIdx]);
            }

            // Wire session ending to auto-collect remaining loot before result is calculated
            if (_mapManager != null && _lootVisualManager != null)
            {
                _mapManager.OnAreaSessionEnding += OnAutoCollectLoot;
            }

            // Save system
            _saveManager = SaveSystemBridge.GetOrCreate();
            if (_autoSave && _mapManager != null)
            {
                _mapManager.OnAreaSessionEnd += OnAutoSave;
            }
        }

        private void OnAutoCollectLoot()
        {
            if (_lootVisualManager != null)
                _lootVisualManager.ReturnAllDrops();
        }

        private void OnAutoSave(AreaResult result)
        {
            if (_saveManager == null) return;

            // Load existing save to preserve inventory, or create default
            var saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();

            // Update character state
            saveData.Version = 2;
            saveData.SelectedClass = _testClass;
            saveData.CharacterLevel = _characterView.State != null ? _characterView.State.Level : 1;
            saveData.CharacterXP = _characterView.State != null ? _characterView.State.XP : 0;
            saveData.LastAreaID = result.AreaID;

            // Add earned gold to existing total
            saveData.Gold += result.GoldEarned;

            // Add collected items to bag
            if (result.ItemsDropped != null && result.ItemsDropped.Length > 0)
            {
                var existingBag = saveData.BagItems ?? System.Array.Empty<SerializedBagItem>();
                var newBag = new SerializedBagItem[existingBag.Length + result.ItemsDropped.Length];
                System.Array.Copy(existingBag, newBag, existingBag.Length);

                for (int i = 0; i < result.ItemsDropped.Length; i++)
                {
                    newBag[existingBag.Length + i] = SerializedBagItem.FromEquipment(new SerializedEquipment
                    {
                        DataID = result.ItemsDropped[i],
                        UpgradeLevel = 0,
                        Gems = System.Array.Empty<SerializedGem>()
                    });
                }

                saveData.BagItems = newBag;
            }

            _saveManager.SaveGame(saveData);
            Debug.Log($"[SaveSystem] Auto-saved after area session. Level={saveData.CharacterLevel}, Gold={saveData.Gold}, Bag={saveData.BagItems.Length} items");
        }

        private void OnAreaContinue()
        {
            // Re-enter the same area for testing
            var maps = TestMaps.AllMaps;
            int mapIdx = Mathf.Clamp(_testMapIndex, 0, maps.Length - 1);
            var map = maps[mapIdx];
            int areaIdx = Mathf.Clamp(_testAreaIndex, 0, map.Areas.Length - 1);
            _mapManager.EnterArea(map, map.Areas[areaIdx]);
        }

        private void OnDestroy()
        {
            if (_mapManager != null)
            {
                if (_waveAnnouncer != null)
                    _mapManager.OnAreaAnnouncement -= _waveAnnouncer.ShowAnnouncement;
                if (_runSummary != null)
                    _mapManager.OnAreaSessionEnd -= _runSummary.Show;
                if (_autoSave)
                    _mapManager.OnAreaSessionEnd -= OnAutoSave;
                if (_combatManager != null)
                    _combatManager.OnEnemyKilled -= _mapManager.OnEnemyDied;
                if (_lootVisualManager != null)
                {
                    _mapManager.OnGoldDropped -= _lootVisualManager.SpawnGoldCoin;
                    _mapManager.OnEquipmentDropped -= _lootVisualManager.SpawnEquipmentDrop;
                    _lootVisualManager.OnEquipmentCollected -= _mapManager.CollectItem;
                    _mapManager.OnAreaSessionEnding -= OnAutoCollectLoot;
                }
            }
            if (_runSummary != null)
                _runSummary.OnContinue -= OnAreaContinue;
        }
    }
}
