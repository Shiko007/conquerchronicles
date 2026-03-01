using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Core.Save;
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
        private EquipmentInstance _selectedItem;
        private int _selectedSlotIndex = -1;
        private bool _selectedIsEquipped;

        private void Start()
        {
            // Get save manager and load save data
            _saveManager = SaveSystemBridge.GetOrCreate();
            _saveData = _saveManager.LoadGame() ?? SaveData.CreateDefault();

            _playerLevel = _saveData.CharacterLevel;
            _playerClass = _saveData.SelectedClass;

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
                    var serialized = _saveData.BagItems[i];
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
                                {
                                    var gem = new GemData((GemType)serialized.Gems[g].Type, serialized.Gems[g].Tier);
                                    instance.Socket(g, gem);
                                }
                            }
                        }

                        _inventory.AddToBag(instance);
                    }
                }
            }

            // Restore gem bag
            if (_saveData.GemBag != null)
            {
                for (int i = 0; i < _saveData.GemBag.Length; i++)
                {
                    var serializedGem = _saveData.GemBag[i];
                    if (serializedGem.Tier > 0)
                    {
                        var gem = new GemData((GemType)serializedGem.Type, serializedGem.Tier);
                        _inventory.AddGem(gem);
                    }
                }
            }

            // Wire UI events
            _equipmentUI.OnBackPressed = () => SceneManager.LoadScene("MainMenu");

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

            _equipmentUI.OnBagItemPressed = (bagIndex) =>
            {
                if (bagIndex >= 0 && bagIndex < _inventory.Bag.Count)
                {
                    var bagItem = _inventory.Bag[bagIndex];
                    _selectedItem = bagItem;
                    _selectedSlotIndex = (int)bagItem.Data.Slot;
                    _selectedIsEquipped = false;

                    bool canEquip = _inventory.CanEquip(bagItem, _playerLevel, _playerClass);
                    _equipmentUI.ShowItemDetail(bagItem, canEquip, false);
                }
            };

            _equipmentUI.OnEquipPressed = () =>
            {
                if (_selectedItem == null) return;

                if (_selectedIsEquipped)
                {
                    // Unequip
                    var slot = (EquipmentSlot)_selectedSlotIndex;
                    _inventory.Unequip(slot);
                    _equipmentUI.HideItemDetail();
                    _selectedItem = null;
                    _selectedSlotIndex = -1;
                }
                else
                {
                    // Equip from bag
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
                }
                else if (result.Success)
                {
                    Debug.Log($"[Equipment] Upgrade success! Now +{result.NewLevel}");
                    // Item's UpgradeLevel is already updated by TryUpgrade
                    bool canEquip = !_selectedIsEquipped && _inventory.CanEquip(_selectedItem, _playerLevel, _playerClass);
                    _equipmentUI.ShowItemDetail(_selectedItem, canEquip, _selectedIsEquipped);
                }
                else
                {
                    Debug.Log($"[Equipment] Upgrade failed. Level is now +{result.NewLevel}");
                    bool canEquip = !_selectedIsEquipped && _inventory.CanEquip(_selectedItem, _playerLevel, _playerClass);
                    _equipmentUI.ShowItemDetail(_selectedItem, canEquip, _selectedIsEquipped);
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

            // Initialize UI and refresh
            _equipmentUI.Initialize();
            RefreshAll();
        }

        private void RefreshAll()
        {
            _equipmentUI.RefreshEquippedSlots(_inventory.EquippedItems);
            _equipmentUI.RefreshStats(_inventory.GetEquippedStats());
            _equipmentUI.RefreshBag(_inventory.Bag);
            _equipmentUI.RefreshGems(_inventory.GemBag);
            _equipmentUI.RefreshGold(_inventory.Gold);
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
            _saveData.BagItems = new SerializedEquipment[_inventory.Bag.Count];
            for (int i = 0; i < _inventory.Bag.Count; i++)
            {
                _saveData.BagItems[i] = SerializeEquipment(_inventory.Bag[i]);
            }

            // Serialize gem bag
            _saveData.GemBag = new SerializedGem[_inventory.GemBag.Count];
            for (int i = 0; i < _inventory.GemBag.Count; i++)
            {
                _saveData.GemBag[i] = new SerializedGem
                {
                    Type = (int)_inventory.GemBag[i].Type,
                    Tier = _inventory.GemBag[i].Tier
                };
            }

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
