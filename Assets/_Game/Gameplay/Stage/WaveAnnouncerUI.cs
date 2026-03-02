using System.Collections;
using UnityEngine;
using TMPro;

namespace ConquerChronicles.Gameplay.Stage
{
    public class WaveAnnouncerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _displayDuration = 2f;

        private Coroutine _routine;

        public void Initialize(TextMeshProUGUI text, CanvasGroup canvasGroup)
        {
            _text = text;
            _canvasGroup = canvasGroup;
        }

        public void ShowAnnouncement(string message)
        {
            if (_text == null || _canvasGroup == null) return;

            if (_routine != null) StopCoroutine(_routine);
            _text.text = message;
            _canvasGroup.alpha = 0f;
            _routine = StartCoroutine(AnnouncementRoutine());
        }

        private IEnumerator AnnouncementRoutine()
        {
            // Fade in
            float t = 0f;
            while (t < 0.3f)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(t / 0.3f);
                yield return null;
            }
            _canvasGroup.alpha = 1f;

            // Hold
            yield return new WaitForSeconds(_displayDuration);

            // Fade out
            t = 0f;
            while (t < 0.5f)
            {
                t += Time.deltaTime;
                _canvasGroup.alpha = 1f - Mathf.Clamp01(t / 0.5f);
                yield return null;
            }
            _canvasGroup.alpha = 0f;
            _routine = null;
        }

        private void OnDestroy()
        {
            if (_routine != null) StopCoroutine(_routine);
        }
    }
}
