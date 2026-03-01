using UnityEngine;
using UnityEngine.SceneManagement;
using ConquerChronicles.Core.Market;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.Market
{
    public class MarketController : MonoBehaviour
    {
        [SerializeField] private MarketSceneUI _marketUI;

        private MarketState _marketState;
        private PlayerBoothState _boothState;
        private SaveManager _saveManager;
        private SaveData _saveData;
        private string _selectedListingID;

        private void Start()
        {
            if (_marketUI == null) return;

            // Get save manager and load save data
            _saveManager = SaveSystemBridge.GetOrCreate();
            _saveData = _saveManager.LoadGame();
            if (_saveData == null)
                _saveData = SaveData.CreateDefault();

            // Create MarketState
            _marketState = new MarketState();

            // Restore PlayerBoothState from save data
            _boothState = new PlayerBoothState();
            if (_saveData.PlayerBoothItemIDs != null && _saveData.PlayerBoothPrices != null)
            {
                int count = Mathf.Min(_saveData.PlayerBoothItemIDs.Length, _saveData.PlayerBoothPrices.Length);
                for (int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrEmpty(_saveData.PlayerBoothItemIDs[i]))
                    {
                        _boothState.ListItem(_saveData.PlayerBoothItemIDs[i], _saveData.PlayerBoothPrices[i]);
                    }
                }
            }
            _boothState.Revenue = _saveData.BoothRevenue;

            // Simulate AI booth purchases based on elapsed time
            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long lastSaveTimestamp = _saveData.MiningStartTimestamp; // use mining timestamp as a proxy for last save time
            long elapsed = now - lastSaveTimestamp;
            if (elapsed > 0 && _boothState.ListedItems.Count > 0)
            {
                int seed = (int)(now ^ lastSaveTimestamp);
                _boothState.SimulateAIPurchases(elapsed, seed);
            }

            // Check if market needs refresh and generate new listings if so
            if (_marketState.NeedsRefresh(now))
            {
                int seed = (int)(now / MarketState.RefreshIntervalSeconds);
                var listings = MarketResolver.GenerateListings(12, seed);
                _marketState.SetListings(listings, now);
            }

            // Initialize UI
            _marketUI.Initialize();

            // Wire UI events
            _marketUI.OnBackPressed = () =>
            {
                SaveState();
                SceneManager.LoadScene("MainMenu");
            };

            _marketUI.OnBuyTabPressed = () =>
            {
                _marketUI.ShowBuyTab();
            };

            _marketUI.OnBoothTabPressed = () =>
            {
                _marketUI.ShowBoothTab();
                _marketUI.RefreshBooth(_boothState);
            };

            _marketUI.OnListingPressed = (listingID) =>
            {
                var listings = _marketState.GetListings();
                MarketListing? found = null;
                for (int i = 0; i < listings.Count; i++)
                {
                    if (listings[i].ListingID == listingID)
                    {
                        found = listings[i];
                        break;
                    }
                }

                if (found.HasValue)
                {
                    _selectedListingID = listingID;
                    bool canAfford = _saveData.Gold >= found.Value.Price;
                    _marketUI.ShowListingDetail(found.Value, canAfford);
                }
            };

            _marketUI.OnBuyPressed = () =>
            {
                if (string.IsNullOrEmpty(_selectedListingID)) return;

                // Find the listing
                var listings = _marketState.GetListings();
                MarketListing? found = null;
                for (int i = 0; i < listings.Count; i++)
                {
                    if (listings[i].ListingID == _selectedListingID)
                    {
                        found = listings[i];
                        break;
                    }
                }

                if (!found.HasValue) return;

                var listing = found.Value;

                // Validate gold
                if (_saveData.Gold < listing.Price) return;

                // Deduct gold
                _saveData.Gold -= listing.Price;

                // Remove listing from market state
                _marketState.RemoveListing(_selectedListingID);

                // Add item to save data based on type
                switch (listing.Type)
                {
                    case MarketListingType.Gem:
                        AddGemToSave(listing.GemType, listing.GemTier, listing.Quantity);
                        break;
                    case MarketListingType.Equipment:
                        AddEquipmentToSave(listing.ItemID);
                        break;
                    case MarketListingType.UpgradeMaterial:
                        Debug.Log($"[Market] Purchased upgrade material: {listing.ItemName} x{listing.Quantity}");
                        break;
                }

                _selectedListingID = null;
                _marketUI.HideListingDetail();

                SaveState();
                RefreshAll();

                Debug.Log($"[Market] Purchased: {listing.ItemName} for {listing.Price} gold");
            };

            _marketUI.OnCloseListingPressed = () =>
            {
                _selectedListingID = null;
                _marketUI.HideListingDetail();
            };

            _marketUI.OnCollectRevenuePressed = () =>
            {
                int revenue = _boothState.CollectRevenue();
                if (revenue > 0)
                {
                    _saveData.Gold += revenue;
                    SaveState();
                    RefreshAll();
                    Debug.Log($"[Market] Collected booth revenue: {revenue} gold");
                }
            };

            RefreshAll();
        }

        private void Update()
        {
            // Update refresh timer display
            if (_marketState == null) return;

            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsed = now - _marketState.LastRefreshTimestamp;
            int secondsUntilRefresh = MarketState.RefreshIntervalSeconds - (int)elapsed;

            if (secondsUntilRefresh <= 0)
            {
                // Time to refresh
                int seed = (int)(now / MarketState.RefreshIntervalSeconds);
                var listings = MarketResolver.GenerateListings(12, seed);
                _marketState.SetListings(listings, now);
                RefreshAll();
            }

            _marketUI.RefreshTimer(secondsUntilRefresh);
        }

        private void RefreshAll()
        {
            if (_marketUI == null) return;

            _marketUI.RefreshListings(_marketState.GetListings());
            _marketUI.RefreshGold(_saveData.Gold);
            _marketUI.RefreshBooth(_boothState);

            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long elapsed = now - _marketState.LastRefreshTimestamp;
            int secondsUntilRefresh = MarketState.RefreshIntervalSeconds - (int)elapsed;
            _marketUI.RefreshTimer(secondsUntilRefresh);
        }

        private void SaveState()
        {
            // Write booth state back to save data
            int boothCount = _boothState.ListedItems.Count;
            _saveData.PlayerBoothItemIDs = new string[boothCount];
            _saveData.PlayerBoothPrices = new int[boothCount];
            for (int i = 0; i < boothCount; i++)
            {
                _saveData.PlayerBoothItemIDs[i] = _boothState.ListedItems[i].ItemName;
                _saveData.PlayerBoothPrices[i] = _boothState.ListedItems[i].Price;
            }
            _saveData.BoothRevenue = _boothState.Revenue;

            _saveManager.SaveGame(_saveData);
        }

        private void AddGemToSave(int gemType, int gemTier, int quantity)
        {
            var currentGems = _saveData.GemBag ?? System.Array.Empty<SerializedGem>();
            var newGems = new SerializedGem[currentGems.Length + quantity];
            System.Array.Copy(currentGems, newGems, currentGems.Length);

            for (int i = 0; i < quantity; i++)
            {
                newGems[currentGems.Length + i] = new SerializedGem
                {
                    Type = gemType,
                    Tier = gemTier
                };
            }

            _saveData.GemBag = newGems;
        }

        private void AddEquipmentToSave(string itemID)
        {
            var currentBag = _saveData.BagItems ?? System.Array.Empty<SerializedEquipment>();
            var newBag = new SerializedEquipment[currentBag.Length + 1];
            System.Array.Copy(currentBag, newBag, currentBag.Length);

            newBag[currentBag.Length] = new SerializedEquipment
            {
                DataID = itemID,
                UpgradeLevel = 0,
                Gems = System.Array.Empty<SerializedGem>()
            };

            _saveData.BagItems = newBag;
            Debug.Log($"[Market] Added equipment to bag: {itemID}");
        }
    }
}
