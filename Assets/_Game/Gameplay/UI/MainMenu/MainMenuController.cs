using UnityEngine;
using UnityEngine.SceneManagement;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.UI.MainMenu
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private MainMenuUI _menuUI;

        private void Start()
        {
            if (_menuUI == null) return;

            _menuUI.OnMapSelectPressed = () => SceneManager.LoadScene("Gameplay");
            _menuUI.OnMiningPressed = () => SceneManager.LoadScene("Mining");
            _menuUI.OnEquipmentPressed = () => SceneManager.LoadScene("Equipment");
            _menuUI.OnMarketPressed = () => SceneManager.LoadScene("Market");

            // Load save data and display player info
            var saveManager = SaveSystemBridge.GetOrCreate();
            var saveData = saveManager.LoadGame();
            if (saveData != null)
            {
                string className = saveData.SelectedClass.ToString();
                _menuUI.SetPlayerInfo(className, saveData.CharacterLevel, saveData.Gold);
            }
            else
            {
                _menuUI.SetPlayerInfo("New Hero", 1, 0);
            }
        }
    }
}
