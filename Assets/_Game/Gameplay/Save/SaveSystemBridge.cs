using UnityEngine;
using ConquerChronicles.Core.Save;

namespace ConquerChronicles.Gameplay.Save
{
    /// <summary>
    /// Bridges the Core SaveManager with Unity's JsonUtility for serialization.
    /// Creates the SaveManager with PlayerPrefs backend and Unity JSON delegates.
    /// </summary>
    public static class SaveSystemBridge
    {
        private static SaveManager _instance;

        public static SaveManager GetOrCreate()
        {
            if (_instance != null)
                return _instance;

            var provider = new PlayerPrefsSaveProvider();
            _instance = new SaveManager(
                provider,
                data => JsonUtility.ToJson(data),
                json => JsonUtility.FromJson<SaveData>(json)
            );

            return _instance;
        }
    }
}
