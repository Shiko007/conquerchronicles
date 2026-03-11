using System;
using System.Collections.Generic;
using ConquerChronicles.Core.Save;

namespace ConquerChronicles.Core.Character
{
    public static class RebirthSystem
    {
        public const int RebirthLevel = 100;
        public const int MaxRebirths = 3;

        private static readonly CharacterClass[] RebirthOptions =
        {
            CharacterClass.Archer,
            CharacterClass.Warrior,
            CharacterClass.WaterTaoist
        };

        public static bool CanRebirth(int level, int rebirthCount)
        {
            return level >= RebirthLevel && rebirthCount < MaxRebirths;
        }

        public static CharacterClass[] GetAvailableClasses(int[] unlockedClasses)
        {
            var available = new List<CharacterClass>();

            foreach (var option in RebirthOptions)
            {
                bool alreadyUnlocked = false;

                if (unlockedClasses != null)
                {
                    for (int i = 0; i < unlockedClasses.Length; i++)
                    {
                        if (unlockedClasses[i] == (int)option)
                        {
                            alreadyUnlocked = true;
                            break;
                        }
                    }
                }

                if (!alreadyUnlocked)
                    available.Add(option);
            }

            return available.ToArray();
        }

        public static void PerformRebirth(SaveData save, CharacterClass newClass)
        {
            // 1. Increment rebirth count
            save.RebirthCount++;

            // 2. Expand unlocked rebirth classes to include the new class
            int previousLength = save.UnlockedRebirthClasses != null
                ? save.UnlockedRebirthClasses.Length
                : 0;

            var expanded = new int[previousLength + 1];
            for (int i = 0; i < previousLength; i++)
                expanded[i] = save.UnlockedRebirthClasses[i];

            expanded[previousLength] = (int)newClass;
            save.UnlockedRebirthClasses = expanded;

            // 3. Reset level to 1
            save.CharacterLevel = 1;

            // 4. Reset XP to 0
            save.CharacterXP = 0;

            // 5. Refund allocated stats back to available pool
            save.StatPointsAvailable += save.Vitality + save.Strength + save.Agility + save.Spirit;

            // 6. Reset all stat allocations to 0
            save.Vitality = 0;
            save.Mana = 0;
            save.Strength = 0;
            save.Agility = 0;
            save.Spirit = 0;

            // 7. Move equipped items to bag, then clear equipment slots
            var bagItems = save.BagItems != null
                ? new List<SerializedBagItem>(save.BagItems)
                : new List<SerializedBagItem>();

            if (save.EquippedItems != null)
            {
                for (int i = 0; i < save.EquippedItems.Length; i++)
                {
                    if (!save.EquippedItems[i].IsEmpty)
                    {
                        bagItems.Add(SerializedBagItem.FromEquipment(save.EquippedItems[i]));
                        save.EquippedItems[i] = new SerializedEquipment();
                    }
                }
            }

            save.BagItems = bagItems.ToArray();

            // 8. Gold stays unchanged
        }
    }
}
