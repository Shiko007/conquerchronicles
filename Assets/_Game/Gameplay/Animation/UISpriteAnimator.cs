using UnityEngine;
using UnityEngine.UI;

namespace ConquerChronicles.Gameplay.Animation
{
    /// <summary>
    /// Plays frame-by-frame sprite animations on a UI Image component.
    /// </summary>
    public class UISpriteAnimator : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private string _spritePrefix;
        [SerializeField] private float _fps = 8f;

        private Sprite[] _frames;
        private float _timer;
        private int _currentFrame;

        private void Start()
        {
            if (!string.IsNullOrEmpty(_spritePrefix))
                Play(_spritePrefix, _fps);
        }

        public void Play(string prefix, float fps = 8f)
        {
            if (_image == null)
                _image = GetComponent<Image>();

            _spritePrefix = prefix;
            _fps = fps;
            _frames = SpriteAtlasLoader.GetFrames(prefix);
            _currentFrame = 0;
            _timer = 0f;

            if (_image != null && _frames.Length > 0)
            {
                _image.sprite = _frames[0];
                _image.color = Color.white;
            }
        }

        private void Update()
        {
            if (_frames == null || _frames.Length <= 1) return;

            _timer += Time.deltaTime;
            float frameDuration = 1f / _fps;

            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                _image.sprite = _frames[_currentFrame];
            }
        }
    }
}
