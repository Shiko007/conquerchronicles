using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ConquerChronicles.Gameplay.UI
{
    [RequireComponent(typeof(Image))]
    public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private const float HoldThreshold = 0.5f;

        public Action OnClick;
        public Action OnLongPress;

        private bool _isPressed;
        private bool _longPressFired;
        private float _pressTime;

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressed = true;
            _longPressFired = false;
            _pressTime = Time.unscaledTime;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isPressed && !_longPressFired)
                OnClick?.Invoke();

            _isPressed = false;
        }

        private void Update()
        {
            if (!_isPressed || _longPressFired) return;

            if (Time.unscaledTime - _pressTime >= HoldThreshold)
            {
                _longPressFired = true;
                OnLongPress?.Invoke();
            }
        }
    }
}
