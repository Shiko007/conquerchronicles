namespace ConquerChronicles.Core.Combat
{
    public struct CombatEvent
    {
        public enum EventType { Damage, Heal, Buff, Death }

        public EventType Type;
        public int SourceIndex;     // -1 for player
        public int TargetIndex;     // enemy index, or -1 for player
        public int Value;           // damage amount or heal amount
        public bool IsCritical;
        public DamageType DmgType;
        public AttackPattern Pattern;
        public StatusEffect AppliedEffect;
        public string SkillID;
    }
}
