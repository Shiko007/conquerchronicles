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
            1.00f, // +1 -> +2: 100%
            0.95f, // +2 -> +3: 95%
            0.85f, // +3 -> +4: 85%
            0.70f, // +4 -> +5: 70%
            0.55f, // +5 -> +6: 55%
            0.40f, // +6 -> +7: 40%
            0.25f, // +7 -> +8: 25%
            0.15f, // +8 -> +9: 15%
            0.08f, // +9 -> +10: 8%
            0.04f, // +10 -> +11: 4%
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
        /// On failure: +1/+2 safe, +3-+6 downgrade by 1, +7-+9 reset to +0, +10-+12 destroyed.
        /// DragonBall protection prevents destruction at +10-+12, resetting to +0 instead.
        /// </summary>
        public static UpgradeResult TryUpgrade(EquipmentInstance equipment, int seed, bool useDragonBall = false)
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
            if (currentLevel <= 2)
            {
                // +1, +2: safe, no penalty
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = currentLevel,
                    Destroyed = false
                };
            }

            if (currentLevel <= 6)
            {
                // +3 to +6: downgrade by 1
                equipment.UpgradeLevel = currentLevel - 1;
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = equipment.UpgradeLevel,
                    Destroyed = false
                };
            }

            if (currentLevel <= 9)
            {
                // +7 to +9: reset to +0
                equipment.UpgradeLevel = 0;
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = 0,
                    Destroyed = false
                };
            }

            // +10 to +12: destroyed (unless DragonBall protection is active)
            if (useDragonBall)
            {
                equipment.UpgradeLevel = 0;
                return new UpgradeResult
                {
                    Success = false,
                    NewLevel = 0,
                    Destroyed = false
                };
            }

            equipment.UpgradeLevel = 0;
            return new UpgradeResult
            {
                Success = false,
                NewLevel = 0,
                Destroyed = true
            };
        }
    }
}
