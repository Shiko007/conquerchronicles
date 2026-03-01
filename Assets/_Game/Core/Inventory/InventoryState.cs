using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Inventory
{
    public class InventoryState
    {
        public const int EquipmentSlotCount = 7;
        public const int BagCapacity = 50;
        public const int GemBagCapacity = 100;

        /// <summary>
        /// Equipped items indexed by EquipmentSlot. Null means the slot is empty.
        /// </summary>
        public EquipmentInstance[] EquippedItems { get; private set; }

        /// <summary>
        /// Unequipped equipment items in the bag.
        /// </summary>
        public List<EquipmentInstance> Bag { get; private set; }

        /// <summary>
        /// Stored gems in the gem bag.
        /// </summary>
        public List<GemData> GemBag { get; private set; }

        public int Gold;

        public InventoryState()
        {
            EquippedItems = new EquipmentInstance[EquipmentSlotCount];
            Bag = new List<EquipmentInstance>();
            GemBag = new List<GemData>();
            Gold = 0;
        }

        /// <summary>
        /// Checks whether the given item can be equipped by a character of the specified
        /// level and class. Validates level requirement and class requirement.
        /// </summary>
        public bool CanEquip(EquipmentInstance item, int characterLevel, CharacterClass characterClass)
        {
            if (item == null)
                return false;

            if (characterLevel < item.Data.RequiredLevel)
                return false;

            if (!item.Data.CanBeEquippedBy(characterClass))
                return false;

            return true;
        }

        /// <summary>
        /// Equips an item from the bag into the specified slot.
        /// If the slot is already occupied, the old item is swapped back to the bag.
        /// The item must already be in the bag.
        /// </summary>
        public bool Equip(EquipmentInstance item, EquipmentSlot slot)
        {
            if (item == null)
                return false;

            if (item.Data.Slot != slot)
                return false;

            int bagIndex = Bag.IndexOf(item);
            if (bagIndex < 0)
                return false;

            int slotIndex = (int)slot;
            var currentEquipped = EquippedItems[slotIndex];

            // Remove item from bag
            Bag.RemoveAt(bagIndex);

            // If something is already equipped, swap it to the bag
            if (currentEquipped != null)
            {
                Bag.Add(currentEquipped);
            }

            // Equip the new item
            EquippedItems[slotIndex] = item;
            return true;
        }

        /// <summary>
        /// Unequips the item in the specified slot and moves it to the bag.
        /// Returns false if the slot is empty or the bag is full.
        /// </summary>
        public bool Unequip(EquipmentSlot slot)
        {
            int slotIndex = (int)slot;
            var item = EquippedItems[slotIndex];

            if (item == null)
                return false;

            if (Bag.Count >= BagCapacity)
                return false;

            EquippedItems[slotIndex] = null;
            Bag.Add(item);
            return true;
        }

        /// <summary>
        /// Returns the sum of ComputeStats() for all currently equipped items.
        /// </summary>
        public CharacterStats GetEquippedStats()
        {
            var total = new CharacterStats();

            for (int i = 0; i < EquippedItems.Length; i++)
            {
                if (EquippedItems[i] != null)
                {
                    total = total + EquippedItems[i].ComputeStats();
                }
            }

            return total;
        }

        /// <summary>
        /// Adds an equipment item to the bag if capacity allows.
        /// Returns true if added successfully, false if the bag is full.
        /// </summary>
        public bool AddToBag(EquipmentInstance item)
        {
            if (item == null)
                return false;

            if (Bag.Count >= BagCapacity)
                return false;

            Bag.Add(item);
            return true;
        }

        /// <summary>
        /// Removes an equipment item from the bag.
        /// Returns true if the item was found and removed.
        /// </summary>
        public bool RemoveFromBag(EquipmentInstance item)
        {
            if (item == null)
                return false;

            return Bag.Remove(item);
        }

        /// <summary>
        /// Adds a gem to the gem bag if capacity allows.
        /// Returns true if added successfully, false if the gem bag is full.
        /// </summary>
        public bool AddGem(GemData gem)
        {
            if (GemBag.Count >= GemBagCapacity)
                return false;

            GemBag.Add(gem);
            return true;
        }

        /// <summary>
        /// Removes a gem from the gem bag.
        /// Returns true if a matching gem was found and removed.
        /// </summary>
        public bool RemoveGem(GemData gem)
        {
            for (int i = 0; i < GemBag.Count; i++)
            {
                if (GemBag[i].Type == gem.Type && GemBag[i].Tier == gem.Tier)
                {
                    GemBag.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the equipped item in the given slot, or null if empty.
        /// </summary>
        public EquipmentInstance GetEquipped(EquipmentSlot slot)
        {
            return EquippedItems[(int)slot];
        }

        /// <summary>
        /// Returns true if the bag has reached maximum capacity.
        /// </summary>
        public bool IsBagFull => Bag.Count >= BagCapacity;

        /// <summary>
        /// Returns true if the gem bag has reached maximum capacity.
        /// </summary>
        public bool IsGemBagFull => GemBag.Count >= GemBagCapacity;
    }
}
