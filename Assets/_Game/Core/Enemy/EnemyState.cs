using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Enemy
{
    public class EnemyState
    {
        public EnemyData Data;
        public int CurrentHP;
        public float AttackTimer;

        public bool IsDead => CurrentHP <= 0;

        public EnemyState(EnemyData data)
        {
            Data = data;
            CurrentHP = data.Stats.HP;
            AttackTimer = 0f;
        }

        public void Reset(EnemyData data)
        {
            Data = data;
            CurrentHP = data.Stats.HP;
            AttackTimer = 0f;
        }

        public void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
        }
    }
}
