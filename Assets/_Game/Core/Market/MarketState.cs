using System.Collections.Generic;

namespace ConquerChronicles.Core.Market
{
    [System.Serializable]
    public class MarketState
    {
        public List<MarketListing> ActiveListings = new List<MarketListing>();
        public long LastRefreshTimestamp;

        public const int RefreshIntervalSeconds = 14400; // 4 hours

        public bool NeedsRefresh(long currentTimestamp)
        {
            return currentTimestamp - LastRefreshTimestamp >= RefreshIntervalSeconds;
        }

        public void SetListings(List<MarketListing> listings, long timestamp)
        {
            ActiveListings = new List<MarketListing>(listings);
            LastRefreshTimestamp = timestamp;
        }

        public void RemoveListing(string listingID)
        {
            for (int i = ActiveListings.Count - 1; i >= 0; i--)
            {
                if (ActiveListings[i].ListingID == listingID)
                {
                    ActiveListings.RemoveAt(i);
                    return;
                }
            }
        }

        public List<MarketListing> GetListings()
        {
            return new List<MarketListing>(ActiveListings);
        }
    }
}
