namespace ConquerChronicles.Core.Combat
{
    public struct EnemyTarget
    {
        public int Index;
        public CombatPosition Position;
        public ConquerChronicles.Core.Character.CharacterStats Stats;
        public int CurrentHP;
        public bool IsDead;
    }
}
