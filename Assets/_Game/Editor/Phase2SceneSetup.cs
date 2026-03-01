using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConquerChronicles.Gameplay.Bootstrap;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Map;

namespace ConquerChronicles.Editor
{
    public static class Phase2SceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Phase 2 Scene")]
        public static void Setup()
        {
            // --- Create Enemy Prefab ---
            var enemyPrefab = CreateEnemyPrefab();

            // --- Clear current scene objects (except camera) ---
            var scene = EditorSceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.GetComponent<UnityEngine.Camera>() != null) continue;
                Object.DestroyImmediate(root);
            }

            // --- Main Camera ---
            var cameraGO = GameObject.FindFirstObjectByType<UnityEngine.Camera>()?.gameObject;
            if (cameraGO == null)
            {
                cameraGO = new GameObject("Main Camera");
                var cam = cameraGO.AddComponent<UnityEngine.Camera>();
                cam.orthographic = true;
                cam.orthographicSize = 5f;
                cameraGO.AddComponent<AudioListener>();
                cameraGO.tag = "MainCamera";
            }
            cameraGO.transform.position = new Vector3(0, 0, -10);
            var camera = cameraGO.GetComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5f;
            camera.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            var isoCam = cameraGO.GetComponent<IsometricCamera>();
            if (isoCam == null) isoCam = cameraGO.AddComponent<IsometricCamera>();

            // --- Player ---
            var playerGO = new GameObject("Player");
            playerGO.transform.position = Vector3.zero;
            var playerSR = playerGO.AddComponent<SpriteRenderer>();
            playerSR.sprite = CreateCircleSprite("PlayerSprite", new Color(0.2f, 0.5f, 1f, 1f), 32);
            playerSR.sortingLayerName = "Default";
            playerGO.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            var characterView = playerGO.AddComponent<CharacterView>();
            playerGO.AddComponent<IsometricYSort>();

            // Wire CharacterView sprite renderer via serialized field
            var cvSO = new SerializedObject(characterView);
            cvSO.FindProperty("_spriteRenderer").objectReferenceValue = playerSR;
            cvSO.ApplyModifiedPropertiesWithoutUndo();

            // --- EnemyPool ---
            var poolGO = new GameObject("EnemyPool");
            var enemyPool = poolGO.AddComponent<EnemyPool>();
            var poolSO = new SerializedObject(enemyPool);
            poolSO.FindProperty("_prefab").objectReferenceValue = enemyPrefab.GetComponent<EnemyView>();
            poolSO.FindProperty("_warmupCount").intValue = 64;
            poolSO.ApplyModifiedPropertiesWithoutUndo();

            // --- EnemySpawner ---
            var spawnerGO = new GameObject("EnemySpawner");
            var enemySpawner = spawnerGO.AddComponent<EnemySpawner>();

            // --- MapBoundsProvider ---
            var boundsGO = new GameObject("MapBoundsProvider");
            var mapBounds = boundsGO.AddComponent<MapBoundsProvider>();

            // --- IsometricGrid ---
            var gridGO = new GameObject("IsometricGrid");
            gridGO.AddComponent<IsometricGrid>();

            // --- GameManager (Test Setup) ---
            var managerGO = new GameObject("GameManager");
            var testSetup = managerGO.AddComponent<GameplayTestSetup>();
            var tsSO = new SerializedObject(testSetup);
            tsSO.FindProperty("_isometricCamera").objectReferenceValue = isoCam;
            tsSO.FindProperty("_characterView").objectReferenceValue = characterView;
            tsSO.FindProperty("_enemyPool").objectReferenceValue = enemyPool;
            tsSO.FindProperty("_enemySpawner").objectReferenceValue = enemySpawner;
            tsSO.FindProperty("_mapBoundsProvider").objectReferenceValue = mapBounds;
            tsSO.FindProperty("_spawnInterval").floatValue = 2f;
            tsSO.FindProperty("_maxEnemies").intValue = 30;
            tsSO.ApplyModifiedPropertiesWithoutUndo();

            // --- Mark scene dirty so it can be saved ---
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[Conquer Chronicles] Phase 2 scene setup complete! Hit Play to test.");
        }

        private static GameObject CreateEnemyPrefab()
        {
            // Ensure directory exists
            if (!AssetDatabase.IsValidFolder("Assets/_Game/Data"))
                AssetDatabase.CreateFolder("Assets/_Game", "Data");
            if (!AssetDatabase.IsValidFolder("Assets/_Game/Data/Prefabs"))
                AssetDatabase.CreateFolder("Assets/_Game/Data", "Prefabs");

            // Check if prefab already exists
            var existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");
            if (existingPrefab != null) return existingPrefab;

            // Build the enemy GameObject
            var enemyGO = new GameObject("Enemy_Slime");
            var sr = enemyGO.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite("EnemySprite", new Color(0.3f, 0.9f, 0.3f, 1f), 24);
            sr.sortingLayerName = "Default";

            var enemyView = enemyGO.AddComponent<EnemyView>();
            var evSO = new SerializedObject(enemyView);
            evSO.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            evSO.ApplyModifiedPropertiesWithoutUndo();

            enemyGO.AddComponent<EnemyMovement>();
            enemyGO.AddComponent<IsometricYSort>();

            // Save as prefab
            var prefab = PrefabUtility.SaveAsPrefabAsset(enemyGO, "Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");
            Object.DestroyImmediate(enemyGO);

            Debug.Log("[Conquer Chronicles] Created enemy prefab: Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");
            return prefab;
        }

        private static Sprite CreateCircleSprite(string name, Color color, int size)
        {
            // Check if sprite already exists
            string path = $"Assets/Visual/Sprites/{name}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            // Generate a simple filled circle texture
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            float center = size / 2f;
            float radius = size / 2f - 1f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist <= radius)
                    {
                        // Simple shading: lighter toward top-left
                        float shade = 1f - (dist / radius) * 0.3f;
                        float highlight = Mathf.Max(0, 1f - Vector2.Distance(
                            new Vector2(x, y),
                            new Vector2(center - radius * 0.3f, center + radius * 0.3f)) / radius);
                        Color c = color * shade + Color.white * highlight * 0.2f;
                        c.a = 1f;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();

            // Save texture as PNG
            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();

            // Configure import settings for pixel art
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
