using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Mining
{
    /// <summary>
    /// Hardcoded test mines for development. Will be replaced by ScriptableObjects.
    /// </summary>
    public static class TestMines
    {
        public static MineData CopperMine => new()
        {
            ID = "mine_copper",
            Name = "Copper Mine",
            RequiredLevel = 1,
            DurationSeconds = 3600, // 1 hour
            MinGold = 50,
            MaxGold = 150,
            MinGemTier = 1,
            MaxGemTier = 2,
            GemDropCount = 1,
            Description = "A beginner mine with modest yields. Copper veins occasionally reveal low-tier gems.",
            AvailableGems = new[] { GemType.Dragon, GemType.Tortoise },
            AvailableOres = new[] { OreType.Iron, OreType.Copper },
            MinOreCount = 1,
            MaxOreCount = 2
        };

        public static MineData SilverMine => new()
        {
            ID = "mine_silver",
            Name = "Silver Mine",
            RequiredLevel = 20,
            DurationSeconds = 7200, // 2 hours
            MinGold = 150,
            MaxGold = 400,
            MinGemTier = 2,
            MaxGemTier = 4,
            GemDropCount = 1,
            Description = "Rich silver deposits with a chance of mid-tier gems hiding in the ore.",
            AvailableGems = new[] { GemType.Phoenix, GemType.Moon, GemType.Tortoise },
            AvailableOres = new[] { OreType.Copper, OreType.Silver },
            MinOreCount = 1,
            MaxOreCount = 3
        };

        public static MineData GoldMine => new()
        {
            ID = "mine_gold",
            Name = "Gold Mine",
            RequiredLevel = 50,
            DurationSeconds = 14400, // 4 hours
            MinGold = 400,
            MaxGold = 1000,
            MinGemTier = 3,
            MaxGemTier = 6,
            GemDropCount = 2,
            Description = "Deep gold veins guarded by ancient wards. Yields valuable gems alongside gold.",
            AvailableGems = new[] { GemType.Phoenix, GemType.Fury, GemType.Thunder },
            AvailableOres = new[] { OreType.Silver, OreType.Gold },
            MinOreCount = 2,
            MaxOreCount = 4
        };

        public static MineData CrystalMine => new()
        {
            ID = "mine_crystal",
            Name = "Crystal Mine",
            RequiredLevel = 80,
            DurationSeconds = 28800, // 8 hours
            MinGold = 1000,
            MaxGold = 3000,
            MinGemTier = 5,
            MaxGemTier = 8,
            GemDropCount = 2,
            Description = "A crystalline cavern pulsing with magical energy. High-tier gems are common here.",
            AvailableGems = new[] { GemType.Fury, GemType.Thunder, GemType.Rainbow },
            AvailableOres = new[] { OreType.Gold, OreType.Meteor },
            MinOreCount = 2,
            MaxOreCount = 5
        };

        public static MineData DragonMine => new()
        {
            ID = "mine_dragon",
            Name = "Dragon Mine",
            RequiredLevel = 110,
            DurationSeconds = 43200, // 12 hours
            MinGold = 3000,
            MaxGold = 8000,
            MinGemTier = 7,
            MaxGemTier = 9,
            GemDropCount = 3,
            Description = "The legendary dragon's hoard. Only the strongest dare enter. Supreme gems await.",
            AvailableGems = new[] { GemType.Dragon, GemType.Phoenix, GemType.Fury, GemType.Rainbow },
            AvailableOres = new[] { OreType.Meteor },
            MinOreCount = 3,
            MaxOreCount = 6
        };

        public static MineData[] GetAll() => new[]
        {
            CopperMine, SilverMine, GoldMine, CrystalMine, DragonMine
        };

        public static MineData GetByID(string id)
        {
            var all = GetAll();
            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].ID == id)
                    return all[i];
            }

            return default;
        }
    }
}
