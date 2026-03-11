using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private InventorySceneUI _inventoryUI;

        private InventoryState _inventory;
        private SaveManager _saveManager;
        private SaveData _saveData;
        private int _playerLevel;
        private CharacterClass _playerClass;
        private int[] _unlockedClasses;
        private Dictionary<string, EquipmentData> _equipmentCatalog;
        private BagItem _selectedBagItem;
        private int _selectedBagIndex = -1;
        private int _lastKnownBagCount;
        private int _lastKnownGold;

        private void Update()
        {
            var latestSave = _saveManager.LoadGame();
            if (latestSave == null) return;

            int currentBagCount = latestSave.BagItems != null ? latestSave.BagItems.Length : 0;
            int currentGold = latestSave.Gold;

            if (currentBagCount != _lastKnownBagCount || currentGold != _lastKnownGold)
            {
                // Reload inventory from save
                _saveData = latestSave;
                _inventory = new InventoryState();
                _inventory.Gold = _saveData.Gold;

                RestoreEquippedItems();
                RestoreBagItems();

                _lastKnownBagCount = currentBagCount;
                _lastKnownGold = currentGold;

                RefreshAll();
            }
        }

        private void Start()
        {
            if (_inventoryUI == null) return;

            _saveManager = SaveSystemBridge.GetOrCreate();
            _saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();

            _playerLevel = _saveData.CharacterLevel;
            _playerClass = _saveData.SelectedClass;
            _unlockedClasses = _saveData.UnlockedRebirthClasses ?? new[] { (int)CharacterClass.Trojan };

            // Build equipment catalog
            _equipmentCatalog = new Dictionary<string, EquipmentData>();
            var allEquipment = TestEquipment.GetAll();
            for (int i = 0; i < allEquipment.Length; i++)
                _equipmentCatalog[allEquipment[i].ID] = allEquipment[i];

            // Reconstruct InventoryState from save data
            _inventory = new InventoryState();
            _inventory.Gold = _saveData.Gold;

            RestoreEquippedItems();
            RestoreBagItems();

            _lastKnownBagCount = _saveData.BagItems != null ? _saveData.BagItems.Length : 0;
            _lastKnownGold = _saveData.Gold;

            // Wire UI
            _inventoryUI.Initialize();

            _inventoryUI.OnBackPressed = () =>
            {
                SaveInventory();
                SceneManager.UnloadSceneAsync("Inventory");
            };

            _inventoryUI.OnBagItemPressed = (bagIndex) =>
            {
                if (bagIndex < 0 || bagIndex >= _inventory.Bag.Count) return;

                var bagItem = _inventory.Bag[bagIndex];
                _selectedBagItem = bagItem;
                _selectedBagIndex = bagIndex;

                bool canEquip = false;
                if (bagItem.Type == BagItemType.Equipment)
                    canEquip = _inventory.CanEquip(bagItem.Equipment, _playerLevel, _unlockedClasses);

                _inventoryUI.ShowItemDetail(bagItem, canEquip);
            };

            _inventoryUI.OnEquipPressed = () =>
            {
                if (_selectedBagItem == null || _selectedBagItem.Type != BagItemType.Equipment) return;

                var item = _selectedBagItem.Equipment;
                if (!_inventory.CanEquip(item, _playerLevel, _unlockedClasses)) return;

                _inventory.Equip(item, item.Data.Slot);
                _inventoryUI.HideItemDetail();
                _selectedBagItem = null;
                _selectedBagIndex = -1;

                RefreshAll();
                SaveInventory();
            };

            _inventoryUI.OnDropPressed = () =>
            {
                if (_selectedBagItem == null || _selectedBagIndex < 0) return;
                if (IsValuableItem(_selectedBagItem))
                {
                    _inventoryUI.ShowConfirmDialog($"Drop {GetItemDisplayName(_selectedBagItem)}?", () =>
                    {
                        DropSingleItem();
                    });
                    return;
                }
                DropSingleItem();
            };

            _inventoryUI.OnBagItemLongPressed = (index) =>
            {
                if (index < 0 || index >= _inventory.Bag.Count) return;
                _inventoryUI.HideItemDetail();
                _selectedBagItem = null;
                _selectedBagIndex = -1;
                _inventoryUI.EnterSelectMode(index);
            };

            _inventoryUI.OnDropSelectedPressed = () =>
            {
                var indices = _inventoryUI.GetSelectedIndices();
                bool hasValuable = false;
                foreach (int idx in indices)
                {
                    if (idx >= 0 && idx < _inventory.Bag.Count && IsValuableItem(_inventory.Bag[idx]))
                    {
                        hasValuable = true;
                        break;
                    }
                }

                if (hasValuable)
                {
                    _inventoryUI.ShowConfirmDialog($"Drop {indices.Count} items including rare/elite items?", () =>
                    {
                        DropSelectedItems(indices);
                    });
                    return;
                }
                DropSelectedItems(indices);
            };

            _inventoryUI.OnCancelSelectPressed = () =>
            {
                _inventoryUI.ExitSelectMode();
                RefreshAll();
            };

            _inventoryUI.OnCloseDetailPressed = () =>
            {
                _inventoryUI.HideItemDetail();
                _selectedBagItem = null;
                _selectedBagIndex = -1;
            };

            RefreshAll();
        }

        private void RestoreEquippedItems()
        {
            if (_saveData.EquippedItems == null) return;
            for (int i = 0; i < _saveData.EquippedItems.Length && i < InventoryState.EquipmentSlotCount; i++)
            {
                var serialized = _saveData.EquippedItems[i];
                if (serialized.IsEmpty) continue;

                if (_equipmentCatalog.TryGetValue(serialized.DataID, out var data))
                {
                    var instance = new EquipmentInstance(data);
                    instance.UpgradeLevel = serialized.UpgradeLevel;

                    if (serialized.Gems != null)
                    {
                        for (int g = 0; g < serialized.Gems.Length && g < instance.SocketedGems.Length; g++)
                        {
                            if (serialized.Gems[g].Tier > 0)
                                instance.Socket(g, new GemData((GemType)serialized.Gems[g].Type, serialized.Gems[g].Tier));
                        }
                    }

                    _inventory.EquippedItems[i] = instance;
                }
            }
        }

        private void RestoreBagItems()
        {
            if (_saveData.BagItems == null) return;
            for (int i = 0; i < _saveData.BagItems.Length; i++)
            {
                var bagItem = _saveData.BagItems[i];
                if (bagItem.ItemType == 0)
                {
                    var serialized = bagItem.Equipment;
                    if (serialized.IsEmpty) continue;

                    if (_equipmentCatalog.TryGetValue(serialized.DataID, out var data))
                    {
                        var instance = new EquipmentInstance(data);
                        instance.UpgradeLevel = serialized.UpgradeLevel;

                        if (serialized.Gems != null)
                        {
                            for (int g = 0; g < serialized.Gems.Length && g < instance.SocketedGems.Length; g++)
                            {
                                if (serialized.Gems[g].Tier > 0)
                                    instance.Socket(g, new GemData((GemType)serialized.Gems[g].Type, serialized.Gems[g].Tier));
                            }
                        }

                        _inventory.AddToBag(instance);
                    }
                }
                else if (bagItem.ItemType == 1)
                {
                    if (bagItem.Gem.Tier > 0)
                        _inventory.AddGem(new GemData((GemType)bagItem.Gem.Type, bagItem.Gem.Tier));
                }
                else if (bagItem.ItemType == 2)
                {
                    if (!string.IsNullOrEmpty(bagItem.MaterialID))
                        _inventory.AddMaterial(bagItem.MaterialID, bagItem.MaterialName);
                }
            }
        }

        private void RefreshAll()
        {
            _inventoryUI.RefreshBag(_inventory.Bag);
            _inventoryUI.RefreshGold(_inventory.Gold);
        }

        private void SaveInventory()
        {
            // Serialize equipped items
            _saveData.EquippedItems = new SerializedEquipment[InventoryState.EquipmentSlotCount];
            for (int i = 0; i < InventoryState.EquipmentSlotCount; i++)
            {
                var item = _inventory.EquippedItems[i];
                _saveData.EquippedItems[i] = item != null ? SerializeEquipment(item) : new SerializedEquipment();
            }

            // Serialize bag items
            _saveData.BagItems = new SerializedBagItem[_inventory.Bag.Count];
            for (int i = 0; i < _inventory.Bag.Count; i++)
            {
                var bagItem = _inventory.Bag[i];
                if (bagItem.Type == BagItemType.Equipment)
                    _saveData.BagItems[i] = SerializedBagItem.FromEquipment(SerializeEquipment(bagItem.Equipment));
                else if (bagItem.Type == BagItemType.Gem)
                    _saveData.BagItems[i] = SerializedBagItem.FromGem(new SerializedGem { Type = (int)bagItem.Gem.Type, Tier = bagItem.Gem.Tier });
                else if (bagItem.Type == BagItemType.Material)
                    _saveData.BagItems[i] = SerializedBagItem.FromMaterial(bagItem.MaterialID, bagItem.MaterialName);
            }

            _saveData.Gold = _inventory.Gold;
            _saveManager.SaveGame(_saveData);
        }

        private SerializedEquipment SerializeEquipment(EquipmentInstance item)
        {
            var serialized = new SerializedEquipment
            {
                DataID = item.Data.ID,
                UpgradeLevel = item.UpgradeLevel,
                Gems = new SerializedGem[item.SocketedGems.Length]
            };

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

        private void DropSingleItem()
        {
            if (_selectedBagIndex >= 0 && _selectedBagIndex < _inventory.Bag.Count)
                _inventory.Bag.RemoveAt(_selectedBagIndex);
            _inventoryUI.HideItemDetail();
            _selectedBagItem = null;
            _selectedBagIndex = -1;
            RefreshAll();
            SaveInventory();
        }

        private void DropSelectedItems(List<int> indices)
        {
            indices.Sort((a, b) => b.CompareTo(a)); // descending to preserve indices
            foreach (int idx in indices)
            {
                if (idx >= 0 && idx < _inventory.Bag.Count)
                    _inventory.Bag.RemoveAt(idx);
            }
            _inventoryUI.ExitSelectMode();
            RefreshAll();
            SaveInventory();
        }

        private bool IsValuableItem(BagItem item)
        {
            if (item.Type == BagItemType.Material) return true;
            if (item.Type == BagItemType.Gem) return true;
            if (item.Type == BagItemType.Equipment)
            {
                var q = item.Equipment.Data.Quality;
                return q == EquipmentQuality.Elite || q == EquipmentQuality.Super;
            }
            return false;
        }

        private string GetItemDisplayName(BagItem item)
        {
            if (item.Type == BagItemType.Equipment)
            {
                var eq = item.Equipment;
                return eq.UpgradeLevel > 0 ? $"{eq.Data.Name} +{eq.UpgradeLevel}" : eq.Data.Name;
            }
            if (item.Type == BagItemType.Material)
                return item.MaterialName;
            return item.Gem.Type.ToString() + " Gem";
        }
    }
}
