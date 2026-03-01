namespace ConquerChronicles.Core.Stage
{
    [System.Serializable]
    public struct StageData
    {
        public string ID;
        public string Name;
        public int RecommendedLevel;
        public WaveData[] Waves;
        public WaveData BossWave;
        public float XPMultiplier;
        public int CompletionGold;
        public int CompletionMetaCurrency;
    }
}
