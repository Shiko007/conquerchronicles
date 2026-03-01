namespace ConquerChronicles.Core.Combat
{
    [System.Serializable]
    public struct CombatPosition
    {
        public float X;
        public float Y;

        public CombatPosition(float x, float y) { X = x; Y = y; }

        public float DistanceTo(CombatPosition other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            return (float)System.Math.Sqrt(dx * dx + dy * dy);
        }

        public static CombatPosition Zero => new(0f, 0f);
    }
}
