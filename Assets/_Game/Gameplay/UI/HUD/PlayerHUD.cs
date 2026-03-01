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
        [SerializeField] private TextMeshProUGUI _levelText;

        [Header("Kill Counter")]
        [SerializeField] private TextMeshProUGUI _killCountText;

        private CharacterView _player;
        private CombatManager _combatManager;

        public void Initialize(CharacterView player, CombatManager combatManager)
        {
            _player = player;
            _combatManager = combatManager;

            if (_combatManager != null)
            {
                _combatManager.OnKillCountChanged += UpdateKillCount;
            }
        }

        private void Update()
        {
            if (_player == null || _player.State == null) return;

            var state = _player.State;
            var stats = _player.GetComputedStats();

            // HP
            if (_hpFill != null)
                _hpFill.fillAmount = (float)state.CurrentHP / stats.HP;
            if (_hpText != null)
                _hpText.text = $"{state.CurrentHP}/{stats.HP}";

            // MP
            if (_mpFill != null)
                _mpFill.fillAmount = (float)state.CurrentMP / stats.MP;
            if (_mpText != null)
                _mpText.text = $"{state.CurrentMP}/{stats.MP}";

            // XP / Level
            if (_levelText != null)
                _levelText.text = $"Lv.{state.Level}";
            if (_xpFill != null)
            {
                int required = ConquerChronicles.Core.Character.LevelUpTable.GetRequiredXP(state.Level);
                _xpFill.fillAmount = required > 0 ? (float)state.XP / required : 1f;
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
        }
    }
}
