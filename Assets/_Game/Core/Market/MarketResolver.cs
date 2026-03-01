using System;
using System.Collections.Generic;
using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Core.Market
{
    public static class MarketResolver
    {
        private static readonly string[] UpgradeMaterialNames = { "Meteor", "DragonBall" };
        private static readonly int[] UpgradeMaterialPrices = { 1500, 5000 };

        public static List<MarketListing> GenerateListings(int count, int seed)
        {
            var rng = new Random(seed);
            var listings = new List<MarketListing>(count);
            var allEquipment = TestEquipment.GetAll();
            int gemTypeCount = Enum.GetValues(typeof(GemType)).Length;

            for (int i = 0; i < count; i++)
            {
                double roll = rng.NextDouble();
                MarketListing listing;

                if (roll < 0.60)
                {
                    // ~60% equipment
                    var item = allEquipment[rng.Next(allEquipment.Length)];
                    int basePrice = CalculateBasePrice(item);
                    int price = ApplyVariance(basePrice, rng);

                    listing = new MarketListing
                    {
                        ListingID = "mkt_" + seed + "_" + i,
                        Type = MarketListingType.Equipment,
                        ItemID = item.ID,
                        ItemName = item.Name,
                        Price = price,
                        Quantity = 1,
                        GemType = 0,
                        GemTier = 0
                    };
                }
                else if (roll < 0.90)
                {
                    // ~30% gems
                    int gemType = rng.Next(gemTypeCount);
                    int tier = rng.Next(1, 6); // 1-5
                    int basePrice = CalculateGemPrice(tier);
                    int price = ApplyVariance(basePrice, rng);
                    string gemName = ((GemType)gemType).ToString() + " Gem (Tier " + tier + ")";

                    listing = new MarketListing
                    {
                        ListingID = "mkt_" + seed + "_" + i,
                        Type = MarketListingType.Gem,
                        ItemID = "gem_" + gemType + "_t" + tier,
                        ItemName = gemName,
                        Price = price,
                        Quantity = rng.Next(1, 4), // 1-3
                        GemType = gemType,
                        GemTier = tier
                    };
                }
                else
                {
                    // ~10% upgrade materials
                    int matIndex = rng.Next(UpgradeMaterialNames.Length);
                    int basePrice = UpgradeMaterialPrices[matIndex];
                    int price = ApplyVariance(basePrice, rng);

                    listing = new MarketListing
                    {
                        ListingID = "mkt_" + seed + "_" + i,
                        Type = MarketListingType.UpgradeMaterial,
                        ItemID = "mat_" + UpgradeMaterialNames[matIndex].ToLower(),
                        ItemName = UpgradeMaterialNames[matIndex],
                        Price = price,
                        Quantity = rng.Next(1, 6), // 1-5
                        GemType = 0,
                        GemTier = 0
                    };
                }

                listings.Add(listing);
            }

            return listings;
        }

        public static int CalculateBasePrice(EquipmentData item)
        {
            int qualityPrice;

            switch (item.Quality)
            {
                case EquipmentQuality.Normal:
                    qualityPrice = 100;
                    break;
                case EquipmentQuality.Refined:
                    qualityPrice = 500;
                    break;
                case EquipmentQuality.Unique:
                    qualityPrice = 2000;
                    break;
                case EquipmentQuality.Elite:
                    qualityPrice = 8000;
                    break;
                case EquipmentQuality.Super:
                    qualityPrice = 25000;
                    break;
                default:
                    qualityPrice = 100;
                    break;
            }

            return qualityPrice + item.RequiredLevel * 10;
        }

        public static int CalculateGemPrice(int tier)
        {
            return 50 * tier * tier;
        }

        public static int CalculateSellPrice(int buyPrice)
        {
            return (int)(buyPrice * 0.40);
        }

        private static int ApplyVariance(int basePrice, Random rng)
        {
            // +-15% variance
            double variance = 0.85 + rng.NextDouble() * 0.30;
            return Math.Max(1, (int)(basePrice * variance));
        }
    }
}
