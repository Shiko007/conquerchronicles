using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Equipment
{
    [System.Serializable]
    public struct EquipmentData
    {
        public string ID;
        public string Name;
        public EquipmentSlot Slot;
        public EquipmentQuality Quality;
        public int RequiredLevel;
        public CharacterClass RequiredClass;
        public CharacterStats BaseStats;
        public int MaxSockets;
        public string Description;

        public bool CanBeEquippedBy(CharacterClass characterClass)
        {
            return RequiredClass == CharacterClass.None || RequiredClass == characterClass;
        }

        public bool CanBeEquippedBy(int[] unlockedClasses)
        {
            if (RequiredClass == CharacterClass.None) return true;
            if (unlockedClasses == null) return false;
            for (int i = 0; i < unlockedClasses.Length; i++)
            {
                if ((CharacterClass)unlockedClasses[i] == RequiredClass) return true;
                // Taoist unlock (WaterTaoist) allows both WaterTaoist and FireTaoist weapons
                if ((CharacterClass)unlockedClasses[i] == CharacterClass.WaterTaoist &&
                    (RequiredClass == CharacterClass.WaterTaoist || RequiredClass == CharacterClass.FireTaoist))
                    return true;
            }
            return false;
        }
    }
}
