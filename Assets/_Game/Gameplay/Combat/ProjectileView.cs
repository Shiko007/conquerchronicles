using UnityEngine;

namespace ConquerChronicles.Gameplay.Combat
{
    /// <summary>
    /// Lightweight projectile that moves in a direction and deactivates after
    /// exceeding its max distance or lifetime. Designed for pooled reuse.
    /// </summary>
    public class ProjectileView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private Vector3 _direction;
        private float _speed;
        private float _maxDistance;
        private float _maxLifetime;

        private Vector3 _origin;
        private float _elapsed;
        private bool _active;

        public bool IsActive => _active;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Launch the projectile from a position in a given direction.
        /// </summary>
        public void Launch(Vector3 origin, Vector3 direction, float speed, float maxDistance = 12f, float maxLifetime = 3f)
        {
            _origin = origin;
            _direction = direction.normalized;
            _speed = speed;
            _maxDistance = maxDistance;
            _maxLifetime = maxLifetime;
            _elapsed = 0f;
            _active = true;

            transform.position = origin;

            // Rotate sprite to face direction
            float angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_active) return;

            float dt = Time.deltaTime;
            _elapsed += dt;

            // Move
            transform.position += _direction * (_speed * dt);

            // Check expiry: lifetime or distance
            if (_elapsed >= _maxLifetime ||
                (transform.position - _origin).sqrMagnitude >= _maxDistance * _maxDistance)
            {
                Deactivate();
            }
        }

        public void Deactivate()
        {
            _active = false;
            gameObject.SetActive(false);
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
