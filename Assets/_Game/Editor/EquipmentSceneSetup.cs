using UnityEngine;
using UnityEngine.UI;
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
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // ============================================================
            // TOP HALF CONTAINER — anchored to top 50% of screen
            // ============================================================

            var topHalfGO = new GameObject("TopHalfContainer", typeof(RectTransform));
            topHalfGO.transform.SetParent(canvasGO.transform, false);
            var topHalfRT = topHalfGO.GetComponent<RectTransform>();
            topHalfRT.anchorMin = new Vector2(0, 0.5f);
            topHalfRT.anchorMax = new Vector2(1, 1);
            topHalfRT.offsetMin = Vector2.zero;
            topHalfRT.offsetMax = new Vector2(0, -120); // safe area: clears dynamic island / notch
            var topHalfImg = topHalfGO.AddComponent<Image>();
            topHalfImg.color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

            // ============================================================
            // HEADER (top of container)
            // ============================================================

            var titleGO = CreateUIText(topHalfGO.transform, "TitleText", "EQUIPMENT",
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

            // Close button (X) — top-right corner
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(topHalfGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(1, 1);
            backBtnRT.anchorMax = new Vector2(1, 1);
            backBtnRT.pivot = new Vector2(1, 1);
            backBtnRT.anchoredPosition = new Vector2(-10, -10);
            backBtnRT.sizeDelta = new Vector2(50, 50);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            backBtnImg.color = new Color(0.3f, 0.15f, 0.15f, 0.9f);
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;
            var backBtnTextGO = CreateUIText(backBtnGO.transform, "BackText", "X",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var backBtnTMP = backBtnTextGO.GetComponent<TextMeshProUGUI>();
            backBtnTMP.alignment = TextAlignmentOptions.Center;
            backBtnTMP.fontStyle = FontStyles.Bold;

            // ============================================================
            // TAB BAR (below header)
            // ============================================================

            var tabBarGO = new GameObject("TabBar", typeof(RectTransform));
            tabBarGO.transform.SetParent(topHalfGO.transform, false);
            var tabBarRT = tabBarGO.GetComponent<RectTransform>();
            tabBarRT.anchorMin = new Vector2(0, 1);
            tabBarRT.anchorMax = new Vector2(1, 1);
            tabBarRT.pivot = new Vector2(0.5f, 1);
            tabBarRT.anchoredPosition = new Vector2(0, -75);
            tabBarRT.sizeDelta = new Vector2(0, 50);

            // Tab 1: "Equipment" (left half)
            var equipTabGO = new GameObject("EquipmentTab", typeof(RectTransform));
            equipTabGO.transform.SetParent(tabBarGO.transform, false);
            var equipTabRT = equipTabGO.GetComponent<RectTransform>();
            equipTabRT.anchorMin = new Vector2(0, 0);
            equipTabRT.anchorMax = new Vector2(0.5f, 1);
            equipTabRT.offsetMin = new Vector2(15, 0);
            equipTabRT.offsetMax = new Vector2(-4, 0);
            var equipTabImg = equipTabGO.AddComponent<Image>();
            equipTabImg.color = new Color(0.18f, 0.18f, 0.28f, 1f);
            var equipTabBtn = equipTabGO.AddComponent<Button>();
            equipTabBtn.targetGraphic = equipTabImg;
            var equipTabTextGO = CreateUIText(equipTabGO.transform, "Text", "Equipment",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            var equipTabTMP = equipTabTextGO.GetComponent<TextMeshProUGUI>();
            equipTabTMP.alignment = TextAlignmentOptions.Center;
            equipTabTMP.fontStyle = FontStyles.Bold;
            equipTabTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Tab 2: "Stats" (right half)
            var statsTabGO = new GameObject("StatsTab", typeof(RectTransform));
            statsTabGO.transform.SetParent(tabBarGO.transform, false);
            var statsTabRT = statsTabGO.GetComponent<RectTransform>();
            statsTabRT.anchorMin = new Vector2(0.5f, 0);
            statsTabRT.anchorMax = new Vector2(1, 1);
            statsTabRT.offsetMin = new Vector2(4, 0);
            statsTabRT.offsetMax = new Vector2(-15, 0);
            var statsTabImg = statsTabGO.AddComponent<Image>();
            statsTabImg.color = new Color(0.1f, 0.1f, 0.16f, 1f);
            var statsTabBtn = statsTabGO.AddComponent<Button>();
            statsTabBtn.targetGraphic = statsTabImg;
            var statsTabTextGO = CreateUIText(statsTabGO.transform, "Text", "Stats",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            var statsTabTMP = statsTabTextGO.GetComponent<TextMeshProUGUI>();
            statsTabTMP.alignment = TextAlignmentOptions.Center;
            statsTabTMP.fontStyle = FontStyles.Bold;
            statsTabTMP.color = new Color(0.5f, 0.5f, 0.5f, 1f);

            // ============================================================
            // EQUIPPED PANEL — grid layout (fills container below tab bar)
            // ============================================================

            var equippedPanelGO = CreateUIImage(topHalfGO.transform, "EquippedPanel",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var equippedPanelRT = equippedPanelGO.GetComponent<RectTransform>();
            equippedPanelRT.offsetMin = new Vector2(15, 10);
            equippedPanelRT.offsetMax = new Vector2(-15, -130);

            // Slot positions — 2 compact rows, anchor-based for screen scaling
            // Row 1 (top): Head, Neck, Armor, Boots
            // Row 2 (bot): L.Hand, Ring, R.Hand
            var slotAnchors = new Vector2[]
            {
                new Vector2(0.14f, 0.68f),  // 0: Head
                new Vector2(0.38f, 0.68f),  // 1: Neck
                new Vector2(0.62f, 0.68f),  // 2: Armor
                new Vector2(0.22f, 0.32f),  // 3: L.Hand
                new Vector2(0.78f, 0.32f),  // 4: R.Hand
                new Vector2(0.50f, 0.32f),  // 5: Ring
                new Vector2(0.86f, 0.68f),  // 6: Boots
            };

            string[] slotLabels = { "Head", "Neck", "Armor", "L.Hand", "R.Hand", "Ring", "Boots" };
            var slotButtons = new Button[7];
            var slotTexts = new TextMeshProUGUI[7];
            float slotSize = 130f;

            for (int i = 0; i < 7; i++)
            {
                var anchor = slotAnchors[i];
                var slotBtnGO = new GameObject($"SlotButton_{i}", typeof(RectTransform));
                slotBtnGO.transform.SetParent(equippedPanelGO.transform, false);
                var slotBtnRT = slotBtnGO.GetComponent<RectTransform>();
                slotBtnRT.anchorMin = anchor;
                slotBtnRT.anchorMax = anchor;
                slotBtnRT.pivot = new Vector2(0.5f, 0.5f);
                slotBtnRT.anchoredPosition = Vector2.zero;
                slotBtnRT.sizeDelta = new Vector2(slotSize, slotSize);

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
            // STATS CONTENT PANEL (same area as EquippedPanel, hidden by default)
            // ============================================================

            var statsContentGO = CreateUIImage(topHalfGO.transform, "StatsContentPanel",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var statsContentRT = statsContentGO.GetComponent<RectTransform>();
            statsContentRT.offsetMin = new Vector2(15, 10);
            statsContentRT.offsetMax = new Vector2(-15, -130);

            // --- Info Panel (top ~18% of stats content) ---
            var statsInfoPanel = CreateUIImage(statsContentGO.transform, "StatsInfoPanel",
                new Vector2(0, 0.82f), new Vector2(1, 1f),
                Vector2.zero, Vector2.zero,
                new Color(0.12f, 0.12f, 0.18f, 0.9f));
            var statsInfoPanelRT = statsInfoPanel.GetComponent<RectTransform>();
            statsInfoPanelRT.offsetMin = new Vector2(0, 0);
            statsInfoPanelRT.offsetMax = new Vector2(0, 0);

            var classGO = CreateUIText(statsInfoPanel.transform, "ClassText", "Trojan",
                new Vector2(0, 0), new Vector2(0.35f, 1),
                Vector2.zero, Vector2.zero, 28);
            var classRT = classGO.GetComponent<RectTransform>();
            classRT.anchorMin = new Vector2(0, 0);
            classRT.anchorMax = new Vector2(0.35f, 1);
            classRT.offsetMin = new Vector2(15, 5);
            classRT.offsetMax = new Vector2(0, -5);
            var classTMP = classGO.GetComponent<TextMeshProUGUI>();
            classTMP.alignment = TextAlignmentOptions.Left;
            classTMP.fontStyle = FontStyles.Bold;
            classTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            var levelGO = CreateUIText(statsInfoPanel.transform, "LevelText", "Level 1",
                new Vector2(0.35f, 0), new Vector2(0.65f, 1),
                Vector2.zero, Vector2.zero, 26);
            var levelRT = levelGO.GetComponent<RectTransform>();
            levelRT.anchorMin = new Vector2(0.35f, 0);
            levelRT.anchorMax = new Vector2(0.65f, 1);
            levelRT.offsetMin = new Vector2(0, 5);
            levelRT.offsetMax = new Vector2(0, -5);
            var levelTMP = levelGO.GetComponent<TextMeshProUGUI>();
            levelTMP.alignment = TextAlignmentOptions.Center;

            var xpGO = CreateUIText(statsInfoPanel.transform, "XPText", "XP: 0 / 100",
                new Vector2(0.65f, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 20);
            var xpRT = xpGO.GetComponent<RectTransform>();
            xpRT.anchorMin = new Vector2(0.65f, 0);
            xpRT.anchorMax = new Vector2(1, 1);
            xpRT.offsetMin = new Vector2(0, 5);
            xpRT.offsetMax = new Vector2(-15, -5);
            var xpTMP = xpGO.GetComponent<TextMeshProUGUI>();
            xpTMP.alignment = TextAlignmentOptions.Right;

            // --- Combat Stats Panel (middle ~47%) ---
            var statsCombatPanel = CreateUIImage(statsContentGO.transform, "StatsCombatPanel",
                new Vector2(0, 0.35f), new Vector2(1, 0.82f),
                Vector2.zero, Vector2.zero,
                new Color(0.12f, 0.12f, 0.18f, 0.9f));
            var statsCombatPanelRT = statsCombatPanel.GetComponent<RectTransform>();
            statsCombatPanelRT.offsetMin = new Vector2(0, 5);
            statsCombatPanelRT.offsetMax = new Vector2(0, -5);

            var statsLabelGO = CreateUIText(statsCombatPanel.transform, "StatsLabel", "COMBAT STATS",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 30), 22);
            var statsLabelRT = statsLabelGO.GetComponent<RectTransform>();
            statsLabelRT.anchorMin = new Vector2(0, 1);
            statsLabelRT.anchorMax = new Vector2(1, 1);
            statsLabelRT.pivot = new Vector2(0.5f, 1);
            statsLabelRT.anchoredPosition = new Vector2(0, -5);
            statsLabelRT.sizeDelta = new Vector2(0, 30);
            var statsLabelTMP = statsLabelGO.GetComponent<TextMeshProUGUI>();
            statsLabelTMP.alignment = TextAlignmentOptions.Center;
            statsLabelTMP.fontStyle = FontStyles.Bold;
            statsLabelTMP.color = new Color(0.8f, 0.8f, 0.8f, 1f);

            var statsTextGO = CreateUIText(statsCombatPanel.transform, "StatsText",
                "HP: 0        MP: 0\nATK: 0       DEF: 0\nMATK: 0      MDEF: 0\nAGI: 0       AtkSpd: 1.00\nCrit: 0.0%   CritDmg: 150%",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 22);
            var statsTextRT = statsTextGO.GetComponent<RectTransform>();
            statsTextRT.anchorMin = new Vector2(0, 0);
            statsTextRT.anchorMax = new Vector2(1, 1);
            statsTextRT.offsetMin = new Vector2(20, 10);
            statsTextRT.offsetMax = new Vector2(-20, -38);
            var statsTextTMP = statsTextGO.GetComponent<TextMeshProUGUI>();
            statsTextTMP.alignment = TextAlignmentOptions.Center;

            // --- Allocation Panel (bottom ~35%) ---
            var statsAllocPanel = CreateUIImage(statsContentGO.transform, "StatsAllocPanel",
                new Vector2(0, 0), new Vector2(1, 0.35f),
                Vector2.zero, Vector2.zero,
                new Color(0.12f, 0.12f, 0.18f, 0.9f));
            var statsAllocPanelRT = statsAllocPanel.GetComponent<RectTransform>();
            statsAllocPanelRT.offsetMin = new Vector2(0, 0);
            statsAllocPanelRT.offsetMax = new Vector2(0, -5);

            var statPointsGO = CreateUIText(statsAllocPanel.transform, "StatPointsText", "Stat Points: 0",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 35), 24);
            var statPointsRT = statPointsGO.GetComponent<RectTransform>();
            statPointsRT.anchorMin = new Vector2(0, 1);
            statPointsRT.anchorMax = new Vector2(1, 1);
            statPointsRT.pivot = new Vector2(0.5f, 1);
            statPointsRT.anchoredPosition = new Vector2(0, -5);
            statPointsRT.sizeDelta = new Vector2(0, 35);
            var statPointsTMP = statPointsGO.GetComponent<TextMeshProUGUI>();
            statPointsTMP.alignment = TextAlignmentOptions.Center;
            statPointsTMP.fontStyle = FontStyles.Bold;
            statPointsTMP.color = new Color(0.3f, 0.9f, 0.3f, 1f);

            var allocatedGO = CreateUIText(statsAllocPanel.transform, "AllocatedText",
                "VIT: 0    MAN: 0    STR: 0\nAGI: 0    SPI: 0",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -42), new Vector2(0, 45), 20);
            var allocatedRT = allocatedGO.GetComponent<RectTransform>();
            allocatedRT.anchorMin = new Vector2(0, 1);
            allocatedRT.anchorMax = new Vector2(1, 1);
            allocatedRT.pivot = new Vector2(0.5f, 1);
            allocatedRT.anchoredPosition = new Vector2(0, -42);
            allocatedRT.sizeDelta = new Vector2(-20, 45);
            var allocatedTMP = allocatedGO.GetComponent<TextMeshProUGUI>();
            allocatedTMP.alignment = TextAlignmentOptions.Center;

            // Stat allocation buttons
            string[] statNames = { "VIT", "MAN", "STR", "AGI", "SPI" };
            var statButtons = new Button[5];
            float allocBtnWidth = 140f;
            float allocBtnHeight = 45f;
            float allocSpacing = 12f;
            float allocTotalWidth = (allocBtnWidth * 3) + (allocSpacing * 2);
            float allocStartX = -allocTotalWidth / 2f + allocBtnWidth / 2f;

            // Row 1: VIT, MAN, STR
            for (int i = 0; i < 3; i++)
            {
                var btnGO = new GameObject($"{statNames[i]}Button", typeof(RectTransform));
                btnGO.transform.SetParent(statsAllocPanel.transform, false);
                var btnRT = btnGO.GetComponent<RectTransform>();
                btnRT.anchorMin = new Vector2(0.5f, 0);
                btnRT.anchorMax = new Vector2(0.5f, 0);
                btnRT.pivot = new Vector2(0.5f, 0);
                btnRT.anchoredPosition = new Vector2(allocStartX + i * (allocBtnWidth + allocSpacing), 55);
                btnRT.sizeDelta = new Vector2(allocBtnWidth, allocBtnHeight);
                var btnImg = btnGO.AddComponent<Image>();
                btnImg.color = new Color(0.15f, 0.35f, 0.15f, 1f);
                var btn = btnGO.AddComponent<Button>();
                btn.targetGraphic = btnImg;
                statButtons[i] = btn;

                var btnTextGO = CreateUIText(btnGO.transform, "Text", $"+ {statNames[i]}",
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 20);
                btnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            }

            // Row 2: AGI, SPI
            float row2Width = (allocBtnWidth * 2) + allocSpacing;
            float row2StartX = -row2Width / 2f + allocBtnWidth / 2f;
            for (int i = 0; i < 2; i++)
            {
                var btnGO = new GameObject($"{statNames[3 + i]}Button", typeof(RectTransform));
                btnGO.transform.SetParent(statsAllocPanel.transform, false);
                var btnRT = btnGO.GetComponent<RectTransform>();
                btnRT.anchorMin = new Vector2(0.5f, 0);
                btnRT.anchorMax = new Vector2(0.5f, 0);
                btnRT.pivot = new Vector2(0.5f, 0);
                btnRT.anchoredPosition = new Vector2(row2StartX + i * (allocBtnWidth + allocSpacing), 5);
                btnRT.sizeDelta = new Vector2(allocBtnWidth, allocBtnHeight);
                var btnImg = btnGO.AddComponent<Image>();
                btnImg.color = new Color(0.15f, 0.35f, 0.15f, 1f);
                var btn = btnGO.AddComponent<Button>();
                btn.targetGraphic = btnImg;
                statButtons[3 + i] = btn;

                var btnTextGO = CreateUIText(btnGO.transform, "Text", $"+ {statNames[3 + i]}",
                    Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 20);
                btnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
            }

            statsContentGO.SetActive(false);

            // ============================================================
            // ITEM DETAIL PANEL (overlay within top-half container, hidden by default)
            // ============================================================

            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(topHalfGO.transform, false);
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
            uiSO.FindProperty("_xpText").objectReferenceValue = xpTMP;
            uiSO.FindProperty("_statsText").objectReferenceValue = statsTextTMP;
            uiSO.FindProperty("_statPointsText").objectReferenceValue = statPointsTMP;
            uiSO.FindProperty("_allocatedText").objectReferenceValue = allocatedTMP;
            uiSO.FindProperty("_vitalityButton").objectReferenceValue = statButtons[0];
            uiSO.FindProperty("_manaButton").objectReferenceValue = statButtons[1];
            uiSO.FindProperty("_strengthButton").objectReferenceValue = statButtons[2];
            uiSO.FindProperty("_agilityButton").objectReferenceValue = statButtons[3];
            uiSO.FindProperty("_spiritButton").objectReferenceValue = statButtons[4];

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
