using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class NinjaSkills
    {
        public static SkillData ShurikenFan => new()
        {
            ID = "ninja_shuriken_fan",
            Name = "Shuriken Fan",
            RequiredClass = CharacterClass.Ninja,
            RequiredLevel = 1,
            Cooldown = 1.5f,
            DamageMultiplier = 0.9f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.MultiProjectile,
            Range = 6.0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 3,
            ProjectileSpread = 30f,
            ProjectileSpeed = 14f,
            AppliedEffect = default,
            MPCost = 6,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData ShadowStep => new()
        {
            ID = "ninja_shadow_step",
            Name = "Shadow Step",
            RequiredClass = CharacterClass.Ninja,
            RequiredLevel = 5,
            Cooldown = 10f,
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
            BuffDuration = 5f,
            BuffMultiplier = 1.7f
        };

        public static SkillData ToxicFog => new()
        {
            ID = "ninja_toxic_fog",
            Name = "Toxic Fog",
            RequiredClass = CharacterClass.Ninja,
            RequiredLevel = 10,
            Cooldown = 14f,
            DamageMultiplier = 0.5f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.SummonZone,
            Range = 5.0f,
            AoERadius = 3.5f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = new StatusEffect
            {
                Type = StatusEffectType.Poison,
                Duration = 6.0f,
                TickInterval = 1.0f,
                TickDamage = 7,
                SlowPercent = 0f
            },
            MPCost = 20,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData[] GetAll() => new[] { ShurikenFan, ShadowStep, ToxicFog };
    }
}
