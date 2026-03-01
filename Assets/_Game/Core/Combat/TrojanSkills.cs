using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public static class TrojanSkills
    {
        public static SkillData BladeFury => new()
        {
            ID = "trojan_blade_fury",
            Name = "Blade Fury",
            RequiredClass = CharacterClass.Trojan,
            RequiredLevel = 1,
            Cooldown = 1.5f,
            DamageMultiplier = 1.5f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.MeleeSwing,
            Range = 2.0f,
            AoERadius = 2.0f,
            AoEAngle = 120f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 5,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData Cyclone => new()
        {
            ID = "trojan_cyclone",
            Name = "Cyclone",
            RequiredClass = CharacterClass.Trojan,
            RequiredLevel = 5,
            Cooldown = 8f,
            DamageMultiplier = 2.0f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.AoECircle,
            Range = 3.0f,
            AoERadius = 3.0f,
            AoEAngle = 360f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 15,
            IsSelfBuff = false,
            BuffDuration = 0f,
            BuffMultiplier = 0f
        };

        public static SkillData Hercules => new()
        {
            ID = "trojan_hercules",
            Name = "Hercules",
            RequiredClass = CharacterClass.Trojan,
            RequiredLevel = 10,
            Cooldown = 15f,
            DamageMultiplier = 0f,
            DamageType = DamageType.Physical,
            Pattern = AttackPattern.AoECircle,
            Range = 0f,
            AoERadius = 0f,
            AoEAngle = 0f,
            ProjectileCount = 0,
            ProjectileSpread = 0f,
            ProjectileSpeed = 0f,
            AppliedEffect = default,
            MPCost = 20,
            IsSelfBuff = true,
            BuffDuration = 5f,
            BuffMultiplier = 1.5f
        };

        public static SkillData[] GetAll() => new[] { BladeFury, Cyclone, Hercules };
    }
}
