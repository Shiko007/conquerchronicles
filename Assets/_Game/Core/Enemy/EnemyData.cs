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
    }
}
