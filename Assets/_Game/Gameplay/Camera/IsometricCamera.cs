using UnityEngine;

namespace ConquerChronicles.Gameplay.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class IsometricCamera : MonoBehaviour
    {
        [SerializeField] private float _orthographicSize = 8f;
        [SerializeField] private Transform _followTarget;

        private UnityEngine.Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = _orthographicSize;
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
        }

        private void LateUpdate()
        {
            if (_followTarget != null)
            {
                var pos = _followTarget.position;
                transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            }
        }

        public UnityEngine.Camera Camera => _camera;

        public Rect GetWorldBounds()
        {
            float height = _camera.orthographicSize * 2f;
            float width = height * _camera.aspect;
            var center = transform.position;
            return new Rect(center.x - width / 2f, center.y - height / 2f, width, height);
        }
    }
}
