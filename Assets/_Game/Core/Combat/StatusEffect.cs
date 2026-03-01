namespace ConquerChronicles.Core.Combat
{
    [System.Serializable]
    public struct StatusEffect
    {
        public StatusEffectType Type;
        public float Duration;
        public float TickInterval;
        public int TickDamage;
        public float SlowPercent; // 0-1, only for Slow
    }
}
