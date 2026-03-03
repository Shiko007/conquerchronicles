using UnityEngine;
using UnityEngine.SceneManagement;
using ConquerChronicles.Core.Mining;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Gameplay.Mining
{
    public class MiningController : MonoBehaviour
    {
        [SerializeField] private MiningSceneUI _miningUI;

        private MiningState _miningState;
        private SaveManager _saveManager;

        private void Start()
        {
            if (_miningUI == null) return;

            _saveManager = SaveSystemBridge.GetOrCreate();
            _miningState = new MiningState();

            // Restore mining state from save
            var saveData = _saveManager.LoadGame();
            int playerLevel = 1;
            if (saveData != null)
            {
                playerLevel = saveData.CharacterLevel;
                if (!string.IsNullOrEmpty(saveData.ActiveMineID))
                {
                    _miningState.ActiveMineID = saveData.ActiveMineID;
                    _miningState.StartTimestamp = saveData.MiningStartTimestamp;
                    var mine = TestMines.GetByID(saveData.ActiveMineID);
                    _miningState.DurationSeconds = mine.DurationSeconds;
                }
            }

            _miningUI.Initialize(TestMines.GetAll(), _miningState, playerLevel);

            _miningUI.OnBackPressed = () => SceneManager.UnloadSceneAsync("Mining");

            _miningUI.OnStartMining = (mineID) =>
            {
                var mine = TestMines.GetByID(mineID);
                if (string.IsNullOrEmpty(mine.ID)) return;
                if (!MiningResolver.CanStartMining(_miningState, mine, playerLevel)) return;

                long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                _miningState.StartMining(mine, now);

                // Save mining state
                if (saveData == null) saveData = SaveData.CreateDefault();
                saveData.ActiveMineID = _miningState.ActiveMineID;
                saveData.MiningStartTimestamp = _miningState.StartTimestamp;
                _saveManager.SaveGame(saveData);

                _miningUI.RefreshMineList();
                Debug.Log($"[Mining] Started mining: {mine.Name}");
            };

            _miningUI.OnCollectPressed = () =>
            {
                if (!_miningState.IsMining) return;

                long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (!_miningState.IsComplete(now)) return;

                var mine = TestMines.GetByID(_miningState.ActiveMineID);
                int seed = (int)(now ^ _miningState.StartTimestamp);
                var yield = MiningResolver.CalculateYield(mine, seed);

                _miningState.Clear();

                // Update save
                if (saveData == null) saveData = SaveData.CreateDefault();
                saveData.ActiveMineID = string.Empty;
                saveData.MiningStartTimestamp = 0;
                saveData.Gold += yield.Gold;
                _saveManager.SaveGame(saveData);

                _miningUI.ShowYield(yield);
                Debug.Log($"[Mining] Collected: {yield.Gold} gold, {yield.Gems?.Length ?? 0} gems, {yield.Ores?.Length ?? 0} ores");
            };
        }
    }
}
