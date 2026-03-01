namespace ConquerChronicles.Core.Map
{
    public class AreaState
    {
        public AreaData Data;
        public int EnemiesAlive;
        public int EnemiesKilled;
        public float ElapsedTime;
        public float SpawnTimer;
        public int TotalXPEarned;
        public int TotalGoldEarned;
        public bool PlayerDied;
        public bool PlayerLeft;

        public AreaState(AreaData data)
        {
            Data = data;
            EnemiesAlive = 0;
            EnemiesKilled = 0;
            ElapsedTime = 0f;
            SpawnTimer = 0f;
            TotalXPEarned = 0;
            TotalGoldEarned = 0;
            PlayerDied = false;
            PlayerLeft = false;
        }

        public bool IsActive => !PlayerDied && !PlayerLeft;
        public bool ShouldSpawn => EnemiesAlive < Data.MaxConcurrentEnemies && SpawnTimer <= 0f;
    }
}
