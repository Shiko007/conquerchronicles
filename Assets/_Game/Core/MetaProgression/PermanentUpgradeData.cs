namespace ConquerChronicles.Core.MetaProgression
{
    /// <summary>
    /// Static data definitions for each meta upgrade type.
    /// Provides max levels, cost multipliers, and descriptions.
    /// </summary>
    public static class PermanentUpgradeData
    {
        /// <summary>
        /// Returns the maximum level for the given upgrade type.
        /// Stat upgrades (Vitality/Power/Arcane/Fortitude) cap at 50.
        /// Utility upgrades (Swiftness/Fortune/Wisdom) cap at 30.
        /// Luck caps at 20.
        /// </summary>
        public static int GetMaxLevel(MetaUpgradeType type)
        {
            switch (type)
            {
                case MetaUpgradeType.Vitality:
                case MetaUpgradeType.Power:
                case MetaUpgradeType.Arcane:
                case MetaUpgradeType.Fortitude:
                    return 50;

                case MetaUpgradeType.Swiftness:
                case MetaUpgradeType.Fortune:
                case MetaUpgradeType.Wisdom:
                    return 30;

                case MetaUpgradeType.Luck:
                    return 20;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns the base cost multiplier per level for the given upgrade type.
        /// Stat upgrades cost 10 * level.
        /// Utility upgrades cost 15 * level.
        /// Luck costs 25 * level.
        /// </summary>
        public static int GetCostPerLevel(MetaUpgradeType type)
        {
            switch (type)
            {
                case MetaUpgradeType.Vitality:
                case MetaUpgradeType.Power:
                case MetaUpgradeType.Arcane:
                case MetaUpgradeType.Fortitude:
                    return 10;

                case MetaUpgradeType.Swiftness:
                case MetaUpgradeType.Fortune:
                case MetaUpgradeType.Wisdom:
                    return 15;

                case MetaUpgradeType.Luck:
                    return 25;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Returns a human-readable description for the given upgrade type.
        /// </summary>
        public static string GetDescription(MetaUpgradeType type)
        {
            switch (type)
            {
                case MetaUpgradeType.Vitality:
                    return "Increases maximum HP by 2% per level.";
                case MetaUpgradeType.Power:
                    return "Increases physical attack by 2% per level.";
                case MetaUpgradeType.Arcane:
                    return "Increases magical attack by 2% per level.";
                case MetaUpgradeType.Fortitude:
                    return "Increases defense by 2% per level.";
                case MetaUpgradeType.Swiftness:
                    return "Increases agility by 2% per level.";
                case MetaUpgradeType.Fortune:
                    return "Increases gold earned by 3% per level.";
                case MetaUpgradeType.Wisdom:
                    return "Increases XP earned by 3% per level.";
                case MetaUpgradeType.Luck:
                    return "Increases item drop rate by 2% per level.";
                default:
                    return string.Empty;
            }
        }
    }
}
