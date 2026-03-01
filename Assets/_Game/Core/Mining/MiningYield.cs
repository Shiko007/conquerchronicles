using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Mining
{
    [System.Serializable]
    public struct MiningYield
    {
        public int Gold;
        public GemData[] Gems;
        public OreData[] Ores;
        public bool IsEmpty; // true if nothing mined yet
    }
}
