using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Audio;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.Equipment
{
    public class EquipmentController : MonoBehaviour
    {
        [SerializeField] private EquipmentSceneUI _equipmentUI;

        private InventoryState _inventory;
        private SaveManager _saveManager;
        private SaveData _saveData;
        private int _playerLevel;
        private CharacterClass _playerClass;
        private Dictionary<string, EquipmentData> _equipmentCatalog;
        private AudioManager _audioManager;
        private EquipmentInstance _selectedItem;  // non-null when an equipped item is selected
        private int _selectedSlotIndex = -1;
        private bool _selectedIsEquipped;
        private CharacterStats _baseStats;
        private CharacterStats _growth;

        // Pending stat allocation (not yet confirmed)
        private bool _hasPendingStats;
        private int _pendingVitality, _pendingStrength, _pendingAgility, _pendingSpirit;
        private int _pendingStatPoints;

        // Track last known stat points to detect level-up gains
        private int _lastKnownStatPoints;

        public void SetAudioManager(AudioManager audioManager) { _audioManager = audioManager; }

        private void Update()
        {
            // Check if stat points increased from level-ups during combat
            var latestSave = _saveManager.LoadGame();
            if (latestSave == null) return;

            int currentPoints = latestSave.StatPointsAvailable;
            if (currentPoints > _lastKnownStatPoints)
            {
                int gained = currentPoints - _lastKnownStatPoints;
                _lastKnownStatPoints = currentPoints;
                _saveData.StatPointsAvailable = currentPoints;

                if (_hasPendingStats)
                    _pendingStatPoints += gained;

                RefreshAll();
            }
        }

        private void Start()
        {
            // Get save manager and load save data
            _saveManager = SaveSystemBridge.GetOrCreate();
            _saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();

            _playerLevel = _saveData.CharacterLevel;
            _playerClass = _saveData.SelectedClass;
            _lastKnownStatPoints = _saveData.StatPointsAvailable;
            _baseStats = LevelUpTable.GetClassBaseStats(_playerClass);
            _growth = LevelUpTable.GetClassGrowthPerLevel(_playerClass);

            // Build equipment catalog from TestEquipment
            _equipmentCatalog = new Dictionary<string, EquipmentData>();
            var allEquipment = TestEquipment.GetAll();
            for (int i = 0; i < allEquipment.Length; i++)
            {
                _equipmentCatalog[allEquipment[i].ID] = allEquipment[i];
            }

            // Reconstruct InventoryState from save data
            _inventory = new InventoryState();
            _inventory.Gold = _saveData.Gold;

            // Restore equipped items
            if (_saveData.EquippedItems != null)
            {
                for (int i = 0; i < _saveData.EquippedItems.Length && i < InventoryState.EquipmentSlotCount; i++)
                {
                    var serialized = _saveData.EquippedItems[i];
                    if (serialized.IsEmpty) continue;

                    if (_equipmentCatalog.TryGetValue(serialized.DataID, out var data))
                    {
                        var instance = new EquipmentInstance(data);
                        instance.UpgradeLevel = serialized.UpgradeLevel;

                        // Restore socketed gems
                        if (serialized.Gems != null)
                        {
                            for (int g = 0; g < serialized.Gems.Length && g < instance.SocketedGems.Length; g++)
                            {
                                if (serialized.Gems[g].Tier > 0)
                                {
                                    var gem = new GemData((GemType)serialized.Gems[g].Type, serialized.Gems[g].Tier);
                                    instance.Socket(g, gem);
                                }
                            }
                        }

                        _inventory.EquippedItems[i] = instance;
                    }
                }
            }

            // Restore bag items
            if (_saveData.BagItems != null)
            {
                for (int i = 0; i < _saveData.BagItems.Length; i++)
                {
                    var bagItem = _saveData.BagItems[i];
                    if (bagItem.ItemType == 0 && !bagItem.Equipment.IsEmpty)
                    {
                        if (_equipmentCatalog.TryGetValue(bagItem.Equipment.DataID, out var eqData))
                        {
                            var instance = new EquipmentInstance(eqData);
                            instance.UpgradeLevel = bagItem.Equipment.UpgradeLevel;
                            if (bagItem.Equipment.Gems != null)
                            {
                                for (int g = 0; g < bagItem.Equipment.Gems.Length && g < instance.SocketedGems.Length; g++)
                                {
                                    if (bagItem.Equipment.Gems[g].Tier > 0)
                                    {
                                        var gem = new GemData((GemType)bagItem.Equipment.Gems[g].Type, bagItem.Equipment.Gems[g].Tier);
                                        instance.Socket(g, gem);
                                    }
                                }
                            }
                            _inventory.AddToBag(instance);
                        }
                    }
                    else if (bagItem.ItemType == 1 && bagItem.Gem.Tier > 0)
                    {
                        var gem = new GemData((GemType)bagItem.Gem.Type, bagItem.Gem.Tier);
                        _inventory.AddGem(gem);
                    }
                }
            }

            // Wire UI events
            _equipmentUI.OnBackPressed = () => SceneManager.UnloadSceneAsync("Equipment");

            _equipmentUI.OnSlotPressed = (slotIndex) =>
            {
                var equippedItem = _inventory.EquippedItems[slotIndex];
                if (equippedItem != null)
                {
                    _selectedItem = equippedItem;
                    _selectedSlotIndex = slotIndex;
                    _selectedIsEquipped = true;
                    _equipmentUI.ShowItemDetail(equippedItem, false, true);
                }
            };

            _equipmentUI.OnEquipPressed = () =>
            {
                if (_selectedItem == null) return;

                if (_selectedIsEquipped)
                {
                    // Unequip — check bag space first
                    var slot = (EquipmentSlot)_selectedSlotIndex;
                    if (_inventory.IsBagFull)
                    {
                        Debug.Log("[Equipment] Cannot unequip — inventory is full!");
                        return;
                    }
                    _inventory.Unequip(slot);
                    _equipmentUI.HideItemDetail();
                    _selectedItem = null;
                    _selectedSlotIndex = -1;
                }
                else
                {
                    // Equip from bag (only equipment can be equipped)
                    var slot = _selectedItem.Data.Slot;
                    if (_inventory.CanEquip(_selectedItem, _playerLevel, _playerClass))
                    {
                        _inventory.Equip(_selectedItem, slot);
                        _equipmentUI.HideItemDetail();
                        _selectedItem = null;
                        _selectedSlotIndex = -1;
                    }
                }

                RefreshAll();
                SaveInventory();
            };

            _equipmentUI.OnUpgradePressed = () =>
            {
                if (_selectedItem == null) return;
                if (_selectedItem.UpgradeLevel >= 12) return;

                int seed = System.Environment.TickCount;
                var result = UpgradeCalculator.TryUpgrade(_selectedItem, seed);

                if (result.Destroyed)
                {
                    // Remove the destroyed item
                    if (_selectedIsEquipped)
                    {
                        _inventory.EquippedItems[_selectedSlotIndex] = null;
                    }
                    else
                    {
                        _inventory.RemoveFromBag(_selectedItem);
                    }

                    _equipmentUI.HideItemDetail();
                    _selectedItem = null;
                    _selectedSlotIndex = -1;
                    Debug.Log("[Equipment] Item destroyed during upgrade!");
                    if (_audioManager?.Library != null) _audioManager.PlaySFX(_audioManager.Library.UpgradeDestroy);
                }
                else if (result.Success)
                {
                    Debug.Log($"[Equipment] Upgrade success! Now +{result.NewLevel}");
                    bool canEquip = !_selectedIsEquipped && _inventory.CanEquip(_selectedItem, _playerLevel, _playerClass);
                    _equipmentUI.ShowItemDetail(_selectedItem, canEquip, _selectedIsEquipped);
                    if (_audioManager?.Library != null) _audioManager.PlaySFX(_audioManager.Library.UpgradeSuccess);
                }
                else
                {
                    Debug.Log($"[Equipment] Upgrade failed. Level is now +{result.NewLevel}");
                    bool canEquip = !_selectedIsEquipped && _inventory.CanEquip(_selectedItem, _playerLevel, _playerClass);
                    _equipmentUI.ShowItemDetail(_selectedItem, canEquip, _selectedIsEquipped);
                    if (_audioManager?.Library != null) _audioManager.PlaySFX(_audioManager.Library.UpgradeFail);
                }

                RefreshAll();
                SaveInventory();
            };

            _equipmentUI.OnCloseDetailPressed = () =>
            {
                _equipmentUI.HideItemDetail();
                _selectedItem = null;
                _selectedSlotIndex = -1;
            };

            _equipmentUI.OnAllocateStat = (statName) =>
            {
                // Initialize pending from saved values on first press
                if (!_hasPendingStats)
                {
                    _pendingVitality = _saveData.Vitality;
                    _pendingStrength = _saveData.Strength;
                    _pendingAgility = _saveData.Agility;
                    _pendingSpirit = _saveData.Spirit;
                    _pendingStatPoints = _saveData.StatPointsAvailable;
                }

                if (_pendingStatPoints <= 0) return;

                switch (statName)
                {
                    case "Vitality":  _pendingVitality++; break;
                    case "Strength":  _pendingStrength++; break;
                    case "Agility":   _pendingAgility++; break;
                    case "Spirit":    _pendingSpirit++; break;
                    default: return;
                }

                _pendingStatPoints--;
                _hasPendingStats = true;
                _equipmentUI.ShowConfirmButton();
                RefreshAll();
            };

            _equipmentUI.OnDeallocateStat = (statName) =>
            {
                if (!_hasPendingStats) return;

                // Only allow reverting points added this session (not saved ones)
                switch (statName)
                {
                    case "Vitality":
                        if (_pendingVitality <= _saveData.Vitality) return;
                        _pendingVitality--; break;
                    case "Strength":
                        if (_pendingStrength <= _saveData.Strength) return;
                        _pendingStrength--; break;
                    case "Agility":
                        if (_pendingAgility <= _saveData.Agility) return;
                        _pendingAgility--; break;
                    case "Spirit":
                        if (_pendingSpirit <= _saveData.Spirit) return;
                        _pendingSpirit--; break;
                    default: return;
                }

                _pendingStatPoints++;

                // If all pending values match saved, no longer pending
                if (_pendingVitality == _saveData.Vitality &&
                    _pendingStrength == _saveData.Strength &&
                    _pendingAgility == _saveData.Agility &&
                    _pendingSpirit == _saveData.Spirit)
                {
                    _hasPendingStats = false;
                    _equipmentUI.HideConfirmButton();
                    _equipmentUI.HideAllMinusButtons();
                }

                RefreshAll();
            };

            _equipmentUI.OnConfirmStats = () =>
            {
                if (!_hasPendingStats) return;

                _saveData.Vitality = _pendingVitality;
                _saveData.Strength = _pendingStrength;
                _saveData.Agility = _pendingAgility;
                _saveData.Spirit = _pendingSpirit;
                _saveData.StatPointsAvailable = _pendingStatPoints;
                _saveManager.SaveGame(_saveData);

                _hasPendingStats = false;
                _equipmentUI.HideConfirmButton();
                _equipmentUI.HideAllMinusButtons();
                RefreshAll();
            };

            // Initialize UI and refresh
            _equipmentUI.Initialize();

            // Set character idle animation
            _equipmentUI.SetCharacterPreview("Male_Base_SIdle_");

            RefreshAll();
        }

        private void RefreshAll()
        {
            _equipmentUI.RefreshEquippedSlots(_inventory.EquippedItems);

            int vit = _hasPendingStats ? _pendingVitality : _saveData.Vitality;
            int str = _hasPendingStats ? _pendingStrength : _saveData.Strength;
            int agi = _hasPendingStats ? _pendingAgility : _saveData.Agility;
            int spi = _hasPendingStats ? _pendingSpirit : _saveData.Spirit;
            int pts = _hasPendingStats ? _pendingStatPoints : _saveData.StatPointsAvailable;

            var totalStats = ComputeTotalStats(vit, str, agi, spi);
            _equipmentUI.RefreshStats(_playerClass, _playerLevel, totalStats,
                pts, vit, str, agi, spi);

            // Show minus buttons only for stats that have pending increases
            if (_hasPendingStats)
            {
                _equipmentUI.ShowMinusButtons(
                    _pendingVitality > _saveData.Vitality,
                    _pendingStrength > _saveData.Strength,
                    _pendingAgility > _saveData.Agility,
                    _pendingSpirit > _saveData.Spirit);
            }
        }

        private CharacterStats ComputeTotalStats(int vitality, int strength, int agility, int spirit)
        {
            var stats = _baseStats + _growth * (_playerLevel - 1);

            // Allocated stat points
            stats.HP += vitality * 10;
            stats.MP += _saveData.Mana * 8;
            stats.ATK += strength * 3;
            stats.AGI += agility * 2;
            stats.CritRate += agility * 0.002f;
            stats.MATK += spirit * 3;

            // Equipment bonuses
            for (int i = 0; i < InventoryState.EquipmentSlotCount; i++)
            {
                var item = _inventory.EquippedItems[i];
                if (item != null)
                    stats = stats + item.ComputeStats();
            }

            return stats;
        }

        private void SaveInventory()
        {
            // Serialize equipped items
            _saveData.EquippedItems = new SerializedEquipment[InventoryState.EquipmentSlotCount];
            for (int i = 0; i < InventoryState.EquipmentSlotCount; i++)
            {
                var item = _inventory.EquippedItems[i];
                if (item != null)
                {
                    _saveData.EquippedItems[i] = SerializeEquipment(item);
                }
                else
                {
                    _saveData.EquippedItems[i] = new SerializedEquipment();
                }
            }

            // Serialize bag items
            var bagItems = new SerializedBagItem[_inventory.Bag.Count];
            for (int i = 0; i < _inventory.Bag.Count; i++)
            {
                var bagItem = _inventory.Bag[i];
                if (bagItem.Type == BagItemType.Equipment)
                {
                    bagItems[i] = SerializedBagItem.FromEquipment(SerializeEquipment(bagItem.Equipment));
                }
                else
                {
                    bagItems[i] = SerializedBagItem.FromGem(new SerializedGem
                    {
                        Type = (int)bagItem.Gem.Type,
                        Tier = bagItem.Gem.Tier
                    });
                }
            }
            _saveData.BagItems = bagItems;

            _saveData.Gold = _inventory.Gold;
            _saveManager.SaveGame(_saveData);
        }

        private SerializedEquipment SerializeEquipment(EquipmentInstance item)
        {
            var serialized = new SerializedEquipment
            {
                DataID = item.Data.ID,
                UpgradeLevel = item.UpgradeLevel
            };

            serialized.Gems = new SerializedGem[item.SocketedGems.Length];
            for (int g = 0; g < item.SocketedGems.Length; g++)
            {
                serialized.Gems[g] = new SerializedGem
                {
                    Type = (int)item.SocketedGems[g].Type,
                    Tier = item.SocketedGems[g].Tier
                };
            }

            return serialized;
        }
    }
}
