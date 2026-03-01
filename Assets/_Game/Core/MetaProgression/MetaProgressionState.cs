using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.MetaProgression
{
    /// <summary>
    /// Tracks permanent meta-progression upgrades purchased with Chronicle Coins.
    /// These bonuses persist across runs and are applied as percentage multipliers to stats.
    /// </summary>
    [System.Serializable]
    public class MetaProgressionState
    {
        public const int UpgradeTypeCount = 8;

        /// <summary>
        /// Chronicle Coins — the meta currency earned from completing stages.
        /// </summary>
        public int MetaCurrency;

        /// <summary>
        /// Current level for each upgrade type, indexed by (int)MetaUpgradeType.
        /// </summary>
        public int[] UpgradeLevels;

        public MetaProgressionState()
        {
            MetaCurrency = 0;
            UpgradeLevels = new int[UpgradeTypeCount];
        }

        /// <summary>
        /// Returns the current level of the specified upgrade.
        /// </summary>
        public int GetLevel(MetaUpgradeType type)
        {
            return UpgradeLevels[(int)type];
        }

        /// <summary>
        /// Returns the Chronicle Coin cost to upgrade the specified type to the next level.
        /// Cost formula: costPerLevel * currentLevel (so level 0 -> 1 costs costPerLevel * 1).
        /// </summary>
        public int GetUpgradeCost(MetaUpgradeType type)
        {
            int currentLevel = GetLevel(type);
            int costPerLevel = PermanentUpgradeData.GetCostPerLevel(type);
            return costPerLevel * (currentLevel + 1);
        }

        /// <summary>
        /// Returns true if the player has enough coins and the upgrade is not at max level.
        /// </summary>
        public bool CanUpgrade(MetaUpgradeType type)
        {
            int currentLevel = GetLevel(type);
            int maxLevel = PermanentUpgradeData.GetMaxLevel(type);

            if (currentLevel >= maxLevel)
                return false;

            return MetaCurrency >= GetUpgradeCost(type);
        }

        /// <summary>
        /// Attempts to purchase an upgrade. Deducts coins and increments the level.
        /// Returns true if successful, false if insufficient funds or already at max.
        /// </summary>
        public bool TryUpgrade(MetaUpgradeType type)
        {
            if (!CanUpgrade(type))
                return false;

            int cost = GetUpgradeCost(type);
            MetaCurrency -= cost;
            UpgradeLevels[(int)type]++;
            return true;
        }

        /// <summary>
        /// Computes aggregate stat bonuses from all stat-type meta upgrades.
        /// Each stat upgrade provides a percentage-based bonus applied as flat stats
        /// calculated from a base value:
        ///   Vitality: +2% HP per level (applied as multiplier in gameplay)
        ///   Power: +2% ATK per level
        ///   Arcane: +2% MATK per level
        ///   Fortitude: +2% DEF per level
        ///   Swiftness: +2% AGI per level
        ///
        /// Returns a CharacterStats struct representing percentage multipliers (as whole numbers).
        /// For example, Vitality level 10 = HP bonus of 20 (meaning 20% increase).
        /// The gameplay layer applies these as percentage bonuses to computed stats.
        /// </summary>
        public CharacterStats GetBonusStats()
        {
            return new CharacterStats
            {
                HP = UpgradeLevels[(int)MetaUpgradeType.Vitality] * 2,
                MP = 0,
                ATK = UpgradeLevels[(int)MetaUpgradeType.Power] * 2,
                DEF = UpgradeLevels[(int)MetaUpgradeType.Fortitude] * 2,
                MATK = UpgradeLevels[(int)MetaUpgradeType.Arcane] * 2,
                MDEF = 0,
                AGI = UpgradeLevels[(int)MetaUpgradeType.Swiftness] * 2,
                CritRate = 0f,
                CritDmg = 0f,
                AttackSpeed = 0f
            };
        }

        /// <summary>
        /// Returns the gold earning multiplier. Base is 1.0, +3% per Fortune level.
        /// </summary>
        public float GetGoldMultiplier()
        {
            return 1.0f + UpgradeLevels[(int)MetaUpgradeType.Fortune] * 0.03f;
        }

        /// <summary>
        /// Returns the XP earning multiplier. Base is 1.0, +3% per Wisdom level.
        /// </summary>
        public float GetXPMultiplier()
        {
            return 1.0f + UpgradeLevels[(int)MetaUpgradeType.Wisdom] * 0.03f;
        }

        /// <summary>
        /// Returns the drop rate bonus as a decimal. +2% per Luck level.
        /// </summary>
        public float GetDropRateBonus()
        {
            return UpgradeLevels[(int)MetaUpgradeType.Luck] * 0.02f;
        }
    }
}
