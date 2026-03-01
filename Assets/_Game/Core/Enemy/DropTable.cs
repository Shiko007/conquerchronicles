namespace ConquerChronicles.Core.Enemy
{
    [System.Serializable]
    public struct DropEntry
    {
        public string ItemID;
        public float DropRate; // 0.0 to 1.0
        public int MinQuantity;
        public int MaxQuantity;
    }

    [System.Serializable]
    public struct DropTable
    {
        public DropEntry[] Entries;

        public static System.Collections.Generic.List<(string itemID, int quantity)> Roll(DropTable table, int seed)
        {
            var results = new System.Collections.Generic.List<(string, int)>();
            if (table.Entries == null) return results;

            var rng = new System.Random(seed);
            foreach (var entry in table.Entries)
            {
                if ((float)rng.NextDouble() < entry.DropRate)
                {
                    int qty = entry.MinQuantity + rng.Next(entry.MaxQuantity - entry.MinQuantity + 1);
                    results.Add((entry.ItemID, qty));
                }
            }
            return results;
        }
    }
}
