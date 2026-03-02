using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;

namespace ConquerChronicles.Gameplay.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [Header("HP")]
        [SerializeField] private Image _hpFill;
        [SerializeField] private TextMeshProUGUI _hpText;

        [Header("MP")]
        [SerializeField] private Image _mpFill;
        [SerializeField] private TextMeshProUGUI _mpText;

        [Header("XP")]
        [SerializeField] private Image _xpFill;
        [SerializeField] private TextMeshProUGUI _xpText;

        [Header("Level & Class")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _classText;

        [Header("Kill Counter")]
        [SerializeField] private TextMeshProUGUI _killCountText;

        [Header("Navigation")]
        [SerializeField] private Button _backButton;

        public System.Action OnBackPressed;

        private CharacterView _player;
        private CombatManager _combatManager;
        private int _lastHP = -1;
        private int _lastMaxHP = -1;
        private int _lastMP = -1;
        private int _lastMaxMP = -1;
        private int _lastXP = -1;
        private int _lastLevel = -1;

        public void Initialize(CharacterView player, CombatManager combatManager)
        {
            _player = player;
            _combatManager = combatManager;

            if (_combatManager != null)
            {
                _combatManager.OnKillCountChanged += UpdateKillCount;
            }

            if (_backButton != null)
                _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

            // Set class name once
            if (_classText != null && _player != null && _player.State != null)
                _classText.text = _player.State.Class.ToString();
        }

        private void Update()
        {
            if (_player == null || _player.State == null) return;

            var state = _player.State;
            var stats = _player.GetComputedStats();

            // HP — only update text when values change
            int hp = state.CurrentHP;
            int maxHP = stats.HP;
            if (hp != _lastHP || maxHP != _lastMaxHP)
            {
                _lastHP = hp;
                _lastMaxHP = maxHP;
                if (_hpFill != null)
                    _hpFill.fillAmount = maxHP > 0 ? (float)hp / maxHP : 0f;
                if (_hpText != null)
                    _hpText.text = $"{hp}/{maxHP}";
            }

            // MP
            int mp = state.CurrentMP;
            int maxMP = stats.MP;
            if (mp != _lastMP || maxMP != _lastMaxMP)
            {
                _lastMP = mp;
                _lastMaxMP = maxMP;
                if (_mpFill != null)
                    _mpFill.fillAmount = maxMP > 0 ? (float)mp / maxMP : 0f;
                if (_mpText != null)
                    _mpText.text = $"{mp}/{maxMP}";
            }

            // XP / Level
            int xp = state.XP;
            int level = state.Level;
            if (xp != _lastXP || level != _lastLevel)
            {
                _lastXP = xp;
                _lastLevel = level;
                if (_levelText != null)
                    _levelText.text = $"Lv.{level}";
                if (_xpFill != null)
                {
                    int required = Core.Character.LevelUpTable.GetRequiredXP(level);
                    _xpFill.fillAmount = required > 0 ? (float)xp / required : 1f;
                }
                if (_xpText != null)
                {
                    int required = Core.Character.LevelUpTable.GetRequiredXP(level);
                    _xpText.text = $"{xp}/{required}";
                }
            }
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
