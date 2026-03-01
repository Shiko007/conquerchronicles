using ConquerChronicles.Core.Enemy;

namespace ConquerChronicles.Core.Map
{
    [System.Serializable]
    public struct AreaData
    {
        public string ID;
        public string Name;
        public int MinLevel;
        public int MaxLevel;
        public float XPMultiplier;
        public float GoldMultiplier;
        public int MaxConcurrentEnemies;
        public float SpawnInterval;
        public EnemySpawnWeight[] EnemyPool;
        public DropTable AreaDropTable;
    }
}
