using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Save
{
    /// <summary>
    /// Root serializable save object containing all persistent game state.
    /// This is a plain data container — no logic, no Unity types.
    /// The Gameplay layer is responsible for JSON serialization/deserialization.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        // --- Schema Version ---
        public int Version = 1;

        // --- Character ---
        public CharacterClass SelectedClass;
        public int CharacterLevel;
        public int CharacterXP;
        public int StatPointsAvailable;

        // Allocated stat points
        public int Vitality;
        public int Mana;
        public int Strength;
        public int Agility;
        public int Spirit;

        // --- Inventory ---
        public int Gold;
        public SerializedEquipment[] EquippedItems; // 7 entries, empty ID = empty slot
        public SerializedEquipment[] BagItems;
        public SerializedGem[] GemBag;

        // --- Map Progress ---
        public string[] UnlockedMapIDs;
        public string LastAreaID;

        // --- Meta Progression ---
        public int MetaCurrency; // Chronicle Coins
        public int[] MetaUpgradeLevels; // 8 entries, indexed by MetaUpgradeType

        // --- Mining ---
        public long MiningStartTimestamp; // 0 = not mining
        public string ActiveMineID; // empty = not mining

        // --- Market / Booth ---
        public string[] PlayerBoothItemIDs; // items listed for sale
        public int[] PlayerBoothPrices; // parallel array
        public int BoothRevenue;

        /// <summary>
        /// Creates a default SaveData with properly initialized arrays.
        /// </summary>
        public static SaveData CreateDefault()
        {
            return new SaveData
            {
                Version = 2,
                SelectedClass = CharacterClass.None,
                CharacterLevel = 1,
                CharacterXP = 0,
                StatPointsAvailable = 0,
                Vitality = 0,
                Mana = 0,
                Strength = 0,
                Agility = 0,
                Spirit = 0,
                Gold = 0,
                EquippedItems = new SerializedEquipment[7],
                BagItems = System.Array.Empty<SerializedEquipment>(),
                GemBag = System.Array.Empty<SerializedGem>(),
                UnlockedMapIDs = new[] { "map_slime_fields" },
                LastAreaID = string.Empty,
                MetaCurrency = 0,
                MetaUpgradeLevels = new int[8],
                MiningStartTimestamp = 0,
                ActiveMineID = string.Empty,
                PlayerBoothItemIDs = System.Array.Empty<string>(),
                PlayerBoothPrices = System.Array.Empty<int>(),
                BoothRevenue = 0
            };
        }
    }

    /// <summary>
    /// Serializable representation of an equipment instance.
    /// Uses DataID string to reference EquipmentData from a catalog at restore time.
    /// </summary>
    [System.Serializable]
    public struct SerializedEquipment
    {
        public string DataID;
        public int UpgradeLevel;
        public SerializedGem[] Gems;

        /// <summary>
        /// Returns true if this entry represents an empty slot (no equipment).
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(DataID);
    }

    /// <summary>
    /// Serializable representation of a gem. Stores type and tier as integers
    /// to avoid enum serialization issues across different JSON libraries.
    /// </summary>
    [System.Serializable]
    public struct SerializedGem
    {
        public int Type; // cast from GemType
        public int Tier;
    }
}
