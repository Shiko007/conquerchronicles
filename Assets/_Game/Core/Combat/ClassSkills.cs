using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    /// <summary>
    /// Routes to the correct skill set for any character class.
    /// </summary>
    public static class ClassSkills
    {
        public static SkillData[] GetSkillsForClass(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Trojan => TrojanSkills.GetAll(),
                CharacterClass.Warrior => WarriorSkills.GetAll(),
                CharacterClass.Archer => ArcherSkills.GetAll(),
                CharacterClass.WaterTaoist => WaterTaoistSkills.GetAll(),
                CharacterClass.FireTaoist => FireTaoistSkills.GetAll(),
                CharacterClass.Ninja => NinjaSkills.GetAll(),
                _ => TrojanSkills.GetAll()
            };
        }

        public static SkillData[] GetSkillsForWeapon(CharacterClass weaponClass)
        {
            return weaponClass switch
            {
                CharacterClass.None => TrojanSkills.GetAll(),
                CharacterClass.Trojan => TrojanSkills.GetAll(),
                CharacterClass.Archer => ArcherSkills.GetAll(),
                CharacterClass.Warrior => WarriorSkills.GetAll(),
                CharacterClass.WaterTaoist => TaoistSkills.GetAll(),
                CharacterClass.FireTaoist => TaoistSkills.GetAll(),
                CharacterClass.Ninja => NinjaSkills.GetAll(),
                _ => TrojanSkills.GetAll()
            };
        }
    }
}
