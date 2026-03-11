using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Inventory
{
    public enum BagItemType
    {
        Equipment = 0,
        Gem = 1,
        Material = 2
    }

    public class BagItem
    {
        public BagItemType Type;
        public EquipmentInstance Equipment;
        public GemData Gem;
        public string MaterialID;
        public string MaterialName;

        public static BagItem FromEquipment(EquipmentInstance equipment)
        {
            return new BagItem { Type = BagItemType.Equipment, Equipment = equipment };
        }

        public static BagItem FromGem(GemData gem)
        {
            return new BagItem { Type = BagItemType.Gem, Gem = gem };
        }

        public static BagItem FromMaterial(string id, string name)
        {
            return new BagItem { Type = BagItemType.Material, MaterialID = id, MaterialName = name };
        }
    }

    public class InventoryState
    {
        public const int EquipmentSlotCount = 7;
        public const int BagCapacity = 42;

        public EquipmentInstance[] EquippedItems { get; private set; }
        public List<BagItem> Bag { get; private set; }

        public int Gold;

        public InventoryState()
        {
            EquippedItems = new EquipmentInstance[EquipmentSlotCount];
            Bag = new List<BagItem>();
            Gold = 0;
        }

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

        public bool CanEquip(EquipmentInstance item, int characterLevel, int[] unlockedClasses)
        {
            if (item == null)
                return false;

            if (characterLevel < item.Data.RequiredLevel)
                return false;

            if (!item.Data.CanBeEquippedBy(unlockedClasses))
                return false;

            return true;
        }

        public bool Equip(EquipmentInstance item, EquipmentSlot slot)
        {
            if (item == null)
                return false;

            if (item.Data.Slot != slot)
                return false;

            // Find the bag item wrapping this equipment
            int bagIndex = -1;
            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].Type == BagItemType.Equipment && Bag[i].Equipment == item)
                {
                    bagIndex = i;
                    break;
                }
            }
            if (bagIndex < 0)
                return false;

            int slotIndex = (int)slot;
            var currentEquipped = EquippedItems[slotIndex];

            // Remove item from bag
            Bag.RemoveAt(bagIndex);

            // If something is already equipped, swap it to the bag
            if (currentEquipped != null)
            {
                Bag.Add(BagItem.FromEquipment(currentEquipped));
            }

            EquippedItems[slotIndex] = item;
            return true;
        }

        public bool Unequip(EquipmentSlot slot)
        {
            int slotIndex = (int)slot;
            var item = EquippedItems[slotIndex];

            if (item == null)
                return false;

            if (Bag.Count >= BagCapacity)
                return false;

            EquippedItems[slotIndex] = null;
            Bag.Add(BagItem.FromEquipment(item));
            return true;
        }

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

        public bool AddToBag(EquipmentInstance item)
        {
            if (item == null)
                return false;

            if (Bag.Count >= BagCapacity)
                return false;

            Bag.Add(BagItem.FromEquipment(item));
            return true;
        }

        public bool AddGem(GemData gem)
        {
            if (Bag.Count >= BagCapacity)
                return false;

            Bag.Add(BagItem.FromGem(gem));
            return true;
        }

        public bool RemoveFromBag(EquipmentInstance item)
        {
            if (item == null)
                return false;

            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].Type == BagItemType.Equipment && Bag[i].Equipment == item)
                {
                    Bag.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public bool AddMaterial(string id, string name)
        {
            if (Bag.Count >= BagCapacity)
                return false;

            Bag.Add(BagItem.FromMaterial(id, name));
            return true;
        }

        public bool RemoveGem(GemData gem)
        {
            for (int i = 0; i < Bag.Count; i++)
            {
                if (Bag[i].Type == BagItemType.Gem && Bag[i].Gem.Type == gem.Type && Bag[i].Gem.Tier == gem.Tier)
                {
                    Bag.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public EquipmentInstance GetEquipped(EquipmentSlot slot)
        {
            return EquippedItems[(int)slot];
        }

        public bool IsBagFull => Bag.Count >= BagCapacity;
    }
}
