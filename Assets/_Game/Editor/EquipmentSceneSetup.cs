using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Equipment;

namespace ConquerChronicles.Editor
{
    public static class EquipmentSceneSetup
    {
        // Slot size shared by equipped + bag
        private const float SlotSize = 150f;

        [MenuItem("Conquer Chronicles/Setup Equipment Scene")]
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
            camera.backgroundColor = new Color(0.07f, 0.07f, 0.11f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;

            // --- EventSystem ---
            var existingES = GameObject.FindFirstObjectByType<EventSystem>();
            if (existingES == null)
            {
                var esGO = new GameObject("EventSystem");
                esGO.AddComponent<EventSystem>();
                var inputModuleType = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputModuleType != null)
                    esGO.AddComponent(inputModuleType);
                else
                    esGO.AddComponent<StandaloneInputModule>();
            }

            // --- Canvas (1080x1920 portrait phone) ---
            var canvasGO = new GameObject("Equipment_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // SafeArea
            var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
            safeAreaGO.transform.SetParent(canvasGO.transform, false);
            var safeAreaRT = safeAreaGO.GetComponent<RectTransform>();
            safeAreaRT.anchorMin = Vector2.zero;
            safeAreaRT.anchorMax = Vector2.one;
            safeAreaRT.offsetMin = Vector2.zero;
            safeAreaRT.offsetMax = Vector2.zero;
            safeAreaGO.AddComponent<ConquerChronicles.Gameplay.UI.SafeAreaHandler>();

            // ============================================================
            // HEADER (top ~4%)
            // ============================================================

            var titleGO = CreateUIText(safeAreaGO.transform, "TitleText", "EQUIPMENT",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -10), new Vector2(0, 60), 40);
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 1);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -10);
            titleRT.sizeDelta = new Vector2(0, 60);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Back button
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(safeAreaGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(0, 1);
            backBtnRT.anchorMax = new Vector2(0, 1);
            backBtnRT.pivot = new Vector2(0, 1);
            backBtnRT.anchoredPosition = new Vector2(15, -15);
            backBtnRT.sizeDelta = new Vector2(140, 50);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            backBtnImg.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;
            var backBtnTextGO = CreateUIText(backBtnGO.transform, "BackText", "< Back",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            backBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            // Gold
            var goldGO = CreateUIText(safeAreaGO.transform, "GoldText", "0 Gold",
                new Vector2(1, 1), new Vector2(1, 1),
                new Vector2(-15, -15), new Vector2(180, 50), 24);
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.anchorMin = new Vector2(1, 1);
            goldRT.anchorMax = new Vector2(1, 1);
            goldRT.pivot = new Vector2(1, 1);
            goldRT.anchoredPosition = new Vector2(-15, -15);
            goldRT.sizeDelta = new Vector2(180, 50);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Right;
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Stats button (in header, next to gold)
            var statsBtnGO = new GameObject("StatsButton", typeof(RectTransform));
            statsBtnGO.transform.SetParent(safeAreaGO.transform, false);
            var statsBtnRT = statsBtnGO.GetComponent<RectTransform>();
            statsBtnRT.anchorMin = new Vector2(1, 1);
            statsBtnRT.anchorMax = new Vector2(1, 1);
            statsBtnRT.pivot = new Vector2(1, 1);
            statsBtnRT.anchoredPosition = new Vector2(-200, -15);
            statsBtnRT.sizeDelta = new Vector2(120, 50);
            var statsBtnImg = statsBtnGO.AddComponent<Image>();
            statsBtnImg.color = new Color(0.2f, 0.3f, 0.5f, 0.9f);
            var statsBtn = statsBtnGO.AddComponent<Button>();
            statsBtn.targetGraphic = statsBtnImg;
            var statsBtnTextGO = CreateUIText(statsBtnGO.transform, "StatsBtnText", "Stats",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            statsBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            // ============================================================
            // STATS OVERLAY PANEL (fullscreen overlay, hidden by default)
            // ============================================================

            var statsOverlayGO = new GameObject("StatsPanel", typeof(RectTransform));
            statsOverlayGO.transform.SetParent(safeAreaGO.transform, false);
            var statsOverlayRT = statsOverlayGO.GetComponent<RectTransform>();
            statsOverlayRT.anchorMin = Vector2.zero;
            statsOverlayRT.anchorMax = Vector2.one;
            statsOverlayRT.offsetMin = Vector2.zero;
            statsOverlayRT.offsetMax = Vector2.zero;
            statsOverlayGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0f);

            var statsInnerGO = new GameObject("StatsInner", typeof(RectTransform));
            statsInnerGO.transform.SetParent(statsOverlayGO.transform, false);
            var statsInnerRT = statsInnerGO.GetComponent<RectTransform>();
            statsInnerRT.anchorMin = new Vector2(0.1f, 0.3f);
            statsInnerRT.anchorMax = new Vector2(0.9f, 0.7f);
            statsInnerRT.offsetMin = Vector2.zero;
            statsInnerRT.offsetMax = Vector2.zero;
            statsInnerGO.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.14f, 1f);

            var statsTitleGO = CreateUIText(statsInnerGO.transform, "StatsTitle", "STATS",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -15), new Vector2(0, 40), 30);
            var statsTitleRT = statsTitleGO.GetComponent<RectTransform>();
            statsTitleRT.anchorMin = new Vector2(0, 1);
            statsTitleRT.anchorMax = new Vector2(1, 1);
            statsTitleRT.pivot = new Vector2(0.5f, 1);
            statsTitleRT.anchoredPosition = new Vector2(0, -15);
            statsTitleRT.sizeDelta = new Vector2(0, 40);
            var statsTitleTMP = statsTitleGO.GetComponent<TextMeshProUGUI>();
            statsTitleTMP.alignment = TextAlignmentOptions.Center;
            statsTitleTMP.fontStyle = FontStyles.Bold;
            statsTitleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            var statsTextGO = CreateUIText(statsInnerGO.transform, "StatsText",
                "HP: 0\nMP: 0\nATK: 0\nDEF: 0\nMATK: 0\nMDEF: 0\nAGI: 0\nCrit%: 0.0%",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 30);
            var statsTextRT = statsTextGO.GetComponent<RectTransform>();
            statsTextRT.anchorMin = new Vector2(0, 0);
            statsTextRT.anchorMax = new Vector2(1, 1);
            statsTextRT.offsetMin = new Vector2(20, 70);
            statsTextRT.offsetMax = new Vector2(-20, -60);
            var statsTextTMP = statsTextGO.GetComponent<TextMeshProUGUI>();
            statsTextTMP.alignment = TextAlignmentOptions.Center;

            var closeStatsBtnGO = CreateButton(statsInnerGO.transform, "CloseStatsButton",
                new Vector2(0, 15), new Vector2(0, 50), new Color(0.35f, 0.35f, 0.4f, 1f));
            closeStatsBtnGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0);
            closeStatsBtnGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0);
            var closeStatsBtn = closeStatsBtnGO.GetComponent<Button>();
            var closeStatsBtnTextGO = CreateUIText(closeStatsBtnGO.transform, "CloseText", "Cancel",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            closeStatsBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            // ============================================================
            // EQUIPPED PANEL — circle layout (0.35 to 0.96 — 61%)
            // ============================================================

            var equippedPanelGO = CreateUIImage(safeAreaGO.transform, "EquippedPanel",
                new Vector2(0, 0.35f), new Vector2(1, 0.96f),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var equippedPanelRT = equippedPanelGO.GetComponent<RectTransform>();
            equippedPanelRT.offsetMin = new Vector2(15, 0);
            equippedPanelRT.offsetMax = new Vector2(-15, -80);

            // Circle positions (offsets from center of panel)
            // Arranged for a character silhouette in the center
            var slotPositions = new Vector2[]
            {
                new Vector2(   0, +230),   // 0: Head — top center
                new Vector2(-190, +100),   // 1: Neck — upper-left
                new Vector2(-130, -210),   // 2: Armor — lower-left
                new Vector2(-240,  -60),   // 3: Weapon — mid-left
                new Vector2(+240,  -60),   // 4: Shield — mid-right
                new Vector2(+190, +100),   // 5: Ring — upper-right
                new Vector2(+130, -210),   // 6: Boots — lower-right
            };

            string[] slotLabels = { "Head", "Neck", "Armor", "Wpn", "Shield", "Ring", "Boots" };
            var slotButtons = new Button[7];
            var slotTexts = new TextMeshProUGUI[7];

            for (int i = 0; i < 7; i++)
            {
                var slotBtnGO = new GameObject($"SlotButton_{i}", typeof(RectTransform));
                slotBtnGO.transform.SetParent(equippedPanelGO.transform, false);
                var slotBtnRT = slotBtnGO.GetComponent<RectTransform>();
                // Anchor at center of parent
                slotBtnRT.anchorMin = new Vector2(0.5f, 0.5f);
                slotBtnRT.anchorMax = new Vector2(0.5f, 0.5f);
                slotBtnRT.pivot = new Vector2(0.5f, 0.5f);
                slotBtnRT.anchoredPosition = slotPositions[i];
                slotBtnRT.sizeDelta = new Vector2(SlotSize, SlotSize);

                var slotBtnImg = slotBtnGO.AddComponent<Image>();
                slotBtnImg.color = new Color(0.15f, 0.15f, 0.22f, 0.9f);
                var slotBtn = slotBtnGO.AddComponent<Button>();
                slotBtn.targetGraphic = slotBtnImg;
                slotButtons[i] = slotBtn;

                var slotTextGO = new GameObject("SlotText", typeof(RectTransform));
                slotTextGO.transform.SetParent(slotBtnGO.transform, false);
                var slotTextRT = slotTextGO.GetComponent<RectTransform>();
                slotTextRT.anchorMin = Vector2.zero;
                slotTextRT.anchorMax = Vector2.one;
                slotTextRT.offsetMin = new Vector2(3, 3);
                slotTextRT.offsetMax = new Vector2(-3, -3);
                var slotTextTMP = slotTextGO.AddComponent<TextMeshProUGUI>();
                slotTextTMP.text = slotLabels[i];
                slotTextTMP.fontSize = 20;
                slotTextTMP.color = Color.white;
                slotTextTMP.alignment = TextAlignmentOptions.Center;
                slotTexts[i] = slotTextTMP;
            }

            // ============================================================
            // BAG GRID (0.0 to 0.30 — 30%, scrollable, centered)
            // ============================================================

            var bagPanelGO = CreateUIImage(safeAreaGO.transform, "BagPanel",
                new Vector2(0, 0), new Vector2(1, 0.35f),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var bagPanelRT = bagPanelGO.GetComponent<RectTransform>();
            bagPanelRT.offsetMin = new Vector2(15, 10);
            bagPanelRT.offsetMax = new Vector2(-15, 0);

            var bagCountGO = CreateUIText(bagPanelGO.transform, "BagCountText", "Bag: 0/50",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -2), new Vector2(0, 28), 20);
            var bagCountRT = bagCountGO.GetComponent<RectTransform>();
            bagCountRT.anchorMin = new Vector2(0, 1);
            bagCountRT.anchorMax = new Vector2(1, 1);
            bagCountRT.pivot = new Vector2(0.5f, 1);
            bagCountRT.anchoredPosition = new Vector2(0, -2);
            bagCountRT.sizeDelta = new Vector2(0, 28);
            var bagCountTMP = bagCountGO.GetComponent<TextMeshProUGUI>();
            bagCountTMP.alignment = TextAlignmentOptions.Center;
            bagCountTMP.fontStyle = FontStyles.Bold;

            // Scrollable bag grid
            var bagScrollGO = new GameObject("BagScroll", typeof(RectTransform));
            bagScrollGO.transform.SetParent(bagPanelGO.transform, false);
            var bagScrollRT = bagScrollGO.GetComponent<RectTransform>();
            bagScrollRT.anchorMin = new Vector2(0, 0);
            bagScrollRT.anchorMax = new Vector2(1, 1);
            bagScrollRT.offsetMin = new Vector2(0, 5);
            bagScrollRT.offsetMax = new Vector2(0, -32);
            var bagScrollRect = bagScrollGO.AddComponent<ScrollRect>();
            bagScrollRect.horizontal = false;
            bagScrollRect.vertical = true;
            bagScrollGO.AddComponent<RectMask2D>();

            var bagContentGO = new GameObject("BagContent", typeof(RectTransform));
            bagContentGO.transform.SetParent(bagScrollGO.transform, false);
            var bagContentRT = bagContentGO.GetComponent<RectTransform>();
            bagContentRT.anchorMin = new Vector2(0, 1);
            bagContentRT.anchorMax = new Vector2(1, 1);
            bagContentRT.pivot = new Vector2(0.5f, 1);
            bagContentRT.anchoredPosition = Vector2.zero;
            bagContentGO.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var bagGridLayout = bagContentGO.AddComponent<GridLayoutGroup>();
            bagGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            bagGridLayout.constraintCount = 5;
            bagGridLayout.cellSize = new Vector2(SlotSize, SlotSize);
            bagGridLayout.spacing = new Vector2(8, 8);
            bagGridLayout.padding = new RectOffset(10, 10, 5, 5);
            bagGridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            bagGridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            bagGridLayout.childAlignment = TextAnchor.UpperCenter;

            bagScrollRect.content = bagContentRT;

            // ============================================================
            // ITEM DETAIL PANEL (overlay, hidden by default)
            // ============================================================

            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(safeAreaGO.transform, false);
            var detailPanelRT = detailPanelGO.GetComponent<RectTransform>();
            detailPanelRT.anchorMin = Vector2.zero;
            detailPanelRT.anchorMax = Vector2.one;
            detailPanelRT.offsetMin = Vector2.zero;
            detailPanelRT.offsetMax = Vector2.zero;
            detailPanelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

            var detailInnerGO = new GameObject("DetailInner", typeof(RectTransform));
            detailInnerGO.transform.SetParent(detailPanelGO.transform, false);
            var detailInnerRT = detailInnerGO.GetComponent<RectTransform>();
            detailInnerRT.anchorMin = new Vector2(0.08f, 0.25f);
            detailInnerRT.anchorMax = new Vector2(0.92f, 0.75f);
            detailInnerRT.offsetMin = Vector2.zero;
            detailInnerRT.offsetMax = Vector2.zero;
            detailInnerGO.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.14f, 0.95f);

            // Item name
            var itemNameGO = CreateUIText(detailInnerGO.transform, "ItemNameText", "Item Name",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -15), new Vector2(0, 50), 34);
            var itemNameRT = itemNameGO.GetComponent<RectTransform>();
            itemNameRT.anchorMin = new Vector2(0, 1);
            itemNameRT.anchorMax = new Vector2(1, 1);
            itemNameRT.pivot = new Vector2(0.5f, 1);
            itemNameRT.anchoredPosition = new Vector2(0, -15);
            itemNameRT.sizeDelta = new Vector2(-20, 50);
            var itemNameTMP = itemNameGO.GetComponent<TextMeshProUGUI>();
            itemNameTMP.alignment = TextAlignmentOptions.Center;
            itemNameTMP.fontStyle = FontStyles.Bold;
            itemNameTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Item stats
            var itemStatsGO = CreateUIText(detailInnerGO.transform, "ItemStatsText", "Stats...",
                new Vector2(0, 1), new Vector2(0.5f, 1),
                new Vector2(20, -75), new Vector2(0, 200), 22);
            var itemStatsRT = itemStatsGO.GetComponent<RectTransform>();
            itemStatsRT.anchorMin = new Vector2(0, 1);
            itemStatsRT.anchorMax = new Vector2(0.5f, 1);
            itemStatsRT.pivot = new Vector2(0, 1);
            itemStatsRT.anchoredPosition = new Vector2(20, -75);
            itemStatsRT.sizeDelta = new Vector2(0, 200);
            itemStatsGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Item info
            var itemInfoGO = CreateUIText(detailInnerGO.transform, "ItemInfoText", "Quality: Normal\nReq Lv: 1\nSockets: 0/0",
                new Vector2(0.5f, 1), new Vector2(1, 1),
                new Vector2(10, -75), new Vector2(-20, 200), 22);
            var itemInfoRT = itemInfoGO.GetComponent<RectTransform>();
            itemInfoRT.anchorMin = new Vector2(0.5f, 1);
            itemInfoRT.anchorMax = new Vector2(1, 1);
            itemInfoRT.pivot = new Vector2(0, 1);
            itemInfoRT.anchoredPosition = new Vector2(10, -75);
            itemInfoRT.sizeDelta = new Vector2(-20, 200);
            itemInfoGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Equip button
            var equipBtnGO = CreateButton(detailInnerGO.transform, "EquipButton",
                new Vector2(0, 170), new Vector2(0, 60), new Color(0.2f, 0.4f, 0.75f, 1f));
            var equipBtn = equipBtnGO.GetComponent<Button>();
            var equipBtnTextGO = CreateUIText(equipBtnGO.transform, "EquipText", "Equip",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var equipBtnTMP = equipBtnTextGO.GetComponent<TextMeshProUGUI>();
            equipBtnTMP.alignment = TextAlignmentOptions.Center;
            equipBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade button
            var upgradeBtnGO = CreateButton(detailInnerGO.transform, "UpgradeButton",
                new Vector2(0, 100), new Vector2(0, 60), new Color(0.85f, 0.55f, 0.1f, 1f));
            var upgradeBtn = upgradeBtnGO.GetComponent<Button>();
            var upgradeBtnTextGO = CreateUIText(upgradeBtnGO.transform, "UpgradeText", "Upgrade",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var upgradeBtnTMP = upgradeBtnTextGO.GetComponent<TextMeshProUGUI>();
            upgradeBtnTMP.alignment = TextAlignmentOptions.Center;
            upgradeBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade rate text
            var upgradeRateGO = CreateUIText(detailInnerGO.transform, "UpgradeRateText", "Success: 100%",
                new Vector2(0.1f, 0), new Vector2(0.9f, 0),
                new Vector2(0, 75), new Vector2(0, 25), 20);
            var upgradeRateRT = upgradeRateGO.GetComponent<RectTransform>();
            upgradeRateRT.anchorMin = new Vector2(0.1f, 0);
            upgradeRateRT.anchorMax = new Vector2(0.9f, 0);
            upgradeRateRT.pivot = new Vector2(0.5f, 0);
            upgradeRateRT.anchoredPosition = new Vector2(0, 75);
            upgradeRateRT.sizeDelta = new Vector2(0, 25);
            var upgradeRateTMP = upgradeRateGO.GetComponent<TextMeshProUGUI>();
            upgradeRateTMP.alignment = TextAlignmentOptions.Center;
            upgradeRateTMP.color = new Color(0.9f, 0.9f, 0.5f, 1f);

            // Close button
            var closeBtnGO = CreateButton(detailInnerGO.transform, "CloseDetailButton",
                new Vector2(0, 15), new Vector2(0, 50), new Color(0.35f, 0.35f, 0.4f, 1f));
            closeBtnGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0);
            closeBtnGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0);
            var closeBtn = closeBtnGO.GetComponent<Button>();
            var closeBtnTextGO = CreateUIText(closeBtnGO.transform, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            closeBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // Move stats overlay to render on top of everything
            statsOverlayGO.transform.SetAsLastSibling();
            statsOverlayGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            var equipmentSceneUI = canvasGO.AddComponent<EquipmentSceneUI>();
            var uiSO = new SerializedObject(equipmentSceneUI);

            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_goldText").objectReferenceValue = goldTMP;

            var slotButtonsProp = uiSO.FindProperty("_slotButtons");
            slotButtonsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotButtons[i];

            var slotTextsProp = uiSO.FindProperty("_slotTexts");
            slotTextsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotTextsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotTexts[i];

            uiSO.FindProperty("_statsButton").objectReferenceValue = statsBtn;
            uiSO.FindProperty("_statsPanel").objectReferenceValue = statsOverlayGO;
            uiSO.FindProperty("_statsText").objectReferenceValue = statsTextTMP;
            uiSO.FindProperty("_closeStatsButton").objectReferenceValue = closeStatsBtn;
            uiSO.FindProperty("_bagContainer").objectReferenceValue = bagContentGO.transform;
            uiSO.FindProperty("_bagCountText").objectReferenceValue = bagCountTMP;

            uiSO.FindProperty("_detailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_itemNameText").objectReferenceValue = itemNameTMP;
            uiSO.FindProperty("_itemStatsText").objectReferenceValue = itemStatsGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_itemInfoText").objectReferenceValue = itemInfoGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_equipButton").objectReferenceValue = equipBtn;
            uiSO.FindProperty("_equipButtonText").objectReferenceValue = equipBtnTMP;
            uiSO.FindProperty("_upgradeButton").objectReferenceValue = upgradeBtn;
            uiSO.FindProperty("_upgradeRateText").objectReferenceValue = upgradeRateTMP;
            uiSO.FindProperty("_closeDetailButton").objectReferenceValue = closeBtn;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            var controllerGO = new GameObject("EquipmentController");
            var controller = controllerGO.AddComponent<EquipmentController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_equipmentUI").objectReferenceValue = equipmentSceneUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Equipment scene setup complete!");
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
            go.AddComponent<Image>().color = color;
            return go;
        }

        private static GameObject CreateButton(Transform parent, string name,
            Vector2 anchoredPos, Vector2 sizeDelta, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0);
            rt.anchorMax = new Vector2(0.9f, 0);
            rt.pivot = new Vector2(0.5f, 0);
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var img = go.AddComponent<Image>();
            img.color = color;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;
            return go;
        }
    }
}
