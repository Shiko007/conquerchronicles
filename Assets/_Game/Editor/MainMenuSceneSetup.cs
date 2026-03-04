using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.UI.MainMenu;

namespace ConquerChronicles.Editor
{
    public static class MainMenuSceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Main Menu Scene")]
        public static void Setup()
        {
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
            camera.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;

            // --- EventSystem (required for UI input) ---
            var existingES = GameObject.FindFirstObjectByType<EventSystem>();
            if (existingES == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                // Add InputSystemUIInputModule via reflection to avoid Editor assembly reference issues
                var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputModuleType != null)
                    esGO.AddComponent(inputModuleType);
                else
                    esGO.AddComponent<StandaloneInputModule>();
            }

            // --- Main Menu Canvas ---
            var canvasGO = new GameObject("MainMenu_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // SafeArea container
            var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
            safeAreaGO.transform.SetParent(canvasGO.transform, false);
            var safeAreaRT = safeAreaGO.GetComponent<RectTransform>();
            safeAreaRT.anchorMin = Vector2.zero;
            safeAreaRT.anchorMax = Vector2.one;
            safeAreaRT.offsetMin = Vector2.zero;
            safeAreaRT.offsetMax = Vector2.zero;
            safeAreaGO.AddComponent<ConquerChronicles.Gameplay.UI.SafeAreaHandler>();

            // ======================
            // 1. Title Area (top 20%)
            // ======================
            var titleGO = CreateUIText(safeAreaGO.transform, "TitleText", "CONQUER CHRONICLES",
                new Vector2(0, 0.80f), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 64);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.color = new Color(1.0f, 0.85f, 0.2f, 1f);
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            // Stretch to fill anchor area
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 0.80f);
            titleRT.anchorMax = new Vector2(1, 1f);
            titleRT.offsetMin = Vector2.zero;
            titleRT.offsetMax = Vector2.zero;

            // ======================
            // 2. Player Info Panel
            // ======================
            var infoPanelGO = CreateUIImage(safeAreaGO.transform, "PlayerInfoPanel",
                new Vector2(0.1f, 0.68f), new Vector2(0.9f, 0.78f),
                Vector2.zero, Vector2.zero,
                Color.white);
            UIAtlasHelper.SetSlicedPanel(infoPanelGO.GetComponent<Image>());
            var infoPanelContent = UIAtlasHelper.CreatePanelContent(infoPanelGO.transform);
            // Stretch to fill anchor area
            var infoPanelRT = infoPanelGO.GetComponent<RectTransform>();
            infoPanelRT.offsetMin = Vector2.zero;
            infoPanelRT.offsetMax = Vector2.zero;

            // Player name (class)
            var playerNameGO = CreateUIText(infoPanelContent, "PlayerNameText", "Trojan",
                new Vector2(0, 0.55f), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 32);
            var playerNameTMP = playerNameGO.GetComponent<TextMeshProUGUI>();
            playerNameTMP.alignment = TextAlignmentOptions.Center;
            playerNameTMP.fontStyle = FontStyles.Bold;
            var playerNameRT = playerNameGO.GetComponent<RectTransform>();
            playerNameRT.offsetMin = Vector2.zero;
            playerNameRT.offsetMax = Vector2.zero;

            // Level and Gold on the bottom half of the info panel, side by side
            var levelGO = CreateUIText(infoPanelContent, "LevelText", "Lv. 1",
                new Vector2(0, 0), new Vector2(0.5f, 0.55f),
                Vector2.zero, Vector2.zero, 28);
            var levelTMP = levelGO.GetComponent<TextMeshProUGUI>();
            levelTMP.alignment = TextAlignmentOptions.Center;
            var levelRT = levelGO.GetComponent<RectTransform>();
            levelRT.offsetMin = Vector2.zero;
            levelRT.offsetMax = Vector2.zero;

            var goldGO = CreateUIText(infoPanelContent, "GoldText", "0 Gold",
                new Vector2(0.5f, 0), new Vector2(1, 0.55f),
                Vector2.zero, Vector2.zero, 28);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Center;
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.offsetMin = Vector2.zero;
            goldRT.offsetMax = Vector2.zero;

            // ======================
            // 3. Battle Button (centered)
            // ======================
            float buttonWidthNorm = 0.35f; // narrow, fits "Battle" text
            float buttonLeft = (1f - buttonWidthNorm) / 2f;
            float buttonRight = buttonLeft + buttonWidthNorm;
            float buttonHeightNorm = 80f / 1920f;
            float buttonCenterY = 0.5f;
            float buttonBottom = buttonCenterY - buttonHeightNorm / 2f;
            float buttonTop = buttonCenterY + buttonHeightNorm / 2f;

            var battleBtn = CreateButton(safeAreaGO.transform, "BattleButton", "Battle",
                new Vector2(buttonLeft, buttonBottom), new Vector2(buttonRight, buttonTop),
                Color.white);
            UIAtlasHelper.SetSpriteSwapButton(battleBtn.GetComponent<Button>(), battleBtn.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var battleBtnContent = UIAtlasHelper.CreateButtonContent(battleBtn.transform, 80f);
            var battleLabel = battleBtn.transform.Find("Label");
            if (battleLabel != null) battleLabel.SetParent(battleBtnContent, false);

            // ======================
            // 4. Version Text (bottom)
            // ======================
            var versionGO = CreateUIText(safeAreaGO.transform, "VersionText", "v0.1",
                new Vector2(0, 0), new Vector2(1, 0.05f),
                Vector2.zero, Vector2.zero, 20);
            var versionTMP = versionGO.GetComponent<TextMeshProUGUI>();
            versionTMP.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            versionTMP.alignment = TextAlignmentOptions.Center;
            var versionRT = versionGO.GetComponent<RectTransform>();
            versionRT.offsetMin = Vector2.zero;
            versionRT.offsetMax = Vector2.zero;

            // ======================
            // Wire MainMenuUI Component
            // ======================
            var mainMenuUI = canvasGO.AddComponent<MainMenuUI>();
            var so = new SerializedObject(mainMenuUI);

            so.FindProperty("_titleText").objectReferenceValue = titleTMP;
            so.FindProperty("_playerNameText").objectReferenceValue = playerNameTMP;
            so.FindProperty("_levelText").objectReferenceValue = levelTMP;
            so.FindProperty("_goldText").objectReferenceValue = goldTMP;
            so.FindProperty("_mapSelectButton").objectReferenceValue = battleBtn.GetComponent<Button>();

            so.ApplyModifiedPropertiesWithoutUndo();

            // ======================
            // Wire MainMenuController (handles scene navigation)
            // ======================
            var controllerGO = new GameObject("MainMenuController");
            var controller = controllerGO.AddComponent<MainMenuController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_menuUI").objectReferenceValue = mainMenuUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            // --- Mark scene dirty ---
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[Conquer Chronicles] Main Menu scene setup complete! Hit Play to test.");
        }

        // --- UI Helpers ---

        private static GameObject CreateButton(Transform parent, string name, string label,
            Vector2 anchorMin, Vector2 anchorMax, Color bgColor)
        {
            var btnGO = new GameObject(name, typeof(RectTransform));
            btnGO.transform.SetParent(parent, false);
            var btnRT = btnGO.GetComponent<RectTransform>();
            btnRT.anchorMin = anchorMin;
            btnRT.anchorMax = anchorMax;
            btnRT.offsetMin = Vector2.zero;
            btnRT.offsetMax = Vector2.zero;

            var btnImg = btnGO.AddComponent<Image>();
            btnImg.color = bgColor;

            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = btnImg;

            // Button label text
            var labelGO = new GameObject("Label", typeof(RectTransform));
            labelGO.transform.SetParent(btnGO.transform, false);
            var labelRT = labelGO.GetComponent<RectTransform>();
            labelRT.anchorMin = Vector2.zero;
            labelRT.anchorMax = Vector2.one;
            labelRT.offsetMin = Vector2.zero;
            labelRT.offsetMax = Vector2.zero;

            var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
            labelTMP.text = label;
            labelTMP.fontSize = 36;
            labelTMP.color = Color.white;
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.alignment = TextAlignmentOptions.Center;

            return btnGO;
        }

        private static GameObject CreateUIImage(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var img = go.AddComponent(typeof(Image)) as Image;
            img.color = color;
            return go;
        }

        private static GameObject CreateUIText(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, float fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            // Stretch to fill anchor area if anchors span a range
            if (anchorMin != anchorMax)
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            return go;
        }
    }
}
