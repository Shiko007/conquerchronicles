namespace ConquerChronicles.Core.Stage
{
    [System.Serializable]
    public struct WaveData
    {
        public float DelayBeforeWave;
        public float SpawnInterval; // seconds between spawns for Stream pattern
        public EnemySpawnEntry[] SpawnEntries;
    }
}
