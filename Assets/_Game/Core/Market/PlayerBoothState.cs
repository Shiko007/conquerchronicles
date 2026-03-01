using System;
using System.Collections.Generic;

namespace ConquerChronicles.Core.Market
{
    [System.Serializable]
    public class PlayerBoothState
    {
        public const int MaxListedItems = 10;

        public List<BoothItem> ListedItems = new List<BoothItem>();
        public int Revenue;

        public void ListItem(string itemName, int price)
        {
            if (ListedItems.Count >= MaxListedItems)
                return;

            ListedItems.Add(new BoothItem
            {
                ItemName = itemName,
                Price = price,
                ListedTimestamp = 0
            });
        }

        public void RemoveItem(int index)
        {
            if (index < 0 || index >= ListedItems.Count)
                return;

            ListedItems.RemoveAt(index);
        }

        public int CollectRevenue()
        {
            int collected = Revenue;
            Revenue = 0;
            return collected;
        }

        public void SimulateAIPurchases(long elapsedSeconds, int seed)
        {
            if (ListedItems.Count == 0)
                return;

            var rng = new Random(seed);

            // Roughly 1 sale per 2 hours (7200 seconds) per item
            // Probability = elapsedSeconds / 7200 per item
            for (int i = ListedItems.Count - 1; i >= 0; i--)
            {
                double saleProbability = (double)elapsedSeconds / 7200.0;
                double roll = rng.NextDouble();

                if (roll < saleProbability)
                {
                    Revenue += ListedItems[i].Price;
                    ListedItems.RemoveAt(i);
                }
            }
        }

        [System.Serializable]
        public struct BoothItem
        {
            public string ItemName;
            public int Price;
            public long ListedTimestamp;
        }
    }
}
