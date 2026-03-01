namespace ConquerChronicles.Core.Market
{
    [System.Serializable]
    public struct MarketListing
    {
        public string ListingID;
        public MarketListingType Type;
        public string ItemID;
        public string ItemName;
        public int Price;
        public int Quantity;
        public int GemType;
        public int GemTier;
    }
}
