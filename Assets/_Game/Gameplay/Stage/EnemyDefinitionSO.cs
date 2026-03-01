using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Enemy;

namespace ConquerChronicles.Gameplay.Stage
{
    [CreateAssetMenu(menuName = "Conquer Chronicles/Enemy Definition")]
    public class EnemyDefinitionSO : ScriptableObject
    {
        public string enemyID;
        public string displayName;
        public CharacterStats stats;
        public float moveSpeed = 1.5f;
        public float attackRange = 0.5f;
        public float attackCooldown = 1.5f;
        public int xpReward = 10;
        public int goldReward = 5;
        public bool isBoss;

        public EnemyData ToEnemyData()
        {
            return new EnemyData
            {
                ID = enemyID,
                Name = displayName,
                Stats = stats,
                MoveSpeed = moveSpeed,
                AttackRange = attackRange,
                AttackCooldown = attackCooldown,
                XPReward = xpReward,
                GoldReward = goldReward,
                IsBoss = isBoss
            };
        }
    }
}
