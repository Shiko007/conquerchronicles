using System;

namespace ConquerChronicles.Core.Character
{
    public static class LevelUpTable
    {
        public const int MaxLevel = 130;
        public const int StatPointsPerLevel = 3;

        public static long GetRequiredXP(int level)
        {
            if (level < 1 || level >= MaxLevel) return long.MaxValue;
            return 100;
        }

        public static bool TryLevelUp(CharacterState state)
        {
            if (state.Level >= MaxLevel) return false;

            long required = GetRequiredXP(state.Level);
            if (state.XP < required) return false;

            state.XP -= required;
            state.Level++;
            state.StatPointsAvailable += StatPointsPerLevel;

            // Restore HP/MP on level up
            var newStats = state.ComputeStats();
            state.CurrentHP = newStats.HP;
            state.CurrentMP = newStats.MP;

            return true;
        }

        public static CharacterStats GetClassBaseStats(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Trojan => new CharacterStats
                {
                    HP = 120, MP = 30, ATK = 25, DEF = 12, MATK = 5, MDEF = 5,
                    AGI = 15, CritRate = 0.05f, CritDmg = 1.5f, AttackSpeed = 1.2f
                },
                CharacterClass.Warrior => new CharacterStats
                {
                    HP = 180, MP = 20, ATK = 18, DEF = 22, MATK = 3, MDEF = 8,
                    AGI = 8, CritRate = 0.02f, CritDmg = 1.3f, AttackSpeed = 0.8f
                },
                CharacterClass.Archer => new CharacterStats
                {
                    HP = 90, MP = 40, ATK = 22, DEF = 8, MATK = 5, MDEF = 5,
                    AGI = 18, CritRate = 0.08f, CritDmg = 1.6f, AttackSpeed = 1.0f
                },
                CharacterClass.WaterTaoist => new CharacterStats
                {
                    HP = 100, MP = 120, ATK = 8, DEF = 10, MATK = 25, MDEF = 18,
                    AGI = 10, CritRate = 0.03f, CritDmg = 1.4f, AttackSpeed = 0.9f
                },
                CharacterClass.FireTaoist => new CharacterStats
                {
                    HP = 80, MP = 100, ATK = 10, DEF = 6, MATK = 28, MDEF = 15,
                    AGI = 10, CritRate = 0.03f, CritDmg = 1.5f, AttackSpeed = 0.9f
                },
                CharacterClass.Ninja => new CharacterStats
                {
                    HP = 95, MP = 50, ATK = 20, DEF = 10, MATK = 8, MDEF = 8,
                    AGI = 22, CritRate = 0.12f, CritDmg = 1.8f, AttackSpeed = 1.4f
                },
                _ => default
            };
        }

        public static CharacterStats GetClassGrowthPerLevel(CharacterClass characterClass)
        {
            return characterClass switch
            {
                CharacterClass.Trojan => new CharacterStats
                {
                    HP = 8, MP = 2, ATK = 3, DEF = 1, MATK = 0, MDEF = 0,
                    AGI = 1, CritRate = 0.001f, CritDmg = 0f, AttackSpeed = 0.005f
                },
                CharacterClass.Warrior => new CharacterStats
                {
                    HP = 14, MP = 1, ATK = 2, DEF = 3, MATK = 0, MDEF = 1,
                    AGI = 0, CritRate = 0.0005f, CritDmg = 0f, AttackSpeed = 0.003f
                },
                CharacterClass.Archer => new CharacterStats
                {
                    HP = 6, MP = 3, ATK = 3, DEF = 1, MATK = 0, MDEF = 0,
                    AGI = 2, CritRate = 0.001f, CritDmg = 0f, AttackSpeed = 0.005f
                },
                CharacterClass.WaterTaoist => new CharacterStats
                {
                    HP = 7, MP = 8, ATK = 0, DEF = 1, MATK = 3, MDEF = 2,
                    AGI = 1, CritRate = 0.0005f, CritDmg = 0f, AttackSpeed = 0.003f
                },
                CharacterClass.FireTaoist => new CharacterStats
                {
                    HP = 5, MP = 7, ATK = 0, DEF = 0, MATK = 4, MDEF = 1,
                    AGI = 1, CritRate = 0.0005f, CritDmg = 0f, AttackSpeed = 0.003f
                },
                CharacterClass.Ninja => new CharacterStats
                {
                    HP = 6, MP = 3, ATK = 2, DEF = 1, MATK = 0, MDEF = 0,
                    AGI = 3, CritRate = 0.002f, CritDmg = 0.005f, AttackSpeed = 0.007f
                },
                _ => default
            };
        }
    }
}
