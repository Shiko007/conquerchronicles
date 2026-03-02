using System;
using UnityEngine;

namespace ConquerChronicles.Gameplay.Animation
{
    /// <summary>
    /// Plays frame-by-frame sprite animations on a SpriteRenderer.
    /// Supports looping, one-shot, and completion callbacks.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAnimator : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Sprite[] _currentFrames;
        private float _frameRate = 8f;
        private float _timer;
        private int _currentFrame;
        private bool _playing;
        private bool _loop;
        private Action _onComplete;

        // Cached animation sets keyed by name
        private string _currentAnimName;

        public bool IsPlaying => _playing;
        public string CurrentAnimation => _currentAnimName;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// Play an animation by providing sprite frames directly.
        /// </summary>
        public void Play(string animName, Sprite[] frames, float fps = 8f, bool loop = true, Action onComplete = null)
        {
            if (frames == null || frames.Length == 0) return;

            // Don't restart if already playing the same animation
            if (_playing && _currentAnimName == animName && _loop == loop) return;

            _currentAnimName = animName;
            _currentFrames = frames;
            _frameRate = fps;
            _loop = loop;
            _onComplete = onComplete;
            _currentFrame = 0;
            _timer = 0f;
            _playing = true;

            _spriteRenderer.sprite = _currentFrames[0];
        }

        /// <summary>
        /// Play a one-shot animation, then return to the previous animation.
        /// Always restarts from frame 0, even if the same one-shot is already playing
        /// (e.g. rapid hits should each visually restart the hit animation).
        /// </summary>
        public void PlayOneShot(string animName, Sprite[] frames, float fps = 10f, Action onComplete = null)
        {
            // Force-reset if the same one-shot is already playing so rapid
            // successive calls (e.g. multi-hit skills) always restart visibly.
            if (_playing && _currentAnimName == animName && !_loop)
            {
                _currentFrame = 0;
                _timer = 0f;
                _onComplete = onComplete;
                _spriteRenderer.sprite = frames[0];
                return;
            }

            Play(animName, frames, fps, false, onComplete);
        }

        /// <summary>
        /// Set a static sprite (no animation).
        /// </summary>
        public void SetStatic(Sprite sprite)
        {
            _playing = false;
            _currentAnimName = null;
            if (_spriteRenderer != null && sprite != null)
                _spriteRenderer.sprite = sprite;
        }

        /// <summary>
        /// Stop the current animation.
        /// </summary>
        public void Stop()
        {
            _playing = false;
        }

        /// <summary>
        /// Flip the sprite horizontally.
        /// </summary>
        public void SetFlipX(bool flip)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.flipX = flip;
        }

        public bool FlipX => _spriteRenderer != null && _spriteRenderer.flipX;

        private void Update()
        {
            if (!_playing || _currentFrames == null || _currentFrames.Length <= 1) return;

            _timer += Time.deltaTime;
            float frameDuration = 1f / _frameRate;

            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                _currentFrame++;

                if (_currentFrame >= _currentFrames.Length)
                {
                    if (_loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _currentFrames.Length - 1;
                        _playing = false;
                        _onComplete?.Invoke();
                        return;
                    }
                }

                _spriteRenderer.sprite = _currentFrames[_currentFrame];
            }
        }
    }
}
