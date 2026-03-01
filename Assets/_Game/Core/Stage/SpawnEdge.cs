namespace ConquerChronicles.Core.Stage
{
    public enum SpawnEdge
    {
        North,
        South,
        East,
        West,
        Random,
        All
    }

    public enum SpawnPattern
    {
        Burst,    // All at once
        Stream,   // One by one with delay
        Surround  // From all edges simultaneously
    }
}
