using UnityEngine;
using ConquerChronicles.Core.Save;

namespace ConquerChronicles.Gameplay.Save
{
    /// <summary>
    /// ISaveProvider implementation using Unity's PlayerPrefs.
    /// Simple and reliable for mobile — no file system access needed.
    /// </summary>
    public class PlayerPrefsSaveProvider : ISaveProvider
    {
        private const string SaveKey = "ConquerChronicles_SaveData";

        public bool HasSave()
        {
            return PlayerPrefs.HasKey(SaveKey);
        }

        public string Load()
        {
            return PlayerPrefs.GetString(SaveKey, string.Empty);
        }

        public void Save(string json)
        {
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
        }
    }
}
