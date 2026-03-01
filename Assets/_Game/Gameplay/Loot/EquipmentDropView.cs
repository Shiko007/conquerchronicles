using System;
using UnityEngine;
using DG.Tweening;

namespace ConquerChronicles.Gameplay.Loot
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EquipmentDropView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private string _itemID;
        private int _quantity;
        private Tweener _bobTween;
        private Sequence _collectSequence;
        private CircleCollider2D _collider;

        public string ItemID => _itemID;
        public int Quantity => _quantity;
        public bool IsCollecting { get; private set; }

        public Action OnCollectComplete;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<CircleCollider2D>();
        }

        public void Initialize(string itemID, int quantity, Vector3 worldPos)
        {
            _itemID = itemID;
            _quantity = quantity;
            IsCollecting = false;
            transform.position = worldPos;
            transform.localScale = Vector3.one * 0.6f;

            if (_spriteRenderer != null)
                _spriteRenderer.color = new Color(0.7f, 0.3f, 0.9f, 1f);

            if (_collider != null)
            {
                _collider.enabled = true;
                _collider.radius = 0.5f;
            }

            gameObject.SetActive(true);

            _bobTween?.Kill();
            _bobTween = transform.DOLocalMoveY(worldPos.y + 0.15f, 0.6f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void Collect(Vector3 playerPos)
        {
            if (IsCollecting) return;
            IsCollecting = true;

            // Stop bobbing and disable collider immediately
            _bobTween?.Kill();
            if (_collider != null)
                _collider.enabled = false;

            // Zip to player
            _collectSequence?.Kill();
            _collectSequence = DOTween.Sequence()
                .Append(transform.DOMove(playerPos, 0.25f).SetEase(Ease.InQuad))
                .Join(transform.DOScale(Vector3.one * 0.2f, 0.25f).SetEase(Ease.InQuad))
                .OnComplete(() =>
                {
                    OnCollectComplete?.Invoke();
                    OnCollectComplete = null;
                });
        }

        public void Deactivate()
        {
            _bobTween?.Kill();
            _collectSequence?.Kill();
            IsCollecting = false;
            if (_collider != null)
                _collider.enabled = false;
            OnCollectComplete = null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            _bobTween?.Kill();
            _collectSequence?.Kill();
        }
    }
}
