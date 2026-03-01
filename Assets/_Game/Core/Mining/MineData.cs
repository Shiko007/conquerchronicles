using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Mining
{
    [System.Serializable]
    public struct MineData
    {
        public string ID;
        public string Name;
        public int RequiredLevel;
        public int DurationSeconds;
        public int MinGold;
        public int MaxGold;
        public int MinGemTier;
        public int MaxGemTier;
        public int GemDropCount; // how many gems per mining session (1-3)
        public string Description;
        public GemType[] AvailableGems;   // which gem types this mine can drop
        public OreType[] AvailableOres;   // which ore types this mine can drop
        public int MinOreCount;           // minimum ores per mining session
        public int MaxOreCount;           // maximum ores per mining session
    }
}
