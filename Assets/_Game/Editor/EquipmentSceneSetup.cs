using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Equipment;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Editor
{
    public static class EquipmentSceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Equipment Scene")]
        public static void Setup()
        {
            UIAtlasHelper.ClearCache();

            // --- Clear current scene objects ---
            var scene = EditorSceneManager.GetActiveScene();
            foreach (var root in scene.GetRootGameObjects())
            {
                Object.DestroyImmediate(root);
            }
            // No Camera needed — this is a sub-scene with ScreenSpaceOverlay canvas.
            // No EventSystem needed — the Gameplay scene already provides one.

            // --- Canvas (1080x1920 portrait phone) ---
            var canvasGO = new GameObject("Equipment_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // ============================================================
            // FULL SCREEN CONTAINER
            // ============================================================

            var topHalfGO = new GameObject("ContentContainer", typeof(RectTransform));
            topHalfGO.transform.SetParent(canvasGO.transform, false);
            var topHalfRT = topHalfGO.GetComponent<RectTransform>();
            topHalfRT.anchorMin = new Vector2(0, 0.18f);
            topHalfRT.anchorMax = Vector2.one;
            topHalfRT.offsetMin = Vector2.zero;
            topHalfRT.offsetMax = new Vector2(0, -120); // safe area: clears dynamic island / notch
            var topHalfImg = topHalfGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(topHalfImg, new Color(0.85f, 0.85f, 0.9f, 0.92f));
            var topHalfContent = UIAtlasHelper.CreatePanelContent(topHalfGO.transform);

            // ============================================================
            // HEADER (top of container)
            // ============================================================

            var titleGO = CreateUIText(topHalfContent, "TitleText", "",
                new Vector2(0, 1), new Vector2(1, 1),
                Vector2.zero, new Vector2(0, 0), 1);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();

            // Close button (X) — top-right edge of panel
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(topHalfGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(1, 1);
            backBtnRT.anchorMax = new Vector2(1, 1);
            backBtnRT.pivot = new Vector2(1, 1);
            backBtnRT.anchoredPosition = Vector2.zero;
            backBtnRT.sizeDelta = new Vector2(50, 50);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            var backBtn = backBtnGO.AddComponent<Button>();
            UIAtlasHelper.SetXButton(backBtn, backBtnImg);

            // ============================================================
            // TAB BUTTONS (horizontal, top-left, same row)
            // ============================================================

            float tabSize = 80f;
            float tabGap = 5f;

            // ============================================================
            // EQUIPPED PANEL — slots directly in content, no inner panel
            // ============================================================

            var equippedPanelGO = new GameObject("EquippedPanel", typeof(RectTransform));
            equippedPanelGO.transform.SetParent(topHalfContent, false);
            var equippedPanelRT = equippedPanelGO.GetComponent<RectTransform>();
            equippedPanelRT.anchorMin = Vector2.zero;
            equippedPanelRT.anchorMax = Vector2.one;
            equippedPanelRT.offsetMin = Vector2.zero;
            equippedPanelRT.offsetMax = Vector2.zero;

            // Slot layout: 3 rows × 2 columns + boots, all same size
            // Left column: Head, Neck, L.Hand
            // Center: Character preview
            // Right column: Armor, Ring, R.Hand
            // Bottom center: Boots
            // Compute equal slot size from panel width (left column = ~26% of panel)
            float panelContentWidth = 1080f - UIAtlasHelper.PanelPadL - UIAtlasHelper.PanelPadR;
            float slotSize = Mathf.Floor(panelContentWidth * 0.24f);

            // Slot center positions (anchor-based, point anchors)
            var slotCenters = new Vector2[]
            {
                new Vector2(0.15f, 0.85f),  // 0: Head
                new Vector2(0.15f, 0.58f),  // 1: Neck
                new Vector2(0.85f, 0.85f),  // 2: Armor
                new Vector2(0.15f, 0.31f),  // 3: L.Hand
                new Vector2(0.85f, 0.31f),  // 4: R.Hand
                new Vector2(0.85f, 0.58f),  // 5: Ring
                new Vector2(0.50f, 0.07f),  // 6: Boots (centered)
            };

            // Character preview — centered between slot columns
            var charPreviewGO = new GameObject("CharacterPreview", typeof(RectTransform));
            charPreviewGO.transform.SetParent(equippedPanelRT, false);
            var charPreviewRT = charPreviewGO.GetComponent<RectTransform>();
            charPreviewRT.anchorMin = new Vector2(0.30f, 0.18f);
            charPreviewRT.anchorMax = new Vector2(0.70f, 0.96f);
            charPreviewRT.offsetMin = Vector2.zero;
            charPreviewRT.offsetMax = Vector2.zero;
            var charPreviewImg = charPreviewGO.AddComponent<Image>();
            charPreviewImg.color = new Color(1f, 1f, 1f, 0f); // transparent until sprite loaded at runtime
            charPreviewImg.preserveAspect = true;
            charPreviewImg.raycastTarget = false;
            // Try loading default idle south sprite at editor time
            var atlasAssets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Atlases/GameAtlas.png");
            foreach (var asset in atlasAssets)
            {
                if (asset is Sprite s && s.name == "Male_Base_SIdle_01")
                {
                    charPreviewImg.sprite = s;
                    charPreviewImg.color = Color.white;
                    break;
                }
            }
            // Add UI sprite animator for idle animation at runtime
            var charAnimator = charPreviewGO.AddComponent<UISpriteAnimator>();
            var charAnimSO = new SerializedObject(charAnimator);
            charAnimSO.FindProperty("_image").objectReferenceValue = charPreviewImg;
            charAnimSO.FindProperty("_spritePrefix").stringValue = "Male_Base_SIdle_";
            charAnimSO.FindProperty("_fps").floatValue = 8f;
            charAnimSO.ApplyModifiedPropertiesWithoutUndo();

            string[] slotLabels = { "Head", "Neck", "Armor", "L.Hand", "R.Hand", "Ring", "Boots" };
            var slotButtons = new Button[7];
            var slotTexts = new TextMeshProUGUI[7];

            for (int i = 0; i < 7; i++)
            {
                var slotBtnGO = new GameObject($"SlotButton_{i}", typeof(RectTransform));
                slotBtnGO.transform.SetParent(equippedPanelRT, false);
                var slotBtnRT = slotBtnGO.GetComponent<RectTransform>();
                slotBtnRT.anchorMin = slotCenters[i];
                slotBtnRT.anchorMax = slotCenters[i];
                slotBtnRT.pivot = new Vector2(0.5f, 0.5f);
                slotBtnRT.anchoredPosition = Vector2.zero;
                slotBtnRT.sizeDelta = new Vector2(slotSize, slotSize);

                var slotBtnImg = slotBtnGO.AddComponent<Image>();
                UIAtlasHelper.SetSimpleSprite(slotBtnImg, "Blank_Slot");
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
                slotTextTMP.fontSize = 24;
                slotTextTMP.color = Color.white;
                slotTextTMP.alignment = TextAlignmentOptions.Center;
                slotTextTMP.enableAutoSizing = true;
                slotTextTMP.fontSizeMin = 12;
                slotTextTMP.fontSizeMax = 24;
                slotTexts[i] = slotTextTMP;
            }

            // ============================================================
            // STATS CONTENT PANEL (same area as EquippedPanel, hidden by default)
            // ============================================================

            var statsContentGO = new GameObject("StatsContentPanel", typeof(RectTransform));
            statsContentGO.transform.SetParent(topHalfContent, false);
            var statsContentContent = statsContentGO.transform;
            var statsContentRT = statsContentGO.GetComponent<RectTransform>();
            statsContentRT.anchorMin = Vector2.zero;
            statsContentRT.anchorMax = Vector2.one;
            statsContentRT.offsetMin = Vector2.zero;
            statsContentRT.offsetMax = Vector2.zero;

            // --- Info Panel (top ~18% of stats content) ---
            var statsInfoPanel = CreateUIImage(statsContentContent, "StatsInfoPanel",
                new Vector2(0, 0.82f), new Vector2(1, 1f),
                Vector2.zero, Vector2.zero,
                Color.white);
            statsInfoPanel.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.3f);
            var statsInfoContent = statsInfoPanel.transform;
            var statsInfoPanelRT = statsInfoPanel.GetComponent<RectTransform>();
            statsInfoPanelRT.offsetMin = new Vector2(0, 0);
            statsInfoPanelRT.offsetMax = new Vector2(0, 0);

            var classGO = CreateUIText(statsInfoContent, "ClassText", "Trojan",
                new Vector2(0, 0.5f), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 36);
            var classRT = classGO.GetComponent<RectTransform>();
            classRT.anchorMin = new Vector2(0, 0.5f);
            classRT.anchorMax = new Vector2(1, 1);
            classRT.offsetMin = new Vector2(10, 0);
            classRT.offsetMax = new Vector2(-10, -5);
            var classTMP = classGO.GetComponent<TextMeshProUGUI>();
            classTMP.alignment = TextAlignmentOptions.Center;
            classTMP.fontStyle = FontStyles.Bold;
            classTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            var levelGO = CreateUIText(statsInfoContent, "LevelText", "Level 1",
                new Vector2(0, 0), new Vector2(1, 0.5f),
                Vector2.zero, Vector2.zero, 32);
            var levelRT = levelGO.GetComponent<RectTransform>();
            levelRT.anchorMin = new Vector2(0, 0);
            levelRT.anchorMax = new Vector2(1, 0.5f);
            levelRT.offsetMin = new Vector2(10, 5);
            levelRT.offsetMax = new Vector2(-10, 0);
            var levelTMP = levelGO.GetComponent<TextMeshProUGUI>();
            levelTMP.alignment = TextAlignmentOptions.Center;

            // --- Combat Stats Panel (middle ~47%) ---
            var statsCombatPanel = CreateUIImage(statsContentContent, "StatsCombatPanel",
                new Vector2(0, 0.35f), new Vector2(1, 0.82f),
                Vector2.zero, Vector2.zero,
                Color.white);
            statsCombatPanel.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.3f);
            var statsCombatContent = statsCombatPanel.transform;
            var statsCombatPanelRT = statsCombatPanel.GetComponent<RectTransform>();
            statsCombatPanelRT.offsetMin = new Vector2(0, 5);
            statsCombatPanelRT.offsetMax = new Vector2(0, -5);

            var statsLabelGO = CreateUIText(statsCombatContent, "StatsLabel", "COMBAT STATS",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 36), 28);
            var statsLabelRT = statsLabelGO.GetComponent<RectTransform>();
            statsLabelRT.anchorMin = new Vector2(0, 1);
            statsLabelRT.anchorMax = new Vector2(1, 1);
            statsLabelRT.pivot = new Vector2(0.5f, 1);
            statsLabelRT.anchoredPosition = new Vector2(0, -5);
            statsLabelRT.sizeDelta = new Vector2(0, 36);
            var statsLabelTMP = statsLabelGO.GetComponent<TextMeshProUGUI>();
            statsLabelTMP.alignment = TextAlignmentOptions.Center;
            statsLabelTMP.fontStyle = FontStyles.Bold;
            statsLabelTMP.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            var statsTextGO = CreateUIText(statsCombatContent, "StatsText",
                "HP: 0        MP: 0\nATK: 0       DEF: 0\nMATK: 0      MDEF: 0\nAGI: 0       AtkSpd: 1.00\nCrit: 0.0%   CritDmg: 150%",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 28);
            var statsTextRT = statsTextGO.GetComponent<RectTransform>();
            statsTextRT.anchorMin = new Vector2(0, 0);
            statsTextRT.anchorMax = new Vector2(1, 1);
            statsTextRT.offsetMin = new Vector2(20, 10);
            statsTextRT.offsetMax = new Vector2(-20, -38);
            var statsTextTMP = statsTextGO.GetComponent<TextMeshProUGUI>();
            statsTextTMP.alignment = TextAlignmentOptions.Left;
            statsTextTMP.enableAutoSizing = true;
            statsTextTMP.fontSizeMin = 20;
            statsTextTMP.fontSizeMax = 34;

            // --- Allocation Panel (bottom ~35%) ---
            var statsAllocPanel = CreateUIImage(statsContentContent, "StatsAllocPanel",
                new Vector2(0, 0), new Vector2(1, 0.35f),
                Vector2.zero, Vector2.zero,
                Color.white);
            statsAllocPanel.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.3f);
            var statsAllocContent = statsAllocPanel.transform;
            var statsAllocPanelRT = statsAllocPanel.GetComponent<RectTransform>();
            statsAllocPanelRT.offsetMin = new Vector2(0, 0);
            statsAllocPanelRT.offsetMax = new Vector2(0, -5);

            var statPointsGO = CreateUIText(statsAllocContent, "StatPointsText", "Stat Points: 0",
                new Vector2(0, 0.78f), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 28);
            var statPointsRT = statPointsGO.GetComponent<RectTransform>();
            statPointsRT.anchorMin = new Vector2(0, 0.78f);
            statPointsRT.anchorMax = new Vector2(1, 1);
            statPointsRT.offsetMin = new Vector2(5, 0);
            statPointsRT.offsetMax = new Vector2(-5, -2);
            var statPointsTMP = statPointsGO.GetComponent<TextMeshProUGUI>();
            statPointsTMP.alignment = TextAlignmentOptions.Center;
            statPointsTMP.fontStyle = FontStyles.Bold;
            statPointsTMP.color = new Color(0.3f, 0.9f, 0.3f, 1f);
            statPointsTMP.enableAutoSizing = true;
            statPointsTMP.fontSizeMin = 18;
            statPointsTMP.fontSizeMax = 28;

            // Stat entries — 2×2 grid, each: "StatName: value" text + small +Button
            string[] statDisplayNames = { "Vitality", "Agility", "Strength", "Spirit" };
            string[] statFieldNames = { "Vitality", "Agility", "Strength", "Spirit" };
            var statTexts = new TextMeshProUGUI[4];
            var statButtons = new Button[4];

            // Row 1 (Vitality, Agility) — Y: 44% to 78%
            // Row 2 (Strength, Spirit) — Y: 04% to 38%
            float[][] cellAnchors = {
                new[] { 0.02f, 0.44f, 0.48f, 0.78f },  // Vitality
                new[] { 0.52f, 0.44f, 0.98f, 0.78f },  // Agility
                new[] { 0.02f, 0.04f, 0.48f, 0.38f },  // Strength
                new[] { 0.52f, 0.04f, 0.98f, 0.38f },  // Spirit
            };

            for (int i = 0; i < 4; i++)
            {
                // Container cell
                var cellGO = new GameObject($"{statFieldNames[i]}Cell", typeof(RectTransform));
                cellGO.transform.SetParent(statsAllocContent, false);
                var cellRT = cellGO.GetComponent<RectTransform>();
                cellRT.anchorMin = new Vector2(cellAnchors[i][0], cellAnchors[i][1]);
                cellRT.anchorMax = new Vector2(cellAnchors[i][2], cellAnchors[i][3]);
                cellRT.offsetMin = Vector2.zero;
                cellRT.offsetMax = Vector2.zero;

                // Text label: "Vitality: 0" — left portion
                var textGO = CreateUIText(cellGO.transform, "Text", $"{statDisplayNames[i]}: 0",
                    new Vector2(0, 0), new Vector2(0.70f, 1),
                    Vector2.zero, Vector2.zero, 26);
                var textRT = textGO.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = new Vector2(0.70f, 1);
                textRT.offsetMin = new Vector2(5, 0);
                textRT.offsetMax = Vector2.zero;
                var textTMP = textGO.GetComponent<TextMeshProUGUI>();
                textTMP.alignment = TextAlignmentOptions.Left;
                textTMP.enableAutoSizing = true;
                textTMP.fontSizeMin = 14;
                textTMP.fontSizeMax = 26;
                statTexts[i] = textTMP;

                // +Button — right side, square, preserveAspect
                var btnGO = new GameObject($"{statFieldNames[i]}Btn", typeof(RectTransform));
                btnGO.transform.SetParent(cellGO.transform, false);
                var btnRT = btnGO.GetComponent<RectTransform>();
                btnRT.anchorMin = new Vector2(0.78f, 0.15f);
                btnRT.anchorMax = new Vector2(0.98f, 0.85f);
                btnRT.offsetMin = Vector2.zero;
                btnRT.offsetMax = Vector2.zero;
                var btnImg = btnGO.AddComponent<Image>();
                btnImg.preserveAspect = true;
                var unpressed = UIAtlasHelper.GetSprite("PlusButton_Unpressed");
                var pressed = UIAtlasHelper.GetSprite("PlusButton_Pressed");
                var disabled = UIAtlasHelper.GetSprite("PlusButton_Disabled");
                if (unpressed != null)
                {
                    btnImg.sprite = unpressed;
                    btnImg.type = Image.Type.Simple;
                }
                btnImg.color = Color.white;
                var btn = btnGO.AddComponent<Button>();
                btn.transition = Selectable.Transition.SpriteSwap;
                btn.targetGraphic = btnImg;
                var spriteState = new SpriteState();
                spriteState.pressedSprite = pressed;
                spriteState.disabledSprite = disabled;
                btn.spriteState = spriteState;
                statButtons[i] = btn;
            }

            statsContentGO.SetActive(false);

            // ============================================================
            // ITEM DETAIL PANEL (overlay within top-half container, hidden by default)
            // ============================================================

            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(topHalfContent, false);
            var detailPanelRT = detailPanelGO.GetComponent<RectTransform>();
            detailPanelRT.anchorMin = Vector2.zero;
            detailPanelRT.anchorMax = Vector2.one;
            detailPanelRT.offsetMin = Vector2.zero;
            detailPanelRT.offsetMax = Vector2.zero;
            detailPanelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

            var detailInnerGO = new GameObject("DetailInner", typeof(RectTransform));
            detailInnerGO.transform.SetParent(detailPanelGO.transform, false);
            var detailInnerRT = detailInnerGO.GetComponent<RectTransform>();
            detailInnerRT.anchorMin = new Vector2(0.08f, 0.1f);
            detailInnerRT.anchorMax = new Vector2(0.92f, 0.9f);
            detailInnerRT.offsetMin = Vector2.zero;
            detailInnerRT.offsetMax = Vector2.zero;
            var detailInnerImg = detailInnerGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(detailInnerImg);
            var detailInnerContent = UIAtlasHelper.CreatePanelContent(detailInnerGO.transform);

            // Item name
            var itemNameGO = CreateUIText(detailInnerContent, "ItemNameText", "Item Name",
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
            var itemStatsGO = CreateUIText(detailInnerContent, "ItemStatsText", "Stats...",
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
            var itemInfoGO = CreateUIText(detailInnerContent, "ItemInfoText", "Quality: Normal\nReq Lv: 1\nSockets: 0/0",
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
            var equipBtnGO = CreateButton(detailInnerContent, "EquipButton",
                new Vector2(0, 170), new Vector2(0, 60), Color.white);
            var equipBtn = equipBtnGO.GetComponent<Button>();
            UIAtlasHelper.SetSpriteSwapButton(equipBtn, equipBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var equipBtnContent = UIAtlasHelper.CreateButtonContent(equipBtnGO.transform, 60f);
            var equipBtnTextGO = CreateUIText(equipBtnContent, "EquipText", "Equip",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var equipBtnTMP = equipBtnTextGO.GetComponent<TextMeshProUGUI>();
            equipBtnTMP.alignment = TextAlignmentOptions.Center;
            equipBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade button
            var upgradeBtnGO = CreateButton(detailInnerContent, "UpgradeButton",
                new Vector2(0, 100), new Vector2(0, 60), Color.white);
            var upgradeBtn = upgradeBtnGO.GetComponent<Button>();
            UIAtlasHelper.SetSpriteSwapButton(upgradeBtn, upgradeBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var upgradeBtnContent = UIAtlasHelper.CreateButtonContent(upgradeBtnGO.transform, 60f);
            var upgradeBtnTextGO = CreateUIText(upgradeBtnContent, "UpgradeText", "Upgrade",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var upgradeBtnTMP = upgradeBtnTextGO.GetComponent<TextMeshProUGUI>();
            upgradeBtnTMP.alignment = TextAlignmentOptions.Center;
            upgradeBtnTMP.fontStyle = FontStyles.Bold;

            // Upgrade rate text
            var upgradeRateGO = CreateUIText(detailInnerContent, "UpgradeRateText", "Success: 100%",
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
            var closeBtnGO = CreateButton(detailInnerContent, "CloseDetailButton",
                new Vector2(0, 15), new Vector2(0, 50), Color.white);
            UIAtlasHelper.SetSpriteSwapButton(closeBtnGO.GetComponent<Button>(), closeBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            closeBtnGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0);
            closeBtnGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0);
            var closeBtn = closeBtnGO.GetComponent<Button>();
            var closeBtnContent = UIAtlasHelper.CreateButtonContent(closeBtnGO.transform, 50f);
            var closeBtnTextGO = CreateUIText(closeBtnContent, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            closeBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // ============================================================
            // TAB & X BUTTONS — created last so they render on top of all panels
            // ============================================================

            // Tab 1: Equipment (top-left edge of panel)
            var equipTabGO = new GameObject("EquipmentTab", typeof(RectTransform));
            equipTabGO.transform.SetParent(topHalfGO.transform, false);
            var equipTabRT = equipTabGO.GetComponent<RectTransform>();
            equipTabRT.anchorMin = new Vector2(0, 1);
            equipTabRT.anchorMax = new Vector2(0, 1);
            equipTabRT.pivot = new Vector2(0, 1);
            equipTabRT.anchoredPosition = Vector2.zero;
            equipTabRT.sizeDelta = new Vector2(tabSize, tabSize);
            var equipTabImg = equipTabGO.AddComponent<Image>();
            UIAtlasHelper.SetSimpleSprite(equipTabImg, "EquipmentTab_Opened");
            var equipTabBtn = equipTabGO.AddComponent<Button>();
            equipTabBtn.targetGraphic = equipTabImg;
            var equipTabTextGO = CreateUIText(equipTabGO.transform, "Text", "",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 1);
            var equipTabTMP = equipTabTextGO.GetComponent<TextMeshProUGUI>();

            // Tab 2: Character (next to equipment tab, top-left edge)
            var statsTabGO = new GameObject("StatsTab", typeof(RectTransform));
            statsTabGO.transform.SetParent(topHalfGO.transform, false);
            var statsTabRT = statsTabGO.GetComponent<RectTransform>();
            statsTabRT.anchorMin = new Vector2(0, 1);
            statsTabRT.anchorMax = new Vector2(0, 1);
            statsTabRT.pivot = new Vector2(0, 1);
            statsTabRT.anchoredPosition = new Vector2(tabSize + tabGap, 0);
            statsTabRT.sizeDelta = new Vector2(tabSize, tabSize);
            var statsTabImg = statsTabGO.AddComponent<Image>();
            UIAtlasHelper.SetSimpleSprite(statsTabImg, "CharacterTab_Closed");
            var statsTabBtn = statsTabGO.AddComponent<Button>();
            statsTabBtn.targetGraphic = statsTabImg;
            var statsTabTextGO = CreateUIText(statsTabGO.transform, "Text", "",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 1);
            var statsTabTMP = statsTabTextGO.GetComponent<TextMeshProUGUI>();

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            var equipmentSceneUI = canvasGO.AddComponent<EquipmentSceneUI>();
            var uiSO = new SerializedObject(equipmentSceneUI);

            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;

            // Tab buttons
            uiSO.FindProperty("_equipmentTabButton").objectReferenceValue = equipTabBtn;
            uiSO.FindProperty("_statsTabButton").objectReferenceValue = statsTabBtn;
            uiSO.FindProperty("_equipmentTabImage").objectReferenceValue = equipTabImg;
            uiSO.FindProperty("_statsTabImage").objectReferenceValue = statsTabImg;
            uiSO.FindProperty("_equipmentTabText").objectReferenceValue = equipTabTMP;
            uiSO.FindProperty("_statsTabText").objectReferenceValue = statsTabTMP;

            // Character preview
            uiSO.FindProperty("_characterPreview").objectReferenceValue = charPreviewImg;
            uiSO.FindProperty("_characterAnimator").objectReferenceValue = charAnimator;

            // Content panels
            uiSO.FindProperty("_equipmentContent").objectReferenceValue = equippedPanelGO;
            uiSO.FindProperty("_statsContent").objectReferenceValue = statsContentGO;

            var slotButtonsProp = uiSO.FindProperty("_slotButtons");
            slotButtonsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotButtonsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotButtons[i];

            var slotTextsProp = uiSO.FindProperty("_slotTexts");
            slotTextsProp.arraySize = 7;
            for (int i = 0; i < 7; i++)
                slotTextsProp.GetArrayElementAtIndex(i).objectReferenceValue = slotTexts[i];

            uiSO.FindProperty("_detailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_itemNameText").objectReferenceValue = itemNameTMP;
            uiSO.FindProperty("_itemStatsText").objectReferenceValue = itemStatsGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_itemInfoText").objectReferenceValue = itemInfoGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_equipButton").objectReferenceValue = equipBtn;
            uiSO.FindProperty("_equipButtonText").objectReferenceValue = equipBtnTMP;
            uiSO.FindProperty("_upgradeButton").objectReferenceValue = upgradeBtn;
            uiSO.FindProperty("_upgradeRateText").objectReferenceValue = upgradeRateTMP;
            uiSO.FindProperty("_closeDetailButton").objectReferenceValue = closeBtn;

            // Stats fields
            uiSO.FindProperty("_classText").objectReferenceValue = classTMP;
            uiSO.FindProperty("_levelText").objectReferenceValue = levelTMP;
            uiSO.FindProperty("_statsText").objectReferenceValue = statsTextTMP;
            uiSO.FindProperty("_statPointsText").objectReferenceValue = statPointsTMP;
            uiSO.FindProperty("_vitalityText").objectReferenceValue = statTexts[0];
            uiSO.FindProperty("_strengthText").objectReferenceValue = statTexts[2];
            uiSO.FindProperty("_agilityText").objectReferenceValue = statTexts[1];
            uiSO.FindProperty("_spiritText").objectReferenceValue = statTexts[3];
            uiSO.FindProperty("_vitalityButton").objectReferenceValue = statButtons[0];
            uiSO.FindProperty("_strengthButton").objectReferenceValue = statButtons[2];
            uiSO.FindProperty("_agilityButton").objectReferenceValue = statButtons[1];
            uiSO.FindProperty("_spiritButton").objectReferenceValue = statButtons[3];

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
