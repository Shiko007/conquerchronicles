using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;

namespace ConquerChronicles.Gameplay.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [Header("Orb")]
        [SerializeField] private Image _hpFill;
        [SerializeField] private Image _mpFill;
        [SerializeField] private TextMeshProUGUI _orbText;

        [Header("XP")]
        [SerializeField] private Image _xpFill;

        [Header("Level & Class")]
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Kill Counter")]
        [SerializeField] private TextMeshProUGUI _killCountText;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        public System.Action OnBackPressed;

        private CharacterView _player;
        private CombatManager _combatManager;
        private bool _isSplitOrb;
        private int _lastHP = -1;
        private int _lastMaxHP = -1;
        private int _lastMP = -1;
        private int _lastMaxMP = -1;
        private int _lastXP = -1;
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

        public void Initialize(CharacterView player, CombatManager combatManager)
        {
            _player = player;
            _combatManager = combatManager;

            if (_combatManager != null)
                _combatManager.OnKillCountChanged += UpdateKillCount;

            if (_backButton != null)
                _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

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
                    _orbText.text = $"{hp}\n{state.CurrentMP}";
                else
                    _orbText.text = $"{hp}";
            }

            // XP / Level
            int xp = state.XP;
            int level = state.Level;
            if (xp != _lastXP || level != _lastLevel)
            {
                _lastXP = xp;
                _lastLevel = level;
                if (_levelText != null)
                    _levelText.text = $"{level}";
                int required = Core.Character.LevelUpTable.GetRequiredXP(level);
                _xpTarget = required > 0 ? (float)xp / required : 1f;
            }

            if (!Mathf.Approximately(_xpCurrent, _xpTarget))
            {
                _xpCurrent = Mathf.MoveTowards(_xpCurrent, _xpTarget, dt * FillSpeed);
                ApplyXPFill(_xpCurrent);
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
            _xpFillRT.anchorMin = Vector2.zero;
            _xpFillRT.anchorMax = new Vector2(t, 1f);
            _xpFillRT.offsetMin = new Vector2(1, 1);
            _xpFillRT.offsetMax = new Vector2(-1, -1);
        }

        private void UpdateKillCount(int count)
        {
            if (_killCountText != null)
                _killCountText.text = $"Kills: {count}";
        }

        private void OnDestroy()
        {
            if (_combatManager != null)
                _combatManager.OnKillCountChanged -= UpdateKillCount;
            if (_backButton != null)
                _backButton.onClick.RemoveAllListeners();
        }
    }
}
