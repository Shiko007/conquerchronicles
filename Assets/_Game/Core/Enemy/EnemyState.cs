using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;

namespace ConquerChronicles.Core.Enemy
{
    public struct ActiveStatusEffect
    {
        public StatusEffectType Type;
        public float RemainingDuration;
        public float TickTimer;
        public int TickDamage;
        public float SlowPercent;
    }

    public class EnemyState
    {
        public EnemyData Data;
        public int CurrentHP;
        public float AttackTimer;
        public List<ActiveStatusEffect> ActiveEffects;
        public float MoveSpeedMultiplier;
        public bool IsStunned;

        public bool IsDead => CurrentHP <= 0;

        public EnemyState(EnemyData data)
        {
            Data = data;
            CurrentHP = data.Stats.HP;
            AttackTimer = 0f;
            ActiveEffects = new List<ActiveStatusEffect>(4);
            MoveSpeedMultiplier = 1f;
            IsStunned = false;
        }

        public void Reset(EnemyData data)
        {
            Data = data;
            CurrentHP = data.Stats.HP;
            AttackTimer = 0f;
            ActiveEffects.Clear();
            MoveSpeedMultiplier = 1f;
            IsStunned = false;
        }

        public void TakeDamage(int damage)
        {
            CurrentHP -= damage;
            if (CurrentHP < 0) CurrentHP = 0;
        }
    }
}
