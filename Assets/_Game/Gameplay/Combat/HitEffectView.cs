using System.Collections;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Combat
{
    public class HitEffectView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private Coroutine _routine;

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
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
            gameObject.SetActive(true);

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(AnimateRoutine());
        }

        private IEnumerator AnimateRoutine()
        {
            // Scale up (OutQuad)
            float t = 0f;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.15f);
                float ease = 1f - (1f - p) * (1f - p); // OutQuad
                transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.2f, ease);
                yield return null;
            }

            // Scale down (InQuad)
            t = 0f;
            while (t < 0.1f)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / 0.1f);
                float ease = p * p; // InQuad
                transform.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.zero, ease);
                yield return null;
            }

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
