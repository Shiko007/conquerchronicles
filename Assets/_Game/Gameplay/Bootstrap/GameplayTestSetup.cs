using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Core.Map;
using ConquerChronicles.Core.MetaProgression;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Loot;
using ConquerChronicles.Gameplay.Map;
using ConquerChronicles.Gameplay.Save;
using ConquerChronicles.Gameplay.Stage;
using ConquerChronicles.Gameplay.Audio;
using ConquerChronicles.Gameplay.UI.HUD;
using ConquerChronicles.Gameplay.UI.Tutorial;
using ConquerChronicles.Gameplay.Animation;
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

        [Header("Audio")]
        [SerializeField] private AudioManager _audioManager;

        [Header("Tutorial")]
        [SerializeField] private TutorialOverlay _tutorialOverlay;

        [Header("Settings")]
        [SerializeField] private CharacterClass _testClass = CharacterClass.Trojan;
        [SerializeField] private int _testMapIndex = 0;
        [SerializeField] private int _testAreaIndex = 0;
        [SerializeField] private bool _autoSave = true;

        private SaveManager _saveManager;
        private MetaProgressionState _metaState;
        private bool _leavingToMenu;
        private bool _playerDied;

        public AudioManager AudioManager => _audioManager;

        private void Start()
        {
            Application.targetFrameRate = 60;

            // Load sprite atlas for animations
            SpriteAtlasLoader.EnsureLoaded();

            // Initialize player
            _characterView.Initialize(_testClass);

            // Check hero recovery cooldown
            _saveManager = SaveSystemBridge.GetOrCreate();
            var checkSave = _saveManager.LoadGame();
            if (checkSave != null && checkSave.HeroRecoveryTimestamp > 0)
            {
                long nowTicks = System.DateTimeOffset.UtcNow.Ticks;
                if (nowTicks < checkSave.HeroRecoveryTimestamp)
                {
                    var remaining = System.TimeSpan.FromTicks(checkSave.HeroRecoveryTimestamp - nowTicks);
                    Debug.Log($"[Recovery] Hero is recovering! {remaining.Minutes}m {remaining.Seconds}s remaining.");
                    SceneManager.LoadScene("MainMenu");
                    return;
                }
                else
                {
                    // Recovery complete — clear the timestamp
                    checkSave.HeroRecoveryTimestamp = 0;
                    _saveManager.SaveGame(checkSave);
                }
            }

            // Load meta progression
            _metaState = new MetaProgressionState();
            var existingSave = _saveManager.LoadGame();
            if (existingSave != null && existingSave.MetaUpgradeLevels != null)
            {
                _metaState.MetaCurrency = existingSave.MetaCurrency;
                int count = System.Math.Min(existingSave.MetaUpgradeLevels.Length, MetaProgressionState.UpgradeTypeCount);
                for (int i = 0; i < count; i++)
                    _metaState.UpgradeLevels[i] = existingSave.MetaUpgradeLevels[i];
            }

            // Apply meta stat bonuses to character
            _characterView.SetMetaBonus(_metaState.GetBonusStats());

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
                    skills,
                    _audioManager);
            }

            // HUD
            if (_playerHUD != null)
            {
                _playerHUD.Initialize(_characterView, _combatManager);
                _playerHUD.OnBackPressed = () =>
                {
                    _leavingToMenu = true;
                    if (_mapManager != null)
                        _mapManager.LeaveArea();
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
                _mapManager.SetMetaMultipliers(
                    _metaState.GetGoldMultiplier(),
                    _metaState.GetXPMultiplier(),
                    _metaState.GetDropRateBonus());

                if (_audioManager != null)
                {
                    _mapManager.SetAudioManager(_audioManager);

                    // Start combat music
                    if (_audioManager.Library != null && _audioManager.Library.CombatMusic != null)
                        _audioManager.PlayMusic(_audioManager.Library.CombatMusic);
                }

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
            if (_saveManager == null) _saveManager = SaveSystemBridge.GetOrCreate();
            if (_autoSave && _mapManager != null)
            {
                _mapManager.OnAreaSessionEnd += OnAutoSave;
            }

            // Show tutorial on first run
            if (_tutorialOverlay != null)
            {
                var tutSave = _saveManager.LoadGame();
                if (tutSave == null || !tutSave.TutorialComplete)
                {
                    _tutorialOverlay.Show(TutorialSequences.Gameplay, () =>
                    {
                        var s = _saveManager.LoadGame() ?? SaveData.CreateDefault();
                        s.TutorialComplete = true;
                        _saveManager.SaveGame(s);
                    });
                }
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

            // Add earned Chronicle Coins
            saveData.MetaCurrency += result.ChronicleCoinsEarned;

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

            // Death penalty — lose gold and items
            if (result.PlayerDied)
            {
                var rng = new System.Random(System.Environment.TickCount);
                float lossPercent = 0.15f + (float)rng.NextDouble() * 0.10f; // 15-25%
                int goldLost = (int)(saveData.Gold * lossPercent);
                saveData.Gold = Mathf.Max(0, saveData.Gold - goldLost);

                // Lose 1-3 random bag items
                if (saveData.BagItems != null && saveData.BagItems.Length > 0)
                {
                    int itemsToLose = Mathf.Min(rng.Next(1, 4), saveData.BagItems.Length);
                    var bagList = new System.Collections.Generic.List<SerializedBagItem>(saveData.BagItems);
                    for (int i = 0; i < itemsToLose; i++)
                    {
                        int removeIdx = rng.Next(0, bagList.Count);
                        bagList.RemoveAt(removeIdx);
                    }
                    saveData.BagItems = bagList.ToArray();
                }

                // Set recovery cooldown — 2 minutes from now
                saveData.HeroRecoveryTimestamp = System.DateTimeOffset.UtcNow.AddMinutes(2).Ticks;
                Debug.Log($"[DeathPenalty] Lost {goldLost} gold, recovery until {System.DateTimeOffset.UtcNow.AddMinutes(2):HH:mm:ss}");
            }

            _playerDied = result.PlayerDied;

            _saveManager.SaveGame(saveData);
            Debug.Log($"[SaveSystem] Auto-saved after area session. Level={saveData.CharacterLevel}, Gold={saveData.Gold}, Bag={saveData.BagItems.Length} items");
        }

        private void OnAreaContinue()
        {
            if (_leavingToMenu || _playerDied)
            {
                SceneManager.LoadScene("MainMenu");
                return;
            }

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
