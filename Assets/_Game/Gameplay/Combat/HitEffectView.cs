using UnityEngine;
using DG.Tweening;

namespace ConquerChronicles.Gameplay.Combat
{
    public class HitEffectView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(Vector3 position)
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            transform.position = position;
            transform.localScale = Vector3.one * 0.5f;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
            }
            gameObject.SetActive(true);

            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * 1.2f, 0.15f).SetEase(Ease.OutQuad))
                .Append(transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InQuad))
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }
    }
}
