using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Gameplay.Character
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(SpriteAnimator))]
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private CharacterStats _metaBonus;
        private SpriteAnimator _animator;

        // Cached animation frames
        private Sprite[] _idleFrames;   // 8-direction idle frames
        private Sprite[] _attackFrames;
        private Sprite[] _hitFrames;
        private Sprite[] _deathFrames;

        // Gender prefix for sprite loading: "Male_Base" or "Female_Base"
        private string _spritePrefix = "Male_Base";
        private bool _isMale = true;

        private bool _playingAction;

        public CharacterState State { get; private set; }

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            _animator = GetComponent<SpriteAnimator>();
            if (_animator == null)
                _animator = gameObject.AddComponent<SpriteAnimator>();
        }

        public void Initialize(CharacterClass characterClass)
        {
            var baseStats = LevelUpTable.GetClassBaseStats(characterClass);
            var growth = LevelUpTable.GetClassGrowthPerLevel(characterClass);
            State = new CharacterState(characterClass, baseStats, growth);

            LoadSprites();
            PlayIdle();
        }

        /// <summary>
        /// Set gender for sprite selection. Call before Initialize.
        /// </summary>
        public void SetGender(bool isMale)
        {
            _isMale = isMale;
            _spritePrefix = isMale ? "Male_Base" : "Female_Base";
        }

        private void LoadSprites()
        {
            _idleFrames = SpriteAtlasLoader.GetFrames($"{_spritePrefix}_SIdle_");
            _attackFrames = SpriteAtlasLoader.GetFrames($"{_spritePrefix}_EAttack_");
            _hitFrames = SpriteAtlasLoader.GetFrames($"{_spritePrefix}_EHit_");
            _deathFrames = SpriteAtlasLoader.GetFrames($"{_spritePrefix}_SDeath_");
        }

        /// <summary>
        /// Play the idle animation (loops continuously).
        /// Called after attack/hit finishes, or on initialize.
        /// </summary>
        public void PlayIdle()
        {
            if (_playingAction) return;
            if (_idleFrames != null && _idleFrames.Length > 0)
                _animator.Play("idle", _idleFrames, 6f, true);
        }

        /// <summary>
        /// Face toward a world position (flip sprite left/right).
        /// </summary>
        public void FaceToward(Vector3 targetPosition)
        {
            if (_playingAction) return;

            Vector3 dir = targetPosition - transform.position;
            _animator.SetFlipX(dir.x < 0);
        }

        /// <summary>
        /// Play attack animation (one-shot, facing right; flip for left targets).
        /// </summary>
        public void PlayAttack(bool faceLeft)
        {
            if (_attackFrames == null || _attackFrames.Length == 0) return;

            _playingAction = true;
            _animator.SetFlipX(faceLeft);
            _animator.PlayOneShot("attack", _attackFrames, 10f, () =>
            {
                _playingAction = false;
                PlayIdle();
            });
        }

        /// <summary>
        /// Play hit animation (one-shot).
        /// </summary>
        public void PlayHit(bool faceLeft)
        {
            if (_hitFrames == null || _hitFrames.Length == 0) return;

            _playingAction = true;
            _animator.SetFlipX(faceLeft);
            _animator.PlayOneShot("hit", _hitFrames, 12f, () =>
            {
                _playingAction = false;
                PlayIdle();
            });
        }

        /// <summary>
        /// Play death animation (one-shot, no return to idle).
        /// </summary>
        public void PlayDeath()
        {
            if (_deathFrames == null || _deathFrames.Length == 0) return;

            _playingAction = true;
            _animator.SetFlipX(false);
            _animator.PlayOneShot("death", _deathFrames, 8f);
        }

        /// <summary>
        /// Reset the action-lock flag so PlayIdle() can be called again.
        /// Used after reviving from death, since the death animation sets
        /// _playingAction = true and never clears it.
        /// </summary>
        public void ResetActionState()
        {
            _playingAction = false;
        }

        public void SetMetaBonus(CharacterStats bonus)
        {
            _metaBonus = bonus;
        }

        public CharacterStats GetComputedStats()
        {
            return State.ComputeStats() + _metaBonus;
        }

        public void TakeDamage(int damage)
        {
            State.CurrentHP -= damage;
            if (State.CurrentHP < 0) State.CurrentHP = 0;

            if (State.IsDead)
                PlayDeath();
            else
                PlayHit(false);
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
        public SpriteAnimator Animator => _animator;
    }
}
