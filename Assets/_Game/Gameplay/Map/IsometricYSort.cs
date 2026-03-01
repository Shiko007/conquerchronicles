using UnityEngine;

namespace ConquerChronicles.Gameplay.Map
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class IsometricYSort : MonoBehaviour
    {
        [SerializeField] private int _sortingPrecision = 100;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void LateUpdate()
        {
            _spriteRenderer.sortingOrder = -(int)(transform.position.y * _sortingPrecision);
        }
    }
}
