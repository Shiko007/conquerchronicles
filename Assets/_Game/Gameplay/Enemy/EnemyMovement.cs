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
                // In range — stop and attack (slowed enemies attack slower)
                _attackTimer -= Time.deltaTime * speedMult;
                if (_attackTimer <= 0f)
                {
                    _readyToAttack = true;
                }

                // Still face the target
                Vector3 dir = _target.position - transform.position;
                if (_enemyView.SpriteRenderer != null)
                    _enemyView.SpriteRenderer.flipX = dir.x < 0;
            }
            else
            {
                // Move toward target (slowed enemies move slower)
                Vector3 direction = (_target.position - transform.position).normalized;
                transform.position += direction * _moveSpeed * speedMult * Time.deltaTime;

                if (_enemyView.SpriteRenderer != null)
                    _enemyView.SpriteRenderer.flipX = direction.x < 0;
            }
        }

        public float DistanceToTarget()
        {
            if (_target == null) return float.MaxValue;
            return Vector3.Distance(transform.position, _target.position);
        }
    }
}
