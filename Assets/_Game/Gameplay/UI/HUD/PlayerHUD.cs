using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Gameplay.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Orb")]
        [SerializeField] private Image _hpFill;
        [SerializeField] private Image _mpFill;
        [SerializeField] private TextMeshProUGUI _orbText;
        [SerializeField] private Button _orbButton;
        [SerializeField] private GameObject _orbTextGO;

        [Header("XP")]
        [SerializeField] private Image _xpFill;
        [SerializeField] private TextMeshProUGUI _xpText;
        [SerializeField] private Button _expOrbButton;
        [SerializeField] private GameObject _expTextGO;
        [SerializeField] private GameObject _levelTextGO;

        [Header("XP Gain Popup")]
        [SerializeField] private RectTransform _xpGainAnchor;

        [Header("Level & Class")]
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Kill Counter")]
        [SerializeField] private TextMeshProUGUI _killCountText;

        [Header("Revive")]
        [SerializeField] private GameObject _reviveOverlay;
        [SerializeField] private TextMeshProUGUI _reviveTimerText;

        [Header("Skill Slots")]
        [SerializeField] private Image[] _skillSlotIcons; // 4 skill slot icons

        [Header("Navigation")]
        [SerializeField] private Button _equipmentButton;
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private Button _mineButton;
        [SerializeField] private Button _marketButton;

        [Header("Navigation Icons")]
        [SerializeField] private Image _equipmentIcon;
        [SerializeField] private Image _inventoryIcon;
        [SerializeField] private Image _mineIcon;
        [SerializeField] private Image _marketIcon;

        public System.Action OnEquipmentPressed;
        public System.Action OnInventoryPressed;
        public System.Action OnMinePressed;
        public System.Action OnMarketPressed;

        private CharacterView _player;
        private CombatManager _combatManager;
        private bool _isSplitOrb;
        private int _lastHP = -1;
        private int _lastMaxHP = -1;
        private int _lastMP = -1;
        private int _lastMaxMP = -1;
        private long _lastXP = -1;
        private int _lastLevel = -1;

        // RectTransforms for anchor-based fill animation
        private RectTransform _hpFillRT;
        private RectTransform _mpFillRT;
        private RectTransform _xpFillRT;

        // Current and target fill levels (0-1)
        private float _hpCurrent = 1f;
        private float _hpTarget = 1f;
        private float _mpCurrent = 1f;
        private float _mpTarget = 1f;
        private float _xpCurrent = 0f;
        private float _xpTarget = 0f;

        // Max X anchor for HP (0.5 for split, 1.0 for full orb)
        private float _hpMaxX = 0.5f;

        private const float FillSpeed = 2f;

        // XP gain popup pool
        private const float PopupDuration = 1.2f;
        private const float PopupRiseDistance = 60f;
        private readonly System.Collections.Generic.List<XPPopup> _xpPopups = new();

        private struct XPPopup
        {
            public GameObject GO;
            public RectTransform RT;
            public CanvasGroup CG;
            public float StartY;
            public float Timer;
        }

        public void Initialize(CharacterView player, CombatManager combatManager)
        {
            _player = player;
            _combatManager = combatManager;

            if (_combatManager != null)
                _combatManager.OnKillCountChanged += UpdateKillCount;

            if (_orbButton != null)
                _orbButton.onClick.AddListener(ToggleOrbText);
            if (_expOrbButton != null)
                _expOrbButton.onClick.AddListener(ToggleExpText);

            if (_equipmentButton != null)
                _equipmentButton.onClick.AddListener(() => OnEquipmentPressed?.Invoke());
            if (_inventoryButton != null)
                _inventoryButton.onClick.AddListener(() => OnInventoryPressed?.Invoke());
            if (_mineButton != null)
                _mineButton.onClick.AddListener(() => OnMinePressed?.Invoke());
            if (_marketButton != null)
                _marketButton.onClick.AddListener(() => OnMarketPressed?.Invoke());

            // Cache RectTransforms
            if (_hpFill != null) _hpFillRT = _hpFill.GetComponent<RectTransform>();
            if (_mpFill != null) _mpFillRT = _mpFill.GetComponent<RectTransform>();
            if (_xpFill != null) _xpFillRT = _xpFill.GetComponent<RectTransform>();

            // Determine if split orb (Taoist classes use MP)
            if (_player != null && _player.State != null)
            {
                var cls = _player.State.Class;
                _isSplitOrb = cls == CharacterClass.WaterTaoist || cls == CharacterClass.FireTaoist;

                if (!_isSplitOrb)
                {
                    // Full orb for HP
                    _hpMaxX = 1f;

                    // Hide MP fill and divider
                    if (_mpFill != null)
                        _mpFill.gameObject.SetActive(false);

                    if (_hpFillRT != null)
                    {
                        var maskParent = _hpFill.transform.parent;
                        var divider = maskParent.Find("Divider");
                        if (divider != null)
                            divider.gameObject.SetActive(false);
                    }
                }
            }

            // Set initial fill positions
            _hpCurrent = 1f;
            _hpTarget = 1f;
            _mpCurrent = 1f;
            _mpTarget = 1f;
            _xpCurrent = 0f;
            _xpTarget = 0f;
            ApplyHPFill(1f);
            ApplyMPFill(1f);
            ApplyXPFill(0f);
        }

        private void Update()
        {
            if (_player == null || _player.State == null) return;

            var state = _player.State;
            var stats = _player.GetComputedStats();
            float dt = Time.deltaTime;

            // HP — update target when value changes
            int hp = state.CurrentHP;
            int maxHP = stats.HP;
            bool hpChanged = hp != _lastHP || maxHP != _lastMaxHP;
            if (hpChanged)
            {
                _lastHP = hp;
                _lastMaxHP = maxHP;
                _hpTarget = maxHP > 0 ? (float)hp / maxHP : 0f;
            }

            // Animate HP fill
            if (!Mathf.Approximately(_hpCurrent, _hpTarget))
            {
                _hpCurrent = Mathf.MoveTowards(_hpCurrent, _hpTarget, dt * FillSpeed);
                ApplyHPFill(_hpCurrent);
            }

            // MP (only for split orb)
            bool mpChanged = false;
            if (_isSplitOrb)
            {
                int mp = state.CurrentMP;
                int maxMP = stats.MP;
                mpChanged = mp != _lastMP || maxMP != _lastMaxMP;
                if (mpChanged)
                {
                    _lastMP = mp;
                    _lastMaxMP = maxMP;
                    _mpTarget = maxMP > 0 ? (float)mp / maxMP : 0f;
                }

                if (!Mathf.Approximately(_mpCurrent, _mpTarget))
                {
                    _mpCurrent = Mathf.MoveTowards(_mpCurrent, _mpTarget, dt * FillSpeed);
                    ApplyMPFill(_mpCurrent);
                }
            }

            // Orb text — immediate number update
            if (_orbText != null && (hpChanged || mpChanged))
            {
                if (_isSplitOrb)
                    _orbText.text = $"HP: {hp}\nMP: {state.CurrentMP}";
                else
                    _orbText.text = $"HP: {hp}";
            }

            // XP / Level
            long xp = state.XP;
            int level = state.Level;
            if (xp != _lastXP || level != _lastLevel)
            {
                _lastXP = xp;
                _lastLevel = level;
                if (_levelText != null)
                    _levelText.text = $"Lv.{level}";
                long required = Core.Character.LevelUpTable.GetRequiredXP(level);
                if (_xpText != null)
                {
                    float xpPercent = required > 0 ? (float)xp / required * 100f : 100f;
                    _xpText.text = $"{xpPercent:F6}%";
                }
                _xpTarget = required > 0 ? (float)xp / required : 1f;
            }

            if (!Mathf.Approximately(_xpCurrent, _xpTarget))
            {
                _xpCurrent = Mathf.MoveTowards(_xpCurrent, _xpTarget, dt * FillSpeed);
                ApplyXPFill(_xpCurrent);
            }

            // Animate XP gain popups
            UpdateXPPopups(dt);
        }

        public void ShowXPGain(long xpAmount)
        {
            if (_xpGainAnchor == null) return;
            if (_player == null || _player.State == null) return;

            long required = Core.Character.LevelUpTable.GetRequiredXP(_player.State.Level);
            if (required <= 0) return;

            float percent = (float)xpAmount / required * 100f;

            var go = new GameObject("XPGain", typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(_xpGainAnchor, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1f);
            rt.anchorMax = new Vector2(0.5f, 1f);
            rt.pivot = new Vector2(0.5f, 0f);
            rt.sizeDelta = new Vector2(200f, 30f);
            rt.anchoredPosition = Vector2.zero;

            var cg = go.GetComponent<CanvasGroup>();

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = $"+{percent:F6}%";
            tmp.fontSize = 22;
            tmp.color = new Color(0.4f, 1f, 0.4f);
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 12;
            tmp.fontSizeMax = 22;
            tmp.outlineWidth = 0.3f;
            tmp.outlineColor = Color.black;
            tmp.raycastTarget = false;

            _xpPopups.Add(new XPPopup
            {
                GO = go,
                RT = rt,
                CG = cg,
                StartY = 0f,
                Timer = 0f
            });
        }

        private void UpdateXPPopups(float dt)
        {
            for (int i = _xpPopups.Count - 1; i >= 0; i--)
            {
                var p = _xpPopups[i];
                p.Timer += dt;

                float t = p.Timer / PopupDuration;
                p.RT.anchoredPosition = new Vector2(0, p.StartY + PopupRiseDistance * t);
                p.CG.alpha = 1f - t;

                _xpPopups[i] = p;

                if (p.Timer >= PopupDuration)
                {
                    Destroy(p.GO);
                    _xpPopups.RemoveAt(i);
                }
            }
        }

        private void ApplyHPFill(float t)
        {
            if (_hpFillRT == null) return;
            // Anchors: bottom-left to (maxX, t) — rectangle height = t% of mask
            _hpFillRT.anchorMin = new Vector2(0, 0);
            _hpFillRT.anchorMax = new Vector2(_hpMaxX, t);
            _hpFillRT.offsetMin = Vector2.zero;
            _hpFillRT.offsetMax = Vector2.zero;
        }

        private void ApplyMPFill(float t)
        {
            if (_mpFillRT == null) return;
            _mpFillRT.anchorMin = new Vector2(0.5f, 0);
            _mpFillRT.anchorMax = new Vector2(1f, t);
            _mpFillRT.offsetMin = Vector2.zero;
            _mpFillRT.offsetMax = Vector2.zero;
        }

        private void ApplyXPFill(float t)
        {
            if (_xpFillRT == null) return;
            // Bottom-up fill inside circular EXP orb mask
            _xpFillRT.anchorMin = Vector2.zero;
            _xpFillRT.anchorMax = new Vector2(1f, t);
            _xpFillRT.offsetMin = Vector2.zero;
            _xpFillRT.offsetMax = Vector2.zero;
        }

        public void ShowReviveTimer(float secondsRemaining)
        {
            if (_reviveOverlay != null && !_reviveOverlay.activeSelf)
            {
                _reviveOverlay.SetActive(true);
                // Don't block raycasts to nav buttons
                var overlayImg = _reviveOverlay.GetComponent<UnityEngine.UI.Image>();
                if (overlayImg != null) overlayImg.raycastTarget = false;
            }
            if (_reviveTimerText != null)
            {
                int mins = (int)(secondsRemaining / 60f);
                int secs = (int)(secondsRemaining % 60f);
                _reviveTimerText.text = $"Reviving in {mins}:{secs:00}";
            }
        }

        public void HideReviveTimer()
        {
            if (_reviveOverlay != null)
                _reviveOverlay.SetActive(false);
        }

        public void SetNavIconState(string sceneName, bool isOpen)
        {
            string suffix = isOpen ? "_Open" : "_Closed";
            Image icon = null;
            string prefix = null;
            switch (sceneName)
            {
                case "Equipment": icon = _equipmentIcon; prefix = "Equipment"; break;
                case "Inventory": icon = _inventoryIcon; prefix = "Inventory"; break;
                case "Mining":    icon = _mineIcon;      prefix = "Mining";    break;
                case "Market":    icon = _marketIcon;     prefix = "Market";    break;
            }
            if (icon == null) return;
            var sprite = SpriteAtlasLoader.GetSprite(prefix + suffix);
            if (sprite != null) icon.sprite = sprite;
        }

        private void UpdateKillCount(int count)
        {
            if (_killCountText != null)
                _killCountText.text = $"Kills: {count}";
        }

        private void ToggleOrbText()
        {
            if (_orbTextGO != null)
                _orbTextGO.SetActive(!_orbTextGO.activeSelf);
        }

        private void ToggleExpText()
        {
            if (_expTextGO != null)
                _expTextGO.SetActive(!_expTextGO.activeSelf);
            if (_levelTextGO != null)
                _levelTextGO.SetActive(!_levelTextGO.activeSelf);
        }

        private void OnDestroy()
        {
            if (_combatManager != null)
                _combatManager.OnKillCountChanged -= UpdateKillCount;
            if (_orbButton != null)
                _orbButton.onClick.RemoveAllListeners();
            if (_expOrbButton != null)
                _expOrbButton.onClick.RemoveAllListeners();
            if (_equipmentButton != null)
                _equipmentButton.onClick.RemoveAllListeners();
            if (_inventoryButton != null)
                _inventoryButton.onClick.RemoveAllListeners();
            if (_mineButton != null)
                _mineButton.onClick.RemoveAllListeners();
            if (_marketButton != null)
                _marketButton.onClick.RemoveAllListeners();
        }
    }
}
