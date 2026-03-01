using UnityEngine;

namespace ConquerChronicles.Gameplay.Enemy
{
    [RequireComponent(typeof(EnemyView))]
    public class EnemyMovement : MonoBehaviour
    {
        private EnemyView _enemyView;
        private Transform _target;
        private float _moveSpeed;

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

        private void Update()
        {
            if (!_enemyView.IsActive || _target == null) return;

            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            // Flip sprite based on movement direction
            if (_enemyView.SpriteRenderer != null)
            {
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
