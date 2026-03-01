using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class WarriorSkills
    {
        public static SkillData ShieldBash => new()
        {
            ID = "warrior_shield_bash",
            Name = "Shield Bash",
            RequiredClass = CharacterClass.Warrior,
            RequiredLevel = 1,
            Cooldown = 2.0f,
            DamageMultiplier = 1.2f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.MeleeSwing,
            Range = 1.5f,
            AoERadius = 1.5f,
            AoEAngle = 90f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Stun,
                Duration = 1.5f,
                TickInterval = 0f,
                TickDamage = 0,
                SlowPercent = 0f
            },
            MPCost = 8,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData DefensiveStance => new()
        {
            ID = "warrior_defensive_stance",
            Name = "Defensive Stance",
            RequiredClass = CharacterClass.Warrior,
            RequiredLevel = 5,
            Cooldown = 12f,
            DamageMultiplier = 0f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.MeleeSwing,
            Range = 0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 15,
            IsSelfBuff = true,
            BuffDuration = 6f,
            BuffMultiplier = 1.8f
        };

        public static SkillData Superman => new()
        {
            ID = "warrior_superman",
            Name = "Superman",
            RequiredClass = CharacterClass.Warrior,
            RequiredLevel = 10,
            Cooldown = 15f,
            DamageMultiplier = 2.5f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.AoECircle,
            Range = 4.0f,
            AoERadius = 4.0f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 25,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData[] GetAll() => new[] { ShieldBash, DefensiveStance, Superman };
    }
}
