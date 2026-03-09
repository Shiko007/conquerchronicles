using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ConquerChronicles.Core.Mining;
using ConquerChronicles.Core.Equipment;

namespace ConquerChronicles.Gameplay.Mining
{
    public class MiningSceneUI : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private Button _backButton;

        [Header("Mine List")]
        [SerializeField] private Transform _mineListContainer;
        [SerializeField] private MineCardUI[] _mineCards; // pre-created cards

        [Header("Active Mining Panel")]
        [SerializeField] private GameObject _activeMiningPanel;
        [SerializeField] private TextMeshProUGUI _activeMineNameText;
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private Image _progressFill;
        [SerializeField] private Button _collectButton;
        [SerializeField] private TextMeshProUGUI _collectButtonText;

        [Header("Yield Result Panel")]
        [SerializeField] private GameObject _yieldPanel;
        [SerializeField] private TextMeshProUGUI _yieldGoldText;
        [SerializeField] private TextMeshProUGUI _yieldGemsText;
        [SerializeField] private TextMeshProUGUI _yieldOresText;
        [SerializeField] private Button _yieldCloseButton;

        [Header("Notification")]
        [SerializeField] private TextMeshProUGUI _notificationText;

        public System.Action OnBackPressed;
        public System.Action<string> OnStartMining;
        public System.Action OnCollectPressed;

        private MiningState _miningState;
        private MineData[] _allMines;
        private int _playerLevel;

        public void Initialize(MineData[] mines, MiningState miningState, int playerLevel)
        {
            _allMines = mines;
            _miningState = miningState;
            _playerLevel = playerLevel;

            if (_backButton != null)
                _backButton.onClick.AddListener(() => OnBackPressed?.Invoke());

            if (_collectButton != null)
                _collectButton.onClick.AddListener(() => OnCollectPressed?.Invoke());

            if (_yieldCloseButton != null)
                _yieldCloseButton.onClick.AddListener(() =>
                {
                    if (_yieldPanel != null) _yieldPanel.SetActive(false);
                    RefreshMineList();
                });

            if (_yieldPanel != null) _yieldPanel.SetActive(false);

            RefreshMineList();
            UpdateActiveMiningPanel();
        }

        public void RefreshMineList()
        {
            if (_mineCards == null || _allMines == null) return;

            for (int i = 0; i < _mineCards.Length; i++)
            {
                if (i < _allMines.Length)
                {
                    _mineCards[i].gameObject.SetActive(true);
                    _mineCards[i].Setup(_allMines[i], _playerLevel, _miningState.IsMining);
                    _mineCards[i].OnStartMining = (id) => OnStartMining?.Invoke(id);
                }
                else
                {
                    _mineCards[i].gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            UpdateActiveMiningPanel();
        }

        private void UpdateActiveMiningPanel()
        {
            if (_activeMiningPanel == null) return;

            bool mining = _miningState != null && _miningState.IsMining;
            _activeMiningPanel.SetActive(mining);

            if (!mining) return;

            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            float progress = _miningState.GetProgress(now);
            int remaining = _miningState.GetRemainingSeconds(now);
            bool complete = _miningState.IsComplete(now);

            if (_activeMineNameText != null)
            {
                var mine = TestMines.GetByID(_miningState.ActiveMineID);
                _activeMineNameText.text = mine.Name;
            }

            if (_progressFill != null) _progressFill.fillAmount = progress;

            if (_timerText != null)
            {
                if (complete)
                    _timerText.text = "Complete!";
                else
                {
                    int h = remaining / 3600;
                    int m = (remaining % 3600) / 60;
                    int s = remaining % 60;
                    _timerText.text = $"{h:00}:{m:00}:{s:00}";
                }
            }

            if (_collectButton != null) _collectButton.interactable = complete;
            if (_collectButtonText != null)
                _collectButtonText.text = complete ? "Collect!" : "Mining...";
        }

        public void ShowYield(MiningYield yield)
        {
            if (_yieldPanel != null) _yieldPanel.SetActive(true);

            if (_yieldGoldText != null)
                _yieldGoldText.text = $"Gold: +{yield.Gold:N0}";

            if (_yieldGemsText != null && yield.Gems != null)
            {
                var sb = new System.Text.StringBuilder("Gems:\n");
                for (int i = 0; i < yield.Gems.Length; i++)
                {
                    sb.AppendLine($"  {yield.Gems[i].Type} (Tier {yield.Gems[i].Tier})");
                }
                _yieldGemsText.text = sb.ToString();
            }

            if (_yieldOresText != null && yield.Ores != null)
            {
                var sb = new System.Text.StringBuilder("Ores:\n");
                for (int i = 0; i < yield.Ores.Length; i++)
                {
                    sb.AppendLine($"  {yield.Ores[i].Type} x{yield.Ores[i].Quantity}");
                }
                _yieldOresText.text = sb.ToString();
            }
        }

        public void ShowNotification(string message)
        {
            if (_notificationText == null) return;
            _notificationText.text = message;
            _notificationText.gameObject.SetActive(true);
            StopCoroutine(nameof(HideNotificationAfterDelay));
            StartCoroutine(HideNotificationAfterDelay());
        }

        private IEnumerator HideNotificationAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            if (_notificationText != null)
                _notificationText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
            if (_collectButton != null) _collectButton.onClick.RemoveAllListeners();
            if (_yieldCloseButton != null) _yieldCloseButton.onClick.RemoveAllListeners();
        }
    }
}
