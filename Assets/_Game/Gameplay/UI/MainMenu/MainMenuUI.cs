using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ConquerChronicles.Gameplay.UI.MainMenu
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI _playerNameText;   // Shows class name
        [SerializeField] private TextMeshProUGUI _levelText;        // Shows "Lv. X"
        [SerializeField] private TextMeshProUGUI _goldText;         // Shows gold amount

        [Header("Navigation Buttons")]
        [SerializeField] private Button _mapSelectButton;    // Go to map/gameplay

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI _titleText;

        // Events for external wiring
        public System.Action OnMapSelectPressed;

        private void Awake()
        {
            if (_mapSelectButton != null)
                _mapSelectButton.onClick.AddListener(() => OnMapSelectPressed?.Invoke());
        }

        public void SetPlayerInfo(string className, int level, int gold)
        {
            if (_playerNameText != null) _playerNameText.text = className;
            if (_levelText != null) _levelText.text = $"Lv. {level}";
            if (_goldText != null) _goldText.text = $"{gold:N0} Gold";
        }

        public void SetBattleButtonInteractable(bool interactable)
        {
            if (_mapSelectButton != null)
                _mapSelectButton.interactable = interactable;
        }

        public void SetBattleButtonText(string text)
        {
            if (_mapSelectButton == null) return;
            var label = _mapSelectButton.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = text;
        }

        private void OnDestroy()
        {
            if (_mapSelectButton != null) _mapSelectButton.onClick.RemoveAllListeners();
        }
    }
}
