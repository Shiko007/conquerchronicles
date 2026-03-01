using System;
using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class DamageFormula
    {
        public static DamageResult Calculate(CharacterStats attacker, CharacterStats defender, SkillData skill, int seed)
        {
            var rng = new Random(seed);

            float baseAtk = skill.DamageType == DamageType.Physical ? attacker.ATK : attacker.MATK;
            float baseDef = skill.DamageType == DamageType.Physical ? defender.DEF : defender.MDEF;

            float rawDamage = (baseAtk - baseDef * 0.5f) * skill.DamageMultiplier;

            // Variance +/- 10%
            float variance = 0.9f + (float)rng.NextDouble() * 0.2f;
            rawDamage *= variance;

            // Crit check
            bool isCrit = (float)rng.NextDouble() < attacker.CritRate;
            if (isCrit)
                rawDamage *= attacker.CritDmg;

            int finalDamage = Math.Max(1, (int)rawDamage);

            return new DamageResult { Damage = finalDamage, IsCritical = isCrit };
        }
    }

    public struct DamageResult
    {
        public int Damage;
        public bool IsCritical;
    }
}
