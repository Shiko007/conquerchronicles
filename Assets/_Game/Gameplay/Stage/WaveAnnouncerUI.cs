using UnityEngine;
using TMPro;
using DG.Tweening;

namespace ConquerChronicles.Gameplay.Stage
{
    public class WaveAnnouncerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _displayDuration = 2f;

        private Sequence _sequence;

        public void Initialize(TextMeshProUGUI text, CanvasGroup canvasGroup)
        {
            _text = text;
            _canvasGroup = canvasGroup;
        }

        public void ShowAnnouncement(string message)
        {
            if (_text == null || _canvasGroup == null) return;

            _sequence?.Kill();
            _text.text = message;
            _canvasGroup.alpha = 0f;

            _sequence = DOTween.Sequence()
                .Append(DOTween.To(() => _canvasGroup.alpha, a => _canvasGroup.alpha = a, 1f, 0.3f).SetEase(Ease.OutQuad))
                .AppendInterval(_displayDuration)
                .Append(DOTween.To(() => _canvasGroup.alpha, a => _canvasGroup.alpha = a, 0f, 0.5f).SetEase(Ease.InQuad));
        }

        private void OnDestroy()
        {
            _sequence?.Kill();
        }
    }
}
