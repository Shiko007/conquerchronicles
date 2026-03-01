namespace ConquerChronicles.Core.Save
{
    /// <summary>
    /// Abstraction for save storage backends.
    /// Implementations handle the actual persistence (file system, PlayerPrefs, cloud, etc.)
    /// while the Core layer remains platform-agnostic.
    /// </summary>
    public interface ISaveProvider
    {
        /// <summary>
        /// Returns true if a save file exists.
        /// </summary>
        bool HasSave();

        /// <summary>
        /// Loads and returns the raw JSON string from storage.
        /// Returns null or empty if no save exists.
        /// </summary>
        string Load();

        /// <summary>
        /// Persists the given JSON string to storage.
        /// </summary>
        void Save(string json);

        /// <summary>
        /// Deletes the save data from storage.
        /// </summary>
        void Delete();
    }
}
