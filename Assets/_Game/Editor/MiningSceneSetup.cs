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
            // --- Clear current scene objects ---
            var scene = EditorSceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                Object.DestroyImmediate(root);
            }
            // No Camera needed — this is a sub-scene with ScreenSpaceOverlay canvas.
            // No EventSystem needed — the Gameplay scene already provides one.

            // --- Canvas ---
            var canvasGO = new GameObject("Mining_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Content container (no SafeAreaHandler — it overrides anchors in sub-scenes)
            var safeAreaGO = new GameObject("ContentContainer", typeof(RectTransform));
            safeAreaGO.transform.SetParent(canvasGO.transform, false);
            var safeAreaRT = safeAreaGO.GetComponent<RectTransform>();
            safeAreaRT.anchorMin = new Vector2(0, 0.18f); // clears HP orb on 4:3 and taller
            safeAreaRT.anchorMax = Vector2.one;
            safeAreaRT.offsetMin = Vector2.zero;
            safeAreaRT.offsetMax = new Vector2(0, -120); // safe area: clears dynamic island / notch
            var contentBgImg = safeAreaGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(contentBgImg, new Color(0.85f, 0.85f, 0.9f, 0.92f));
            var safeAreaContent = UIAtlasHelper.CreatePanelContent(safeAreaGO.transform);

            // ============================================================
            // HEADER
            // ============================================================

            var titleGO = CreateUIText(safeAreaContent, "TitleText", "",
                new Vector2(0, 1), new Vector2(1, 1),
                Vector2.zero, new Vector2(0, 0), 1);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();

            // Close button (X) — top-right edge of panel
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(safeAreaGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(1, 1);
            backBtnRT.anchorMax = new Vector2(1, 1);
            backBtnRT.pivot = new Vector2(1, 1);
            backBtnRT.anchoredPosition = Vector2.zero;
            backBtnRT.sizeDelta = new Vector2(50, 50);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;
            UIAtlasHelper.SetXButton(backBtn, backBtnImg);

            // Instruction text
            var instructionGO = CreateUIText(safeAreaContent, "InstructionText",
                "Select which mine to teleport to",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -92), new Vector2(0, 30), 20);
            var instructionRT = instructionGO.GetComponent<RectTransform>();
            instructionRT.anchorMin = new Vector2(0, 1);
            instructionRT.anchorMax = new Vector2(1, 1);
            instructionRT.pivot = new Vector2(0.5f, 1);
            instructionRT.anchoredPosition = new Vector2(0, -92);
            instructionRT.sizeDelta = new Vector2(0, 30);
            var instructionTMP = instructionGO.GetComponent<TextMeshProUGUI>();
            instructionTMP.alignment = TextAlignmentOptions.Center;
            instructionTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // ============================================================
            // MINE LIST (compact cards, non-scrollable)
            // ============================================================

            var gridPanelGO = new GameObject("MineGridPanel", typeof(RectTransform));
            gridPanelGO.transform.SetParent(safeAreaContent, false);
            var gridPanelRT = gridPanelGO.GetComponent<RectTransform>();
            gridPanelRT.anchorMin = new Vector2(0, 0.15f);
            gridPanelRT.anchorMax = new Vector2(1, 0.87f);
            gridPanelRT.offsetMin = new Vector2(15, 0);
            gridPanelRT.offsetMax = new Vector2(-15, 0);

            // Content fills the panel directly — no ScrollRect
            var contentGO = new GameObject("Content", typeof(RectTransform));
            contentGO.transform.SetParent(gridPanelGO.transform, false);
            var contentRT = contentGO.GetComponent<RectTransform>();
            contentRT.anchorMin = Vector2.zero;
            contentRT.anchorMax = Vector2.one;
            contentRT.offsetMin = Vector2.zero;
            contentRT.offsetMax = Vector2.zero;

            var gridLayout = contentGO.AddComponent<GridLayoutGroup>();
            gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 1;
            gridLayout.cellSize = new Vector2(1020, 110);
            gridLayout.spacing = new Vector2(0, 10);
            gridLayout.padding = new RectOffset(5, 5, 5, 5);
            gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            gridLayout.childAlignment = TextAnchor.UpperCenter;

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
            activePanelGO.transform.SetParent(safeAreaContent, false);
            var activePanelRT = activePanelGO.GetComponent<RectTransform>();
            activePanelRT.anchorMin = new Vector2(0, 0);
            activePanelRT.anchorMax = new Vector2(1, 0.15f);
            activePanelRT.offsetMin = Vector2.zero;
            activePanelRT.offsetMax = Vector2.zero;
            var activePanelImg = activePanelGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(activePanelImg);
            var activePanelContent = UIAtlasHelper.CreatePanelContent(activePanelGO.transform);

            // Active mine name
            var activeMineNameGO = CreateUIText(activePanelContent, "ActiveMineName", "Mine Name",
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
            var progressBgGO = CreateUIImage(activePanelContent, "ProgressBG",
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
            collectBtnGO.transform.SetParent(activePanelContent, false);
            var collectBtnRT = collectBtnGO.GetComponent<RectTransform>();
            collectBtnRT.anchorMin = new Vector2(0.72f, 0.2f);
            collectBtnRT.anchorMax = new Vector2(0.95f, 0.8f);
            collectBtnRT.offsetMin = Vector2.zero;
            collectBtnRT.offsetMax = Vector2.zero;
            var collectBtnImg = collectBtnGO.AddComponent<Image>();
            var collectBtn = collectBtnGO.AddComponent<Button>();
            collectBtn.targetGraphic = collectBtnImg;
            collectBtn.interactable = false;
            UIAtlasHelper.SetSpriteSwapButton(collectBtn, collectBtnImg, "Button_Unpressed", "Button_Pressed");
            var collectBtnContent = UIAtlasHelper.CreateButtonContent(collectBtnGO.transform, 60f);

            var collectBtnTextGO = CreateUIText(collectBtnContent, "CollectText", "Mining...",
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
            yieldPanelGO.transform.SetParent(safeAreaContent, false);
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
            UIAtlasHelper.SetSlicedPanel(yieldInnerImg);
            var yieldInnerContent = UIAtlasHelper.CreatePanelContent(yieldInnerGO.transform);

            // Yield title
            var yieldTitleGO = CreateUIText(yieldInnerContent, "YieldTitle", "Mining Results",
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
            var yieldGoldGO = CreateUIText(yieldInnerContent, "YieldGoldText", "Gold: +0",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -80), new Vector2(-40, 40), 26);
            var yieldGoldRT = yieldGoldGO.GetComponent<RectTransform>();
            yieldGoldRT.anchorMin = new Vector2(0, 1);
            yieldGoldRT.anchorMax = new Vector2(1, 1);
            yieldGoldRT.pivot = new Vector2(0.5f, 1);
            yieldGoldRT.anchoredPosition = new Vector2(0, -80);
            yieldGoldRT.sizeDelta = new Vector2(-40, 40);

            // Gems text
            var yieldGemsGO = CreateUIText(yieldInnerContent, "YieldGemsText", "Gems:\n  ...",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -130), new Vector2(-40, 120), 22);
            var yieldGemsRT = yieldGemsGO.GetComponent<RectTransform>();
            yieldGemsRT.anchorMin = new Vector2(0, 1);
            yieldGemsRT.anchorMax = new Vector2(1, 1);
            yieldGemsRT.pivot = new Vector2(0.5f, 1);
            yieldGemsRT.anchoredPosition = new Vector2(0, -130);
            yieldGemsRT.sizeDelta = new Vector2(-40, 120);

            // Ores text
            var yieldOresGO = CreateUIText(yieldInnerContent, "YieldOresText", "Ores:\n  ...",
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
            yieldCloseBtnGO.transform.SetParent(yieldInnerContent, false);
            var yieldCloseBtnRT = yieldCloseBtnGO.GetComponent<RectTransform>();
            yieldCloseBtnRT.anchorMin = new Vector2(0.2f, 0);
            yieldCloseBtnRT.anchorMax = new Vector2(0.8f, 0);
            yieldCloseBtnRT.pivot = new Vector2(0.5f, 0);
            yieldCloseBtnRT.anchoredPosition = new Vector2(0, 20);
            yieldCloseBtnRT.sizeDelta = new Vector2(0, 60);
            var yieldCloseBtnImg = yieldCloseBtnGO.AddComponent<Image>();
            var yieldCloseBtn = yieldCloseBtnGO.AddComponent<Button>();
            yieldCloseBtn.targetGraphic = yieldCloseBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(yieldCloseBtn, yieldCloseBtnImg, "Button_Unpressed", "Button_Pressed");
            var yieldCloseBtnContent = UIAtlasHelper.CreateButtonContent(yieldCloseBtnGO.transform, 60f);

            var yieldCloseBtnTextGO = CreateUIText(yieldCloseBtnContent, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var yieldCloseBtnTMP = yieldCloseBtnTextGO.GetComponent<TextMeshProUGUI>();
            yieldCloseBtnTMP.alignment = TextAlignmentOptions.Center;

            yieldPanelGO.SetActive(false);

            // ============================================================
            // NOTIFICATION TEXT (centered, red, hidden by default)
            // ============================================================

            var notifGO = CreateUIText(safeAreaContent, "NotificationText", "Inventory is full",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -92), new Vector2(-40, 50), 30);
            var notifRT = notifGO.GetComponent<RectTransform>();
            notifRT.anchorMin = new Vector2(0, 1);
            notifRT.anchorMax = new Vector2(1, 1);
            notifRT.pivot = new Vector2(0.5f, 1);
            notifRT.anchoredPosition = new Vector2(0, -92);
            notifRT.sizeDelta = new Vector2(-40, 50);
            var notifTMP = notifGO.GetComponent<TextMeshProUGUI>();
            notifTMP.alignment = TextAlignmentOptions.Center;
            notifTMP.color = new Color(1f, 0.3f, 0.3f, 1f);
            notifTMP.fontStyle = FontStyles.Bold;
            notifGO.SetActive(false);

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

            // Notification
            uiSO.FindProperty("_notificationText").objectReferenceValue = notifTMP;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            // Wire MiningController (handles start mining, collect, back navigation)
            var controllerGO = new GameObject("MiningController");
            var controller = controllerGO.AddComponent<MiningController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_miningUI").objectReferenceValue = miningSceneUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            // --- Finalize ---
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Mining scene setup complete! Hit Play to test.");
        }

        // ============================================================
        // MINE CARD CREATOR
        // ============================================================

        private static GameObject CreateMineCard(Transform parent, MineData mine, int index)
        {
            // Compact card — the whole card is the button
            var cardGO = new GameObject($"MineCard_{mine.ID}", typeof(RectTransform));
            cardGO.transform.SetParent(parent, false);

            var bgImg = cardGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(bgImg);
            var cardContent = UIAtlasHelper.CreatePanelContent(cardGO.transform);
            var startBtn = cardGO.AddComponent<Button>();
            startBtn.targetGraphic = bgImg;

            // Mine name (bold, top)
            var nameGO = CreateStretchText(cardContent, "NameText", mine.Name,
                15, -15, -6, 30, 22);
            var nameTMP = nameGO.GetComponent<TextMeshProUGUI>();
            nameTMP.fontStyle = FontStyles.Bold;
            nameTMP.alignment = TextAlignmentOptions.Center;

            // Level + Duration on one line
            int hours = mine.DurationSeconds / 3600;
            int mins = (mine.DurationSeconds % 3600) / 60;
            string durationStr = hours > 0 ? $"{hours}h {mins}m" : $"{mins}m";

            var levelGO = CreateStretchText(cardContent, "LevelText", $"Lv.{mine.RequiredLevel}",
                15, -15, -38, 22, 16);
            var levelTMP = levelGO.GetComponent<TextMeshProUGUI>();
            levelTMP.alignment = TextAlignmentOptions.Center;

            var durationGO = CreateStretchText(cardContent, "DurationText", durationStr,
                15, -15, -58, 22, 16);
            var durationTMP = durationGO.GetComponent<TextMeshProUGUI>();
            durationTMP.alignment = TextAlignmentOptions.Center;
            durationTMP.color = new Color(0.8f, 0.8f, 0.6f, 1f);

            // Gold range
            var goldGO = CreateStretchText(cardContent, "GoldRange", $"{mine.MinGold}-{mine.MaxGold} Gold",
                15, -15, -80, 22, 16);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
            goldTMP.alignment = TextAlignmentOptions.Center;

            // Hidden status text (MineCardUI uses this for "Locked"/"Mining..." display)
            var statusGO = new GameObject("StatusText", typeof(RectTransform));
            statusGO.transform.SetParent(cardContent, false);
            var statusRT = statusGO.GetComponent<RectTransform>();
            statusRT.anchorMin = Vector2.zero;
            statusRT.anchorMax = Vector2.one;
            statusRT.offsetMin = Vector2.zero;
            statusRT.offsetMax = Vector2.zero;
            var statusTMP = statusGO.AddComponent<TextMeshProUGUI>();
            statusTMP.text = "";
            statusTMP.fontSize = 0;
            statusTMP.color = Color.clear;
            statusTMP.raycastTarget = false;

            // Wire MineCardUI — gems/ores/description left null (handled by null checks)
            var mineCard = cardGO.AddComponent<MineCardUI>();
            var cardSO = new SerializedObject(mineCard);
            cardSO.FindProperty("_nameText").objectReferenceValue = nameTMP;
            cardSO.FindProperty("_levelText").objectReferenceValue = levelTMP;
            cardSO.FindProperty("_durationText").objectReferenceValue = durationTMP;
            cardSO.FindProperty("_startButton").objectReferenceValue = startBtn;
            cardSO.FindProperty("_startButtonText").objectReferenceValue = statusTMP;
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

        /// <summary>
        /// Creates a text element that stretches horizontally across its parent,
        /// anchored to the top edge. yPos should be negative (e.g. -10 = 10px below top).
        /// </summary>
        private static GameObject CreateStretchText(Transform parent, string name, string text,
            float leftPad, float rightPad, float yPos, float height, float fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0, 1);
            rt.offsetMin = new Vector2(leftPad, yPos - height);
            rt.offsetMax = new Vector2(rightPad, yPos);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.margin = new Vector4(40, 0, 0, 0); // left margin to prevent RectMask2D clipping + visual padding
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
