namespace ConquerChronicles.Core.Mining
{
    [System.Serializable]
    public class MiningState
    {
        public string ActiveMineID;
        public long StartTimestamp; // Unix seconds
        public int DurationSeconds;

        public bool IsMining => !string.IsNullOrEmpty(ActiveMineID);

        public bool IsComplete(long currentTimestamp)
        {
            return IsMining && (currentTimestamp - StartTimestamp) >= DurationSeconds;
        }

        public float GetProgress(long currentTimestamp)
        {
            if (!IsMining || DurationSeconds <= 0) return 0f;
            float elapsed = currentTimestamp - StartTimestamp;
            float ratio = elapsed / DurationSeconds;
            return ratio < 0f ? 0f : ratio > 1f ? 1f : ratio;
        }

        public int GetRemainingSeconds(long currentTimestamp)
        {
            if (!IsMining) return 0;
            long elapsed = currentTimestamp - StartTimestamp;
            long remaining = DurationSeconds - elapsed;
            return remaining < 0 ? 0 : (int)remaining;
        }

        public void StartMining(MineData mine, long currentTimestamp)
        {
            ActiveMineID = mine.ID;
            StartTimestamp = currentTimestamp;
            DurationSeconds = mine.DurationSeconds;
        }

        public void Clear()
        {
            ActiveMineID = null;
            StartTimestamp = 0;
            DurationSeconds = 0;
        }
    }
}
