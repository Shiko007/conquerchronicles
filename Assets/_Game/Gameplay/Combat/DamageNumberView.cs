using System.Collections;
using UnityEngine;
using TMPro;

namespace ConquerChronicles.Gameplay.Combat
{
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageNumberView : MonoBehaviour
    {
        private TextMeshPro _text;
        private Coroutine _routine;

        private void Awake()
        {
            _text = GetComponent<TextMeshPro>();
        }

        public void Play(int damage, bool isCritical, Vector3 worldPosition, Color? colorOverride = null)
        {
            Color color = colorOverride ?? (isCritical ? new Color(1f, 0.85f, 0f, 1f) : Color.white);
            PlayText(
                damage.ToString(),
                isCritical ? 6f : 4f,
                color,
                worldPosition);
        }

        public void PlayText(string text, float fontSize, Color color, Vector3 worldPosition)
        {
            if (_text == null) _text = GetComponent<TextMeshPro>();
            transform.position = worldPosition;
            _text.text = text;
            _text.fontSize = fontSize;
            _text.color = color;
            _text.alpha = 1f;
            gameObject.SetActive(true);

            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(AnimateRoutine(worldPosition));
        }

        private IEnumerator AnimateRoutine(Vector3 startPos)
        {
            float offsetX = Random.Range(-0.3f, 0.3f);
            Vector3 endPos = startPos + new Vector3(offsetX, 1.2f, 0f);
            float duration = 0.8f;
            float t = 0f;

            while (t < duration)
            {
                t += Time.deltaTime;
                float p = Mathf.Clamp01(t / duration);

                // OutCubic ease for movement
                float moveEase = 1f - Mathf.Pow(1f - p, 3f);
                transform.position = Vector3.Lerp(startPos, endPos, moveEase);

                // InQuad ease for alpha fade
                float alphaEase = p * p;
                _text.alpha = 1f - alphaEase;

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
