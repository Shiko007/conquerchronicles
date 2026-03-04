using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Gameplay.Equipment
{
    public class EquipmentSceneUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _backButton;

        [Header("Equipped Slots (7)")]
        [SerializeField] private Button[] _slotButtons;       // 7 buttons
        [SerializeField] private TextMeshProUGUI[] _slotTexts; // 7 texts showing item name

        [Header("Tabs")]
        [SerializeField] private Button _equipmentTabButton;
        [SerializeField] private Button _statsTabButton;
        [SerializeField] private Image _equipmentTabImage;
        [SerializeField] private Image _statsTabImage;
        [SerializeField] private TextMeshProUGUI _equipmentTabText;
        [SerializeField] private TextMeshProUGUI _statsTabText;

        [Header("Character Preview")]
        [SerializeField] private Image _characterPreview;
        [SerializeField] private UISpriteAnimator _characterAnimator;

        [Header("Content Panels")]
        [SerializeField] private GameObject _equipmentContent;
        [SerializeField] private GameObject _statsContent;

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

        [Header("Stats - Character Info")]
        [SerializeField] private TextMeshProUGUI _classText;
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Stats - Combat Stats")]
        [SerializeField] private TextMeshProUGUI _statsText;

        [Header("Stats - Stat Points")]
        [SerializeField] private TextMeshProUGUI _statPointsText;
        [SerializeField] private TextMeshProUGUI _vitalityText;
        [SerializeField] private TextMeshProUGUI _strengthText;
        [SerializeField] private TextMeshProUGUI _agilityText;
        [SerializeField] private TextMeshProUGUI _spiritText;
        [SerializeField] private Button _vitalityButton;
        [SerializeField] private Button _strengthButton;
        [SerializeField] private Button _agilityButton;
        [SerializeField] private Button _spiritButton;
        [SerializeField] private Button _vitalityMinusButton;
        [SerializeField] private Button _strengthMinusButton;
        [SerializeField] private Button _agilityMinusButton;
        [SerializeField] private Button _spiritMinusButton;
        [SerializeField] private Button _confirmStatsButton;

        // Events
        public System.Action OnBackPressed;
        public System.Action<int> OnSlotPressed;          // slot index 0-6
        public System.Action OnEquipPressed;
        public System.Action OnUpgradePressed;
        public System.Action OnCloseDetailPressed;
        public System.Action<string> OnAllocateStat;
        public System.Action<string> OnDeallocateStat;
        public System.Action OnConfirmStats;

        private static readonly string[] SlotLabels = { "Head", "Neck", "Armor", "L.Hand", "R.Hand", "Ring", "Boots" };

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

        // Tab colors
        private static readonly Color ActiveTabBg = new Color(0.18f, 0.18f, 0.28f, 1f);
        private static readonly Color InactiveTabBg = new Color(0.1f, 0.1f, 0.16f, 1f);
        private static readonly Color ActiveTabText = new Color(1f, 0.85f, 0.2f, 1f);
        private static readonly Color InactiveTabText = new Color(0.5f, 0.5f, 0.5f, 1f);

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

            // Wire tab buttons
            _equipmentTabButton.onClick.AddListener(() => SwitchToTab(0));
            _statsTabButton.onClick.AddListener(() => SwitchToTab(1));

            // Wire stat allocation buttons
            _vitalityButton.onClick.AddListener(() => OnAllocateStat?.Invoke("Vitality"));
            _strengthButton.onClick.AddListener(() => OnAllocateStat?.Invoke("Strength"));
            _agilityButton.onClick.AddListener(() => OnAllocateStat?.Invoke("Agility"));
            _spiritButton.onClick.AddListener(() => OnAllocateStat?.Invoke("Spirit"));

            // Wire minus buttons
            if (_vitalityMinusButton != null)
                _vitalityMinusButton.onClick.AddListener(() => OnDeallocateStat?.Invoke("Vitality"));
            if (_strengthMinusButton != null)
                _strengthMinusButton.onClick.AddListener(() => OnDeallocateStat?.Invoke("Strength"));
            if (_agilityMinusButton != null)
                _agilityMinusButton.onClick.AddListener(() => OnDeallocateStat?.Invoke("Agility"));
            if (_spiritMinusButton != null)
                _spiritMinusButton.onClick.AddListener(() => OnDeallocateStat?.Invoke("Spirit"));

            // Wire confirm button
            if (_confirmStatsButton != null)
            {
                _confirmStatsButton.onClick.AddListener(() => OnConfirmStats?.Invoke());
                _confirmStatsButton.gameObject.SetActive(false);
            }

            HideItemDetail();
            SwitchToTab(0);
        }

        public void SetCharacterPreview(string spritePrefix)
        {
            if (_characterAnimator != null)
                _characterAnimator.Play(spritePrefix);
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

        public void HideItemDetail()
        {
            _detailPanel.SetActive(false);
        }

        // --- Helpers ---

        private static Color GetEquipmentQualityColor(EquipmentQuality quality)
        {
            return QualityColors.TryGetValue(quality, out var c) ? c : Color.gray;
        }

        private static Color GetGemTypeColor(GemType gemType)
        {
            return GemColors.TryGetValue(gemType, out var c) ? c : Color.gray;
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

        public void SwitchToTab(int tabIndex)
        {
            bool isEquipment = tabIndex == 0;

            _equipmentContent.SetActive(isEquipment);
            _statsContent.SetActive(!isEquipment);

            _equipmentTabImage.sprite = SpriteAtlasLoader.GetSprite(isEquipment ? "EquipmentTab_Opened" : "EquipmentTab_Closed");
            _statsTabImage.sprite = SpriteAtlasLoader.GetSprite(isEquipment ? "CharacterTab_Closed" : "CharacterTab_Opened");
            _equipmentTabText.color = isEquipment ? ActiveTabText : InactiveTabText;
            _statsTabText.color = isEquipment ? InactiveTabText : ActiveTabText;

            _titleText.text = isEquipment ? "EQUIPMENT" : "CHARACTER STATS";

            if (!isEquipment)
                HideItemDetail();
        }

        public void RefreshStats(CharacterClass characterClass, int level,
            CharacterStats stats, int statPointsAvailable,
            int vitality, int strength, int agility, int spirit)
        {
            _classText.text = characterClass.ToString();
            _levelText.text = $"Level {level}";

            _statsText.text =
                $"Health: {stats.HP}\n" +
                $"Mana: {stats.MP}\n" +
                $"Attack: {stats.ATK}\n" +
                $"Defence: {stats.DEF}\n" +
                $"Magic Attack: {stats.MATK}\n" +
                $"Magic Defence: {stats.MDEF}\n" +
                $"Attack Speed: {stats.AttackSpeed:F2}\n" +
                $"Critical Rate: {(stats.CritRate * 100f):F1}%\n" +
                $"Critical Damage: {(stats.CritDmg * 100f):F0}%";

            _statPointsText.text = $"Stat Points: {statPointsAvailable}";

            _vitalityText.text = $"Vitality: {vitality}";
            _strengthText.text = $"Strength: {strength}";
            _agilityText.text = $"Agility: {agility}";
            _spiritText.text = $"Spirit: {spirit}";

            bool hasPoints = statPointsAvailable > 0;
            _vitalityButton.interactable = hasPoints;
            _strengthButton.interactable = hasPoints;
            _agilityButton.interactable = hasPoints;
            _spiritButton.interactable = hasPoints;
        }

        public void ShowMinusButtons(bool vitality, bool strength, bool agility, bool spirit)
        {
            if (_vitalityMinusButton != null) _vitalityMinusButton.gameObject.SetActive(vitality);
            if (_strengthMinusButton != null) _strengthMinusButton.gameObject.SetActive(strength);
            if (_agilityMinusButton != null) _agilityMinusButton.gameObject.SetActive(agility);
            if (_spiritMinusButton != null) _spiritMinusButton.gameObject.SetActive(spirit);
        }

        public void HideAllMinusButtons()
        {
            ShowMinusButtons(false, false, false, false);
        }

        public void ShowConfirmButton()
        {
            if (_confirmStatsButton != null)
                _confirmStatsButton.gameObject.SetActive(true);
        }

        public void HideConfirmButton()
        {
            if (_confirmStatsButton != null)
                _confirmStatsButton.gameObject.SetActive(false);
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

            if (_equipmentTabButton != null) _equipmentTabButton.onClick.RemoveAllListeners();
            if (_statsTabButton != null) _statsTabButton.onClick.RemoveAllListeners();
            if (_vitalityButton != null) _vitalityButton.onClick.RemoveAllListeners();
            if (_strengthButton != null) _strengthButton.onClick.RemoveAllListeners();
            if (_agilityButton != null) _agilityButton.onClick.RemoveAllListeners();
            if (_spiritButton != null) _spiritButton.onClick.RemoveAllListeners();
            if (_vitalityMinusButton != null) _vitalityMinusButton.onClick.RemoveAllListeners();
            if (_strengthMinusButton != null) _strengthMinusButton.onClick.RemoveAllListeners();
            if (_agilityMinusButton != null) _agilityMinusButton.onClick.RemoveAllListeners();
            if (_spiritMinusButton != null) _spiritMinusButton.onClick.RemoveAllListeners();
            if (_confirmStatsButton != null) _confirmStatsButton.onClick.RemoveAllListeners();
        }

    }
}
