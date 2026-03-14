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
        [SerializeField] private TextMesh _nameLabel;

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

        // Death delay — corpse stays visible for this duration after dying
        private const float DeathLingerDuration = 3f;
        private float _deathTimer = -1f;
        /// <summary>Set by EnemySpawner to track whether the death callback has already fired.</summary>
        public bool DeathCallbackFired;

        /// <summary>True when the enemy is dead AND the linger timer has expired.</summary>
        public bool IsReadyForRemoval => _deathTimer == 0f;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            Movement = GetComponent<EnemyMovement>();
            Animator = GetComponent<SpriteAnimator>();
            if (Animator == null)
                Animator = gameObject.AddComponent<SpriteAnimator>();
        }

        public void Initialize(EnemyData data, Vector3 spawnPosition, int playerLevel = 0, int areaLevel = 0)
        {
            if (State == null)
                State = new EnemyState(data);
            else
                State.Reset(data);

            transform.position = spawnPosition;
            IsActive = true;
            _deathTimer = -1f;
            DeathCallbackFired = false;
            gameObject.SetActive(true);
            UpdateHealthBar();
            UpdateNameLabel(data.Name, playerLevel, areaLevel);

            // Apply tint color (0 alpha = default white)
            if (data.TintA > 0f)
                _spriteRenderer.color = new Color(data.TintR, data.TintG, data.TintB, data.TintA);
            else
                _spriteRenderer.color = Color.white;

            // Apply scale to sprite, counter-scale UI so only the sprite looks bigger
            float s = data.Scale > 0f ? data.Scale : 1f;
            transform.localScale = new Vector3(s, s, 1f);
            float inv = 1f / s;
            if (_healthBarRoot != null)
                _healthBarRoot.transform.localScale = new Vector3(inv, inv, 1f);
            if (_nameLabel != null)
                _nameLabel.transform.localScale = new Vector3(inv, inv, 1f);

            // Load sprites for this enemy type (cache if same type)
            LoadSprites(data.ID);
            PlayWalk();
        }

        private void UpdateNameLabel(string enemyName, int playerLevel, int areaLevel)
        {
            if (_nameLabel == null) return;
            _nameLabel.text = enemyName;
            _nameLabel.gameObject.SetActive(true);

            // Color based on level difference (player vs area)
            int diff = playerLevel - areaLevel;
            if (diff >= 10)
                _nameLabel.color = Color.green;       // much stronger than enemies
            else if (diff >= -5)
                _nameLabel.color = Color.white;       // around same level
            else if (diff >= -15)
                _nameLabel.color = Color.red;         // enemies are stronger
            else
                _nameLabel.color = Color.black;       // extremely dangerous
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

            if (enemyID.Contains("rat")) return "Rat_L";
            if (enemyID.Contains("drake") || enemyID.Contains("dragon")) return "Drake_L";
            if (enemyID == "skeleton_knight") return "SkeletonKnight_L";
            if (enemyID.Contains("skeleton")) return "Skeleton_L";
            if (enemyID.Contains("gladiator")) return "Gladiator_L";
            if (enemyID.Contains("acolyte")) return "Acolyte_L";
            if (enemyID.Contains("dark")) return "DarkKnight_L";
            if (enemyID.Contains("fire_imp")) return "FireImp_L";
            if (enemyID.Contains("lava") || enemyID.Contains("golem")) return "LavaGolem_L";
            if (enemyID.Contains("inferno") || enemyID.Contains("beetle")) return "InfernoBeetle_L";
            if (enemyID.Contains("frost_wolf")) return "FrostWolf_L";
            if (enemyID.Contains("ice_wraith")) return "IceWraith_L";
            if (enemyID.Contains("glacier")) return "GlacierGiant_L";
            if (enemyID.Contains("shadow")) return "ShadowStalker_L";
            if (enemyID.Contains("void")) return "VoidWeaver_L";
            if (enemyID.Contains("abyssal")) return "AbyssalHorror_L";

            return "Rat_L";
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
            {
                PlayDeathAnim();
                if (_deathTimer < 0f)
                {
                    _deathTimer = DeathLingerDuration;
                    if (_healthBarRoot != null) _healthBarRoot.SetActive(false);
                    if (_nameLabel != null) _nameLabel.gameObject.SetActive(false);
                }
            }
            else
            {
                PlayHit();
            }
        }

        private void Update()
        {
            if (_deathTimer > 0f)
            {
                _deathTimer -= Time.deltaTime;
                if (_deathTimer <= 0f)
                    _deathTimer = 0f; // signals ready for removal
            }
        }

        public void Deactivate()
        {
            IsActive = false;
            Animator.Stop();
            gameObject.SetActive(false);
        }

        // Full bar width in world units (24px sprite at 32 PPU)
        private const float BarWidth = 0.75f;

        private void UpdateHealthBar()
        {
            if (_healthBarFill == null || _healthBarRoot == null) return;

            float ratio = (float)State.CurrentHP / State.Data.Stats.HP;
            _healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
            // Offset so the bar shrinks from right to left (left edge stays fixed)
            float offset = -BarWidth * (1f - ratio) * 0.5f;
            _healthBarFill.localPosition = new Vector3(offset, 0f, 0f);
            _healthBarRoot.SetActive(true);
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
