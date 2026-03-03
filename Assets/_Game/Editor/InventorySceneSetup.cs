using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Inventory;

namespace ConquerChronicles.Editor
{
    public static class InventorySceneSetup
    {
        private const float SlotSize = 130f;
        private const float HeaderHeight = 60f;

        [MenuItem("Conquer Chronicles/Setup Inventory Scene")]
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

            // --- Canvas (1080x1920 portrait, bottom half only) ---
            var canvasGO = new GameObject("Inventory_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Constrain the entire canvas content to the bottom half.
            // The canvas RectTransform itself is managed by Unity for ScreenSpaceOverlay,
            // so we use a child container anchored to the bottom 50%.
            var bottomHalfGO = new GameObject("BottomHalfContainer", typeof(RectTransform));
            bottomHalfGO.transform.SetParent(canvasGO.transform, false);
            var bottomHalfRT = bottomHalfGO.GetComponent<RectTransform>();
            // Bottom edge sits above the HP orb (orb top ≈ 220px out of 1920 ≈ 0.115 anchor).
            bottomHalfRT.anchorMin = new Vector2(0, 0.18f); // clears HP orb on 4:3 and taller
            bottomHalfRT.anchorMax = new Vector2(1, 0.5f);
            bottomHalfRT.offsetMin = Vector2.zero;
            bottomHalfRT.offsetMax = Vector2.zero;
            var bottomHalfImg = bottomHalfGO.AddComponent<Image>();
            bottomHalfImg.color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

            // No SafeAreaHandler here — it overrides anchors to fill the safe area,
            // which would break our bottom-half constraint.

            // ============================================================
            // HEADER (inside bottom-half, anchored to top)
            // ============================================================

            var headerGO = new GameObject("Header", typeof(RectTransform));
            headerGO.transform.SetParent(bottomHalfGO.transform, false);
            var headerRT = headerGO.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0, 1);
            headerRT.anchorMax = new Vector2(1, 1);
            headerRT.pivot = new Vector2(0.5f, 1);
            headerRT.anchoredPosition = Vector2.zero;
            headerRT.sizeDelta = new Vector2(0, HeaderHeight);

            // Title text — centered in header
            var titleGO = CreateUIText(headerGO.transform, "TitleText", "INVENTORY",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, 32);
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = Vector2.zero;
            titleRT.anchorMax = Vector2.one;
            titleRT.offsetMin = Vector2.zero;
            titleRT.offsetMax = Vector2.zero;
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Close button (X) — right side of header
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(headerGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(1, 0);
            backBtnRT.anchorMax = new Vector2(1, 1);
            backBtnRT.pivot = new Vector2(1, 0.5f);
            backBtnRT.anchoredPosition = new Vector2(-10, 0);
            backBtnRT.sizeDelta = new Vector2(50, 0);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            backBtnImg.color = new Color(0.3f, 0.15f, 0.15f, 0.9f);
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;
            var backBtnTextGO = CreateUIText(backBtnGO.transform, "BackText", "X",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            var backBtnTMP = backBtnTextGO.GetComponent<TextMeshProUGUI>();
            backBtnTMP.alignment = TextAlignmentOptions.Center;
            backBtnTMP.fontStyle = FontStyles.Bold;

            // Gold text — right side of header, left of X button
            var goldGO = CreateUIText(headerGO.transform, "GoldText", "0 Gold",
                new Vector2(1, 0), new Vector2(1, 1),
                new Vector2(-65, 0), new Vector2(160, 0), 22);
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.anchorMin = new Vector2(1, 0);
            goldRT.anchorMax = new Vector2(1, 1);
            goldRT.pivot = new Vector2(1, 0.5f);
            goldRT.anchoredPosition = new Vector2(-65, 0);
            goldRT.sizeDelta = new Vector2(160, 0);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Right;
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Bag count text — centered below title, small sub-line
            var bagCountGO = CreateUIText(bottomHalfGO.transform, "BagCountText", "Bag: 0/50",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -HeaderHeight), new Vector2(0, 28), 20);
            var bagCountRT = bagCountGO.GetComponent<RectTransform>();
            bagCountRT.anchorMin = new Vector2(0, 1);
            bagCountRT.anchorMax = new Vector2(1, 1);
            bagCountRT.pivot = new Vector2(0.5f, 1);
            bagCountRT.anchoredPosition = new Vector2(0, -HeaderHeight);
            bagCountRT.sizeDelta = new Vector2(0, 28);
            var bagCountTMP = bagCountGO.GetComponent<TextMeshProUGUI>();
            bagCountTMP.alignment = TextAlignmentOptions.Center;
            bagCountTMP.fontStyle = FontStyles.Bold;

            // ============================================================
            // BAG GRID (vertical scrolling, 5 columns x 10 rows)
            // ============================================================

            var bagPanelGO = CreateUIImage(bottomHalfGO.transform, "BagPanel",
                new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero,
                new Color(0.1f, 0.1f, 0.15f, 0.9f));
            var bagPanelRT = bagPanelGO.GetComponent<RectTransform>();
            bagPanelRT.anchorMin = Vector2.zero;
            bagPanelRT.anchorMax = Vector2.one;
            bagPanelRT.pivot = new Vector2(0.5f, 0.5f);
            bagPanelRT.offsetMin = new Vector2(10, 8);
            bagPanelRT.offsetMax = new Vector2(-10, -(HeaderHeight + 28 + 4));

            var bagScrollGO = new GameObject("BagScroll", typeof(RectTransform));
            bagScrollGO.transform.SetParent(bagPanelGO.transform, false);
            var bagScrollRT = bagScrollGO.GetComponent<RectTransform>();
            bagScrollRT.anchorMin = Vector2.zero;
            bagScrollRT.anchorMax = Vector2.one;
            bagScrollRT.offsetMin = Vector2.zero;
            bagScrollRT.offsetMax = Vector2.zero;
            var bagScrollRect = bagScrollGO.AddComponent<ScrollRect>();
            bagScrollRect.horizontal = false;
            bagScrollRect.vertical = true;
            bagScrollGO.AddComponent<RectMask2D>();

            // Content anchored to top, grows downward
            var bagContentGO = new GameObject("BagContent", typeof(RectTransform));
            bagContentGO.transform.SetParent(bagScrollGO.transform, false);
            var bagContentRT = bagContentGO.GetComponent<RectTransform>();
            bagContentRT.anchorMin = new Vector2(0, 1);
            bagContentRT.anchorMax = new Vector2(1, 1);
            bagContentRT.pivot = new Vector2(0.5f, 1);
            bagContentRT.anchoredPosition = Vector2.zero;
            bagContentRT.sizeDelta = new Vector2(0, 0);
            var contentFitter = bagContentGO.AddComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var bagGridLayout = bagContentGO.AddComponent<GridLayoutGroup>();
            bagGridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
            bagGridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            bagGridLayout.constraintCount = 5;
            bagGridLayout.cellSize = new Vector2(SlotSize, SlotSize);
            bagGridLayout.spacing = new Vector2(8, 8);
            bagGridLayout.padding = new RectOffset(5, 5, 5, 5);
            bagGridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
            bagGridLayout.childAlignment = TextAnchor.UpperCenter;

            bagScrollRect.content = bagContentRT;

            // ============================================================
            // ITEM DETAIL PANEL (overlay, constrained within bottom-half)
            // ============================================================

            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(bottomHalfGO.transform, false);
            var detailPanelRT = detailPanelGO.GetComponent<RectTransform>();
            detailPanelRT.anchorMin = Vector2.zero;
            detailPanelRT.anchorMax = Vector2.one;
            detailPanelRT.offsetMin = Vector2.zero;
            detailPanelRT.offsetMax = Vector2.zero;
            detailPanelGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.7f);

            var detailInnerGO = new GameObject("DetailInner", typeof(RectTransform));
            detailInnerGO.transform.SetParent(detailPanelGO.transform, false);
            var detailInnerRT = detailInnerGO.GetComponent<RectTransform>();
            detailInnerRT.anchorMin = new Vector2(0.05f, 0.05f);
            detailInnerRT.anchorMax = new Vector2(0.95f, 0.95f);
            detailInnerRT.offsetMin = Vector2.zero;
            detailInnerRT.offsetMax = Vector2.zero;
            detailInnerGO.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.14f, 0.95f);

            // Item name
            var itemNameGO = CreateUIText(detailInnerGO.transform, "ItemNameText", "Item Name",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -10), new Vector2(0, 40), 28);
            var itemNameRT = itemNameGO.GetComponent<RectTransform>();
            itemNameRT.anchorMin = new Vector2(0, 1);
            itemNameRT.anchorMax = new Vector2(1, 1);
            itemNameRT.pivot = new Vector2(0.5f, 1);
            itemNameRT.anchoredPosition = new Vector2(0, -10);
            itemNameRT.sizeDelta = new Vector2(-20, 40);
            var itemNameTMP = itemNameGO.GetComponent<TextMeshProUGUI>();
            itemNameTMP.alignment = TextAlignmentOptions.Center;
            itemNameTMP.fontStyle = FontStyles.Bold;
            itemNameTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // Item stats
            var itemStatsGO = CreateUIText(detailInnerGO.transform, "ItemStatsText", "Stats...",
                new Vector2(0, 1), new Vector2(0.5f, 1),
                new Vector2(15, -55), new Vector2(0, 150), 18);
            var itemStatsRT = itemStatsGO.GetComponent<RectTransform>();
            itemStatsRT.anchorMin = new Vector2(0, 1);
            itemStatsRT.anchorMax = new Vector2(0.5f, 1);
            itemStatsRT.pivot = new Vector2(0, 1);
            itemStatsRT.anchoredPosition = new Vector2(15, -55);
            itemStatsRT.sizeDelta = new Vector2(0, 150);
            itemStatsGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Item info
            var itemInfoGO = CreateUIText(detailInnerGO.transform, "ItemInfoText", "Quality: Normal\nReq Lv: 1\nSockets: 0/0",
                new Vector2(0.5f, 1), new Vector2(1, 1),
                new Vector2(10, -55), new Vector2(-15, 150), 18);
            var itemInfoRT = itemInfoGO.GetComponent<RectTransform>();
            itemInfoRT.anchorMin = new Vector2(0.5f, 1);
            itemInfoRT.anchorMax = new Vector2(1, 1);
            itemInfoRT.pivot = new Vector2(0, 1);
            itemInfoRT.anchoredPosition = new Vector2(10, -55);
            itemInfoRT.sizeDelta = new Vector2(-15, 150);
            itemInfoGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Equip button
            var equipBtnGO = CreateButton(detailInnerGO.transform, "EquipButton",
                new Vector2(0, 70), new Vector2(0, 45), new Color(0.2f, 0.4f, 0.75f, 1f));
            var equipBtn = equipBtnGO.GetComponent<Button>();
            var equipBtnTextGO = CreateUIText(equipBtnGO.transform, "EquipText", "Equip",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            var equipBtnTMP = equipBtnTextGO.GetComponent<TextMeshProUGUI>();
            equipBtnTMP.alignment = TextAlignmentOptions.Center;
            equipBtnTMP.fontStyle = FontStyles.Bold;

            // Close button
            var closeBtnGO = CreateButton(detailInnerGO.transform, "CloseDetailButton",
                new Vector2(0, 12), new Vector2(0, 40), new Color(0.35f, 0.35f, 0.4f, 1f));
            closeBtnGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0);
            closeBtnGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0);
            var closeBtn = closeBtnGO.GetComponent<Button>();
            var closeBtnTextGO = CreateUIText(closeBtnGO.transform, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            closeBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            var inventorySceneUI = canvasGO.AddComponent<InventorySceneUI>();
            var uiSO = new SerializedObject(inventorySceneUI);

            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_goldText").objectReferenceValue = goldTMP;
            uiSO.FindProperty("_bagContainer").objectReferenceValue = bagContentGO.transform;
            uiSO.FindProperty("_bagCountText").objectReferenceValue = bagCountTMP;
            uiSO.FindProperty("_detailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_itemNameText").objectReferenceValue = itemNameTMP;
            uiSO.FindProperty("_itemStatsText").objectReferenceValue = itemStatsGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_itemInfoText").objectReferenceValue = itemInfoGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_equipButton").objectReferenceValue = equipBtn;
            uiSO.FindProperty("_equipButtonText").objectReferenceValue = equipBtnTMP;
            uiSO.FindProperty("_closeDetailButton").objectReferenceValue = closeBtn;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            var controllerGO = new GameObject("InventoryController");
            var controller = controllerGO.AddComponent<InventoryController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_inventoryUI").objectReferenceValue = inventorySceneUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Inventory scene setup complete (bottom-half, horizontal scroll)!");
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
