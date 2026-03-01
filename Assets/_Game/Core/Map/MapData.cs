namespace ConquerChronicles.Core.Map
{
    [System.Serializable]
    public struct MapData
    {
        public string ID;
        public string Name;
        public string Description;
        public int MinLevel;
        public int MaxLevel;
        public string BackgroundID;
        public AreaData[] Areas;
    }
}
