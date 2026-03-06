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
                    canEquip = _inventory.CanEquip(bagItem.Equipment, _playerLevel, _playerClass);

                _inventoryUI.ShowItemDetail(bagItem, canEquip);
            };

            _inventoryUI.OnEquipPressed = () =>
            {
                if (_selectedBagItem == null || _selectedBagItem.Type != BagItemType.Equipment) return;

                var item = _selectedBagItem.Equipment;
                if (!_inventory.CanEquip(item, _playerLevel, _playerClass)) return;

                _inventory.Equip(item, item.Data.Slot);
                _inventoryUI.HideItemDetail();
                _selectedBagItem = null;
                _selectedBagIndex = -1;

                RefreshAll();
                SaveInventory();
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
                else
                    _saveData.BagItems[i] = SerializedBagItem.FromGem(new SerializedGem { Type = (int)bagItem.Gem.Type, Tier = bagItem.Gem.Tier });
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
    }
}
