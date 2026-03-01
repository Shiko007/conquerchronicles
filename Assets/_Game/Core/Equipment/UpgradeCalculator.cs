namespace ConquerChronicles.Core.Equipment
{
    [System.Serializable]
    public struct UpgradeResult
    {
        public bool Success;
        public int NewLevel;
        public bool Destroyed;
    }

    public static class UpgradeCalculator
    {
        private static readonly float[] SuccessRates = new float[]
        {
            1.00f, // +0 -> +1: 100%
            0.95f, // +1 -> +2: 95%
            0.85f, // +2 -> +3: 85%
            0.70f, // +3 -> +4: 70%
            0.55f, // +4 -> +5: 55%
            0.40f, // +5 -> +6: 40%
            0.30f, // +6 -> +7: 30%
            0.20f, // +7 -> +8: 20%
            0.12f, // +8 -> +9: 12%
            0.08f, // +9 -> +10: 8%
            0.05f, // +10 -> +11: 5%
            0.02f  // +11 -> +12: 2%
        };

        /// <summary>
        /// Returns the success rate for upgrading from the current level to current level + 1.
        /// </summary>
        public static float GetSuccessRate(int currentLevel)
        {
            if (currentLevel < 0 || currentLevel >= 12)
                return 0f;

            return SuccessRates[currentLevel];
        }

        /// <summary>
        /// Attempts to upgrade the equipment using a seeded random number generator.
        /// On failure: level 1-3 stays the same, level 4-6 downgrades by 1, level 7+ destroys the item.
        /// </summary>
        public static UpgradeResult TryUpgrade(EquipmentInstance equipment, int seed)
        {
            int currentLevel = equipment.UpgradeLevel;

            if (currentLevel >= 12)
            {
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = currentLevel,
                    Destroyed = false
                };
            }

            float successRate = GetSuccessRate(currentLevel);
            var random = new System.Random(seed);
            float roll = (float)random.NextDouble();

            if (roll < successRate)
            {
                // Success
                equipment.UpgradeLevel = currentLevel + 1;
                return new UpgradeResult
                {
                    Success = true,
                    NewLevel = equipment.UpgradeLevel,
                    Destroyed = false
                };
            }

            // Failure
            if (currentLevel >= 7)
            {
                // Destroyed
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = currentLevel,
                    Destroyed = true
                };
            }

            if (currentLevel >= 4)
            {
                // Downgrade by 1
                equipment.UpgradeLevel = currentLevel - 1;
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = equipment.UpgradeLevel,
                    Destroyed = false
                };
            }

            // Level 1-3: stays the same
            return new UpgradeResult
            {
                Success = false,
                NewLevel = currentLevel,
                Destroyed = false
            };
        }
    }
}
