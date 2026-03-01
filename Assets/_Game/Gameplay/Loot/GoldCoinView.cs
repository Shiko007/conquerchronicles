using System;
using UnityEngine;
using DG.Tweening;

namespace ConquerChronicles.Gameplay.Loot
{
    public class GoldCoinView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Sequence _sequence;

        public int Amount { get; private set; }
        public Action OnReachedPlayer;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Play(Vector3 spawnPos, Vector3 playerPos, int amount)
        {
            if (_spriteRenderer == null) _spriteRenderer = GetComponent<SpriteRenderer>();
            Amount = amount;
            transform.position = spawnPos;
            transform.localScale = Vector3.one * 0.5f;
            if (_spriteRenderer != null)
                _spriteRenderer.color = new Color(1f, 0.85f, 0.2f, 1f);
            gameObject.SetActive(true);

            var popTarget = spawnPos + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0f);

            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(transform.DOMove(popTarget, 0.15f).SetEase(Ease.OutCubic))
                .Join(transform.DOScale(Vector3.one * 0.7f, 0.15f).SetEase(Ease.OutQuad))
                .Append(transform.DOMove(playerPos, 0.2f).SetEase(Ease.InQuad))
                .Join(transform.DOScale(Vector3.one * 0.2f, 0.2f).SetEase(Ease.InQuad))
                .OnComplete(() =>
                {
                    OnReachedPlayer?.Invoke();
                    OnReachedPlayer = null;
                    gameObject.SetActive(false);
                });
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }
    }
}
