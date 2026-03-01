using System;
using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Mining
{
    /// <summary>
    /// Pure static logic for mining calculations. Uses System.Random with a seed
    /// so results are deterministic for the same seed.
    /// </summary>
    public static class MiningResolver
    {
        private static readonly GemType[] AllGemTypes = (GemType[])Enum.GetValues(typeof(GemType));

        public static MiningYield CalculateYield(MineData mine, int seed)
        {
            var rng = new Random(seed);

            int gold = rng.Next(mine.MinGold, mine.MaxGold + 1);

            int gemCount = mine.GemDropCount;
            var gems = new GemData[gemCount];
            for (int i = 0; i < gemCount; i++)
            {
                var type = AllGemTypes[rng.Next(AllGemTypes.Length)];
                int tier = rng.Next(mine.MinGemTier, mine.MaxGemTier + 1);
                gems[i] = new GemData(type, tier);
            }

            return new MiningYield
            {
                Gold = gold,
                Gems = gems,
                IsEmpty = false
            };
        }

        public static bool CanStartMining(MiningState state, MineData mine, int playerLevel)
        {
            if (state.IsMining) return false;
            if (playerLevel < mine.RequiredLevel) return false;
            return true;
        }
    }
}
