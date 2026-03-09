using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ConquerChronicles.Gameplay.UI.Tutorial
{
    public class TutorialOverlay : MonoBehaviour
    {
        [SerializeField] private GameObject _overlayRoot;
        [SerializeField] private Image _backdrop;
        [SerializeField] private TextMeshProUGUI _tooltipText;
        [SerializeField] private Button _dismissButton;
        [SerializeField] private TextMeshProUGUI _dismissButtonText;

        private string[] _steps;
        private int _currentStep;
        private System.Action _onComplete;

        public void Show(string[] steps, System.Action onComplete = null)
        {
            if (steps == null || steps.Length == 0) return;
            _steps = steps;
            _currentStep = 0;
            _onComplete = onComplete;

            if (_overlayRoot != null)
                _overlayRoot.SetActive(true);

            ShowCurrentStep();
            Time.timeScale = 0f;
        }

        public void Hide()
        {
            if (_overlayRoot != null)
                _overlayRoot.SetActive(false);
            Time.timeScale = 1f;
        }

        private void Awake()
        {
            if (_dismissButton != null)
                _dismissButton.onClick.AddListener(OnDismiss);
            if (_overlayRoot != null && _steps == null)
                _overlayRoot.SetActive(false);
        }

        private void OnDismiss()
        {
            _currentStep++;
            if (_currentStep >= _steps.Length)
            {
                Hide();
                _onComplete?.Invoke();
            }
            else
            {
                ShowCurrentStep();
            }
        }

        private void ShowCurrentStep()
        {
            if (_tooltipText != null)
                _tooltipText.text = _steps[_currentStep];

            if (_dismissButtonText != null)
                _dismissButtonText.text = _currentStep < _steps.Length - 1 ? "Next" : "Got it!";
        }

        private void OnDestroy()
        {
            if (_dismissButton != null)
                _dismissButton.onClick.RemoveAllListeners();
        }
    }
}
