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
        [SerializeField] private Button _miningButton;       // Go to mining
        [SerializeField] private Button _equipmentButton;    // Go to equipment (placeholder)
        [SerializeField] private Button _marketButton;       // Go to market (placeholder)

        [Header("Title")]
        [SerializeField] private TextMeshProUGUI _titleText;

        // Events for external wiring
        public System.Action OnMapSelectPressed;
        public System.Action OnMiningPressed;
        public System.Action OnEquipmentPressed;
        public System.Action OnMarketPressed;

        private void Awake()
        {
            if (_mapSelectButton != null)
                _mapSelectButton.onClick.AddListener(() => OnMapSelectPressed?.Invoke());
            if (_miningButton != null)
                _miningButton.onClick.AddListener(() => OnMiningPressed?.Invoke());
            if (_equipmentButton != null)
                _equipmentButton.onClick.AddListener(() => OnEquipmentPressed?.Invoke());
            if (_marketButton != null)
                _marketButton.onClick.AddListener(() => OnMarketPressed?.Invoke());
        }

        public void SetPlayerInfo(string className, int level, int gold)
        {
            if (_playerNameText != null) _playerNameText.text = className;
            if (_levelText != null) _levelText.text = $"Lv. {level}";
            if (_goldText != null) _goldText.text = $"{gold:N0} Gold";
        }

        private void OnDestroy()
        {
            if (_mapSelectButton != null) _mapSelectButton.onClick.RemoveAllListeners();
            if (_miningButton != null) _miningButton.onClick.RemoveAllListeners();
            if (_equipmentButton != null) _equipmentButton.onClick.RemoveAllListeners();
            if (_marketButton != null) _marketButton.onClick.RemoveAllListeners();
        }
    }
}
