using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Equipment
{
    [System.Serializable]
    public struct GemData
    {
        public GemType Type;
        public int Tier;

        public GemData(GemType type, int tier)
        {
            Type = type;
            Tier = System.Math.Clamp(tier, 1, 9);
        }

        public CharacterStats GetBonus()
        {
            return GetBonusForTier(Type, Tier);
        }

        public static CharacterStats GetBonusForTier(GemType type, int tier)
        {
            var stats = new CharacterStats();

            // Exponential scaling: base * 2^(tier-1) per GDD
            int mult = 1 << (tier - 1); // 2^(tier-1): 1, 2, 4, 8, 16, 32, 64, 128, 256

            switch (type)
            {
                case GemType.Dragon:
                    stats.HP = 100 * mult;
                    break;
                case GemType.Phoenix:
                    stats.ATK = 5 * mult;
                    break;
                case GemType.Moon:
                    stats.MP = 40 * mult;
                    break;
                case GemType.Fury:
                    stats.CritRate = 0.01f * mult;
                    break;
                case GemType.Tortoise:
                    stats.DEF = 4 * mult;
                    break;
                case GemType.Thunder:
                    stats.MATK = 5 * mult;
                    break;
                case GemType.Violet:
                    // XP bonus is tracked separately, no stat bonus
                    break;
                case GemType.Rainbow:
                    stats.HP = 10 * mult;
                    stats.MP = 8 * mult;
                    stats.ATK = 1 * mult;
                    stats.DEF = 1 * mult;
                    stats.MATK = 1 * mult;
                    stats.MDEF = 1 * mult;
                    stats.AGI = 1 * mult;
                    stats.CritRate = 0.002f * mult;
                    break;
            }

            return stats;
        }
    }
}
