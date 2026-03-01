using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class FireTaoistSkills
    {
        public static SkillData FireRing => new()
        {
            ID = "fire_taoist_fire_ring",
            Name = "Fire Ring",
            RequiredClass = CharacterClass.FireTaoist,
            RequiredLevel = 1,
            Cooldown = 2.0f,
            DamageMultiplier = 0.7f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.Orbiting,
            Range = 2.5f,
            AoERadius = 2.5f,
            AoEAngle = 360f,
            ProjectileCount = 3,
            ProjectileSpread = 120f,
            ProjectileSpeed = 4f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Burn,
                Duration = 3.0f,
                TickInterval = 1.0f,
                TickDamage = 5,
                SlowPercent = 0f
            },
            MPCost = 8,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData Meteor => new()
        {
            ID = "fire_taoist_meteor",
            Name = "Meteor",
            RequiredClass = CharacterClass.FireTaoist,
            RequiredLevel = 5,
            Cooldown = 10f,
            DamageMultiplier = 3.0f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.AoECircle,
            Range = 7.0f,
            AoERadius = 3.5f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Burn,
                Duration = 4.0f,
                TickInterval = 1.0f,
                TickDamage = 8,
                SlowPercent = 0f
            },
            MPCost = 25,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData FireCircle => new()
        {
            ID = "fire_taoist_fire_circle",
            Name = "Fire Circle",
            RequiredClass = CharacterClass.FireTaoist,
            RequiredLevel = 10,
            Cooldown = 12f,
            DamageMultiplier = 2.0f,
            DamageType = DamageType.Magical,
            Pattern = AttackPattern.Nova,
            Range = 5.0f,
            AoERadius = 5.0f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 6f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Burn,
                Duration = 3.0f,
                TickInterval = 1.0f,
                TickDamage = 6,
                SlowPercent = 0f
            },
            MPCost = 22,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData[] GetAll() => new[] { FireRing, Meteor, FireCircle };
    }
}
