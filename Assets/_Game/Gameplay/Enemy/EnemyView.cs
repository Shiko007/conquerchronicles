using UnityEngine;
using ConquerChronicles.Core.Enemy;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Gameplay.Enemy
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _healthBarFill;
        [SerializeField] private GameObject _healthBarRoot;

        public EnemyState State { get; private set; }
        public bool IsActive { get; private set; }
        public EnemyMovement Movement { get; private set; }
        public SpriteAnimator Animator { get; private set; }

        // Cached animation frames per enemy type
        private Sprite[] _idleFrames;
        private Sprite[] _walkFrames;
        private Sprite[] _attackFrames;
        private Sprite[] _hitFrames;
        private Sprite[] _deathFrames;

        private string _lastEnemyID;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            Movement = GetComponent<EnemyMovement>();
            Animator = GetComponent<SpriteAnimator>();
            if (Animator == null)
                Animator = gameObject.AddComponent<SpriteAnimator>();
        }

        public void Initialize(EnemyData data, Vector3 spawnPosition)
        {
            if (State == null)
                State = new EnemyState(data);
            else
                State.Reset(data);

            transform.position = spawnPosition;
            IsActive = true;
            gameObject.SetActive(true);
            UpdateHealthBar();

            // Load sprites for this enemy type (cache if same type)
            LoadSprites(data.ID);
            PlayWalk();
        }

        private void LoadSprites(string enemyID)
        {
            if (_lastEnemyID == enemyID) return;
            _lastEnemyID = enemyID;

            // Map enemy ID to sprite prefix
            // e.g. "enemy_rat" -> "Rat_L"
            string prefix = GetSpritePrefix(enemyID);

            _idleFrames = SpriteAtlasLoader.GetFrames($"{prefix}Idle_");
            _walkFrames = SpriteAtlasLoader.GetFrames($"{prefix}Walk_");
            _attackFrames = SpriteAtlasLoader.GetFrames($"{prefix}Attack_");
            _hitFrames = SpriteAtlasLoader.GetFrames($"{prefix}Hit_");
            _deathFrames = SpriteAtlasLoader.GetFrames($"{prefix}Death_");
        }

        /// <summary>
        /// Maps enemy data ID to atlas sprite prefix.
        /// Add entries here as you create more enemy sprites.
        /// Falls back to Rat sprites for enemies without dedicated art.
        /// </summary>
        private static string GetSpritePrefix(string enemyID)
        {
            if (string.IsNullOrEmpty(enemyID)) return "Rat_L";

            // Check for known prefixes in the enemy ID
            if (enemyID.Contains("rat")) return "Rat_L";
            if (enemyID.Contains("slime")) return "Rat_L"; // TODO: Slime_L when sprites exist
            if (enemyID.Contains("skeleton")) return "Rat_L"; // TODO: Skeleton_L
            if (enemyID.Contains("dark")) return "Rat_L"; // TODO: Cultivator_L

            return "Rat_L"; // fallback to rat for all enemies until more sprites are created
        }

        public void PlayIdle()
        {
            if (_idleFrames != null && _idleFrames.Length > 0)
                Animator.Play("idle", _idleFrames, 6f, true);
        }

        public void PlayWalk()
        {
            if (_walkFrames != null && _walkFrames.Length > 0)
                Animator.Play("walk", _walkFrames, 8f, true);
        }

        public void PlayAttack()
        {
            if (_attackFrames != null && _attackFrames.Length > 0)
                Animator.PlayOneShot("attack", _attackFrames, 10f, PlayIdle);
        }

        public void PlayHit()
        {
            if (Animator.CurrentAnimation == "death") return; // Don't interrupt death
            if (_hitFrames != null && _hitFrames.Length > 0)
                Animator.PlayOneShot("hit", _hitFrames, 12f, PlayIdle);
        }

        public void PlayDeathAnim()
        {
            if (_deathFrames != null && _deathFrames.Length > 0)
                Animator.PlayOneShot("death", _deathFrames, 8f);
        }

        public void TakeDamage(int damage)
        {
            State.TakeDamage(damage);
            UpdateHealthBar();

            if (State.IsDead)
                PlayDeathAnim();
            else
                PlayHit();
        }

        public void Deactivate()
        {
            IsActive = false;
            Animator.Stop();
            gameObject.SetActive(false);
        }

        private void UpdateHealthBar()
        {
            if (_healthBarFill == null || _healthBarRoot == null) return;

            float ratio = (float)State.CurrentHP / State.Data.Stats.HP;
            _healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
            _healthBarRoot.SetActive(true);
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
