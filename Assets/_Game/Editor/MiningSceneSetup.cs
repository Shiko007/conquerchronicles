using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Mining;
using ConquerChronicles.Core.Mining;

namespace ConquerChronicles.Editor
{
    public static class MiningSceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Mining Scene")]
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
            camera.backgroundColor = new Color(0.06f, 0.06f, 0.1f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;

            // --- Canvas ---
            var canvasGO = new GameObject("Mining_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // ============================================================
            // HEADER
            // ============================================================

            // Title text - centered at top
            var titleGO = CreateUIText(canvasGO.transform, "TitleText", "MINING",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -20), new Vector2(0, 70), 48);
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 1);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -20);
            titleRT.sizeDelta = new Vector2(0, 70);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f); // gold

            // Back button - top-left
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(canvasGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(0, 1);
            backBtnRT.anchorMax = new Vector2(0, 1);
            backBtnRT.pivot = new Vector2(0, 1);
            backBtnRT.anchoredPosition = new Vector2(20, -20);
            backBtnRT.sizeDelta = new Vector2(160, 60);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            backBtnImg.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;

            var backBtnTextGO = CreateUIText(backBtnGO.transform, "BackText", "< Back",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var backBtnTMP = backBtnTextGO.GetComponent<TextMeshProUGUI>();
            backBtnTMP.alignment = TextAlignmentOptions.Center;

            // ============================================================
            // MINE LIST (scrollable area)
            // ============================================================

            // Scroll View container
            var scrollGO = new GameObject("MineListScroll", typeof(RectTransform));
            scrollGO.transform.SetParent(canvasGO.transform, false);
            var scrollRT = scrollGO.GetComponent<RectTransform>();
            scrollRT.anchorMin = new Vector2(0, 0.15f);
            scrollRT.anchorMax = new Vector2(1, 0.93f);
            scrollRT.offsetMin = new Vector2(20, 0);
            scrollRT.offsetMax = new Vector2(-20, 0);
            var scrollRect = scrollGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            var scrollMask = scrollGO.AddComponent<RectMask2D>();

            // Content container for mine cards
            var contentGO = new GameObject("Content", typeof(RectTransform));
            contentGO.transform.SetParent(scrollGO.transform, false);
            var contentRT = contentGO.GetComponent<RectTransform>();
            contentRT.anchorMin = new Vector2(0, 1);
            contentRT.anchorMax = new Vector2(1, 1);
            contentRT.pivot = new Vector2(0.5f, 1);
            contentRT.anchoredPosition = Vector2.zero;
            var contentSizeFitter = contentGO.AddComponent<ContentSizeFitter>();
            contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            var vertLayout = contentGO.AddComponent<VerticalLayoutGroup>();
            vertLayout.spacing = 15f;
            vertLayout.padding = new RectOffset(10, 10, 10, 10);
            vertLayout.childForceExpandWidth = true;
            vertLayout.childForceExpandHeight = false;
            vertLayout.childControlWidth = true;
            vertLayout.childControlHeight = false;

            scrollRect.content = contentRT;

            // Create 5 mine cards
            var mines = TestMines.GetAll();
            var mineCards = new MineCardUI[mines.Length];

            for (int i = 0; i < mines.Length; i++)
            {
                var mine = mines[i];
                var cardGO = CreateMineCard(contentGO.transform, mine, i);
                mineCards[i] = cardGO.GetComponent<MineCardUI>();
            }

            // ============================================================
            // ACTIVE MINING PANEL (bottom overlay, hidden by default)
            // ============================================================

            var activePanelGO = new GameObject("ActiveMiningPanel", typeof(RectTransform));
            activePanelGO.transform.SetParent(canvasGO.transform, false);
            var activePanelRT = activePanelGO.GetComponent<RectTransform>();
            activePanelRT.anchorMin = new Vector2(0, 0);
            activePanelRT.anchorMax = new Vector2(1, 0.15f);
            activePanelRT.offsetMin = Vector2.zero;
            activePanelRT.offsetMax = Vector2.zero;
            var activePanelImg = activePanelGO.AddComponent<Image>();
            activePanelImg.color = new Color(0.08f, 0.08f, 0.14f, 0.95f);

            // Active mine name
            var activeMineNameGO = CreateUIText(activePanelGO.transform, "ActiveMineName", "Mine Name",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 40), 28);
            var activeMineNameRT = activeMineNameGO.GetComponent<RectTransform>();
            activeMineNameRT.anchorMin = new Vector2(0.05f, 0.7f);
            activeMineNameRT.anchorMax = new Vector2(0.95f, 1);
            activeMineNameRT.offsetMin = Vector2.zero;
            activeMineNameRT.offsetMax = Vector2.zero;
            activeMineNameRT.anchoredPosition = Vector2.zero;
            activeMineNameRT.sizeDelta = Vector2.zero;
            var activeMineNameTMP = activeMineNameGO.GetComponent<TextMeshProUGUI>();
            activeMineNameTMP.alignment = TextAlignmentOptions.Center;
            activeMineNameTMP.fontStyle = FontStyles.Bold;

            // Progress bar background
            var progressBgGO = CreateUIImage(activePanelGO.transform, "ProgressBG",
                new Vector2(0.05f, 0.35f), new Vector2(0.7f, 0.65f),
                Vector2.zero, Vector2.zero,
                new Color(0.15f, 0.15f, 0.2f, 1f));
            var progressBgRT = progressBgGO.GetComponent<RectTransform>();
            progressBgRT.offsetMin = Vector2.zero;
            progressBgRT.offsetMax = Vector2.zero;

            // Progress bar fill
            var progressFillGO = CreateUIImage(progressBgGO.transform, "ProgressFill",
                Vector2.zero, Vector2.one,
                Vector2.zero, Vector2.zero,
                new Color(0.2f, 0.7f, 0.3f, 1f));
            var progressFillRT = progressFillGO.GetComponent<RectTransform>();
            progressFillRT.offsetMin = new Vector2(2, 2);
            progressFillRT.offsetMax = new Vector2(-2, -2);
            var progressFillImg = progressFillGO.GetComponent<Image>();
            progressFillImg.type = Image.Type.Filled;
            progressFillImg.fillMethod = Image.FillMethod.Horizontal;

            // Timer text (overlaid on progress bar)
            var timerGO = CreateUIText(progressBgGO.transform, "TimerText", "00:00:00",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var timerTMP = timerGO.GetComponent<TextMeshProUGUI>();
            timerTMP.alignment = TextAlignmentOptions.Center;

            // Collect button
            var collectBtnGO = new GameObject("CollectButton", typeof(RectTransform));
            collectBtnGO.transform.SetParent(activePanelGO.transform, false);
            var collectBtnRT = collectBtnGO.GetComponent<RectTransform>();
            collectBtnRT.anchorMin = new Vector2(0.72f, 0.2f);
            collectBtnRT.anchorMax = new Vector2(0.95f, 0.8f);
            collectBtnRT.offsetMin = Vector2.zero;
            collectBtnRT.offsetMax = Vector2.zero;
            var collectBtnImg = collectBtnGO.AddComponent<Image>();
            collectBtnImg.color = new Color(0.85f, 0.7f, 0.1f, 1f); // gold
            var collectBtn = collectBtnGO.AddComponent<Button>();
            collectBtn.targetGraphic = collectBtnImg;
            collectBtn.interactable = false;

            var collectBtnTextGO = CreateUIText(collectBtnGO.transform, "CollectText", "Mining...",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var collectBtnTMP = collectBtnTextGO.GetComponent<TextMeshProUGUI>();
            collectBtnTMP.alignment = TextAlignmentOptions.Center;
            collectBtnTMP.color = Color.black;

            activePanelGO.SetActive(false);

            // ============================================================
            // YIELD RESULT PANEL (center overlay, hidden by default)
            // ============================================================

            // Backdrop (full-screen semi-transparent)
            var yieldPanelGO = new GameObject("YieldPanel", typeof(RectTransform));
            yieldPanelGO.transform.SetParent(canvasGO.transform, false);
            var yieldPanelRT = yieldPanelGO.GetComponent<RectTransform>();
            yieldPanelRT.anchorMin = Vector2.zero;
            yieldPanelRT.anchorMax = Vector2.one;
            yieldPanelRT.offsetMin = Vector2.zero;
            yieldPanelRT.offsetMax = Vector2.zero;
            var yieldBgImg = yieldPanelGO.AddComponent<Image>();
            yieldBgImg.color = new Color(0f, 0f, 0f, 0.7f);

            // Inner panel
            var yieldInnerGO = new GameObject("YieldInner", typeof(RectTransform));
            yieldInnerGO.transform.SetParent(yieldPanelGO.transform, false);
            var yieldInnerRT = yieldInnerGO.GetComponent<RectTransform>();
            yieldInnerRT.anchorMin = new Vector2(0.1f, 0.25f);
            yieldInnerRT.anchorMax = new Vector2(0.9f, 0.75f);
            yieldInnerRT.offsetMin = Vector2.zero;
            yieldInnerRT.offsetMax = Vector2.zero;
            var yieldInnerImg = yieldInnerGO.AddComponent<Image>();
            yieldInnerImg.color = new Color(0.08f, 0.08f, 0.14f, 0.95f);

            // Yield title
            var yieldTitleGO = CreateUIText(yieldInnerGO.transform, "YieldTitle", "Mining Results",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -10), new Vector2(0, 60), 36);
            var yieldTitleRT = yieldTitleGO.GetComponent<RectTransform>();
            yieldTitleRT.anchorMin = new Vector2(0, 1);
            yieldTitleRT.anchorMax = new Vector2(1, 1);
            yieldTitleRT.pivot = new Vector2(0.5f, 1);
            yieldTitleRT.anchoredPosition = new Vector2(0, -10);
            yieldTitleRT.sizeDelta = new Vector2(0, 60);
            var yieldTitleTMP = yieldTitleGO.GetComponent<TextMeshProUGUI>();
            yieldTitleTMP.alignment = TextAlignmentOptions.Center;
            yieldTitleTMP.fontStyle = FontStyles.Bold;
            yieldTitleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Gold text
            var yieldGoldGO = CreateUIText(yieldInnerGO.transform, "YieldGoldText", "Gold: +0",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -80), new Vector2(-40, 40), 26);
            var yieldGoldRT = yieldGoldGO.GetComponent<RectTransform>();
            yieldGoldRT.anchorMin = new Vector2(0, 1);
            yieldGoldRT.anchorMax = new Vector2(1, 1);
            yieldGoldRT.pivot = new Vector2(0.5f, 1);
            yieldGoldRT.anchoredPosition = new Vector2(0, -80);
            yieldGoldRT.sizeDelta = new Vector2(-40, 40);

            // Gems text
            var yieldGemsGO = CreateUIText(yieldInnerGO.transform, "YieldGemsText", "Gems:\n  ...",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -130), new Vector2(-40, 120), 22);
            var yieldGemsRT = yieldGemsGO.GetComponent<RectTransform>();
            yieldGemsRT.anchorMin = new Vector2(0, 1);
            yieldGemsRT.anchorMax = new Vector2(1, 1);
            yieldGemsRT.pivot = new Vector2(0.5f, 1);
            yieldGemsRT.anchoredPosition = new Vector2(0, -130);
            yieldGemsRT.sizeDelta = new Vector2(-40, 120);

            // Ores text
            var yieldOresGO = CreateUIText(yieldInnerGO.transform, "YieldOresText", "Ores:\n  ...",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -260), new Vector2(-40, 120), 22);
            var yieldOresRT = yieldOresGO.GetComponent<RectTransform>();
            yieldOresRT.anchorMin = new Vector2(0, 1);
            yieldOresRT.anchorMax = new Vector2(1, 1);
            yieldOresRT.pivot = new Vector2(0.5f, 1);
            yieldOresRT.anchoredPosition = new Vector2(0, -260);
            yieldOresRT.sizeDelta = new Vector2(-40, 120);

            // Close button
            var yieldCloseBtnGO = new GameObject("YieldCloseButton", typeof(RectTransform));
            yieldCloseBtnGO.transform.SetParent(yieldInnerGO.transform, false);
            var yieldCloseBtnRT = yieldCloseBtnGO.GetComponent<RectTransform>();
            yieldCloseBtnRT.anchorMin = new Vector2(0.2f, 0);
            yieldCloseBtnRT.anchorMax = new Vector2(0.8f, 0);
            yieldCloseBtnRT.pivot = new Vector2(0.5f, 0);
            yieldCloseBtnRT.anchoredPosition = new Vector2(0, 20);
            yieldCloseBtnRT.sizeDelta = new Vector2(0, 60);
            var yieldCloseBtnImg = yieldCloseBtnGO.AddComponent<Image>();
            yieldCloseBtnImg.color = new Color(0.2f, 0.5f, 0.2f, 1f);
            var yieldCloseBtn = yieldCloseBtnGO.AddComponent<Button>();
            yieldCloseBtn.targetGraphic = yieldCloseBtnImg;

            var yieldCloseBtnTextGO = CreateUIText(yieldCloseBtnGO.transform, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var yieldCloseBtnTMP = yieldCloseBtnTextGO.GetComponent<TextMeshProUGUI>();
            yieldCloseBtnTMP.alignment = TextAlignmentOptions.Center;

            yieldPanelGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            // MiningSceneUI on the canvas
            var miningSceneUI = canvasGO.AddComponent<MiningSceneUI>();
            var uiSO = new SerializedObject(miningSceneUI);

            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_mineListContainer").objectReferenceValue = contentGO.transform;

            // Wire mine cards array
            var mineCardsProperty = uiSO.FindProperty("_mineCards");
            mineCardsProperty.arraySize = mineCards.Length;
            for (int i = 0; i < mineCards.Length; i++)
            {
                mineCardsProperty.GetArrayElementAtIndex(i).objectReferenceValue = mineCards[i];
            }

            // Active mining panel
            uiSO.FindProperty("_activeMiningPanel").objectReferenceValue = activePanelGO;
            uiSO.FindProperty("_activeMineNameText").objectReferenceValue = activeMineNameGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_timerText").objectReferenceValue = timerTMP;
            uiSO.FindProperty("_progressFill").objectReferenceValue = progressFillImg;
            uiSO.FindProperty("_collectButton").objectReferenceValue = collectBtn;
            uiSO.FindProperty("_collectButtonText").objectReferenceValue = collectBtnTMP;

            // Yield panel
            uiSO.FindProperty("_yieldPanel").objectReferenceValue = yieldPanelGO;
            uiSO.FindProperty("_yieldGoldText").objectReferenceValue = yieldGoldGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_yieldGemsText").objectReferenceValue = yieldGemsGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_yieldOresText").objectReferenceValue = yieldOresGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_yieldCloseButton").objectReferenceValue = yieldCloseBtn;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            // --- Finalize ---
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Mining scene setup complete! Hit Play to test.");
        }

        // ============================================================
        // MINE CARD CREATOR
        // ============================================================

        private static GameObject CreateMineCard(Transform parent, MineData mine, int index)
        {
            // Card container
            var cardGO = new GameObject($"MineCard_{mine.ID}", typeof(RectTransform));
            cardGO.transform.SetParent(parent, false);
            var cardRT = cardGO.GetComponent<RectTransform>();
            cardRT.sizeDelta = new Vector2(0, 260);
            var layoutElement = cardGO.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = 260;
            layoutElement.flexibleWidth = 1;

            // Background
            var bgImg = cardGO.AddComponent<Image>();
            bgImg.color = new Color(0.12f, 0.12f, 0.18f, 0.9f);

            // Mine name (bold, size 32)
            var nameGO = CreateUIText(cardGO.transform, "NameText", mine.Name,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -10), new Vector2(600, 40), 32);
            var nameTMP = nameGO.GetComponent<TextMeshProUGUI>();
            nameTMP.fontStyle = FontStyles.Bold;

            // "Req. Lv. X" text (size 22)
            var levelGO = CreateUIText(cardGO.transform, "LevelText", $"Req. Lv. {mine.RequiredLevel}",
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -50), new Vector2(300, 28), 22);

            // Duration text (size 22)
            int hours = mine.DurationSeconds / 3600;
            int mins = (mine.DurationSeconds % 3600) / 60;
            string durationStr = hours > 0 ? $"{hours}h {mins}m" : $"{mins}m";
            var durationGO = CreateUIText(cardGO.transform, "DurationText", durationStr,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -78), new Vector2(300, 28), 22);

            // Gems text (size 20)
            string gemsStr = "Gems: ";
            if (mine.AvailableGems != null)
            {
                for (int i = 0; i < mine.AvailableGems.Length; i++)
                {
                    if (i > 0) gemsStr += ", ";
                    gemsStr += mine.AvailableGems[i].ToString();
                }
            }
            var gemsGO = CreateUIText(cardGO.transform, "GemsText", gemsStr,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -106), new Vector2(700, 26), 20);
            var gemsTMP = gemsGO.GetComponent<TextMeshProUGUI>();
            gemsTMP.color = new Color(0.6f, 0.9f, 1f, 1f);

            // Ores text (size 20)
            string oresStr = "Ores: ";
            if (mine.AvailableOres != null)
            {
                for (int i = 0; i < mine.AvailableOres.Length; i++)
                {
                    if (i > 0) oresStr += ", ";
                    oresStr += mine.AvailableOres[i].ToString();
                }
            }
            var oresGO = CreateUIText(cardGO.transform, "OresText", oresStr,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -132), new Vector2(700, 26), 20);
            var oresTMP = oresGO.GetComponent<TextMeshProUGUI>();
            oresTMP.color = new Color(1f, 0.8f, 0.5f, 1f);

            // Description text (size 18, gray)
            var descGO = CreateUIText(cardGO.transform, "DescriptionText", mine.Description,
                new Vector2(0, 1), new Vector2(0, 1),
                new Vector2(15, -160), new Vector2(700, 50), 18);
            var descTMP = descGO.GetComponent<TextMeshProUGUI>();
            descTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // "Start Mining" button (green, right side)
            var startBtnGO = new GameObject("StartButton", typeof(RectTransform));
            startBtnGO.transform.SetParent(cardGO.transform, false);
            var startBtnRT = startBtnGO.GetComponent<RectTransform>();
            startBtnRT.anchorMin = new Vector2(1, 0);
            startBtnRT.anchorMax = new Vector2(1, 0);
            startBtnRT.pivot = new Vector2(1, 0);
            startBtnRT.anchoredPosition = new Vector2(-15, 15);
            startBtnRT.sizeDelta = new Vector2(220, 55);
            var startBtnImg = startBtnGO.AddComponent<Image>();
            startBtnImg.color = new Color(0.15f, 0.55f, 0.15f, 1f); // green
            var startBtn = startBtnGO.AddComponent<Button>();
            startBtn.targetGraphic = startBtnImg;

            var startBtnTextGO = CreateUIText(startBtnGO.transform, "StartText", "Start Mining",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var startBtnTMP = startBtnTextGO.GetComponent<TextMeshProUGUI>();
            startBtnTMP.alignment = TextAlignmentOptions.Center;

            // Wire MineCardUI component
            var mineCard = cardGO.AddComponent<MineCardUI>();
            var cardSO = new SerializedObject(mineCard);
            cardSO.FindProperty("_nameText").objectReferenceValue = nameTMP;
            cardSO.FindProperty("_levelText").objectReferenceValue = levelGO.GetComponent<TextMeshProUGUI>();
            cardSO.FindProperty("_durationText").objectReferenceValue = durationGO.GetComponent<TextMeshProUGUI>();
            cardSO.FindProperty("_gemsText").objectReferenceValue = gemsTMP;
            cardSO.FindProperty("_oresText").objectReferenceValue = oresTMP;
            cardSO.FindProperty("_descriptionText").objectReferenceValue = descTMP;
            cardSO.FindProperty("_startButton").objectReferenceValue = startBtn;
            cardSO.FindProperty("_startButtonText").objectReferenceValue = startBtnTMP;
            cardSO.FindProperty("_background").objectReferenceValue = bgImg;
            cardSO.ApplyModifiedPropertiesWithoutUndo();

            return cardGO;
        }

        // ============================================================
        // UI HELPERS
        // ============================================================

        private static GameObject CreateUIText(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, float fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;

            // Stretch to fill parent if anchors span full range
            if (anchorMin == Vector2.zero && anchorMax == Vector2.one)
            {
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                tmp.alignment = TextAlignmentOptions.Center;
            }

            return go;
        }

        private static GameObject CreateUIImage(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }
    }
}
