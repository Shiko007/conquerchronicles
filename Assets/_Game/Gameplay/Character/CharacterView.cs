using UnityEngine;
using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Gameplay.Character
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public CharacterState State { get; private set; }

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(CharacterClass characterClass)
        {
            var baseStats = LevelUpTable.GetClassBaseStats(characterClass);
            var growth = LevelUpTable.GetClassGrowthPerLevel(characterClass);
            State = new CharacterState(characterClass, baseStats, growth);
        }

        public CharacterStats GetComputedStats()
        {
            return State.ComputeStats();
        }

        public void TakeDamage(int damage)
        {
            State.CurrentHP -= damage;
            if (State.CurrentHP < 0) State.CurrentHP = 0;
        }

        public void GainXP(int amount)
        {
            State.XP += amount;
            while (LevelUpTable.TryLevelUp(State))
            {
                // Level up happened — could raise event here
            }
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
