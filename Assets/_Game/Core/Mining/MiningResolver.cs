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

            // Pick gems from mine-specific types if available, otherwise fall back to all
            GemType[] gemPool = (mine.AvailableGems != null && mine.AvailableGems.Length > 0)
                ? mine.AvailableGems
                : AllGemTypes;

            int gemCount = mine.GemDropCount;
            var gems = new GemData[gemCount];
            for (int i = 0; i < gemCount; i++)
            {
                var type = gemPool[rng.Next(gemPool.Length)];
                int tier = rng.Next(mine.MinGemTier, mine.MaxGemTier + 1);
                gems[i] = new GemData(type, tier);
            }

            // Generate ores from mine-specific ore types
            OreData[] ores = null;
            if (mine.AvailableOres != null && mine.AvailableOres.Length > 0)
            {
                int oreCount = rng.Next(mine.MinOreCount, mine.MaxOreCount + 1);
                ores = new OreData[oreCount];
                for (int i = 0; i < oreCount; i++)
                {
                    var oreType = mine.AvailableOres[rng.Next(mine.AvailableOres.Length)];
                    int quantity = rng.Next(1, 4); // 1-3 quantity per ore drop
                    ores[i] = new OreData(oreType, quantity);
                }
            }

            return new MiningYield
            {
                Gold = gold,
                Gems = gems,
                Ores = ores,
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
