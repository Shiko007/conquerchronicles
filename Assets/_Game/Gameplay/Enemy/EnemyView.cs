using UnityEngine;
using ConquerChronicles.Core.Enemy;

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

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
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
        }

        public void TakeDamage(int damage)
        {
            State.TakeDamage(damage);
            UpdateHealthBar();
        }

        public void Deactivate()
        {
            IsActive = false;
            gameObject.SetActive(false);
        }

        private void UpdateHealthBar()
        {
            if (_healthBarFill == null || _healthBarRoot == null) return;

            float ratio = (float)State.CurrentHP / State.Data.Stats.HP;
            _healthBarFill.localScale = new Vector3(ratio, 1f, 1f);
            _healthBarRoot.SetActive(ratio < 1f);
        }

        public SpriteRenderer SpriteRenderer => _spriteRenderer;
    }
}
