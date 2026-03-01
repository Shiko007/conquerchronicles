using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    [System.Serializable]
    public struct SkillData
    {
        public string ID;
        public string Name;
        public CharacterClass RequiredClass;
        public int RequiredLevel;
        public float Cooldown;
        public float DamageMultiplier;
        public DamageType DamageType;
        public AttackPattern Pattern;
        public float Range;
        public float AoERadius;
        public float AoEAngle;        // For cone patterns, degrees
        public int ProjectileCount;
        public float ProjectileSpread; // Angle spread for MultiProjectile
        public float ProjectileSpeed;
        public StatusEffect AppliedEffect;
        public int MPCost;
        public bool IsSelfBuff;
        public float BuffDuration;
        public float BuffMultiplier;  // e.g. 1.5 = +50% ATK
    }
}
