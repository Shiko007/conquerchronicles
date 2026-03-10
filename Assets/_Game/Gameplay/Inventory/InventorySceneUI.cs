using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Gameplay.Animation;
using ConquerChronicles.Gameplay.UI;

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
        [SerializeField] private Button _dropButton;
        [SerializeField] private TextMeshProUGUI _dropButtonText;
        [SerializeField] private Button _closeDetailButton;

        [Header("Selection Bar")]
        [SerializeField] private GameObject _selectionBar;
        [SerializeField] private Button _dropSelectedButton;
        [SerializeField] private TextMeshProUGUI _dropSelectedText;
        [SerializeField] private Button _cancelSelectButton;

        [Header("Confirm Dialog")]
        [SerializeField] private GameObject _confirmDialog;
        [SerializeField] private TextMeshProUGUI _confirmText;
        [SerializeField] private Button _confirmYesButton;
        [SerializeField] private Button _confirmNoButton;

        // Events
        public System.Action OnBackPressed;
        public System.Action<int> OnBagItemPressed;
        public System.Action<int> OnBagItemLongPressed;
        public System.Action OnEquipPressed;
        public System.Action OnDropPressed;
        public System.Action OnCloseDetailPressed;
        public System.Action OnDropSelectedPressed;
        public System.Action OnCancelSelectPressed;

        private readonly List<GameObject> _bagSlotObjects = new List<GameObject>();
        private readonly HashSet<int> _selectedIndices = new();
        private bool _isSelectMode;
        public bool IsSelectMode => _isSelectMode;

        private static readonly Color SelectionOverlayColor = new Color(1f, 0.2f, 0.2f, 0.35f);

        private static readonly Dictionary<EquipmentQuality, Color> QualityColors = new()
        {
            { EquipmentQuality.Normal,  Color.white },
            { EquipmentQuality.Refined, new Color(0.5f, 0.7f, 1f) },      // light blue
            { EquipmentQuality.Unique,  new Color(0.2f, 0.3f, 0.8f) },    // dark blue
            { EquipmentQuality.Elite,   new Color(1f, 0.4f, 0.7f) },      // pink
            { EquipmentQuality.Super,   new Color(1f, 0.2f, 0.2f) },      // red
        };

        private static readonly Color GemNameColor = new Color(1f, 0.9f, 0.2f, 1f); // yellow
        private static readonly Color MaterialNameColor2 = new Color(0.7f, 0.3f, 0.9f, 1f); // purple

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
        private static readonly Color GemSmokeColor = new Color(1f, 0.9f, 0.2f, 0.2f); // yellow tint
        private static readonly Color MaterialSmokeColor = new Color(1f, 1f, 1f, 0.2f); // white
        private static readonly Color MaterialNameColor = new Color(1f, 0.85f, 0.2f, 1f); // gold

        private static readonly Dictionary<EquipmentQuality, Color> QualitySmokeColors = new()
        {
            { EquipmentQuality.Normal,  new Color(1f, 1f, 1f, 0.2f) },           // white (rare items like Meteors/Dragonballs)
            { EquipmentQuality.Refined, new Color(0.5f, 0.7f, 1f, 0.2f) },       // light blue
            { EquipmentQuality.Unique,  new Color(0.2f, 0.3f, 0.8f, 0.2f) },     // dark blue
            { EquipmentQuality.Elite,   new Color(1f, 0.4f, 0.7f, 0.2f) },       // pink
            { EquipmentQuality.Super,   new Color(1f, 0.2f, 0.2f, 0.2f) },       // red
        };

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
            if (_dropButton != null)
                _dropButton.onClick.AddListener(() => OnDropPressed?.Invoke());
            _closeDetailButton.onClick.AddListener(() => OnCloseDetailPressed?.Invoke());
            if (_dropSelectedButton != null)
                _dropSelectedButton.onClick.AddListener(() => OnDropSelectedPressed?.Invoke());
            if (_cancelSelectButton != null)
                _cancelSelectButton.onClick.AddListener(() => OnCancelSelectPressed?.Invoke());

            HideItemDetail();
            if (_selectionBar != null) _selectionBar.SetActive(false);
            if (_confirmDialog != null) _confirmDialog.SetActive(false);
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

                    var lpBtn = slotGO.AddComponent<LongPressButton>();
                    lpBtn.OnClick = () =>
                    {
                        if (_isSelectMode)
                            ToggleSelection(index);
                        else
                            OnBagItemPressed?.Invoke(index);
                    };
                    lpBtn.OnLongPress = () => OnBagItemLongPressed?.Invoke(index);

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
                    else if (bagItem.Type == BagItemType.Material)
                    {
                        // White smoke for materials
                        var matSmokeGO = new GameObject("Smoke", typeof(RectTransform));
                        matSmokeGO.transform.SetParent(slotGO.transform, false);
                        var matSmokeRT = matSmokeGO.GetComponent<RectTransform>();
                        matSmokeRT.anchorMin = Vector2.zero;
                        matSmokeRT.anchorMax = Vector2.one;
                        float matInset = slotSize * 0.15f;
                        matSmokeRT.offsetMin = new Vector2(matInset, matInset);
                        matSmokeRT.offsetMax = new Vector2(-matInset, -matInset);
                        var matSmokeImg = matSmokeGO.AddComponent<Image>();
                        var matAnim = matSmokeGO.AddComponent<UISpriteAnimator>();
                        matAnim.Play(GetSmokeFrames(), 6f, MaterialSmokeColor);

                        // Name label
                        var matTextGO = new GameObject("Label", typeof(RectTransform));
                        matTextGO.transform.SetParent(slotGO.transform, false);
                        var matTextRT = matTextGO.GetComponent<RectTransform>();
                        matTextRT.anchorMin = Vector2.zero;
                        matTextRT.anchorMax = Vector2.one;
                        matTextRT.offsetMin = new Vector2(2, 2);
                        matTextRT.offsetMax = new Vector2(-2, -2);
                        var matTmp = matTextGO.AddComponent<TextMeshProUGUI>();
                        matTmp.text = GetSlotLabel(bagItem);
                        matTmp.fontSize = 18;
                        matTmp.color = Color.white;
                        matTmp.alignment = TextAlignmentOptions.Center;
                        matTmp.enableAutoSizing = true;
                        matTmp.fontSizeMin = 8;
                        matTmp.fontSizeMax = 18;
                        matTmp.textWrappingMode = TextWrappingModes.NoWrap;
                        matTmp.overflowMode = TextOverflowModes.Truncate;
                    }
                    else
                    {
                        // Smoke effect for equipment based on quality (skip Normal)
                        var quality = bagItem.Equipment.Data.Quality;
                        if (quality != EquipmentQuality.Normal && QualitySmokeColors.TryGetValue(quality, out var smokeColor))
                        {
                            var eqSmokeGO = new GameObject("Smoke", typeof(RectTransform));
                            eqSmokeGO.transform.SetParent(slotGO.transform, false);
                            var eqSmokeRT = eqSmokeGO.GetComponent<RectTransform>();
                            eqSmokeRT.anchorMin = Vector2.zero;
                            eqSmokeRT.anchorMax = Vector2.one;
                            float eqInset = slotSize * 0.15f;
                            eqSmokeRT.offsetMin = new Vector2(eqInset, eqInset);
                            eqSmokeRT.offsetMax = new Vector2(-eqInset, -eqInset);
                            var eqSmokeImg = eqSmokeGO.AddComponent<Image>();
                            var eqAnim = eqSmokeGO.AddComponent<UISpriteAnimator>();
                            eqAnim.Play(GetSmokeFrames(), 6f, smokeColor);
                        }

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

                    // Selection overlay (visible only in multi-select mode)
                    if (_isSelectMode && _selectedIndices.Contains(index))
                    {
                        var overlayGO = new GameObject("SelectOverlay", typeof(RectTransform));
                        overlayGO.transform.SetParent(slotGO.transform, false);
                        var overlayRT = overlayGO.GetComponent<RectTransform>();
                        overlayRT.anchorMin = Vector2.zero;
                        overlayRT.anchorMax = Vector2.one;
                        overlayRT.offsetMin = Vector2.zero;
                        overlayRT.offsetMax = Vector2.zero;
                        var overlayImg = overlayGO.AddComponent<Image>();
                        overlayImg.color = SelectionOverlayColor;
                        overlayImg.raycastTarget = false;
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
            else if (bagItem.Type == BagItemType.Gem)
            {
                var gem = bagItem.Gem;
                _itemNameText.text = $"{gem.Type} Gem";
                _itemNameText.color = GemNameColor;

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
            else if (bagItem.Type == BagItemType.Material)
            {
                _itemNameText.text = bagItem.MaterialName;
                _itemNameText.color = MaterialNameColor2;
                _itemStatsText.text = "Upgrade Material";
                _itemInfoText.text = "Used to upgrade\nequipment beyond +3";
                _equipButton.gameObject.SetActive(false);
            }

            if (_dropButton != null) _dropButton.gameObject.SetActive(true);
        }

        public void HideItemDetail()
        {
            _detailPanel.SetActive(false);
        }

        private static Color GetSlotColor(BagItem item)
        {
            if (item.Type == BagItemType.Equipment)
                return GetEquipmentQualityColor(item.Equipment.Data.Quality);
            if (item.Type == BagItemType.Material)
                return MaterialNameColor;
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
            else if (item.Type == BagItemType.Material)
            {
                string name = item.MaterialName ?? "";
                return name.Length > 4 ? name.Substring(0, 4) : name;
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

        public void EnterSelectMode(int firstIndex)
        {
            _isSelectMode = true;
            _selectedIndices.Clear();
            _selectedIndices.Add(firstIndex);
            if (_selectionBar != null) _selectionBar.SetActive(true);
            UpdateDropSelectedCount();
            AddSelectionOverlay(firstIndex);
        }

        public void ExitSelectMode()
        {
            _isSelectMode = false;
            _selectedIndices.Clear();
            if (_selectionBar != null) _selectionBar.SetActive(false);
        }

        public void ToggleSelection(int index)
        {
            if (_selectedIndices.Contains(index))
                _selectedIndices.Remove(index);
            else
                _selectedIndices.Add(index);

            UpdateDropSelectedCount();

            // Update overlay on this specific slot
            if (index >= 0 && index < _bagSlotObjects.Count)
            {
                if (_selectedIndices.Contains(index))
                    AddSelectionOverlay(index);
                else
                    RemoveSelectionOverlay(index);
            }
        }

        private void AddSelectionOverlay(int index)
        {
            if (index < 0 || index >= _bagSlotObjects.Count) return;
            var slotGO = _bagSlotObjects[index];
            if (slotGO.transform.Find("SelectOverlay") != null) return;

            var overlayGO = new GameObject("SelectOverlay", typeof(RectTransform));
            overlayGO.transform.SetParent(slotGO.transform, false);
            var overlayRT = overlayGO.GetComponent<RectTransform>();
            overlayRT.anchorMin = Vector2.zero;
            overlayRT.anchorMax = Vector2.one;
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;
            var overlayImg = overlayGO.AddComponent<Image>();
            overlayImg.color = SelectionOverlayColor;
            overlayImg.raycastTarget = false;
        }

        private void RemoveSelectionOverlay(int index)
        {
            if (index < 0 || index >= _bagSlotObjects.Count) return;
            var existing = _bagSlotObjects[index].transform.Find("SelectOverlay");
            if (existing != null) Destroy(existing.gameObject);
        }

        public List<int> GetSelectedIndices()
        {
            var list = new List<int>(_selectedIndices);
            return list;
        }

        private void UpdateDropSelectedCount()
        {
            if (_dropSelectedText != null)
                _dropSelectedText.text = $"Drop Selected ({_selectedIndices.Count})";
        }

        public void ShowConfirmDialog(string message, System.Action onConfirm)
        {
            if (_confirmDialog == null) return;
            _confirmDialog.SetActive(true);
            _confirmText.text = message;
            _confirmYesButton.onClick.RemoveAllListeners();
            _confirmYesButton.onClick.AddListener(() =>
            {
                _confirmDialog.SetActive(false);
                onConfirm?.Invoke();
            });
            _confirmNoButton.onClick.RemoveAllListeners();
            _confirmNoButton.onClick.AddListener(() => _confirmDialog.SetActive(false));
        }

        private void OnDestroy()
        {
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
            if (_equipButton != null) _equipButton.onClick.RemoveAllListeners();
            if (_dropButton != null) _dropButton.onClick.RemoveAllListeners();
            if (_closeDetailButton != null) _closeDetailButton.onClick.RemoveAllListeners();
            if (_dropSelectedButton != null) _dropSelectedButton.onClick.RemoveAllListeners();
            if (_cancelSelectButton != null) _cancelSelectButton.onClick.RemoveAllListeners();
            if (_confirmYesButton != null) _confirmYesButton.onClick.RemoveAllListeners();
            if (_confirmNoButton != null) _confirmNoButton.onClick.RemoveAllListeners();
        }
    }
}
