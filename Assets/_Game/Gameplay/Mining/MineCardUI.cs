using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ConquerChronicles.Gameplay.Mining
{
    public class MineCardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _durationText;
        [SerializeField] private TextMeshProUGUI _gemsText;
        [SerializeField] private TextMeshProUGUI _oresText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _startButton;
        [SerializeField] private TextMeshProUGUI _startButtonText;
        [SerializeField] private Image _background;

        private string _mineID;
        public System.Action<string> OnStartMining;

        public void Setup(Core.Mining.MineData mine, int playerLevel, bool isMiningActive)
        {
            _mineID = mine.ID;
            bool canMine = playerLevel >= mine.RequiredLevel && !isMiningActive;
            bool levelOk = playerLevel >= mine.RequiredLevel;

            if (_nameText != null) _nameText.text = mine.Name;
            if (_levelText != null)
            {
                _levelText.text = $"Req. Lv. {mine.RequiredLevel}";
                _levelText.color = levelOk ? Color.white : new Color(1f, 0.3f, 0.3f);
            }

            if (_durationText != null)
            {
                int hours = mine.DurationSeconds / 3600;
                int mins = (mine.DurationSeconds % 3600) / 60;
                _durationText.text = hours > 0 ? $"{hours}h {mins}m" : $"{mins}m";
            }

            if (_gemsText != null && mine.AvailableGems != null)
            {
                var sb = new System.Text.StringBuilder("Gems: ");
                for (int i = 0; i < mine.AvailableGems.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(mine.AvailableGems[i]);
                }
                _gemsText.text = sb.ToString();
            }

            if (_oresText != null && mine.AvailableOres != null)
            {
                var sb = new System.Text.StringBuilder("Ores: ");
                for (int i = 0; i < mine.AvailableOres.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append(mine.AvailableOres[i]);
                }
                _oresText.text = sb.ToString();
            }

            if (_descriptionText != null) _descriptionText.text = mine.Description;

            if (_startButton != null)
            {
                _startButton.interactable = canMine;
                _startButton.onClick.RemoveAllListeners();
                _startButton.onClick.AddListener(() => OnStartMining?.Invoke(_mineID));
            }

            if (_startButtonText != null)
            {
                if (isMiningActive) _startButtonText.text = "Mining...";
                else if (!levelOk) _startButtonText.text = "Locked";
                else _startButtonText.text = "Teleport";
            }

            if (_background != null)
            {
                _background.color = levelOk
                    ? new Color(0.12f, 0.12f, 0.18f, 0.9f)
                    : new Color(0.15f, 0.1f, 0.1f, 0.7f);
            }
        }
    }
}
