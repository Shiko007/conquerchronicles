namespace ConquerChronicles.Core.Stage
{
    public struct StageResult
    {
        public string StageID;
        public bool Victory;
        public int EnemiesKilled;
        public float TimeElapsed;
        public int GoldEarned;
        public int XPEarned;
        public int MetaCurrencyEarned;
        public int StarRating; // 1-3 based on performance

        public static StageResult Calculate(StageState state, int totalXPFromKills)
        {
            bool victory = state.IsComplete && !state.PlayerDied;
            float xpMult = state.Data.XPMultiplier;

            // Star rating: 3 = no damage taken concept (simplified for now)
            int stars = victory ? 1 : 0;
            if (victory && state.ElapsedTime < 120f) stars = 2;
            if (victory && state.ElapsedTime < 60f) stars = 3;

            return new StageResult
            {
                StageID = state.Data.ID,
                Victory = victory,
                EnemiesKilled = state.EnemiesKilled,
                TimeElapsed = state.ElapsedTime,
                GoldEarned = victory ? state.Data.CompletionGold + state.EnemiesKilled * 2 : state.EnemiesKilled,
                XPEarned = (int)(totalXPFromKills * xpMult),
                MetaCurrencyEarned = victory ? state.Data.CompletionMetaCurrency * stars : 0,
                StarRating = stars
            };
        }
    }
}
