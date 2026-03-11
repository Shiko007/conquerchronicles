using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Enemy
{
    [System.Serializable]
    public struct EnemyData
    {
        public string ID;
        public string Name;
        public CharacterStats Stats;
        public float MoveSpeed;
        public float AttackRange;
        public float AttackCooldown;
        public int XPReward;
        public int GoldReward;
        public bool IsBoss;
        public DropTable DropTable;
        // RGBA tint (0 = use default white). Applied to sprite renderer.
        public float TintR, TintG, TintB, TintA;
        // Scale multiplier (0 = use default 1x).
        public float Scale;
    }
}
