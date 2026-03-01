using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class WaterTaoistSkills
    {
        public static SkillData HealingWave => new()
        {
            ID = "water_taoist_healing_wave",
            Name = "Healing Wave",
            RequiredClass = CharacterClass.WaterTaoist,
            RequiredLevel = 1,
            Cooldown = 4.0f,
            DamageMultiplier = 0f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.MeleeSwing,
            Range = 0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 10,
            IsSelfBuff = true,
            BuffDuration = 5f,
            BuffMultiplier = 1.3f
        };

        public static SkillData Stigma => new()
        {
            ID = "water_taoist_stigma",
            Name = "Stigma",
            RequiredClass = CharacterClass.WaterTaoist,
            RequiredLevel = 5,
            Cooldown = 6.0f,
            DamageMultiplier = 1.0f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.AoECircle,
            Range = 6.0f,
            AoERadius = 3.5f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Slow,
                Duration = 4.0f,
                TickInterval = 0f,
                TickDamage = 0,
                SlowPercent = 0.5f
            },
            MPCost = 18,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData Revive => new()
        {
            ID = "water_taoist_revive",
            Name = "Revive",
            RequiredClass = CharacterClass.WaterTaoist,
            RequiredLevel = 10,
            Cooldown = 15f,
            DamageMultiplier = 0f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.MeleeSwing,
            Range = 0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 30,
            IsSelfBuff = true,
            BuffDuration = 3f,
            BuffMultiplier = 2.5f
        };

        public static SkillData[] GetAll() => new[] { HealingWave, Stigma, Revive };
    }
}
