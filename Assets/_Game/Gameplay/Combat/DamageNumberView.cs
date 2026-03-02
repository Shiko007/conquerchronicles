using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ConquerChronicles.Gameplay.Combat
{
    [RequireComponent(typeof(TextMeshPro))]
    public class DamageNumberView : MonoBehaviour
    {
        private TextMeshPro _text;
        private Sequence _sequence;

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

            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(transform.DOMove(worldPosition + new Vector3(Random.Range(-0.3f, 0.3f), 1.2f, 0f), 0.8f).SetEase(Ease.OutCubic))
                .Join(DOTween.To(() => _text.alpha, a => _text.alpha = a, 0f, 0.8f).SetEase(Ease.InQuad))
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void OnDisable()
        {
            _sequence?.Kill();
        }
    }
}
