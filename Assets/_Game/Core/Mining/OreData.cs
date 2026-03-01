namespace ConquerChronicles.Core.Mining
{
    [System.Serializable]
    public struct OreData
    {
        public OreType Type;
        public int Quantity;

        public OreData(OreType type, int quantity)
        {
            Type = type;
            Quantity = System.Math.Max(1, quantity);
        }
    }
}
