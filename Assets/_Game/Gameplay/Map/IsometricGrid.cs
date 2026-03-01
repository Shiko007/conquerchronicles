using UnityEngine;

namespace ConquerChronicles.Gameplay.Map
{
    public class IsometricGrid : MonoBehaviour
    {
        [SerializeField] private float _tileWidth = 64f;
        [SerializeField] private float _tileHeight = 32f;
        [SerializeField] private float _pixelsPerUnit = 32f;

        public float TileWidth => _tileWidth;
        public float TileHeight => _tileHeight;

        public Vector3 GridToWorld(Vector2Int gridPos)
        {
            float worldX = (gridPos.x - gridPos.y) * (_tileWidth / 2f) / _pixelsPerUnit;
            float worldY = (gridPos.x + gridPos.y) * (_tileHeight / 2f) / _pixelsPerUnit;
            return new Vector3(worldX, worldY, 0f);
        }

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            float px = worldPos.x * _pixelsPerUnit;
            float py = worldPos.y * _pixelsPerUnit;

            float gx = (px / (_tileWidth / 2f) + py / (_tileHeight / 2f)) / 2f;
            float gy = (py / (_tileHeight / 2f) - px / (_tileWidth / 2f)) / 2f;

            return new Vector2Int(Mathf.RoundToInt(gx), Mathf.RoundToInt(gy));
        }

        public Vector3 SnapToGrid(Vector3 worldPos)
        {
            var grid = WorldToGrid(worldPos);
            return GridToWorld(grid);
        }
    }
}
