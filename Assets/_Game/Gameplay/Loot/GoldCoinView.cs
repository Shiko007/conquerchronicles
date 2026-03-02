using System;
using System.Collections;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Loot
{
    public class GoldCoinView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Coroutine _routine;

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

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(AnimateRoutine(spawnPos, playerPos));
        }

        private IEnumerator AnimateRoutine(Vector3 spawnPos, Vector3 playerPos)
        {
            var popTarget = spawnPos + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0f);

            // Pop up phase (OutCubic)
            float t = 0f;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.15f);
                float moveEase = 1f - Mathf.Pow(1f - p, 3f);
                float scaleEase = 1f - (1f - p) * (1f - p);
                transform.position = Vector3.Lerp(spawnPos, popTarget, moveEase);
                transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 0.7f, scaleEase);
                yield return null;
            }

            // Zip to player phase (InQuad)
            t = 0f;
            Vector3 startPos = transform.position;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.2f);
                float ease = p * p;
                transform.position = Vector3.Lerp(startPos, playerPos, ease);
                transform.localScale = Vector3.Lerp(Vector3.one * 0.7f, Vector3.one * 0.2f, ease);
                yield return null;
            }

            OnReachedPlayer?.Invoke();
            OnReachedPlayer = null;
            gameObject.SetActive(false);
            _routine = null;
        }

        private void OnDisable()
        {
            if (_routine != null)
            {
                StopCoroutine(_routine);
                _routine = null;
            }
        }
    }
}
