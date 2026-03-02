using System;
using System.Collections;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Loot
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class EquipmentDropView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private string _itemID;
        private int _quantity;
        private Coroutine _bobRoutine;
        private Coroutine _collectRoutine;
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

            if (_bobRoutine != null) StopCoroutine(_bobRoutine);
            _bobRoutine = StartCoroutine(BobRoutine(worldPos));
        }

        private IEnumerator BobRoutine(Vector3 basePos)
        {
            float t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                float offset = Mathf.Sin(t * Mathf.PI / 0.6f) * 0.15f;
                transform.position = new Vector3(basePos.x, basePos.y + offset, basePos.z);
                yield return null;
            }
        }

        public void Collect(Vector3 playerPos)
        {
            if (IsCollecting) return;
            IsCollecting = true;

            if (_bobRoutine != null) { StopCoroutine(_bobRoutine); _bobRoutine = null; }
            if (_collider != null)
                _collider.enabled = false;

            if (_collectRoutine != null) StopCoroutine(_collectRoutine);
            _collectRoutine = StartCoroutine(CollectRoutine(playerPos));
        }

        private IEnumerator CollectRoutine(Vector3 playerPos)
        {
            Vector3 startPos = transform.position;
            Vector3 startScale = transform.localScale;
            float t = 0f;

            while (t < 0.25f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.25f);
                float ease = p * p; // InQuad
                transform.position = Vector3.Lerp(startPos, playerPos, ease);
                transform.localScale = Vector3.Lerp(startScale, Vector3.one * 0.2f, ease);
                yield return null;
            }

            OnCollectComplete?.Invoke();
            OnCollectComplete = null;
            _collectRoutine = null;
        }

        public void Deactivate()
        {
            if (_bobRoutine != null) { StopCoroutine(_bobRoutine); _bobRoutine = null; }
            if (_collectRoutine != null) { StopCoroutine(_collectRoutine); _collectRoutine = null; }
            IsCollecting = false;
            if (_collider != null)
                _collider.enabled = false;
            OnCollectComplete = null;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (_bobRoutine != null) { StopCoroutine(_bobRoutine); _bobRoutine = null; }
            if (_collectRoutine != null) { StopCoroutine(_collectRoutine); _collectRoutine = null; }
        }
    }
}
