using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;

namespace ConquerChronicles.Gameplay.Equipment
{
    public class EquipmentSceneUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _goldText;

        [Header("Equipped Slots (7)")]
        [SerializeField] private Button[] _slotButtons;       // 7 buttons
        [SerializeField] private TextMeshProUGUI[] _slotTexts; // 7 texts showing item name

        [Header("Stats Panel")]
        [SerializeField] private TextMeshProUGUI _statsText;

        [Header("Bag Panel")]
        [SerializeField] private Transform _bagContainer;
        [SerializeField] private TextMeshProUGUI _bagCountText;

        [Header("Item Detail Panel")]
        [SerializeField] private GameObject _detailPanel;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemStatsText;
        [SerializeField] private TextMeshProUGUI _itemInfoText;     // quality, level req, sockets
        [SerializeField] private Button _equipButton;
        [SerializeField] private TextMeshProUGUI _equipButtonText;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TextMeshProUGUI _upgradeRateText;
        [SerializeField] private Button _closeDetailButton;

        [Header("Gem Panel")]
        [SerializeField] private TextMeshProUGUI _gemBagCountText;
        [SerializeField] private TextMeshProUGUI _gemListText;      // shows gems in bag as text list

        // Events
        public System.Action OnBackPressed;
        public System.Action<int> OnSlotPressed;          // slot index 0-6
        public System.Action<int> OnBagItemPressed;       // bag index
        public System.Action OnEquipPressed;
        public System.Action OnUpgradePressed;
        public System.Action OnCloseDetailPressed;

        private static readonly string[] SlotLabels = { "Head", "Neck", "Armor", "Weapon", "Shield", "Ring", "Boots" };

        // Bag item buttons created at runtime
        private readonly List<Button> _bagItemButtons = new List<Button>();

        public void Initialize()
        {
            // Wire header buttons
            _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

            // Wire slot buttons
            for (int i = 0; i < _slotButtons.Length; i++)
            {
                int index = i;
                _slotButtons[i].onClick.AddListener(() => OnSlotPressed?.Invoke(index));
            }

            // Wire detail panel buttons
            _equipButton.onClick.AddListener(() => OnEquipPressed?.Invoke());
            _upgradeButton.onClick.AddListener(() => OnUpgradePressed?.Invoke());
            _closeDetailButton.onClick.AddListener(() => OnCloseDetailPressed?.Invoke());

            HideItemDetail();
        }

        public void RefreshEquippedSlots(EquipmentInstance[] equipped)
        {
            for (int i = 0; i < _slotTexts.Length; i++)
            {
                if (i < equipped.Length && equipped[i] != null)
                {
                    var item = equipped[i];
                    string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
                    _slotTexts[i].text = $"{SlotLabels[i]}: {item.Data.Name}{upgradeStr}";
                }
                else
                {
                    _slotTexts[i].text = $"{SlotLabels[i]}: Empty";
                }
            }
        }

        public void RefreshStats(CharacterStats stats)
        {
            _statsText.text =
                $"HP: {stats.HP}\n" +
                $"MP: {stats.MP}\n" +
                $"ATK: {stats.ATK}\n" +
                $"DEF: {stats.DEF}\n" +
                $"MATK: {stats.MATK}\n" +
                $"MDEF: {stats.MDEF}\n" +
                $"AGI: {stats.AGI}\n" +
                $"Crit%: {(stats.CritRate * 100f):F1}%";
        }

        public void RefreshBag(List<EquipmentInstance> bag)
        {
            _bagCountText.text = $"Bag: {bag.Count}/{InventoryState.BagCapacity}";

            // Clear existing bag item buttons
            foreach (var btn in _bagItemButtons)
            {
                if (btn != null)
                    Destroy(btn.gameObject);
            }
            _bagItemButtons.Clear();

            // Create text list entries for each bag item
            for (int i = 0; i < bag.Count; i++)
            {
                int index = i;
                var item = bag[i];
                string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
                string label = $"{item.Data.Name}{upgradeStr} [{item.Data.Quality}]";

                var itemGO = new GameObject($"BagItem_{i}", typeof(RectTransform));
                itemGO.transform.SetParent(_bagContainer, false);

                var rt = itemGO.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(0, 48);

                var layoutElem = itemGO.AddComponent<LayoutElement>();
                layoutElem.preferredHeight = 48;
                layoutElem.flexibleWidth = 1;

                var img = itemGO.AddComponent<Image>();
                img.color = new Color(0.15f, 0.15f, 0.22f, 0.8f);

                var btn = itemGO.AddComponent<Button>();
                btn.targetGraphic = img;
                btn.onClick.AddListener(() => OnBagItemPressed?.Invoke(index));

                var textGO = new GameObject("Text", typeof(RectTransform));
                textGO.transform.SetParent(itemGO.transform, false);
                var textRT = textGO.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = new Vector2(10, 0);
                textRT.offsetMax = new Vector2(-10, 0);

                var tmp = textGO.AddComponent<TextMeshProUGUI>();
                tmp.text = label;
                tmp.fontSize = 22;
                tmp.color = Color.white;
                tmp.alignment = TextAlignmentOptions.MidlineLeft;

                _bagItemButtons.Add(btn);
            }
        }

        public void RefreshGems(List<GemData> gemBag)
        {
            _gemBagCountText.text = $"Gems: {gemBag.Count}/{InventoryState.GemBagCapacity}";

            if (gemBag.Count == 0)
            {
                _gemListText.text = "No gems";
                return;
            }

            // Group gems by type and tier for a compact display
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < gemBag.Count; i++)
            {
                if (i > 0) sb.Append("\n");
                sb.Append($"{gemBag[i].Type} Gem (Tier {gemBag[i].Tier})");
            }
            _gemListText.text = sb.ToString();
        }

        public void RefreshGold(int gold)
        {
            _goldText.text = $"{gold} Gold";
        }

        public void ShowItemDetail(EquipmentInstance item, bool canEquip, bool isEquipped)
        {
            _detailPanel.SetActive(true);

            // Item name with upgrade level
            string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
            _itemNameText.text = $"{item.Data.Name}{upgradeStr}";

            // Computed stats
            var stats = item.ComputeStats();
            var sb = new System.Text.StringBuilder();
            if (stats.HP != 0) sb.AppendLine($"HP: {stats.HP}");
            if (stats.MP != 0) sb.AppendLine($"MP: {stats.MP}");
            if (stats.ATK != 0) sb.AppendLine($"ATK: {stats.ATK}");
            if (stats.DEF != 0) sb.AppendLine($"DEF: {stats.DEF}");
            if (stats.MATK != 0) sb.AppendLine($"MATK: {stats.MATK}");
            if (stats.MDEF != 0) sb.AppendLine($"MDEF: {stats.MDEF}");
            if (stats.AGI != 0) sb.AppendLine($"AGI: {stats.AGI}");
            if (stats.CritRate > 0f) sb.AppendLine($"Crit%: {(stats.CritRate * 100f):F1}%");
            _itemStatsText.text = sb.ToString().TrimEnd();

            // Info: quality, level req, sockets
            int socketedCount = 0;
            for (int i = 0; i < item.SocketedGems.Length; i++)
            {
                if (item.SocketedGems[i].Tier > 0) socketedCount++;
            }
            _itemInfoText.text =
                $"Quality: {item.Data.Quality}\n" +
                $"Req Lv: {item.Data.RequiredLevel}\n" +
                $"Sockets: {socketedCount}/{item.Data.MaxSockets}";

            // Equip / Unequip button
            if (isEquipped)
            {
                _equipButtonText.text = "Unequip";
                _equipButton.interactable = true;
            }
            else if (canEquip)
            {
                _equipButtonText.text = "Equip";
                _equipButton.interactable = true;
            }
            else
            {
                _equipButtonText.text = "Equip";
                _equipButton.interactable = false;
            }

            // Upgrade rate
            if (item.UpgradeLevel < 12)
            {
                float rate = UpgradeCalculator.GetSuccessRate(item.UpgradeLevel);
                _upgradeRateText.text = $"Success: {(rate * 100f):F0}%";
                _upgradeButton.interactable = true;
            }
            else
            {
                _upgradeRateText.text = "MAX LEVEL";
                _upgradeButton.interactable = false;
            }
        }

        public void HideItemDetail()
        {
            _detailPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            // Remove all listeners
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();

            if (_slotButtons != null)
            {
                for (int i = 0; i < _slotButtons.Length; i++)
                {
                    if (_slotButtons[i] != null)
                        _slotButtons[i].onClick.RemoveAllListeners();
                }
            }

            if (_equipButton != null) _equipButton.onClick.RemoveAllListeners();
            if (_upgradeButton != null) _upgradeButton.onClick.RemoveAllListeners();
            if (_closeDetailButton != null) _closeDetailButton.onClick.RemoveAllListeners();

            foreach (var btn in _bagItemButtons)
            {
                if (btn != null) btn.onClick.RemoveAllListeners();
            }
        }

    }
}
