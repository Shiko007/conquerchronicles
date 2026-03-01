namespace ConquerChronicles.Core.Map
{
    public struct AreaResult
    {
        public string MapID;
        public string AreaID;
        public int EnemiesKilled;
        public float TimeElapsed;
        public int GoldEarned;
        public int XPEarned;
        public string[] ItemsDropped;

        public static AreaResult Calculate(AreaState state, string mapID, string[] droppedItems)
        {
            return new AreaResult
            {
                MapID = mapID,
                AreaID = state.Data.ID,
                EnemiesKilled = state.EnemiesKilled,
                TimeElapsed = state.ElapsedTime,
                GoldEarned = state.TotalGoldEarned,
                XPEarned = state.TotalXPEarned,
                ItemsDropped = droppedItems ?? System.Array.Empty<string>()
            };
        }
    }
}
