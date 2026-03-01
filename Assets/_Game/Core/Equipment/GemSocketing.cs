namespace ConquerChronicles.Core.Equipment
{
    public static class GemSocketing
    {
        /// <summary>
        /// Checks if two gems can be combined. Both must be the same type,
        /// the same tier, and the tier must be less than 9.
        /// </summary>
        public static bool CanCombineGems(GemData gem1, GemData gem2)
        {
            return gem1.Type == gem2.Type
                && gem1.Tier == gem2.Tier
                && gem1.Tier < 9;
        }

        /// <summary>
        /// Combines two gems of the same type and tier into a new gem with tier + 1.
        /// Returns the combined gem. Caller must verify CanCombineGems first.
        /// </summary>
        public static GemData CombineGems(GemData gem1, GemData gem2)
        {
            return new GemData(gem1.Type, gem1.Tier + 1);
        }
    }
}
