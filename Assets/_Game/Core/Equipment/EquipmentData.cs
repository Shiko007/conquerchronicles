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
    }
}
