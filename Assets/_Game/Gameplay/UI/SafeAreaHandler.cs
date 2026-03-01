using UnityEngine;

namespace ConquerChronicles.Gameplay.UI
{
    /// <summary>
    /// Adjusts the attached RectTransform to fit within Screen.safeArea.
    /// Attach to a full-screen child panel that acts as the root content container.
    /// Handles notches, camera cutouts, and orientation changes.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaHandler : MonoBehaviour
    {
        private RectTransform _rt;
        private Rect _lastSafeArea;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        private void Update()
        {
            if (_lastSafeArea != Screen.safeArea)
                ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            var safeArea = Screen.safeArea;
            _lastSafeArea = safeArea;

            var anchorMin = new Vector2(safeArea.x / Screen.width, safeArea.y / Screen.height);
            var anchorMax = new Vector2(
                (safeArea.x + safeArea.width) / Screen.width,
                (safeArea.y + safeArea.height) / Screen.height);

            _rt.anchorMin = anchorMin;
            _rt.anchorMax = anchorMax;
            _rt.offsetMin = Vector2.zero;
            _rt.offsetMax = Vector2.zero;
        }
    }
}
