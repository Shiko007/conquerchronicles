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

            // --- EventSystem (required for UI input) ---
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

            // --- Canvas ---
            var canvasGO = new GameObject("Equipment_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f;
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

            // ============================================================
            // HEADER
            // ============================================================

            // Title text - "EQUIPMENT" in gold, centered
            var titleGO = CreateUIText(safeAreaGO.transform, "TitleText", "EQUIPMENT",
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
            backBtnGO.transform.SetParent(safeAreaGO.transform, false);
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

            // Gold display - top-right
            var goldGO = CreateUIText(safeAreaGO.transform, "GoldText", "0 Gold",
                new Vector2(1, 1), new Vector2(1, 1),
                new Vector2(-20, -20), new Vector2(200, 60), 28);
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.anchorMin = new Vector2(1, 1);
            goldRT.anchorMax = new Vector2(1, 1);
            goldRT.pivot = new Vector2(1, 1);
            goldRT.anchoredPosition = new Vector2(-20, -20);
            goldRT.sizeDelta = new Vector2(200, 60);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Right;
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // ============================================================
            // LEFT PANEL - Equipped Slots (40% width, below header)
            // ============================================================

            var leftPanelGO = CreateUIImage(safeAreaGO.transform, "EquippedSlotsPanel",
                new Vector2(0, 0.35f), new Vector2(0.4f, 0.93f),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var leftPanelRT = leftPanelGO.GetComponent<RectTransform>();
            leftPanelRT.offsetMin = new Vector2(10, 0);
            leftPanelRT.offsetMax = new Vector2(0, 0);

            // Equipped Slots title
            var slotsTitleGO = CreateUIText(leftPanelGO.transform, "SlotsTitle", "EQUIPPED",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 40), 26);
            var slotsTitleRT = slotsTitleGO.GetComponent<RectTransform>();
            slotsTitleRT.anchorMin = new Vector2(0, 1);
            slotsTitleRT.anchorMax = new Vector2(1, 1);
            slotsTitleRT.pivot = new Vector2(0.5f, 1);
            slotsTitleRT.anchoredPosition = new Vector2(0, -5);
            slotsTitleRT.sizeDelta = new Vector2(0, 40);
            var slotsTitleTMP = slotsTitleGO.GetComponent<TextMeshProUGUI>();
            slotsTitleTMP.alignment = TextAlignmentOptions.Center;
            slotsTitleTMP.fontStyle = FontStyles.Bold;
            slotsTitleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Slot container with VerticalLayoutGroup
            var slotContainerGO = new GameObject("SlotContainer", typeof(RectTransform));
            slotContainerGO.transform.SetParent(leftPanelGO.transform, false);
            var slotContainerRT = slotContainerGO.GetComponent<RectTransform>();
            slotContainerRT.anchorMin = new Vector2(0.05f, 0);
            slotContainerRT.anchorMax = new Vector2(0.95f, 1);
            slotContainerRT.offsetMin = new Vector2(0, 10);
            slotContainerRT.offsetMax = new Vector2(0, -50); // below "EQUIPPED" title
            var slotVLG = slotContainerGO.AddComponent<VerticalLayoutGroup>();
            slotVLG.spacing = 5f;
            slotVLG.padding = new RectOffset(0, 0, 0, 0);
            slotVLG.childForceExpandWidth = true;
            slotVLG.childForceExpandHeight = false;
            slotVLG.childControlWidth = true;
            slotVLG.childControlHeight = false;

            // Create 7 slot buttons
            string[] slotLabels = { "Head:", "Neck:", "Armor:", "Weapon:", "Shield:", "Ring:", "Boots:" };
            var slotButtons = new Button[7];
            var slotTexts = new TextMeshProUGUI[7];

            for (int i = 0; i < 7; i++)
            {
                var slotBtnGO = new GameObject($"SlotButton_{i}", typeof(RectTransform));
                slotBtnGO.transform.SetParent(slotContainerGO.transform, false);
                var slotBtnRT = slotBtnGO.GetComponent<RectTransform>();
                slotBtnRT.sizeDelta = new Vector2(0, 70);
                var slotLayout = slotBtnGO.AddComponent<LayoutElement>();
                slotLayout.preferredHeight = 70;
                slotLayout.flexibleWidth = 1;
                var slotBtnImg = slotBtnGO.AddComponent<Image>();
                slotBtnImg.color = new Color(0.15f, 0.15f, 0.22f, 0.9f);
                var slotBtn = slotBtnGO.AddComponent<Button>();
                slotBtn.targetGraphic = slotBtnImg;
                slotButtons[i] = slotBtn;

                var slotTextGO = CreateUIText(slotBtnGO.transform, "SlotText", $"{slotLabels[i]} Empty",
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
                var slotTextRT = slotTextGO.GetComponent<RectTransform>();
                slotTextRT.offsetMin = new Vector2(10, 0);
                slotTextRT.offsetMax = new Vector2(-10, 0);
                var slotTextTMP = slotTextGO.GetComponent<TextMeshProUGUI>();
                slotTextTMP.alignment = TextAlignmentOptions.MidlineLeft;
                slotTexts[i] = slotTextTMP;
            }

            // ============================================================
            // RIGHT PANEL - Stats Display (60% width)
            // ============================================================

            var rightPanelGO = CreateUIImage(safeAreaGO.transform, "StatsPanel",
                new Vector2(0.4f, 0.35f), new Vector2(1, 0.93f),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var rightPanelRT = rightPanelGO.GetComponent<RectTransform>();
            rightPanelRT.offsetMin = new Vector2(0, 0);
            rightPanelRT.offsetMax = new Vector2(-10, 0);

            // Stats title
            var statsTitleGO = CreateUIText(rightPanelGO.transform, "StatsTitle", "STATS",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 40), 26);
            var statsTitleRT = statsTitleGO.GetComponent<RectTransform>();
            statsTitleRT.anchorMin = new Vector2(0, 1);
            statsTitleRT.anchorMax = new Vector2(1, 1);
            statsTitleRT.pivot = new Vector2(0.5f, 1);
            statsTitleRT.anchoredPosition = new Vector2(0, -5);
            statsTitleRT.sizeDelta = new Vector2(0, 40);
            var statsTitleTMP = statsTitleGO.GetComponent<TextMeshProUGUI>();
            statsTitleTMP.alignment = TextAlignmentOptions.Center;
            statsTitleTMP.fontStyle = FontStyles.Bold;
            statsTitleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Stats text
            var statsTextGO = CreateUIText(rightPanelGO.transform, "StatsText",
                "HP: 0\nMP: 0\nATK: 0\nDEF: 0\nMATK: 0\nMDEF: 0\nAGI: 0\nCrit%: 0.0%",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 24);
            var statsTextRT = statsTextGO.GetComponent<RectTransform>();
            statsTextRT.anchorMin = new Vector2(0, 0);
            statsTextRT.anchorMax = new Vector2(1, 1);
            statsTextRT.offsetMin = new Vector2(20, 10);
            statsTextRT.offsetMax = new Vector2(-10, -50);
            var statsTextTMP = statsTextGO.GetComponent<TextMeshProUGUI>();
            statsTextTMP.alignment = TextAlignmentOptions.TopLeft;

            // ============================================================
            // BOTTOM AREA - Unified Bag Grid
            // ============================================================

            var bottomPanelGO = CreateUIImage(safeAreaGO.transform, "BottomPanel",
                new Vector2(0, 0), new Vector2(1, 0.35f),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var bottomPanelRT = bottomPanelGO.GetComponent<RectTransform>();
            bottomPanelRT.offsetMin = new Vector2(10, 10);
            bottomPanelRT.offsetMax = new Vector2(-10, 0);

            // Bag count text (full width)
            var bagCountGO = CreateUIText(bottomPanelGO.transform, "BagCountText", "Bag: 0/50",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 35), 24);
            var bagCountRT = bagCountGO.GetComponent<RectTransform>();
            bagCountRT.anchorMin = new Vector2(0, 1);
            bagCountRT.anchorMax = new Vector2(1, 1);
            bagCountRT.pivot = new Vector2(0, 1);
            bagCountRT.anchoredPosition = new Vector2(10, -5);
            bagCountRT.sizeDelta = new Vector2(0, 35);
            var bagCountTMP = bagCountGO.GetComponent<TextMeshProUGUI>();
            bagCountTMP.alignment = TextAlignmentOptions.Left;
            bagCountTMP.fontStyle = FontStyles.Bold;

            // Bag container (scrollable, full width)
            var bagScrollGO = new GameObject("BagScroll", typeof(RectTransform));
            bagScrollGO.transform.SetParent(bottomPanelGO.transform, false);
            var bagScrollRT = bagScrollGO.GetComponent<RectTransform>();
            bagScrollRT.anchorMin = new Vector2(0, 0);
            bagScrollRT.anchorMax = new Vector2(1, 1);
            bagScrollRT.offsetMin = new Vector2(5, 5);
            bagScrollRT.offsetMax = new Vector2(-5, -42);
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
            var bagContentSizeFitter = bagContentGO.AddComponent<ContentSizeFitter>();
            bagContentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // GridLayoutGroup: 5 columns of square cells
            var bagGridLayout = bagContentGO.AddComponent<GridLayoutGroup>();
            bagGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            bagGridLayout.constraintCount = 5;
            bagGridLayout.cellSize = new Vector2(190, 190);
            bagGridLayout.spacing = new Vector2(8, 8);
            bagGridLayout.padding = new RectOffset(8, 8, 8, 8);
            bagGridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            bagGridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            bagGridLayout.childAlignment = TextAnchor.UpperLeft;

            bagScrollRect.content = bagContentRT;

            // ============================================================
            // ITEM DETAIL PANEL (center overlay, hidden by default)
            // ============================================================

            // Backdrop (full-screen semi-transparent)
            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(safeAreaGO.transform, false);
            var detailPanelRT = detailPanelGO.GetComponent<RectTransform>();
            detailPanelRT.anchorMin = Vector2.zero;
            detailPanelRT.anchorMax = Vector2.one;
            detailPanelRT.offsetMin = Vector2.zero;
            detailPanelRT.offsetMax = Vector2.zero;
            var detailBgImg = detailPanelGO.AddComponent<Image>();
            detailBgImg.color = new Color(0f, 0f, 0f, 0.7f);

            // Inner panel (80% width, 60% height)
            var detailInnerGO = new GameObject("DetailInner", typeof(RectTransform));
            detailInnerGO.transform.SetParent(detailPanelGO.transform, false);
            var detailInnerRT = detailInnerGO.GetComponent<RectTransform>();
            detailInnerRT.anchorMin = new Vector2(0.1f, 0.2f);
            detailInnerRT.anchorMax = new Vector2(0.9f, 0.8f);
            detailInnerRT.offsetMin = Vector2.zero;
            detailInnerRT.offsetMax = Vector2.zero;
            var detailInnerImg = detailInnerGO.AddComponent<Image>();
            detailInnerImg.color = new Color(0.08f, 0.08f, 0.14f, 0.95f);

            // Item name (bold, large)
            var itemNameGO = CreateUIText(detailInnerGO.transform, "ItemNameText", "Item Name",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -15), new Vector2(0, 50), 36);
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

            // Item stats text
            var itemStatsGO = CreateUIText(detailInnerGO.transform, "ItemStatsText", "Stats...",
                new Vector2(0, 1), new Vector2(0.5f, 1),
                new Vector2(20, -75), new Vector2(0, 200), 22);
            var itemStatsRT = itemStatsGO.GetComponent<RectTransform>();
            itemStatsRT.anchorMin = new Vector2(0, 1);
            itemStatsRT.anchorMax = new Vector2(0.5f, 1);
            itemStatsRT.pivot = new Vector2(0, 1);
            itemStatsRT.anchoredPosition = new Vector2(20, -75);
            itemStatsRT.sizeDelta = new Vector2(0, 200);
            var itemStatsTMP = itemStatsGO.GetComponent<TextMeshProUGUI>();
            itemStatsTMP.alignment = TextAlignmentOptions.TopLeft;

            // Item info text (quality, level req, sockets)
            var itemInfoGO = CreateUIText(detailInnerGO.transform, "ItemInfoText", "Quality: Normal\nReq Lv: 1\nSockets: 0/0",
                new Vector2(0.5f, 1), new Vector2(1, 1),
                new Vector2(0, -75), new Vector2(-20, 200), 22);
            var itemInfoRT = itemInfoGO.GetComponent<RectTransform>();
            itemInfoRT.anchorMin = new Vector2(0.5f, 1);
            itemInfoRT.anchorMax = new Vector2(1, 1);
            itemInfoRT.pivot = new Vector2(0, 1);
            itemInfoRT.anchoredPosition = new Vector2(10, -75);
            itemInfoRT.sizeDelta = new Vector2(-20, 200);
            var itemInfoTMP = itemInfoGO.GetComponent<TextMeshProUGUI>();
            itemInfoTMP.alignment = TextAlignmentOptions.TopLeft;

            // Equip/Unequip button (blue)
            var equipBtnGO = new GameObject("EquipButton", typeof(RectTransform));
            equipBtnGO.transform.SetParent(detailInnerGO.transform, false);
            var equipBtnRT = equipBtnGO.GetComponent<RectTransform>();
            equipBtnRT.anchorMin = new Vector2(0.1f, 0);
            equipBtnRT.anchorMax = new Vector2(0.9f, 0);
            equipBtnRT.pivot = new Vector2(0.5f, 0);
            equipBtnRT.anchoredPosition = new Vector2(0, 170);
            equipBtnRT.sizeDelta = new Vector2(0, 60);
            var equipBtnImg = equipBtnGO.AddComponent<Image>();
            equipBtnImg.color = new Color(0.2f, 0.4f, 0.75f, 1f); // blue
            var equipBtn = equipBtnGO.AddComponent<Button>();
            equipBtn.targetGraphic = equipBtnImg;

            var equipBtnTextGO = CreateUIText(equipBtnGO.transform, "EquipText", "Equip",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var equipBtnTMP = equipBtnTextGO.GetComponent<TextMeshProUGUI>();
            equipBtnTMP.alignment = TextAlignmentOptions.Center;
            equipBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade button (orange)
            var upgradeBtnGO = new GameObject("UpgradeButton", typeof(RectTransform));
            upgradeBtnGO.transform.SetParent(detailInnerGO.transform, false);
            var upgradeBtnRT = upgradeBtnGO.GetComponent<RectTransform>();
            upgradeBtnRT.anchorMin = new Vector2(0.1f, 0);
            upgradeBtnRT.anchorMax = new Vector2(0.9f, 0);
            upgradeBtnRT.pivot = new Vector2(0.5f, 0);
            upgradeBtnRT.anchoredPosition = new Vector2(0, 100);
            upgradeBtnRT.sizeDelta = new Vector2(0, 60);
            var upgradeBtnImg = upgradeBtnGO.AddComponent<Image>();
            upgradeBtnImg.color = new Color(0.85f, 0.55f, 0.1f, 1f); // orange
            var upgradeBtn = upgradeBtnGO.AddComponent<Button>();
            upgradeBtn.targetGraphic = upgradeBtnImg;

            var upgradeBtnTextGO = CreateUIText(upgradeBtnGO.transform, "UpgradeText", "Upgrade",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var upgradeBtnTMP = upgradeBtnTextGO.GetComponent<TextMeshProUGUI>();
            upgradeBtnTMP.alignment = TextAlignmentOptions.Center;
            upgradeBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade rate text (below upgrade button)
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

            // Close button (gray, bottom)
            var closeBtnGO = new GameObject("CloseDetailButton", typeof(RectTransform));
            closeBtnGO.transform.SetParent(detailInnerGO.transform, false);
            var closeBtnRT = closeBtnGO.GetComponent<RectTransform>();
            closeBtnRT.anchorMin = new Vector2(0.2f, 0);
            closeBtnRT.anchorMax = new Vector2(0.8f, 0);
            closeBtnRT.pivot = new Vector2(0.5f, 0);
            closeBtnRT.anchoredPosition = new Vector2(0, 15);
            closeBtnRT.sizeDelta = new Vector2(0, 50);
            var closeBtnImg = closeBtnGO.AddComponent<Image>();
            closeBtnImg.color = new Color(0.35f, 0.35f, 0.4f, 1f); // gray
            var closeBtn = closeBtnGO.AddComponent<Button>();
            closeBtn.targetGraphic = closeBtnImg;

            var closeBtnTextGO = CreateUIText(closeBtnGO.transform, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            var closeBtnTMP = closeBtnTextGO.GetComponent<TextMeshProUGUI>();
            closeBtnTMP.alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            // EquipmentSceneUI on the canvas
            var equipmentSceneUI = canvasGO.AddComponent<EquipmentSceneUI>();
            var uiSO = new SerializedObject(equipmentSceneUI);

            // Header
            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_goldText").objectReferenceValue = goldTMP;

            // Equipped Slots (arrays)
            var slotButtonsProp = uiSO.FindProperty("_slotButtons");
            slotButtonsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotButtons[i];

            var slotTextsProp = uiSO.FindProperty("_slotTexts");
            slotTextsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotTextsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotTexts[i];

            // Stats Panel
            uiSO.FindProperty("_statsText").objectReferenceValue = statsTextTMP;

            // Bag Panel
            uiSO.FindProperty("_bagContainer").objectReferenceValue = bagContentGO.transform;
            uiSO.FindProperty("_bagCountText").objectReferenceValue = bagCountTMP;

            // Item Detail Panel
            uiSO.FindProperty("_detailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_itemNameText").objectReferenceValue = itemNameTMP;
            uiSO.FindProperty("_itemStatsText").objectReferenceValue = itemStatsTMP;
            uiSO.FindProperty("_itemInfoText").objectReferenceValue = itemInfoTMP;
            uiSO.FindProperty("_equipButton").objectReferenceValue = equipBtn;
            uiSO.FindProperty("_equipButtonText").objectReferenceValue = equipBtnTMP;
            uiSO.FindProperty("_upgradeButton").objectReferenceValue = upgradeBtn;
            uiSO.FindProperty("_upgradeRateText").objectReferenceValue = upgradeRateTMP;
            uiSO.FindProperty("_closeDetailButton").objectReferenceValue = closeBtn;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            // Wire EquipmentController
            var controllerGO = new GameObject("EquipmentController");
            var controller = controllerGO.AddComponent<EquipmentController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_equipmentUI").objectReferenceValue = equipmentSceneUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            // --- Finalize ---
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Equipment scene setup complete! Hit Play to test.");
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
