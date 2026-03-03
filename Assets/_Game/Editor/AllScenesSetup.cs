using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ConquerChronicles.Editor
{
    /// <summary>
    /// Creates all game scenes as separate .unity files and adds them to Build Settings.
    /// Run this once via the menu, then use individual scene setups to regenerate content.
    /// </summary>
    public static class AllScenesSetup
    {
        private const string ScenesFolder = "Assets/Scenes";

        [MenuItem("Conquer Chronicles/Setup All Scenes (Create + Build Settings)")]
        public static void SetupAll()
        {
            // Ensure Scenes folder exists
            if (!AssetDatabase.IsValidFolder(ScenesFolder))
                AssetDatabase.CreateFolder("Assets", "Scenes");

            string originalScene = EditorSceneManager.GetActiveScene().path;

            // 1. Create and setup MainMenu scene
            CreateAndSetupScene("MainMenu", () => MainMenuSceneSetup.Setup());

            // 2. Create and setup Gameplay scene
            CreateAndSetupScene("Gameplay", () => Phase2SceneSetup.SetupMap());

            // 3. Create and setup Mining scene
            CreateAndSetupScene("Mining", () => MiningSceneSetup.Setup());

            // 4. Create and setup Equipment scene
            CreateAndSetupScene("Equipment", () => EquipmentSceneSetup.Setup());

            // 5. Create and setup Market scene
            CreateAndSetupScene("Market", () => MarketSceneSetup.Setup());

            // 6. Create and setup Inventory scene
            CreateAndSetupScene("Inventory", () => InventorySceneSetup.Setup());

            // 7. Add all scenes to Build Settings
            AddScenesToBuildSettings(new[]
            {
                $"{ScenesFolder}/MainMenu.unity",
                $"{ScenesFolder}/Gameplay.unity",
                $"{ScenesFolder}/Mining.unity",
                $"{ScenesFolder}/Equipment.unity",
                $"{ScenesFolder}/Market.unity",
                $"{ScenesFolder}/Inventory.unity"
            });

            // 8. Return to MainMenu scene
            EditorSceneManager.OpenScene($"{ScenesFolder}/MainMenu.unity");

            Debug.Log("[Conquer Chronicles] All scenes created and added to Build Settings!\n" +
                      "  - MainMenu (index 0)\n" +
                      "  - Gameplay (index 1)\n" +
                      "  - Mining (index 2)\n" +
                      "  - Equipment (index 3)\n" +
                      "  - Market (index 4)\n" +
                      "  - Inventory (index 5)");
        }

        private static void CreateAndSetupScene(string sceneName, System.Action setupAction)
        {
            string path = $"{ScenesFolder}/{sceneName}.unity";

            // Create a fresh empty scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Run the setup logic (creates camera, canvas, UI, etc.)
            setupAction();

            // Save the scene
            EditorSceneManager.SaveScene(scene, path);
            Debug.Log($"[Conquer Chronicles] Created scene: {path}");
        }

        private static void AddScenesToBuildSettings(string[] scenePaths)
        {
            var buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            foreach (var path in scenePaths)
            {
                // Skip if already in build settings
                if (buildScenes.Any(s => s.path == path))
                    continue;

                buildScenes.Add(new EditorBuildSettingsScene(path, true));
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();
        }
    }
}
