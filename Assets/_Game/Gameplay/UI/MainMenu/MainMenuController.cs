using UnityEngine;
using UnityEngine.SceneManagement;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuUI _menuUI;

        private SaveManager _saveManager;
        private long _recoveryTimestamp;
        private bool _recovering;

        private void Start()
        {
            if (_menuUI == null) return;

            _menuUI.OnMapSelectPressed = OnBattlePressed;
            _menuUI.OnMiningPressed = () => SceneManager.LoadScene("Mining");
            _menuUI.OnEquipmentPressed = () => SceneManager.LoadScene("Equipment");
            _menuUI.OnMarketPressed = () => SceneManager.LoadScene("Market");

            // Load save data and display player info
            _saveManager = SaveSystemBridge.GetOrCreate();
            var saveData = _saveManager.LoadGame();
            if (saveData != null)
            {
                string className = saveData.SelectedClass.ToString();
                _menuUI.SetPlayerInfo(className, saveData.CharacterLevel, saveData.Gold);

                // Check recovery cooldown
                _recoveryTimestamp = saveData.HeroRecoveryTimestamp;
                if (_recoveryTimestamp > 0)
                {
                    long nowTicks = System.DateTimeOffset.UtcNow.Ticks;
                    if (nowTicks < _recoveryTimestamp)
                    {
                        _recovering = true;
                        _menuUI.SetBattleButtonInteractable(false);
                    }
                    else
                    {
                        // Recovery done — clear it
                        saveData.HeroRecoveryTimestamp = 0;
                        _saveManager.SaveGame(saveData);
                    }
                }
            }
            else
            {
                _menuUI.SetPlayerInfo("New Hero", 1, 0);
            }
        }

        private void Update()
        {
            if (!_recovering) return;

            long nowTicks = System.DateTimeOffset.UtcNow.Ticks;
            if (nowTicks >= _recoveryTimestamp)
            {
                // Recovery complete
                _recovering = false;
                _menuUI.SetBattleButtonInteractable(true);
                _menuUI.SetBattleButtonText("Battle");

                // Clear timestamp in save
                var saveData = _saveManager.LoadGame();
                if (saveData != null)
                {
                    saveData.HeroRecoveryTimestamp = 0;
                    _saveManager.SaveGame(saveData);
                }
            }
            else
            {
                var remaining = System.TimeSpan.FromTicks(_recoveryTimestamp - nowTicks);
                _menuUI.SetBattleButtonText($"Recovering {remaining.Minutes:0}:{remaining.Seconds:00}");
            }
        }

        private void OnBattlePressed()
        {
            if (_recovering) return;
            SceneManager.LoadScene("Gameplay");
        }
    }
}
