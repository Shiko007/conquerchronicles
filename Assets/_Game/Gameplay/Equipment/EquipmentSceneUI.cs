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
        [SerializeField] private Button _statsButton;
        [SerializeField] private GameObject _statsPanel;
        [SerializeField] private TextMeshProUGUI _statsText;
        [SerializeField] private Button _closeStatsButton;

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

        // Events
        public System.Action OnBackPressed;
        public System.Action<int> OnSlotPressed;          // slot index 0-6
        public System.Action<int> OnBagItemPressed;       // bag index
        public System.Action OnEquipPressed;
        public System.Action OnUpgradePressed;
        public System.Action OnCloseDetailPressed;

        private static readonly string[] SlotLabels = { "Head", "Neck", "Armor", "L.Hand", "R.Hand", "Ring", "Boots" };

        // Bag slot GameObjects created at runtime
        private readonly List<GameObject> _bagSlotObjects = new List<GameObject>();

        // Color maps
        private static readonly Dictionary<EquipmentQuality, Color> QualityColors = new()
        {
            { EquipmentQuality.Normal,  new Color(0.5f, 0.5f, 0.5f) },
            { EquipmentQuality.Refined, new Color(0.3f, 0.7f, 0.3f) },
            { EquipmentQuality.Unique,  new Color(0.3f, 0.5f, 0.9f) },
            { EquipmentQuality.Elite,   new Color(0.6f, 0.3f, 0.8f) },
            { EquipmentQuality.Super,   new Color(1.0f, 0.85f, 0.2f) },
        };

        private static readonly Dictionary<GemType, Color> GemColors = new()
        {
            { GemType.Dragon,   new Color(0.8f, 0.2f, 0.2f) },
            { GemType.Phoenix,  new Color(0.9f, 0.5f, 0.1f) },
            { GemType.Moon,     new Color(0.4f, 0.6f, 0.9f) },
            { GemType.Fury,     new Color(0.9f, 0.2f, 0.4f) },
            { GemType.Tortoise, new Color(0.3f, 0.7f, 0.4f) },
            { GemType.Thunder,  new Color(0.9f, 0.9f, 0.3f) },
            { GemType.Violet,   new Color(0.7f, 0.3f, 0.9f) },
            { GemType.Rainbow,  new Color(0.9f, 0.5f, 0.9f) },
        };

        private static readonly Color EmptySlotColor = new Color(0.12f, 0.12f, 0.18f, 0.6f);

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

            // Wire stats button
            _statsButton.onClick.AddListener(() => _statsPanel.SetActive(true));
            _closeStatsButton.onClick.AddListener(() => _statsPanel.SetActive(false));

            // Wire detail panel buttons
            _equipButton.onClick.AddListener(() => OnEquipPressed?.Invoke());
            _upgradeButton.onClick.AddListener(() => OnUpgradePressed?.Invoke());
            _closeDetailButton.onClick.AddListener(() => OnCloseDetailPressed?.Invoke());

            HideItemDetail();
            _statsPanel.SetActive(false);
        }

        public void RefreshEquippedSlots(EquipmentInstance[] equipped)
        {
            for (int i = 0; i < _slotTexts.Length; i++)
            {
                var btnImg = _slotButtons[i].targetGraphic as Image;

                if (i < equipped.Length && equipped[i] != null)
                {
                    var item = equipped[i];
                    string upgradeStr = item.UpgradeLevel > 0 ? $"\n+{item.UpgradeLevel}" : "";
                    _slotTexts[i].text = $"{SlotLabels[i]}{upgradeStr}";

                    if (btnImg != null)
                        btnImg.color = GetEquipmentQualityColor(item.Data.Quality);
                }
                else
                {
                    _slotTexts[i].text = SlotLabels[i];

                    if (btnImg != null)
                        btnImg.color = EmptySlotColor;
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

        public void RefreshBag(List<BagItem> bag)
        {
            _bagCountText.text = $"Bag: {bag.Count}/{InventoryState.BagCapacity}";

            // Clear existing bag slot objects
            foreach (var go in _bagSlotObjects)
            {
                if (go != null)
                    Destroy(go);
            }
            _bagSlotObjects.Clear();

            // Always show all 50 slots
            int totalSlots = InventoryState.BagCapacity;

            for (int i = 0; i < totalSlots; i++)
            {
                int index = i;
                bool hasItem = i < bag.Count;

                var slotGO = new GameObject($"BagSlot_{i}", typeof(RectTransform));
                slotGO.transform.SetParent(_bagContainer, false);

                var img = slotGO.AddComponent<Image>();

                if (hasItem)
                {
                    var bagItem = bag[i];
                    img.color = GetSlotColor(bagItem);

                    var btn = slotGO.AddComponent<Button>();
                    btn.targetGraphic = img;
                    btn.onClick.AddListener(() => OnBagItemPressed?.Invoke(index));

                    // Label text
                    var textGO = new GameObject("Label", typeof(RectTransform));
                    textGO.transform.SetParent(slotGO.transform, false);
                    var textRT = textGO.GetComponent<RectTransform>();
                    textRT.anchorMin = Vector2.zero;
                    textRT.anchorMax = Vector2.one;
                    textRT.offsetMin = new Vector2(2, 2);
                    textRT.offsetMax = new Vector2(-2, -2);

                    var tmp = textGO.AddComponent<TextMeshProUGUI>();
                    tmp.text = GetSlotLabel(bagItem);
                    tmp.fontSize = 18;
                    tmp.color = Color.white;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.textWrappingMode = TextWrappingModes.NoWrap;
                    tmp.overflowMode = TextOverflowModes.Truncate;
                }
                else
                {
                    img.color = EmptySlotColor;
                }

                _bagSlotObjects.Add(slotGO);
            }
        }

        public void RefreshGold(int gold)
        {
            _goldText.text = $"{gold} Gold";
        }

        /// <summary>
        /// Show detail for an equipped item (EquipmentInstance directly).
        /// </summary>
        public void ShowItemDetail(EquipmentInstance item, bool canEquip, bool isEquipped)
        {
            _detailPanel.SetActive(true);

            // Item name with upgrade level
            string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
            _itemNameText.text = $"{item.Data.Name}{upgradeStr}";
            _itemNameText.color = GetEquipmentQualityColor(item.Data.Quality);

            // Computed stats
            var stats = item.ComputeStats();
            _itemStatsText.text = FormatStats(stats);

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
                _equipButton.gameObject.SetActive(true);
            }
            else if (canEquip)
            {
                _equipButtonText.text = "Equip";
                _equipButton.interactable = true;
                _equipButton.gameObject.SetActive(true);
            }
            else
            {
                _equipButtonText.text = "Equip";
                _equipButton.interactable = false;
                _equipButton.gameObject.SetActive(true);
            }

            // Upgrade button visible for equipment
            _upgradeButton.gameObject.SetActive(true);
            _upgradeRateText.gameObject.SetActive(true);
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

        /// <summary>
        /// Show detail for a bag item (equipment or gem).
        /// </summary>
        public void ShowItemDetail(BagItem bagItem, bool canEquip, bool isEquipped)
        {
            if (bagItem.Type == BagItemType.Equipment)
            {
                ShowItemDetail(bagItem.Equipment, canEquip, isEquipped);
                return;
            }

            // Gem detail
            _detailPanel.SetActive(true);
            var gem = bagItem.Gem;

            _itemNameText.text = $"{gem.Type} Gem";
            _itemNameText.color = GetGemTypeColor(gem.Type);

            // Gem stats
            var bonus = gem.GetBonus();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Tier: {gem.Tier}");
            sb.AppendLine("");
            string bonusText = FormatStats(bonus);
            if (string.IsNullOrEmpty(bonusText))
                sb.Append("XP Bonus");
            else
                sb.Append(bonusText);
            _itemStatsText.text = sb.ToString().TrimEnd();

            _itemInfoText.text = $"Type: {gem.Type}\nTier: {gem.Tier}/9";

            // Hide equip and upgrade buttons for gems
            _equipButton.gameObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(false);
            _upgradeRateText.gameObject.SetActive(false);
        }

        public void HideItemDetail()
        {
            _detailPanel.SetActive(false);
        }

        // --- Helpers ---

        private static Color GetSlotColor(BagItem item)
        {
            if (item.Type == BagItemType.Equipment)
                return GetEquipmentQualityColor(item.Equipment.Data.Quality);
            return GetGemTypeColor(item.Gem.Type);
        }

        private static Color GetEquipmentQualityColor(EquipmentQuality quality)
        {
            return QualityColors.TryGetValue(quality, out var c) ? c : Color.gray;
        }

        private static Color GetGemTypeColor(GemType gemType)
        {
            return GemColors.TryGetValue(gemType, out var c) ? c : Color.gray;
        }

        private static string GetSlotLabel(BagItem item)
        {
            if (item.Type == BagItemType.Equipment)
            {
                var eq = item.Equipment;
                // Short name: first 3 chars + upgrade
                string name = eq.Data.Name.Length > 4 ? eq.Data.Name.Substring(0, 4) : eq.Data.Name;
                string upgrade = eq.UpgradeLevel > 0 ? $"\n+{eq.UpgradeLevel}" : "";
                return $"{name}{upgrade}";
            }
            else
            {
                // Gem: type initial + tier
                string initial = item.Gem.Type.ToString().Substring(0, 1);
                return $"{initial}{item.Gem.Tier}";
            }
        }

        private static string FormatStats(CharacterStats stats)
        {
            var sb = new System.Text.StringBuilder();
            if (stats.HP != 0) sb.AppendLine($"HP: {stats.HP}");
            if (stats.MP != 0) sb.AppendLine($"MP: {stats.MP}");
            if (stats.ATK != 0) sb.AppendLine($"ATK: {stats.ATK}");
            if (stats.DEF != 0) sb.AppendLine($"DEF: {stats.DEF}");
            if (stats.MATK != 0) sb.AppendLine($"MATK: {stats.MATK}");
            if (stats.MDEF != 0) sb.AppendLine($"MDEF: {stats.MDEF}");
            if (stats.AGI != 0) sb.AppendLine($"AGI: {stats.AGI}");
            if (stats.CritRate > 0f) sb.AppendLine($"Crit%: {(stats.CritRate * 100f):F1}%");
            return sb.ToString().TrimEnd();
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

            if (_statsButton != null) _statsButton.onClick.RemoveAllListeners();
            if (_closeStatsButton != null) _closeStatsButton.onClick.RemoveAllListeners();
            if (_equipButton != null) _equipButton.onClick.RemoveAllListeners();
            if (_upgradeButton != null) _upgradeButton.onClick.RemoveAllListeners();
            if (_closeDetailButton != null) _closeDetailButton.onClick.RemoveAllListeners();
        }

    }
}
