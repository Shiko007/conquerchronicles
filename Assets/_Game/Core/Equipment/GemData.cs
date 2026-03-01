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

            switch (type)
            {
                case GemType.Dragon:
                    stats.HP = tier * 50;
                    break;
                case GemType.Phoenix:
                    stats.ATK = tier * 5;
                    break;
                case GemType.Moon:
                    stats.MP = tier * 40;
                    break;
                case GemType.Fury:
                    stats.CritRate = tier * 0.01f;
                    break;
                case GemType.Tortoise:
                    stats.DEF = tier * 4;
                    break;
                case GemType.Thunder:
                    stats.MATK = tier * 5;
                    break;
                case GemType.Violet:
                    // XP bonus is tracked separately, no stat bonus
                    break;
                case GemType.Rainbow:
                    stats.HP = tier * 10;
                    stats.MP = tier * 8;
                    stats.ATK = tier * 1;
                    stats.DEF = tier * 1;
                    stats.MATK = tier * 1;
                    stats.MDEF = tier * 1;
                    stats.AGI = tier * 1;
                    stats.CritRate = tier * 0.002f;
                    break;
            }

            return stats;
        }
    }
}
