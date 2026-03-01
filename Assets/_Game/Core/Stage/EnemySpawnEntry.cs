namespace ConquerChronicles.Core.Stage
{
    [System.Serializable]
    public struct EnemySpawnEntry
    {
        public string EnemyID;
        public int Count;
        public SpawnEdge Edge;
        public SpawnPattern Pattern;
    }
}
