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
        private readonly System.Collections.Generic.HashSet<string> _activeSubScenes = new();

        // Incremental save tracking — delta counters reset each area session
        private int _savedGoldFromSession;
        private int _savedCoinsFromSession;
        private int _savedItemCountFromSession;
        private readonly List<string> _collectedItemsThisSession = new();


        public AudioManager AudioManager => _audioManager;

        private void Start()
        {
            Application.targetFrameRate = 60;
            Debug.Log("[GameplaySetup] Start() begin");

            // Load sprite atlas for animations
            SpriteAtlasLoader.EnsureLoaded();
            Debug.Log($"[GameplaySetup] Atlas loaded, sprite count: {SpriteAtlasLoader.SpriteCount}");

            // Initialize player
            _characterView.Initialize(_testClass);
            Debug.Log("[GameplaySetup] Player initialized");

            _saveManager = SaveSystemBridge.GetOrCreate();

            // Load saved progression
            _metaState = new MetaProgressionState();
            var existingSave = _saveManager.LoadGame();
            if (existingSave != null)
            {
                // Restore character level, XP, and stat allocations
                var state = _characterView.State;
                if (existingSave.CharacterLevel > 1)
                {
                    state.Level = existingSave.CharacterLevel;
                    state.StatPointsAvailable = existingSave.StatPointsAvailable;
                    state.Vitality = existingSave.Vitality;
                    state.Mana = existingSave.Mana;
                    state.Strength = existingSave.Strength;
                    state.Agility = existingSave.Agility;
                    state.Spirit = existingSave.Spirit;

                    // Recompute stats at restored level
                    var computed = state.ComputeStats();
                    state.CurrentHP = computed.HP;
                    state.CurrentMP = computed.MP;
                }
                state.XP = existingSave.CharacterXP;

                // Restore meta progression
                if (existingSave.MetaUpgradeLevels != null)
                {
                    _metaState.MetaCurrency = existingSave.MetaCurrency;
                    int count = System.Math.Min(existingSave.MetaUpgradeLevels.Length, MetaProgressionState.UpgradeTypeCount);
                    for (int i = 0; i < count; i++)
                        _metaState.UpgradeLevels[i] = existingSave.MetaUpgradeLevels[i];
                }
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
                _playerHUD.OnEquipmentPressed = () => ToggleSubScene("Equipment");
                _playerHUD.OnInventoryPressed = () => ToggleSubScene("Inventory");
                _playerHUD.OnMinePressed = () => ToggleSubScene("Mining");
                _playerHUD.OnMarketPressed = () => ToggleSubScene("Market");
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

                // Wire revive timer to HUD
                if (_playerHUD != null)
                {
                    _mapManager.OnReviveTimerTick += _playerHUD.ShowReviveTimer;
                    _mapManager.OnPlayerRevived += _playerHUD.HideReviveTimer;
                }

                // Enter the test area
                var maps = TestMaps.AllMaps;
                int mapIdx = Mathf.Clamp(_testMapIndex, 0, maps.Length - 1);
                var map = maps[mapIdx];
                int areaIdx = Mathf.Clamp(_testAreaIndex, 0, map.Areas.Length - 1);
                Debug.Log($"[GameplaySetup] Entering area: {map.Name} / {map.Areas[areaIdx].Name}");
                _mapManager.EnterArea(map, map.Areas[areaIdx]);
                Debug.Log("[GameplaySetup] Area entered successfully");
            }

            // Wire session ending to auto-collect remaining loot before result is calculated
            if (_mapManager != null && _lootVisualManager != null)
            {
                _mapManager.OnAreaSessionEnding += OnAutoCollectLoot;
            }

            // Save system — incremental saves on every enemy kill and item pickup
            if (_saveManager == null) _saveManager = SaveSystemBridge.GetOrCreate();
            if (_autoSave && _mapManager != null)
            {
                _mapManager.OnEnemyKilledInArea += OnEnemyKilledSave;
                _mapManager.OnItemDropped += OnItemCollectedSave;
                _mapManager.OnPlayerRevived += OnPlayerRevivedSave;
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

            // Listen for sub-scene unloads
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void ToggleSubScene(string sceneName)
        {
            if (_activeSubScenes.Contains(sceneName))
            {
                // Close: unload the scene
                _activeSubScenes.Remove(sceneName);
                SceneManager.UnloadSceneAsync(sceneName);
                if (_playerHUD != null) _playerHUD.SetNavIconState(sceneName, false);
                return;
            }

            // Open: load additively
            _activeSubScenes.Add(sceneName);
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            op.completed += _ => OnSubSceneLoaded(sceneName);
            if (_playerHUD != null) _playerHUD.SetNavIconState(sceneName, true);
        }

        private void OnSubSceneLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid()) return;

            foreach (var root in scene.GetRootGameObjects())
            {
                // Destroy duplicate EventSystem (Gameplay scene already has one)
                var es = root.GetComponent<UnityEngine.EventSystems.EventSystem>();
                if (es != null) { Object.Destroy(root); continue; }

                // Destroy duplicate Camera
                var cam = root.GetComponent<UnityEngine.Camera>();
                if (cam != null) { Object.Destroy(root); continue; }
            }
        }

        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
        {
            _activeSubScenes.Remove(scene.name);
            if (_playerHUD != null) _playerHUD.SetNavIconState(scene.name, false);

            // Reload stat allocations from save when Equipment scene closes
            if (scene.name == "Equipment")
                ReloadStatsFromSave();
        }

        private void ReloadStatsFromSave()
        {
            var save = _saveManager.LoadGame();
            if (save == null || _characterView.State == null) return;

            var state = _characterView.State;
            state.Vitality = save.Vitality;
            state.Mana = save.Mana;
            state.Strength = save.Strength;
            state.Agility = save.Agility;
            state.Spirit = save.Spirit;
            state.StatPointsAvailable = save.StatPointsAvailable;

            // Recompute HP/MP with new stats
            var computed = state.ComputeStats();
            state.CurrentHP = computed.HP;
            state.CurrentMP = computed.MP;
        }

        private void OnAutoCollectLoot()
        {
            if (_lootVisualManager != null)
                _lootVisualManager.ReturnAllDrops();
        }

        private void OnEnemyKilledSave(int enemiesKilled)
        {
            IncrementalSave();
        }

        private void OnItemCollectedSave(string itemID, int quantity)
        {
            for (int i = 0; i < quantity; i++)
                _collectedItemsThisSession.Add(itemID);
            IncrementalSave();
        }

        private void IncrementalSave()
        {
            if (_saveManager == null || _mapManager?.AreaState == null) return;

            var saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();

            // Character state
            saveData.Version = 2;
            saveData.SelectedClass = _testClass;
            if (_characterView.State != null)
            {
                saveData.CharacterLevel = _characterView.State.Level;
                saveData.CharacterXP = _characterView.State.XP;
                // Save stat points earned from level-ups during combat.
                // Stat allocations (Vitality, etc.) are saved by EquipmentController only.
                saveData.StatPointsAvailable = _characterView.State.StatPointsAvailable;
            }

            // Gold delta
            int currentGold = _mapManager.AreaState.TotalGoldEarned;
            int goldDelta = currentGold - _savedGoldFromSession;
            if (goldDelta > 0)
            {
                saveData.Gold += goldDelta;
                _savedGoldFromSession = currentGold;
            }

            // Chronicle Coins delta
            int currentCoins = _mapManager.AreaState.EnemiesKilled / 5;
            int coinsDelta = currentCoins - _savedCoinsFromSession;
            if (coinsDelta > 0)
            {
                saveData.MetaCurrency += coinsDelta;
                _savedCoinsFromSession = currentCoins;
            }

            // New items delta
            int newItemCount = _collectedItemsThisSession.Count - _savedItemCountFromSession;
            if (newItemCount > 0)
            {
                var existingBag = saveData.BagItems ?? System.Array.Empty<SerializedBagItem>();
                var newBag = new SerializedBagItem[existingBag.Length + newItemCount];
                System.Array.Copy(existingBag, newBag, existingBag.Length);
                for (int i = 0; i < newItemCount; i++)
                {
                    newBag[existingBag.Length + i] = SerializedBagItem.FromEquipment(new SerializedEquipment
                    {
                        DataID = _collectedItemsThisSession[_savedItemCountFromSession + i],
                        UpgradeLevel = 0,
                        Gems = System.Array.Empty<SerializedGem>()
                    });
                }
                saveData.BagItems = newBag;
                _savedItemCountFromSession = _collectedItemsThisSession.Count;
            }

            _saveManager.SaveGame(saveData);
        }

        private void OnPlayerRevivedSave()
        {
            if (_saveManager == null) return;

            // Apply death penalty — lose 15-25% gold and 1-3 bag items
            var saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();
            var rng = new System.Random(System.Environment.TickCount);
            float lossPercent = 0.15f + (float)rng.NextDouble() * 0.10f;
            int goldLost = (int)(saveData.Gold * lossPercent);
            saveData.Gold = Mathf.Max(0, saveData.Gold - goldLost);

            if (saveData.BagItems != null && saveData.BagItems.Length > 0)
            {
                int itemsToLose = Mathf.Min(rng.Next(1, 4), saveData.BagItems.Length);
                var bagList = new List<SerializedBagItem>(saveData.BagItems);
                for (int i = 0; i < itemsToLose; i++)
                {
                    int removeIdx = rng.Next(0, bagList.Count);
                    bagList.RemoveAt(removeIdx);
                }
                saveData.BagItems = bagList.ToArray();
            }

            _saveManager.SaveGame(saveData);
            Debug.Log($"[DeathPenalty] Lost {goldLost} gold.");

            // Reset session counters for the new area session
            _savedGoldFromSession = 0;
            _savedCoinsFromSession = 0;
            _savedItemCountFromSession = 0;
            _collectedItemsThisSession.Clear();
        }

        private void OnAreaContinue()
        {
            // Re-enter the same area
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
                {
                    _mapManager.OnEnemyKilledInArea -= OnEnemyKilledSave;
                    _mapManager.OnItemDropped -= OnItemCollectedSave;
                    _mapManager.OnPlayerRevived -= OnPlayerRevivedSave;
                }
                if (_combatManager != null)
                    _combatManager.OnEnemyKilled -= _mapManager.OnEnemyDied;
                if (_lootVisualManager != null)
                {
                    _mapManager.OnGoldDropped -= _lootVisualManager.SpawnGoldCoin;
                    _mapManager.OnEquipmentDropped -= _lootVisualManager.SpawnEquipmentDrop;
                    _lootVisualManager.OnEquipmentCollected -= _mapManager.CollectItem;
                    _mapManager.OnAreaSessionEnding -= OnAutoCollectLoot;
                }
                if (_playerHUD != null)
                {
                    _mapManager.OnReviveTimerTick -= _playerHUD.ShowReviveTimer;
                    _mapManager.OnPlayerRevived -= _playerHUD.HideReviveTimer;
                }
            }
            if (_runSummary != null)
                _runSummary.OnContinue -= OnAreaContinue;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
    }
}
