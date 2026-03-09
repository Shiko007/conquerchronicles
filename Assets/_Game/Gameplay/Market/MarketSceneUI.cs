using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Market;

namespace ConquerChronicles.Gameplay.Market
{
    public class MarketSceneUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _goldText;

        [Header("Tab Buttons")]
        [SerializeField] private Button _buyTabButton;
        [SerializeField] private Button _boothTabButton;

        [Header("Buy Panel")]
        [SerializeField] private GameObject _buyPanel;
        [SerializeField] private Transform _listingsContainer;
        [SerializeField] private TextMeshProUGUI _refreshTimerText;

        [Header("Listing Detail")]
        [SerializeField] private GameObject _listingDetailPanel;
        [SerializeField] private TextMeshProUGUI _listingNameText;
        [SerializeField] private TextMeshProUGUI _listingTypeText;
        [SerializeField] private TextMeshProUGUI _listingPriceText;
        [SerializeField] private Button _buyButton;
        [SerializeField] private TextMeshProUGUI _buyButtonText;
        [SerializeField] private Button _closeListingButton;

        [Header("Booth Panel")]
        [SerializeField] private GameObject _boothPanel;
        [SerializeField] private TextMeshProUGUI _boothRevenueText;
        [SerializeField] private Button _collectRevenueButton;
        [SerializeField] private TextMeshProUGUI _boothItemsText;
        [SerializeField] private TextMeshProUGUI _boothCountText;

        [Header("Notification")]
        [SerializeField] private TextMeshProUGUI _notificationText;

        // Events
        public System.Action OnBackPressed;
        public System.Action OnBuyTabPressed;
        public System.Action OnBoothTabPressed;
        public System.Action<string> OnListingPressed;
        public System.Action OnBuyPressed;
        public System.Action OnCloseListingPressed;
        public System.Action OnCollectRevenuePressed;

        private TextMeshProUGUI[] _listingTexts;

        public void Initialize()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

            if (_buyTabButton != null)
                _buyTabButton.onClick.AddListener(() => OnBuyTabPressed?.Invoke());

            if (_boothTabButton != null)
                _boothTabButton.onClick.AddListener(() => OnBoothTabPressed?.Invoke());

            if (_buyButton != null)
                _buyButton.onClick.AddListener(() => OnBuyPressed?.Invoke());

            if (_closeListingButton != null)
                _closeListingButton.onClick.AddListener(() => OnCloseListingPressed?.Invoke());

            if (_collectRevenueButton != null)
                _collectRevenueButton.onClick.AddListener(() => OnCollectRevenuePressed?.Invoke());

            if (_listingDetailPanel != null)
                _listingDetailPanel.SetActive(false);

            // Show buy panel by default, hide booth panel
            if (_buyPanel != null)
                _buyPanel.SetActive(true);

            if (_boothPanel != null)
                _boothPanel.SetActive(false);

            // Cache listing text entries from the container
            // Each child is a Button GO with a child TextMeshProUGUI
            if (_listingsContainer != null)
            {
                _listingTexts = new TextMeshProUGUI[_listingsContainer.childCount];
                for (int i = 0; i < _listingsContainer.childCount; i++)
                {
                    var child = _listingsContainer.GetChild(i);
                    _listingTexts[i] = child.GetComponentInChildren<TextMeshProUGUI>();

                    int index = i;
                    var btn = child.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.onClick.AddListener(() =>
                        {
                            string listingID = _listingsContainer.GetChild(index).gameObject.name;
                            OnListingPressed?.Invoke(listingID);
                        });
                    }
                }
            }
        }

        public void ShowBuyTab()
        {
            if (_buyPanel != null)
                _buyPanel.SetActive(true);

            if (_boothPanel != null)
                _boothPanel.SetActive(false);
        }

        public void ShowBoothTab()
        {
            if (_boothPanel != null)
                _boothPanel.SetActive(true);

            if (_buyPanel != null)
                _buyPanel.SetActive(false);
        }

        public void RefreshListings(System.Collections.Generic.List<MarketListing> listings)
        {
            if (_listingTexts == null) return;

            for (int i = 0; i < _listingTexts.Length; i++)
            {
                if (_listingTexts[i] == null) continue;

                // The parent GO holds the Button + listing ID; child TMP holds display text
                var parentGO = _listingsContainer.GetChild(i).gameObject;

                if (i < listings.Count)
                {
                    var listing = listings[i];
                    string typeLabel;
                    switch (listing.Type)
                    {
                        case MarketListingType.Equipment:
                            typeLabel = "[Equip]";
                            break;
                        case MarketListingType.Gem:
                            typeLabel = "[Gem]";
                            break;
                        case MarketListingType.UpgradeMaterial:
                            typeLabel = "[Mat]";
                            break;
                        default:
                            typeLabel = "[???]";
                            break;
                    }

                    string qtyStr = listing.Quantity > 1 ? $" x{listing.Quantity}" : "";
                    _listingTexts[i].text = $"{typeLabel} {listing.ItemName}{qtyStr}  —  {listing.Price:N0} Gold";
                    parentGO.name = listing.ListingID;
                    parentGO.SetActive(true);
                }
                else
                {
                    _listingTexts[i].text = "";
                    parentGO.SetActive(false);
                }
            }
        }

        public void RefreshGold(int gold)
        {
            if (_goldText != null)
                _goldText.text = $"{gold:N0} Gold";
        }

        public void RefreshTimer(int secondsUntilRefresh)
        {
            if (_refreshTimerText == null) return;

            if (secondsUntilRefresh <= 0)
            {
                _refreshTimerText.text = "Refreshing...";
            }
            else
            {
                int h = secondsUntilRefresh / 3600;
                int m = (secondsUntilRefresh % 3600) / 60;
                int s = secondsUntilRefresh % 60;
                _refreshTimerText.text = $"Refresh in: {h}:{m:00}:{s:00}";
            }
        }

        public void ShowListingDetail(MarketListing listing, bool canAfford)
        {
            if (_listingDetailPanel != null)
                _listingDetailPanel.SetActive(true);

            if (_listingNameText != null)
                _listingNameText.text = listing.ItemName;

            if (_listingTypeText != null)
            {
                switch (listing.Type)
                {
                    case MarketListingType.Equipment:
                        _listingTypeText.text = "Equipment";
                        break;
                    case MarketListingType.Gem:
                        _listingTypeText.text = $"Gem (Tier {listing.GemTier})";
                        break;
                    case MarketListingType.UpgradeMaterial:
                        _listingTypeText.text = "Upgrade Material";
                        break;
                    default:
                        _listingTypeText.text = "Unknown";
                        break;
                }
            }

            if (_listingPriceText != null)
                _listingPriceText.text = $"{listing.Price:N0} Gold";

            if (_buyButton != null)
                _buyButton.interactable = canAfford;

            if (_buyButtonText != null)
                _buyButtonText.text = canAfford ? "Buy" : "Not Enough Gold";
        }

        public void HideListingDetail()
        {
            if (_listingDetailPanel != null)
                _listingDetailPanel.SetActive(false);
        }

        public void RefreshBooth(PlayerBoothState booth)
        {
            if (booth == null) return;

            if (_boothRevenueText != null)
                _boothRevenueText.text = $"Uncollected Revenue: {booth.Revenue:N0} Gold";

            if (_collectRevenueButton != null)
                _collectRevenueButton.interactable = booth.Revenue > 0;

            if (_boothItemsText != null)
            {
                if (booth.ListedItems.Count == 0)
                {
                    _boothItemsText.text = "No items listed.";
                }
                else
                {
                    var sb = new System.Text.StringBuilder();
                    for (int i = 0; i < booth.ListedItems.Count; i++)
                    {
                        var item = booth.ListedItems[i];
                        sb.AppendLine($"  {item.ItemName}  —  {item.Price:N0} Gold");
                    }
                    _boothItemsText.text = sb.ToString();
                }
            }

            if (_boothCountText != null)
                _boothCountText.text = $"{booth.ListedItems.Count}/{PlayerBoothState.MaxListedItems} items listed";
        }

        public void ShowNotification(string message)
        {
            if (_notificationText == null) return;
            _notificationText.text = message;
            _notificationText.gameObject.SetActive(true);
            StopCoroutine(nameof(HideNotificationAfterDelay));
            StartCoroutine(HideNotificationAfterDelay());
        }

        private IEnumerator HideNotificationAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            if (_notificationText != null)
                _notificationText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
            if (_buyTabButton != null) _buyTabButton.onClick.RemoveAllListeners();
            if (_boothTabButton != null) _boothTabButton.onClick.RemoveAllListeners();
            if (_buyButton != null) _buyButton.onClick.RemoveAllListeners();
            if (_closeListingButton != null) _closeListingButton.onClick.RemoveAllListeners();
            if (_collectRevenueButton != null) _collectRevenueButton.onClick.RemoveAllListeners();

            // Clean up listing button listeners
            if (_listingsContainer != null)
            {
                for (int i = 0; i < _listingsContainer.childCount; i++)
                {
                    var btn = _listingsContainer.GetChild(i).GetComponent<Button>();
                    if (btn != null) btn.onClick.RemoveAllListeners();
                }
            }
        }
    }
}
