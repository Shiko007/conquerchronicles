namespace ConquerChronicles.Core.Character
{
    public class CharacterState
    {
        public CharacterClass Class;
        public int Level;
        public long XP;
        public int CurrentHP;
        public int CurrentMP;
        public int StatPointsAvailable;

        // Allocated stat points
        public int Vitality;
        public int Mana;
        public int Strength;
        public int Agility;
        public int Spirit;

        public CharacterStats BaseStats;
        public CharacterStats PerLevelGrowth;

        public CharacterState(CharacterClass characterClass, CharacterStats baseStats, CharacterStats perLevelGrowth)
        {
            Class = characterClass;
            Level = 1;
            XP = 0;
            BaseStats = baseStats;
            PerLevelGrowth = perLevelGrowth;

            var computed = ComputeStats();
            CurrentHP = computed.HP;
            CurrentMP = computed.MP;
            StatPointsAvailable = 0;
        }

        public CharacterStats ComputeStats()
        {
            var stats = BaseStats + PerLevelGrowth * (Level - 1);

            // Apply allocated stat points
            stats.HP += Vitality * 10;
            stats.MP += Mana * 8;
            stats.ATK += Strength * 3;
            stats.AGI += Agility * 2;
            stats.CritRate += Agility * 0.002f;
            stats.MATK += Spirit * 3;

            return stats;
        }

        public bool IsDead => CurrentHP <= 0;
    }
}
