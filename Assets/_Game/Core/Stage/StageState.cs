namespace ConquerChronicles.Core.Stage
{
    public class StageState
    {
        public StageData Data;
        public int CurrentWaveIndex;
        public int EnemiesAlive;
        public int EnemiesKilled;
        public float ElapsedTime;
        public bool BossSpawned;
        public bool IsComplete;
        public bool PlayerDied;

        public StageState(StageData data)
        {
            Data = data;
            CurrentWaveIndex = 0;
            EnemiesAlive = 0;
            EnemiesKilled = 0;
            ElapsedTime = 0f;
            BossSpawned = false;
            IsComplete = false;
            PlayerDied = false;
        }

        public bool AllWavesCleared => CurrentWaveIndex >= Data.Waves.Length;
        public int TotalWaves => Data.Waves.Length;

        public WaveData? GetCurrentWave()
        {
            if (CurrentWaveIndex < Data.Waves.Length)
                return Data.Waves[CurrentWaveIndex];
            return null;
        }

        public void AdvanceWave()
        {
            CurrentWaveIndex++;
        }
    }
}
