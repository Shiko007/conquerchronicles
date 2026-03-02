using UnityEngine;

namespace ConquerChronicles.Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyView))]
    public class EnemyMovement : MonoBehaviour
    {
        private EnemyView _enemyView;
        private Transform _target;
        private float _moveSpeed;

        private float _attackRange;
        private float _attackCooldown;
        private float _attackTimer;
        private bool _readyToAttack;
        private bool _inRange;

        public bool ReadyToAttack => _readyToAttack;

        private void Awake()
        {
            _enemyView = GetComponent<EnemyView>();
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        public void SetMoveSpeed(float speed)
        {
            _moveSpeed = speed;
        }

        public void SetAttackData(float range, float cooldown)
        {
            _attackRange = range;
            _attackCooldown = cooldown;
            _attackTimer = cooldown;
            _readyToAttack = false;
        }

        public void ConsumeAttack()
        {
            _readyToAttack = false;
            _attackTimer = _attackCooldown;

            // Play attack animation
            _enemyView.PlayAttack();
        }

        private void Update()
        {
            if (!_enemyView.IsActive || _target == null) return;

            // Stun check — skip all movement and attack logic
            if (_enemyView.State != null && _enemyView.State.IsStunned) return;

            float speedMult = (_enemyView.State != null) ? _enemyView.State.MoveSpeedMultiplier : 1f;
            float dist = Vector3.Distance(transform.position, _target.position);

            if (dist <= _attackRange)
            {
                // Just entered range — stop walk, play idle
                if (!_inRange)
                {
                    _inRange = true;
                    _enemyView.PlayIdle();
                }

                // In range — stop and attack (slowed enemies attack slower)
                _attackTimer -= Time.deltaTime * speedMult;
                if (_attackTimer <= 0f)
                {
                    _readyToAttack = true;
                }

                // Still face the target
                Vector3 dir = _target.position - transform.position;
                bool faceLeft = dir.x < 0;
                if (_enemyView.Animator != null)
                    _enemyView.Animator.SetFlipX(faceLeft);
                else if (_enemyView.SpriteRenderer != null)
                    _enemyView.SpriteRenderer.flipX = faceLeft;
            }
            else
            {
                // Left range — resume walking
                if (_inRange)
                {
                    _inRange = false;
                    _enemyView.PlayWalk();
                }

                // Move toward target (slowed enemies move slower)
                Vector3 direction = (_target.position - transform.position).normalized;
                transform.position += direction * _moveSpeed * speedMult * Time.deltaTime;

                bool faceLeft = direction.x < 0;
                if (_enemyView.Animator != null)
                    _enemyView.Animator.SetFlipX(faceLeft);
                else if (_enemyView.SpriteRenderer != null)
                    _enemyView.SpriteRenderer.flipX = faceLeft;
            }
        }

        public float DistanceToTarget()
        {
            if (_target == null) return float.MaxValue;
            return Vector3.Distance(transform.position, _target.position);
        }
    }
}
