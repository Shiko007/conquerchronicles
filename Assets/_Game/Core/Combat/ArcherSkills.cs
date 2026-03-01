using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class ArcherSkills
    {
        public static SkillData ArrowRain => new()
        {
            ID = "archer_arrow_rain",
            Name = "Arrow Rain",
            RequiredClass = CharacterClass.Archer,
            RequiredLevel = 1,
            Cooldown = 3.0f,
            DamageMultiplier = 1.3f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.AoECircle,
            Range = 8.0f,
            AoERadius = 3.0f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 10,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData ScatterShot => new()
        {
            ID = "archer_scatter_shot",
            Name = "Scatter Shot",
            RequiredClass = CharacterClass.Archer,
            RequiredLevel = 5,
            Cooldown = 5.0f,
            DamageMultiplier = 0.8f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.MultiProjectile,
            Range = 7.0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 5,
            ProjectileSpread = 45f,
            ProjectileSpeed = 12f,
            AppliedEffect = default,
            MPCost = 12,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData Fly => new()
        {
            ID = "archer_fly",
            Name = "Fly",
            RequiredClass = CharacterClass.Archer,
            RequiredLevel = 10,
            Cooldown = 15f,
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
            MPCost = 20,
            IsSelfBuff = true,
            BuffDuration = 8f,
            BuffMultiplier = 1.6f
        };

        public static SkillData[] GetAll() => new[] { ArrowRain, ScatterShot, Fly };
    }
}
