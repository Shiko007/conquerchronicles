namespace ConquerChronicles.Core.Save
{
    /// <summary>
    /// Delegate for serializing SaveData to a JSON string.
    /// The implementation lives in the Gameplay layer (e.g., using JsonUtility or Newtonsoft).
    /// </summary>
    public delegate string JsonSerializer(SaveData data);

    /// <summary>
    /// Delegate for deserializing a JSON string back into SaveData.
    /// The implementation lives in the Gameplay layer.
    /// </summary>
    public delegate SaveData JsonDeserializer(string json);

    /// <summary>
    /// Orchestrates save/load operations using an ISaveProvider for storage
    /// and injected delegates for JSON serialization.
    /// This class contains no Unity dependencies — JSON encoding is delegated out.
    /// </summary>
    public class SaveManager
    {
        private readonly ISaveProvider _provider;
        private readonly JsonSerializer _serialize;
        private readonly JsonDeserializer _deserialize;

        public SaveManager(ISaveProvider provider, JsonSerializer serialize, JsonDeserializer deserialize)
        {
            _provider = provider;
            _serialize = serialize;
            _deserialize = deserialize;
        }

        /// <summary>
        /// Serializes the given SaveData to JSON and persists it via the provider.
        /// </summary>
        public void SaveGame(SaveData data)
        {
            string json = _serialize(data);
            _provider.Save(json);
        }

        /// <summary>
        /// Loads JSON from the provider and deserializes it into SaveData.
        /// Returns null if no save exists or the loaded string is empty.
        /// </summary>
        public SaveData LoadGame()
        {
            if (!_provider.HasSave())
                return null;

            string json = _provider.Load();

            if (string.IsNullOrEmpty(json))
                return null;

            return _deserialize(json);
        }

        /// <summary>
        /// Returns true if the provider has a save file.
        /// </summary>
        public bool HasSave()
        {
            return _provider.HasSave();
        }

        /// <summary>
        /// Deletes the save data via the provider.
        /// </summary>
        public void DeleteSave()
        {
            _provider.Delete();
        }
    }
}
