using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Gameplay.Inventory
{
    public class InventorySceneUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _backButton;
        [SerializeField] private TextMeshProUGUI _goldText;

        [Header("Bag")]
        [SerializeField] private Transform _bagContainer;

        [Header("Item Detail")]
        [SerializeField] private GameObject _detailPanel;
        [SerializeField] private TextMeshProUGUI _itemNameText;
        [SerializeField] private TextMeshProUGUI _itemStatsText;
        [SerializeField] private TextMeshProUGUI _itemInfoText;
        [SerializeField] private Button _equipButton;
        [SerializeField] private TextMeshProUGUI _equipButtonText;
        [SerializeField] private Button _closeDetailButton;

        // Events
        public System.Action OnBackPressed;
        public System.Action<int> OnBagItemPressed;
        public System.Action OnEquipPressed;
        public System.Action OnCloseDetailPressed;

        private readonly List<GameObject> _bagSlotObjects = new List<GameObject>();

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
        private static readonly Color GemSmokeColor = new Color(1f, 0.9f, 0.2f, 0.2f); // yellow tint, 50% opacity

        private static Sprite[] _smokeFrames;

        private static Sprite[] GetSmokeFrames()
        {
            if (_smokeFrames != null) return _smokeFrames;
            _smokeFrames = Resources.LoadAll<Sprite>("Smoke");
            if (_smokeFrames == null || _smokeFrames.Length == 0)
            {
                Debug.LogWarning("[InventorySceneUI] No smoke sprites found in Resources/Smoke");
                _smokeFrames = System.Array.Empty<Sprite>();
            }
            else
            {
                System.Array.Sort(_smokeFrames, (a, b) => string.CompareOrdinal(a.name, b.name));
            }
            return _smokeFrames;
        }

        public void Initialize()
        {
            _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());
            _equipButton.onClick.AddListener(() => OnEquipPressed?.Invoke());
            _closeDetailButton.onClick.AddListener(() => OnCloseDetailPressed?.Invoke());

            HideItemDetail();
        }

        public void RefreshBag(List<BagItem> bag)
        {
            foreach (var go in _bagSlotObjects)
            {
                if (go != null) Destroy(go);
            }
            _bagSlotObjects.Clear();

            int totalSlots = InventoryState.BagCapacity;

            // Compute optimal columns and square slot size from actual container
            Canvas.ForceUpdateCanvases();
            var containerRT = _bagContainer.GetComponent<RectTransform>();
            float W = containerRT.rect.width;
            float H = containerRT.rect.height;

            float gap = 4f;
            int cols = 1;
            int rows = totalSlots;
            float slotSize = 0f;

            // Find column count that gives the largest square slots fitting all items
            for (int c = 3; c <= 12; c++)
            {
                int r = Mathf.CeilToInt((float)totalSlots / c);
                float sW = (W - (c - 1) * gap) / c;
                float sH = (H - (r - 1) * gap) / r;
                float s = Mathf.Min(sW, sH);
                if (s > slotSize)
                {
                    slotSize = s;
                    cols = c;
                    rows = r;
                }
            }

            float totalGridW = cols * slotSize + (cols - 1) * gap;
            float totalGridH = rows * slotSize + (rows - 1) * gap;
            float xOffset = (W - totalGridW) / 2f; // center horizontally
            float yOffset = (H - totalGridH) / 2f; // center vertically

            for (int i = 0; i < totalSlots; i++)
            {
                int index = i;
                bool hasItem = i < bag.Count;
                int col = i % cols;
                int row = i / cols;

                var slotGO = new GameObject($"BagSlot_{i}", typeof(RectTransform));
                slotGO.transform.SetParent(_bagContainer, false);
                var slotRT = slotGO.GetComponent<RectTransform>();
                slotRT.anchorMin = new Vector2(0, 1);
                slotRT.anchorMax = new Vector2(0, 1);
                slotRT.pivot = new Vector2(0, 1);
                slotRT.sizeDelta = new Vector2(slotSize, slotSize);
                slotRT.anchoredPosition = new Vector2(
                    xOffset + col * (slotSize + gap),
                    -yOffset - row * (slotSize + gap));

                var img = slotGO.AddComponent<Image>();
                img.sprite = SpriteAtlasLoader.GetSprite("Blank_Slot");

                if (hasItem)
                {
                    var bagItem = bag[i];

                    var btn = slotGO.AddComponent<Button>();
                    btn.targetGraphic = img;
                    btn.onClick.AddListener(() => OnBagItemPressed?.Invoke(index));

                    if (bagItem.Type == BagItemType.Gem)
                    {
                        // Smoke effect inside blank slot with yellow tint
                        var smokeGO = new GameObject("Smoke", typeof(RectTransform));
                        smokeGO.transform.SetParent(slotGO.transform, false);
                        var smokeRT = smokeGO.GetComponent<RectTransform>();
                        smokeRT.anchorMin = Vector2.zero;
                        smokeRT.anchorMax = Vector2.one;
                        float inset = slotSize * 0.15f;
                        smokeRT.offsetMin = new Vector2(inset, inset);
                        smokeRT.offsetMax = new Vector2(-inset, -inset);
                        var smokeImg = smokeGO.AddComponent<Image>();
                        var anim = smokeGO.AddComponent<UISpriteAnimator>();
                        anim.Play(GetSmokeFrames(), 6f, GemSmokeColor);

                        // Tier label in bottom-right corner
                        var tierGO = new GameObject("Tier", typeof(RectTransform));
                        tierGO.transform.SetParent(slotGO.transform, false);
                        var tierRT = tierGO.GetComponent<RectTransform>();
                        tierRT.anchorMin = new Vector2(0.5f, 0f);
                        tierRT.anchorMax = new Vector2(1f, 0.4f);
                        tierRT.offsetMin = Vector2.zero;
                        tierRT.offsetMax = Vector2.zero;
                        var tierTmp = tierGO.AddComponent<TextMeshProUGUI>();
                        tierTmp.text = $"T{bagItem.Gem.Tier}";
                        tierTmp.fontSize = 16;
                        tierTmp.color = Color.white;
                        tierTmp.alignment = TextAlignmentOptions.BottomRight;
                        tierTmp.enableAutoSizing = true;
                        tierTmp.fontSizeMin = 8;
                        tierTmp.fontSizeMax = 16;
                        tierTmp.outlineWidth = 0.3f;
                        tierTmp.outlineColor = Color.black;
                    }
                    else
                    {
                        img.color = GetSlotColor(bagItem);

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
                        tmp.enableAutoSizing = true;
                        tmp.fontSizeMin = 8;
                        tmp.fontSizeMax = 18;
                        tmp.textWrappingMode = TextWrappingModes.NoWrap;
                        tmp.overflowMode = TextOverflowModes.Truncate;
                    }
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
            _goldText.text = $"{gold:N0}";
        }

        public void ShowItemDetail(BagItem bagItem, bool canEquip)
        {
            _detailPanel.SetActive(true);

            if (bagItem.Type == BagItemType.Equipment)
            {
                var item = bagItem.Equipment;
                string upgradeStr = item.UpgradeLevel > 0 ? $" +{item.UpgradeLevel}" : "";
                _itemNameText.text = $"{item.Data.Name}{upgradeStr}";
                _itemNameText.color = GetEquipmentQualityColor(item.Data.Quality);

                var stats = item.ComputeStats();
                _itemStatsText.text = FormatStats(stats);

                int socketedCount = 0;
                for (int i = 0; i < item.SocketedGems.Length; i++)
                {
                    if (item.SocketedGems[i].Tier > 0) socketedCount++;
                }
                _itemInfoText.text =
                    $"Quality: {item.Data.Quality}\n" +
                    $"Req Lv: {item.Data.RequiredLevel}\n" +
                    $"Sockets: {socketedCount}/{item.Data.MaxSockets}";

                _equipButton.gameObject.SetActive(true);
                _equipButtonText.text = "Equip";
                _equipButton.interactable = canEquip;
            }
            else
            {
                var gem = bagItem.Gem;
                _itemNameText.text = $"{gem.Type} Gem";
                _itemNameText.color = GetGemTypeColor(gem.Type);

                var bonus = gem.GetBonus();
                var sb = new System.Text.StringBuilder();
                sb.AppendLine($"Tier: {gem.Tier}");
                sb.AppendLine("");
                string bonusText = FormatStats(bonus);
                sb.Append(string.IsNullOrEmpty(bonusText) ? "XP Bonus" : bonusText);
                _itemStatsText.text = sb.ToString().TrimEnd();

                _itemInfoText.text = $"Type: {gem.Type}\nTier: {gem.Tier}/9";
                _equipButton.gameObject.SetActive(false);
            }
        }

        public void HideItemDetail()
        {
            _detailPanel.SetActive(false);
        }

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
                string name = eq.Data.Name.Length > 4 ? eq.Data.Name.Substring(0, 4) : eq.Data.Name;
                string upgrade = eq.UpgradeLevel > 0 ? $"\n+{eq.UpgradeLevel}" : "";
                return $"{name}{upgrade}";
            }
            else
            {
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
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
            if (_equipButton != null) _equipButton.onClick.RemoveAllListeners();
            if (_closeDetailButton != null) _closeDetailButton.onClick.RemoveAllListeners();
        }
    }
}
