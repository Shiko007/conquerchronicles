namespace ConquerChronicles.Core.Map
{
    public struct GoldDropInfo
    {
        public int Amount;
        public float WorldX;
        public float WorldY;

        public GoldDropInfo(int amount, float worldX, float worldY)
        {
            Amount = amount;
            WorldX = worldX;
            WorldY = worldY;
        }
    }

    public struct LootDropInfo
    {
        public string ItemID;
        public int Quantity;
        public float WorldX;
        public float WorldY;

        public LootDropInfo(string itemID, int quantity, float worldX, float worldY)
        {
            ItemID = itemID;
            Quantity = quantity;
            WorldX = worldX;
            WorldY = worldY;
        }
    }
}
