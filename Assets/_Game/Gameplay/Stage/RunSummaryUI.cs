using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Map;

namespace ConquerChronicles.Gameplay.Stage
{
    public class RunSummaryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _killsText;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _xpText;
        [SerializeField] private TextMeshProUGUI _starsText;
        [SerializeField] private Button _continueButton;

        public System.Action OnContinue;

        public void Initialize()
        {
            if (_panel != null) _panel.SetActive(false);
            if (_continueButton != null)
                _continueButton.onClick.AddListener(() => {
                    if (_panel != null) _panel.SetActive(false);
                    Time.timeScale = 1f;
                    OnContinue?.Invoke();
                });
        }

        public void Show(AreaResult result)
        {
            if (_panel != null) _panel.SetActive(true);

            if (_titleText != null)
                _titleText.text = result.EnemiesKilled > 0 ? "SESSION COMPLETE" : "DEFEATED";

            if (_killsText != null)
                _killsText.text = $"Enemies Killed: {result.EnemiesKilled}";

            if (_timeText != null)
            {
                int minutes = (int)(result.TimeElapsed / 60f);
                int seconds = (int)(result.TimeElapsed % 60f);
                _timeText.text = $"Time: {minutes:00}:{seconds:00}";
            }

            if (_goldText != null)
                _goldText.text = $"Gold: +{result.GoldEarned}";

            if (_xpText != null)
                _xpText.text = $"XP: +{result.XPEarned}";

            if (_starsText != null)
                _starsText.text = $"Items Found: {result.ItemsDropped.Length}";

            Time.timeScale = 0f;
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveAllListeners();
        }
    }
}
