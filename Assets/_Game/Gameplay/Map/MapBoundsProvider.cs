using UnityEngine;
using ConquerChronicles.Gameplay.Camera;

namespace ConquerChronicles.Gameplay.Map
{
    public enum SpawnEdge
    {
        North,
        South,
        East,
        West,
        Random
    }

    public class MapBoundsProvider : MonoBehaviour
    {
        [SerializeField] private IsometricCamera _camera;
        [SerializeField] private float _spawnMargin = 1.5f;

        public void Initialize(IsometricCamera camera)
        {
            _camera = camera;
        }

        public Vector3 GetSpawnPosition(SpawnEdge edge)
        {
            if (edge == SpawnEdge.Random)
                edge = (SpawnEdge)UnityEngine.Random.Range(0, 4);

            var bounds = _camera.GetWorldBounds();
            float margin = _spawnMargin;

            return edge switch
            {
                SpawnEdge.North => new Vector3(
                    UnityEngine.Random.Range(bounds.xMin, bounds.xMax),
                    bounds.yMax + margin,
                    0f),
                SpawnEdge.South => new Vector3(
                    UnityEngine.Random.Range(bounds.xMin, bounds.xMax),
                    bounds.yMin - margin,
                    0f),
                SpawnEdge.East => new Vector3(
                    bounds.xMax + margin,
                    UnityEngine.Random.Range(bounds.yMin, bounds.yMax),
                    0f),
                SpawnEdge.West => new Vector3(
                    bounds.xMin - margin,
                    UnityEngine.Random.Range(bounds.yMin, bounds.yMax),
                    0f),
                _ => new Vector3(
                    UnityEngine.Random.Range(bounds.xMin, bounds.xMax),
                    bounds.yMax + margin,
                    0f)
            };
        }
    }
}
