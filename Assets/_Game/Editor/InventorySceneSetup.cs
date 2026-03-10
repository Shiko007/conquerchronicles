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
            scaler.matchWidthOrHeight = 0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Full screen container
            var bottomHalfGO = new GameObject("ContentContainer", typeof(RectTransform));
            bottomHalfGO.transform.SetParent(canvasGO.transform, false);
            var bottomHalfRT = bottomHalfGO.GetComponent<RectTransform>();
            bottomHalfRT.anchorMin = new Vector2(0, 0.18f);
            bottomHalfRT.anchorMax = Vector2.one;
            bottomHalfRT.offsetMin = Vector2.zero;
            bottomHalfRT.offsetMax = new Vector2(0, -120); // safe area: clears dynamic island / notch
            var bottomHalfImg = bottomHalfGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(bottomHalfImg, new Color(0.85f, 0.85f, 0.9f, 0.92f));
            var bottomHalfContent = UIAtlasHelper.CreatePanelContent(bottomHalfGO.transform);

            // No SafeAreaHandler here — it overrides anchors to fill the safe area,
            // which would break our bottom-half constraint.

            // ============================================================
            // HEADER (inside bottom-half, anchored to top)
            // ============================================================

            var headerGO = new GameObject("Header", typeof(RectTransform));
            headerGO.transform.SetParent(bottomHalfContent, false);
            var headerRT = headerGO.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0, 1);
            headerRT.anchorMax = new Vector2(1, 1);
            headerRT.pivot = new Vector2(0.5f, 1);
            headerRT.anchoredPosition = Vector2.zero;
            headerRT.sizeDelta = new Vector2(0, HeaderHeight);

            var titleGO = CreateUIText(headerGO.transform, "TitleText", "",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 1);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();

            // Close button (X) — top-right edge of panel
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(bottomHalfGO.transform, false);
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

            // Gold counter — bottom-center of panel (icon + number)
            var goldBarGO = new GameObject("GoldBar", typeof(RectTransform));
            goldBarGO.transform.SetParent(bottomHalfContent, false);
            var goldBarRT = goldBarGO.GetComponent<RectTransform>();
            goldBarRT.anchorMin = new Vector2(0.5f, 0);
            goldBarRT.anchorMax = new Vector2(0.5f, 0);
            goldBarRT.pivot = new Vector2(0.5f, 0);
            goldBarRT.anchoredPosition = new Vector2(0, 4);
            goldBarRT.sizeDelta = new Vector2(160, 32);
            var goldBarLayout = goldBarGO.AddComponent<HorizontalLayoutGroup>();
            goldBarLayout.childAlignment = TextAnchor.MiddleCenter;
            goldBarLayout.spacing = 4;
            goldBarLayout.childForceExpandWidth = false;
            goldBarLayout.childForceExpandHeight = false;
            goldBarLayout.childControlWidth = false;
            goldBarLayout.childControlHeight = false;

            // Gold icon
            var goldIconGO = new GameObject("GoldIcon", typeof(RectTransform));
            goldIconGO.transform.SetParent(goldBarGO.transform, false);
            var goldIconRT = goldIconGO.GetComponent<RectTransform>();
            goldIconRT.sizeDelta = new Vector2(28, 28);
            var goldIconImg = goldIconGO.AddComponent<Image>();
            UIAtlasHelper.SetSimpleSprite(goldIconImg, "Gold");
            goldIconImg.raycastTarget = false;

            // Gold text (number only)
            var goldGO = CreateUIText(goldBarGO.transform, "GoldText", "0",
                Vector2.zero, Vector2.one, Vector2.zero, new Vector2(100, 28), 22);
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.sizeDelta = new Vector2(100, 28);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Left;
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f);

            // ============================================================
            // BAG GRID — anchor-based, no scroll, fills available area
            // ============================================================

            var bagContentGO = new GameObject("BagContent", typeof(RectTransform));
            bagContentGO.transform.SetParent(bottomHalfContent, false);
            var bagContentRT = bagContentGO.GetComponent<RectTransform>();
            bagContentRT.anchorMin = Vector2.zero;
            bagContentRT.anchorMax = Vector2.one;
            bagContentRT.offsetMin = new Vector2(0, 36); // room for gold bar at bottom
            bagContentRT.offsetMax = new Vector2(0, -HeaderHeight);

            // ============================================================
            // ITEM DETAIL PANEL (overlay, constrained within bottom-half)
            // ============================================================

            var detailPanelGO = new GameObject("DetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(bottomHalfContent, false);
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
            var detailInnerImg = detailInnerGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(detailInnerImg);
            var detailInnerContent = UIAtlasHelper.CreatePanelContent(detailInnerGO.transform);

            // Item name
            var itemNameGO = CreateUIText(detailInnerContent, "ItemNameText", "Item Name",
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

            // Item stats (full width, below name)
            var itemStatsGO = CreateUIText(detailInnerContent, "ItemStatsText", "Stats...",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(15, -55), new Vector2(-30, 80), 18);
            var itemStatsRT = itemStatsGO.GetComponent<RectTransform>();
            itemStatsRT.anchorMin = new Vector2(0, 1);
            itemStatsRT.anchorMax = new Vector2(1, 1);
            itemStatsRT.pivot = new Vector2(0, 1);
            itemStatsRT.anchoredPosition = new Vector2(15, -55);
            itemStatsRT.sizeDelta = new Vector2(-30, 80);
            itemStatsGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Item info (full width, below stats)
            var itemInfoGO = CreateUIText(detailInnerContent, "ItemInfoText", "Quality: Normal\nReq Lv: 1\nSockets: 0/0",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(15, -140), new Vector2(-30, 70), 18);
            var itemInfoRT = itemInfoGO.GetComponent<RectTransform>();
            itemInfoRT.anchorMin = new Vector2(0, 1);
            itemInfoRT.anchorMax = new Vector2(1, 1);
            itemInfoRT.pivot = new Vector2(0, 1);
            itemInfoRT.anchoredPosition = new Vector2(15, -140);
            itemInfoRT.sizeDelta = new Vector2(-30, 70);
            itemInfoGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.TopLeft;

            // Equip button (shifted up to make room for Drop)
            var equipBtnGO = CreateButton(detailInnerContent, "EquipButton",
                new Vector2(0, 120), new Vector2(0, 45), Color.white);
            var equipBtn = equipBtnGO.GetComponent<Button>();
            UIAtlasHelper.SetSpriteSwapButton(equipBtn, equipBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var equipBtnContent = UIAtlasHelper.CreateButtonContent(equipBtnGO.transform, 45f);
            var equipBtnTextGO = CreateUIText(equipBtnContent, "EquipText", "Equip",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            var equipBtnTMP = equipBtnTextGO.GetComponent<TextMeshProUGUI>();
            equipBtnTMP.alignment = TextAlignmentOptions.Center;
            equipBtnTMP.fontStyle = FontStyles.Bold;

            // Drop button
            var dropBtnGO = CreateButton(detailInnerContent, "DropButton",
                new Vector2(0, 65), new Vector2(0, 45), Color.white);
            var dropBtn = dropBtnGO.GetComponent<Button>();
            UIAtlasHelper.SetSpriteSwapButton(dropBtn, dropBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var dropBtnContent = UIAtlasHelper.CreateButtonContent(dropBtnGO.transform, 45f);
            var dropBtnTextGO = CreateUIText(dropBtnContent, "DropText", "Drop",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 24);
            var dropBtnTMP = dropBtnTextGO.GetComponent<TextMeshProUGUI>();
            dropBtnTMP.alignment = TextAlignmentOptions.Center;
            dropBtnTMP.fontStyle = FontStyles.Bold;
            dropBtnTMP.color = new Color(1f, 0.3f, 0.3f, 1f);

            // Close button
            var closeBtnGO = CreateButton(detailInnerContent, "CloseDetailButton",
                new Vector2(0, 12), new Vector2(0, 40), Color.white);
            closeBtnGO.GetComponent<RectTransform>().anchorMin = new Vector2(0.2f, 0);
            closeBtnGO.GetComponent<RectTransform>().anchorMax = new Vector2(0.8f, 0);
            var closeBtn = closeBtnGO.GetComponent<Button>();
            UIAtlasHelper.SetSpriteSwapButton(closeBtnGO.GetComponent<Button>(), closeBtnGO.GetComponent<Image>(), "Button_Unpressed", "Button_Pressed");
            var closeBtnContent = UIAtlasHelper.CreateButtonContent(closeBtnGO.transform, 40f);
            var closeBtnTextGO = CreateUIText(closeBtnContent, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            closeBtnTextGO.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // ============================================================
            // SELECTION BAR (top of panel, hidden by default)
            // ============================================================

            var selectionBarGO = new GameObject("SelectionBar", typeof(RectTransform));
            selectionBarGO.transform.SetParent(bottomHalfContent, false);
            var selectionBarRT = selectionBarGO.GetComponent<RectTransform>();
            selectionBarRT.anchorMin = new Vector2(0, 1);
            selectionBarRT.anchorMax = new Vector2(1, 1);
            selectionBarRT.pivot = new Vector2(0.5f, 1);
            selectionBarRT.anchoredPosition = Vector2.zero;
            selectionBarRT.sizeDelta = new Vector2(0, HeaderHeight);

            var selBarLayout = selectionBarGO.AddComponent<HorizontalLayoutGroup>();
            selBarLayout.childAlignment = TextAnchor.MiddleCenter;
            selBarLayout.spacing = 10;
            selBarLayout.padding = new RectOffset(10, 10, 5, 5);
            selBarLayout.childForceExpandWidth = true;
            selBarLayout.childForceExpandHeight = true;
            selBarLayout.childControlWidth = true;
            selBarLayout.childControlHeight = true;

            // Drop Selected button
            var dropSelBtnGO = new GameObject("DropSelectedButton", typeof(RectTransform));
            dropSelBtnGO.transform.SetParent(selectionBarGO.transform, false);
            var dropSelBtnImg = dropSelBtnGO.AddComponent<Image>();
            var dropSelBtn = dropSelBtnGO.AddComponent<Button>();
            dropSelBtn.targetGraphic = dropSelBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(dropSelBtn, dropSelBtnImg, "Button_Unpressed", "Button_Pressed");
            var dropSelBtnContent = UIAtlasHelper.CreateButtonContent(dropSelBtnGO.transform, 50f);
            var dropSelTextGO = CreateUIText(dropSelBtnContent, "DropSelectedText", "Drop Selected (0)",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var dropSelTMP = dropSelTextGO.GetComponent<TextMeshProUGUI>();
            dropSelTMP.alignment = TextAlignmentOptions.Center;
            dropSelTMP.color = new Color(1f, 0.3f, 0.3f, 1f);
            dropSelTMP.fontStyle = FontStyles.Bold;

            // Cancel button
            var cancelSelBtnGO = new GameObject("CancelSelectButton", typeof(RectTransform));
            cancelSelBtnGO.transform.SetParent(selectionBarGO.transform, false);
            var cancelSelBtnImg = cancelSelBtnGO.AddComponent<Image>();
            var cancelSelBtn = cancelSelBtnGO.AddComponent<Button>();
            cancelSelBtn.targetGraphic = cancelSelBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(cancelSelBtn, cancelSelBtnImg, "Button_Unpressed", "Button_Pressed");
            var cancelSelBtnContent = UIAtlasHelper.CreateButtonContent(cancelSelBtnGO.transform, 50f);
            var cancelSelTextGO = CreateUIText(cancelSelBtnContent, "CancelText", "Cancel",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var cancelSelTMP = cancelSelTextGO.GetComponent<TextMeshProUGUI>();
            cancelSelTMP.alignment = TextAlignmentOptions.Center;

            selectionBarGO.SetActive(false);

            // ============================================================
            // CONFIRM DIALOG (overlay, higher z-order than detail panel)
            // ============================================================

            var confirmDialogGO = new GameObject("ConfirmDialog", typeof(RectTransform));
            confirmDialogGO.transform.SetParent(bottomHalfContent, false);
            var confirmDialogRT = confirmDialogGO.GetComponent<RectTransform>();
            confirmDialogRT.anchorMin = Vector2.zero;
            confirmDialogRT.anchorMax = Vector2.one;
            confirmDialogRT.offsetMin = Vector2.zero;
            confirmDialogRT.offsetMax = Vector2.zero;
            confirmDialogGO.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

            var confirmInnerGO = new GameObject("ConfirmInner", typeof(RectTransform));
            confirmInnerGO.transform.SetParent(confirmDialogGO.transform, false);
            var confirmInnerRT = confirmInnerGO.GetComponent<RectTransform>();
            confirmInnerRT.anchorMin = new Vector2(0.1f, 0.35f);
            confirmInnerRT.anchorMax = new Vector2(0.9f, 0.65f);
            confirmInnerRT.offsetMin = Vector2.zero;
            confirmInnerRT.offsetMax = Vector2.zero;
            var confirmInnerImg = confirmInnerGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(confirmInnerImg);
            var confirmInnerContent = UIAtlasHelper.CreatePanelContent(confirmInnerGO.transform);

            // Confirm message text
            var confirmTextGO = CreateUIText(confirmInnerContent, "ConfirmText", "Are you sure?",
                new Vector2(0, 0.5f), new Vector2(1, 1),
                new Vector2(0, -10), new Vector2(-20, 0), 22);
            var confirmTextRT = confirmTextGO.GetComponent<RectTransform>();
            confirmTextRT.anchorMin = new Vector2(0, 0.5f);
            confirmTextRT.anchorMax = new Vector2(1, 1);
            confirmTextRT.pivot = new Vector2(0.5f, 1);
            confirmTextRT.anchoredPosition = new Vector2(0, -10);
            confirmTextRT.sizeDelta = new Vector2(-20, 0);
            var confirmTMP = confirmTextGO.GetComponent<TextMeshProUGUI>();
            confirmTMP.alignment = TextAlignmentOptions.Center;
            confirmTMP.fontStyle = FontStyles.Bold;

            // Button container (horizontal layout)
            var confirmBtnBar = new GameObject("ConfirmButtons", typeof(RectTransform));
            confirmBtnBar.transform.SetParent(confirmInnerContent, false);
            var confirmBtnBarRT = confirmBtnBar.GetComponent<RectTransform>();
            confirmBtnBarRT.anchorMin = new Vector2(0, 0);
            confirmBtnBarRT.anchorMax = new Vector2(1, 0.5f);
            confirmBtnBarRT.offsetMin = new Vector2(10, 10);
            confirmBtnBarRT.offsetMax = new Vector2(-10, -5);
            var confirmBtnLayout = confirmBtnBar.AddComponent<HorizontalLayoutGroup>();
            confirmBtnLayout.childAlignment = TextAnchor.MiddleCenter;
            confirmBtnLayout.spacing = 20;
            confirmBtnLayout.childForceExpandWidth = true;
            confirmBtnLayout.childForceExpandHeight = true;
            confirmBtnLayout.childControlWidth = true;
            confirmBtnLayout.childControlHeight = true;

            // Yes button
            var confirmYesBtnGO = new GameObject("ConfirmYesButton", typeof(RectTransform));
            confirmYesBtnGO.transform.SetParent(confirmBtnBar.transform, false);
            var confirmYesBtnImg = confirmYesBtnGO.AddComponent<Image>();
            var confirmYesBtn = confirmYesBtnGO.AddComponent<Button>();
            confirmYesBtn.targetGraphic = confirmYesBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(confirmYesBtn, confirmYesBtnImg, "Button_Unpressed", "Button_Pressed");
            var confirmYesBtnContent = UIAtlasHelper.CreateButtonContent(confirmYesBtnGO.transform, 50f);
            var confirmYesTextGO = CreateUIText(confirmYesBtnContent, "YesText", "Yes",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var confirmYesTMP = confirmYesTextGO.GetComponent<TextMeshProUGUI>();
            confirmYesTMP.alignment = TextAlignmentOptions.Center;
            confirmYesTMP.fontStyle = FontStyles.Bold;
            confirmYesTMP.color = new Color(1f, 0.3f, 0.3f, 1f);

            // No button
            var confirmNoBtnGO = new GameObject("ConfirmNoButton", typeof(RectTransform));
            confirmNoBtnGO.transform.SetParent(confirmBtnBar.transform, false);
            var confirmNoBtnImg = confirmNoBtnGO.AddComponent<Image>();
            var confirmNoBtn = confirmNoBtnGO.AddComponent<Button>();
            confirmNoBtn.targetGraphic = confirmNoBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(confirmNoBtn, confirmNoBtnImg, "Button_Unpressed", "Button_Pressed");
            var confirmNoBtnContent = UIAtlasHelper.CreateButtonContent(confirmNoBtnGO.transform, 50f);
            var confirmNoTextGO = CreateUIText(confirmNoBtnContent, "NoText", "No",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 22);
            var confirmNoTMP = confirmNoTextGO.GetComponent<TextMeshProUGUI>();
            confirmNoTMP.alignment = TextAlignmentOptions.Center;
            confirmNoTMP.fontStyle = FontStyles.Bold;

            confirmDialogGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            var inventorySceneUI = canvasGO.AddComponent<InventorySceneUI>();
            var uiSO = new SerializedObject(inventorySceneUI);

            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_goldText").objectReferenceValue = goldTMP;
            uiSO.FindProperty("_bagContainer").objectReferenceValue = bagContentGO.transform;
            uiSO.FindProperty("_detailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_itemNameText").objectReferenceValue = itemNameTMP;
            uiSO.FindProperty("_itemStatsText").objectReferenceValue = itemStatsGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_itemInfoText").objectReferenceValue = itemInfoGO.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("_equipButton").objectReferenceValue = equipBtn;
            uiSO.FindProperty("_equipButtonText").objectReferenceValue = equipBtnTMP;
            uiSO.FindProperty("_dropButton").objectReferenceValue = dropBtn;
            uiSO.FindProperty("_dropButtonText").objectReferenceValue = dropBtnTMP;
            uiSO.FindProperty("_closeDetailButton").objectReferenceValue = closeBtn;

            // Selection bar
            uiSO.FindProperty("_selectionBar").objectReferenceValue = selectionBarGO;
            uiSO.FindProperty("_dropSelectedButton").objectReferenceValue = dropSelBtn;
            uiSO.FindProperty("_dropSelectedText").objectReferenceValue = dropSelTMP;
            uiSO.FindProperty("_cancelSelectButton").objectReferenceValue = cancelSelBtn;

            // Confirm dialog
            uiSO.FindProperty("_confirmDialog").objectReferenceValue = confirmDialogGO;
            uiSO.FindProperty("_confirmText").objectReferenceValue = confirmTMP;
            uiSO.FindProperty("_confirmYesButton").objectReferenceValue = confirmYesBtn;
            uiSO.FindProperty("_confirmNoButton").objectReferenceValue = confirmNoBtn;

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
