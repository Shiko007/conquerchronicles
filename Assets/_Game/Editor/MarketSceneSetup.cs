using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Market;

namespace ConquerChronicles.Editor
{
    public static class MarketSceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Market Scene")]
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
            var canvasGO = new GameObject("Market_Canvas");
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

            // Gold display — centered below tab bar, above listings
            var goldGO = CreateUIText(safeAreaContent, "GoldText", "0 Gold",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -175), new Vector2(0, 40), 28);
            var goldRT = goldGO.GetComponent<RectTransform>();
            goldRT.anchorMin = new Vector2(0, 1);
            goldRT.anchorMax = new Vector2(1, 1);
            goldRT.pivot = new Vector2(0.5f, 1);
            goldRT.anchoredPosition = new Vector2(0, -175);
            goldRT.sizeDelta = new Vector2(0, 40);
            var goldTMP = goldGO.GetComponent<TextMeshProUGUI>();
            goldTMP.alignment = TextAlignmentOptions.Center;
            goldTMP.color = new Color(1f, 0.85f, 0.2f, 1f); // gold

            // ============================================================
            // TAB BAR (below header)
            // ============================================================

            var tabBarGO = new GameObject("TabBar", typeof(RectTransform));
            tabBarGO.transform.SetParent(safeAreaContent, false);
            var tabBarRT = tabBarGO.GetComponent<RectTransform>();
            tabBarRT.anchorMin = new Vector2(0, 1);
            tabBarRT.anchorMax = new Vector2(1, 1);
            tabBarRT.pivot = new Vector2(0.5f, 1);
            tabBarRT.anchoredPosition = new Vector2(0, -100);
            tabBarRT.sizeDelta = new Vector2(-40, 70); // 20px padding each side

            // Buy tab button (left half)
            var buyTabGO = new GameObject("BuyTabButton", typeof(RectTransform));
            buyTabGO.transform.SetParent(tabBarGO.transform, false);
            var buyTabRT = buyTabGO.GetComponent<RectTransform>();
            buyTabRT.anchorMin = new Vector2(0, 0);
            buyTabRT.anchorMax = new Vector2(0.48f, 1);
            buyTabRT.offsetMin = Vector2.zero;
            buyTabRT.offsetMax = Vector2.zero;
            var buyTabImg = buyTabGO.AddComponent<Image>();
            var buyTabBtn = buyTabGO.AddComponent<Button>();
            buyTabBtn.targetGraphic = buyTabImg;
            UIAtlasHelper.SetSpriteSwapButton(buyTabBtn, buyTabImg, "Button_Unpressed", "Button_Pressed");
            var buyTabBtnContent = UIAtlasHelper.CreateButtonContent(buyTabGO.transform, 70f);

            var buyTabTextGO = CreateUIText(buyTabBtnContent, "BuyTabText", "Buy",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 30);
            var buyTabTMP = buyTabTextGO.GetComponent<TextMeshProUGUI>();
            buyTabTMP.alignment = TextAlignmentOptions.Center;
            buyTabTMP.fontStyle = FontStyles.Bold;

            // Booth tab button (right half)
            var boothTabGO = new GameObject("BoothTabButton", typeof(RectTransform));
            boothTabGO.transform.SetParent(tabBarGO.transform, false);
            var boothTabRT = boothTabGO.GetComponent<RectTransform>();
            boothTabRT.anchorMin = new Vector2(0.52f, 0);
            boothTabRT.anchorMax = new Vector2(1, 1);
            boothTabRT.offsetMin = Vector2.zero;
            boothTabRT.offsetMax = Vector2.zero;
            var boothTabImg = boothTabGO.AddComponent<Image>();
            var boothTabBtn = boothTabGO.AddComponent<Button>();
            boothTabBtn.targetGraphic = boothTabImg;
            UIAtlasHelper.SetSpriteSwapButton(boothTabBtn, boothTabImg, "Button_Unpressed", "Button_Pressed");
            var boothTabBtnContent = UIAtlasHelper.CreateButtonContent(boothTabGO.transform, 70f);

            var boothTabTextGO = CreateUIText(boothTabBtnContent, "BoothTabText", "My Booth",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 30);
            var boothTabTMP = boothTabTextGO.GetComponent<TextMeshProUGUI>();
            boothTabTMP.alignment = TextAlignmentOptions.Center;
            boothTabTMP.fontStyle = FontStyles.Bold;

            // ============================================================
            // BUY PANEL (main area, visible by default)
            // ============================================================

            var buyPanelGO = new GameObject("BuyPanel", typeof(RectTransform));
            buyPanelGO.transform.SetParent(safeAreaContent, false);
            var buyPanelRT = buyPanelGO.GetComponent<RectTransform>();
            buyPanelRT.anchorMin = new Vector2(0, 0);
            buyPanelRT.anchorMax = new Vector2(1, 1);
            buyPanelRT.pivot = new Vector2(0.5f, 0.5f);
            buyPanelRT.offsetMin = new Vector2(0, 0);
            buyPanelRT.offsetMax = new Vector2(0, -220); // below gold display

            // Refresh timer text at top of buy panel
            var refreshTimerGO = CreateUIText(buyPanelGO.transform, "RefreshTimerText", "Refresh in: 3:59:42",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -5), new Vector2(0, 40), 22);
            var refreshTimerRT = refreshTimerGO.GetComponent<RectTransform>();
            refreshTimerRT.anchorMin = new Vector2(0, 1);
            refreshTimerRT.anchorMax = new Vector2(1, 1);
            refreshTimerRT.pivot = new Vector2(0.5f, 1);
            refreshTimerRT.anchoredPosition = new Vector2(0, -5);
            refreshTimerRT.sizeDelta = new Vector2(0, 40);
            var refreshTimerTMP = refreshTimerGO.GetComponent<TextMeshProUGUI>();
            refreshTimerTMP.alignment = TextAlignmentOptions.Center;
            refreshTimerTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // Scroll panel (handles horizontal inset)
            var scrollPanelGO = new GameObject("ListingsPanel", typeof(RectTransform));
            scrollPanelGO.transform.SetParent(buyPanelGO.transform, false);
            var scrollPanelRT = scrollPanelGO.GetComponent<RectTransform>();
            scrollPanelRT.anchorMin = new Vector2(0, 0);
            scrollPanelRT.anchorMax = new Vector2(1, 1);
            scrollPanelRT.offsetMin = new Vector2(20, 20);
            scrollPanelRT.offsetMax = new Vector2(-20, -50); // below refresh timer

            // Scroll View (fills panel, no offset — keeps RectMask2D bounds correct)
            var scrollGO = new GameObject("ListingsScroll", typeof(RectTransform));
            scrollGO.transform.SetParent(scrollPanelGO.transform, false);
            var scrollRT = scrollGO.GetComponent<RectTransform>();
            scrollRT.anchorMin = Vector2.zero;
            scrollRT.anchorMax = Vector2.one;
            scrollRT.offsetMin = Vector2.zero;
            scrollRT.offsetMax = Vector2.zero;
            var scrollRect = scrollGO.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollGO.AddComponent<RectMask2D>();

            // Content container for listings
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
            vertLayout.spacing = 8f;
            vertLayout.padding = new RectOffset(10, 10, 10, 10);
            vertLayout.childForceExpandWidth = true;
            vertLayout.childForceExpandHeight = false;
            vertLayout.childControlWidth = true;
            vertLayout.childControlHeight = false;

            scrollRect.content = contentRT;

            // Create 12 listing entries (Button+Image parent with child TMP text)
            for (int i = 0; i < 12; i++)
            {
                var listingGO = new GameObject($"Listing_{i}", typeof(RectTransform));
                listingGO.transform.SetParent(contentGO.transform, false);
                var listingRT = listingGO.GetComponent<RectTransform>();
                listingRT.sizeDelta = new Vector2(0, 60);
                var layoutElement = listingGO.AddComponent<LayoutElement>();
                layoutElement.preferredHeight = 60;
                layoutElement.flexibleWidth = 1;

                // Add Image (transparent) as raycast target + Button for click
                var listingImg = listingGO.AddComponent<Image>();
                listingImg.color = new Color(0, 0, 0, 0);
                var listingBtn = listingGO.AddComponent<Button>();
                listingBtn.targetGraphic = listingImg;
                var btnColors = listingBtn.colors;
                btnColors.highlightedColor = new Color(1, 1, 1, 0.1f);
                btnColors.pressedColor = new Color(1, 1, 1, 0.2f);
                listingBtn.colors = btnColors;

                // Child text object
                var textGO = new GameObject("Text", typeof(RectTransform));
                textGO.transform.SetParent(listingGO.transform, false);
                var textRT = textGO.GetComponent<RectTransform>();
                textRT.anchorMin = Vector2.zero;
                textRT.anchorMax = Vector2.one;
                textRT.offsetMin = new Vector2(10, 0);
                textRT.offsetMax = new Vector2(-10, 0);

                var listingTMP = textGO.AddComponent<TextMeshProUGUI>();
                listingTMP.text = $"[Equip] Item {i + 1}  --  0 Gold";
                listingTMP.fontSize = 24;
                listingTMP.color = Color.white;
                listingTMP.alignment = TextAlignmentOptions.Left;
                listingTMP.verticalAlignment = VerticalAlignmentOptions.Middle;
                listingTMP.raycastTarget = false;
                listingTMP.margin = new Vector4(40, 0, 0, 0); // left margin to prevent RectMask2D clipping + visual padding
            }

            // ============================================================
            // LISTING DETAIL PANEL (center overlay, hidden by default)
            // ============================================================

            // Backdrop (full-screen semi-transparent)
            var detailPanelGO = new GameObject("ListingDetailPanel", typeof(RectTransform));
            detailPanelGO.transform.SetParent(safeAreaContent, false);
            var detailPanelRT = detailPanelGO.GetComponent<RectTransform>();
            detailPanelRT.anchorMin = Vector2.zero;
            detailPanelRT.anchorMax = Vector2.one;
            detailPanelRT.offsetMin = Vector2.zero;
            detailPanelRT.offsetMax = Vector2.zero;
            var detailBgImg = detailPanelGO.AddComponent<Image>();
            detailBgImg.color = new Color(0f, 0f, 0f, 0.7f);

            // Inner panel
            var detailInnerGO = new GameObject("DetailInner", typeof(RectTransform));
            detailInnerGO.transform.SetParent(detailPanelGO.transform, false);
            var detailInnerRT = detailInnerGO.GetComponent<RectTransform>();
            detailInnerRT.anchorMin = new Vector2(0.1f, 0.3f);
            detailInnerRT.anchorMax = new Vector2(0.9f, 0.7f);
            detailInnerRT.offsetMin = Vector2.zero;
            detailInnerRT.offsetMax = Vector2.zero;
            var detailInnerImg = detailInnerGO.AddComponent<Image>();
            UIAtlasHelper.SetSlicedPanel(detailInnerImg);
            var detailInnerContent = UIAtlasHelper.CreatePanelContent(detailInnerGO.transform);

            // Listing name (bold, large)
            var listingNameGO = CreateUIText(detailInnerContent, "ListingNameText", "Item Name",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -15), new Vector2(0, 60), 36);
            var listingNameRT = listingNameGO.GetComponent<RectTransform>();
            listingNameRT.anchorMin = new Vector2(0, 1);
            listingNameRT.anchorMax = new Vector2(1, 1);
            listingNameRT.pivot = new Vector2(0.5f, 1);
            listingNameRT.anchoredPosition = new Vector2(0, -15);
            listingNameRT.sizeDelta = new Vector2(-40, 60);
            var listingNameTMP = listingNameGO.GetComponent<TextMeshProUGUI>();
            listingNameTMP.alignment = TextAlignmentOptions.Center;
            listingNameTMP.fontStyle = FontStyles.Bold;

            // Type text
            var listingTypeGO = CreateUIText(detailInnerContent, "ListingTypeText", "Equipment",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -85), new Vector2(0, 40), 26);
            var listingTypeRT = listingTypeGO.GetComponent<RectTransform>();
            listingTypeRT.anchorMin = new Vector2(0, 1);
            listingTypeRT.anchorMax = new Vector2(1, 1);
            listingTypeRT.pivot = new Vector2(0.5f, 1);
            listingTypeRT.anchoredPosition = new Vector2(0, -85);
            listingTypeRT.sizeDelta = new Vector2(-40, 40);
            var listingTypeTMP = listingTypeGO.GetComponent<TextMeshProUGUI>();
            listingTypeTMP.alignment = TextAlignmentOptions.Center;
            listingTypeTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            // Price text (gold colored)
            var listingPriceGO = CreateUIText(detailInnerContent, "ListingPriceText", "0 Gold",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -135), new Vector2(0, 50), 32);
            var listingPriceRT = listingPriceGO.GetComponent<RectTransform>();
            listingPriceRT.anchorMin = new Vector2(0, 1);
            listingPriceRT.anchorMax = new Vector2(1, 1);
            listingPriceRT.pivot = new Vector2(0.5f, 1);
            listingPriceRT.anchoredPosition = new Vector2(0, -135);
            listingPriceRT.sizeDelta = new Vector2(-40, 50);
            var listingPriceTMP = listingPriceGO.GetComponent<TextMeshProUGUI>();
            listingPriceTMP.alignment = TextAlignmentOptions.Center;
            listingPriceTMP.color = new Color(1f, 0.85f, 0.2f, 1f); // gold

            // Buy button (green)
            var buyBtnGO = new GameObject("BuyButton", typeof(RectTransform));
            buyBtnGO.transform.SetParent(detailInnerContent, false);
            var buyBtnRT = buyBtnGO.GetComponent<RectTransform>();
            buyBtnRT.anchorMin = new Vector2(0.15f, 0);
            buyBtnRT.anchorMax = new Vector2(0.55f, 0);
            buyBtnRT.pivot = new Vector2(0.5f, 0);
            buyBtnRT.anchoredPosition = new Vector2(0, 20);
            buyBtnRT.sizeDelta = new Vector2(0, 60);
            var buyBtnImg = buyBtnGO.AddComponent<Image>();
            var buyButton = buyBtnGO.AddComponent<Button>();
            buyButton.targetGraphic = buyBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(buyButton, buyBtnImg, "Button_Unpressed", "Button_Pressed");
            var buyBtnContent = UIAtlasHelper.CreateButtonContent(buyBtnGO.transform, 60f);

            var buyBtnTextGO = CreateUIText(buyBtnContent, "BuyButtonText", "Buy",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var buyBtnTMP = buyBtnTextGO.GetComponent<TextMeshProUGUI>();
            buyBtnTMP.alignment = TextAlignmentOptions.Center;
            buyBtnTMP.color = Color.white;

            // Close button (gray)
            var closeBtnGO = new GameObject("CloseListingButton", typeof(RectTransform));
            closeBtnGO.transform.SetParent(detailInnerContent, false);
            var closeBtnRT = closeBtnGO.GetComponent<RectTransform>();
            closeBtnRT.anchorMin = new Vector2(0.55f, 0);
            closeBtnRT.anchorMax = new Vector2(0.85f, 0);
            closeBtnRT.pivot = new Vector2(0.5f, 0);
            closeBtnRT.anchoredPosition = new Vector2(0, 20);
            closeBtnRT.sizeDelta = new Vector2(0, 60);
            var closeBtnImg = closeBtnGO.AddComponent<Image>();
            var closeListingBtn = closeBtnGO.AddComponent<Button>();
            closeListingBtn.targetGraphic = closeBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(closeListingBtn, closeBtnImg, "Button_Unpressed", "Button_Pressed");
            var closeBtnContent = UIAtlasHelper.CreateButtonContent(closeBtnGO.transform, 60f);

            var closeBtnTextGO = CreateUIText(closeBtnContent, "CloseText", "Close",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 28);
            var closeBtnTMP = closeBtnTextGO.GetComponent<TextMeshProUGUI>();
            closeBtnTMP.alignment = TextAlignmentOptions.Center;

            detailPanelGO.SetActive(false);

            // ============================================================
            // BOOTH PANEL (same area as buy panel, hidden by default)
            // ============================================================

            var boothPanelGO = new GameObject("BoothPanel", typeof(RectTransform));
            boothPanelGO.transform.SetParent(safeAreaContent, false);
            var boothPanelRT = boothPanelGO.GetComponent<RectTransform>();
            boothPanelRT.anchorMin = new Vector2(0, 0);
            boothPanelRT.anchorMax = new Vector2(1, 1);
            boothPanelRT.pivot = new Vector2(0.5f, 0.5f);
            boothPanelRT.offsetMin = new Vector2(20, 20);
            boothPanelRT.offsetMax = new Vector2(-20, -220); // below gold display

            // Revenue text
            var boothRevenueGO = CreateUIText(boothPanelGO.transform, "BoothRevenueText", "Uncollected Revenue: 0 Gold",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -10), new Vector2(0, 50), 28);
            var boothRevenueRT = boothRevenueGO.GetComponent<RectTransform>();
            boothRevenueRT.anchorMin = new Vector2(0, 1);
            boothRevenueRT.anchorMax = new Vector2(1, 1);
            boothRevenueRT.pivot = new Vector2(0.5f, 1);
            boothRevenueRT.anchoredPosition = new Vector2(0, -10);
            boothRevenueRT.sizeDelta = new Vector2(0, 50);
            var boothRevenueTMP = boothRevenueGO.GetComponent<TextMeshProUGUI>();
            boothRevenueTMP.alignment = TextAlignmentOptions.Center;
            boothRevenueTMP.color = new Color(1f, 0.85f, 0.2f, 1f); // gold

            // Collect Revenue button (gold color)
            var collectBtnGO = new GameObject("CollectRevenueButton", typeof(RectTransform));
            collectBtnGO.transform.SetParent(boothPanelGO.transform, false);
            var collectBtnRT = collectBtnGO.GetComponent<RectTransform>();
            collectBtnRT.anchorMin = new Vector2(0.2f, 1);
            collectBtnRT.anchorMax = new Vector2(0.8f, 1);
            collectBtnRT.pivot = new Vector2(0.5f, 1);
            collectBtnRT.anchoredPosition = new Vector2(0, -70);
            collectBtnRT.sizeDelta = new Vector2(0, 60);
            var collectBtnImg = collectBtnGO.AddComponent<Image>();
            var collectRevenueBtn = collectBtnGO.AddComponent<Button>();
            collectRevenueBtn.targetGraphic = collectBtnImg;
            UIAtlasHelper.SetSpriteSwapButton(collectRevenueBtn, collectBtnImg, "Button_Unpressed", "Button_Pressed");
            var collectBtnContent = UIAtlasHelper.CreateButtonContent(collectBtnGO.transform, 60f);

            var collectBtnTextGO = CreateUIText(collectBtnContent, "CollectText", "Collect Revenue",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 26);
            var collectBtnTMP = collectBtnTextGO.GetComponent<TextMeshProUGUI>();
            collectBtnTMP.alignment = TextAlignmentOptions.Center;
            collectBtnTMP.color = Color.black;

            // Listed items text (shows what player has listed)
            var boothItemsGO = CreateUIText(boothPanelGO.transform, "BoothItemsText", "No items listed.",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -150), new Vector2(0, 400), 22);
            var boothItemsRT = boothItemsGO.GetComponent<RectTransform>();
            boothItemsRT.anchorMin = new Vector2(0, 1);
            boothItemsRT.anchorMax = new Vector2(1, 1);
            boothItemsRT.pivot = new Vector2(0.5f, 1);
            boothItemsRT.anchoredPosition = new Vector2(0, -150);
            boothItemsRT.sizeDelta = new Vector2(-20, 400);
            var boothItemsTMP = boothItemsGO.GetComponent<TextMeshProUGUI>();
            boothItemsTMP.alignment = TextAlignmentOptions.TopLeft;

            // Count text: "0/10 items listed"
            var boothCountGO = CreateUIText(boothPanelGO.transform, "BoothCountText", "0/10 items listed",
                new Vector2(0, 0), new Vector2(1, 0),
                new Vector2(0, 10), new Vector2(0, 40), 24);
            var boothCountRT = boothCountGO.GetComponent<RectTransform>();
            boothCountRT.anchorMin = new Vector2(0, 0);
            boothCountRT.anchorMax = new Vector2(1, 0);
            boothCountRT.pivot = new Vector2(0.5f, 0);
            boothCountRT.anchoredPosition = new Vector2(0, 10);
            boothCountRT.sizeDelta = new Vector2(0, 40);
            var boothCountTMP = boothCountGO.GetComponent<TextMeshProUGUI>();
            boothCountTMP.alignment = TextAlignmentOptions.Center;
            boothCountTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);

            boothPanelGO.SetActive(false);

            // ============================================================
            // WIRE COMPONENTS
            // ============================================================

            // MarketSceneUI on the canvas
            var marketSceneUI = canvasGO.AddComponent<MarketSceneUI>();
            var uiSO = new SerializedObject(marketSceneUI);

            // Header
            uiSO.FindProperty("_titleText").objectReferenceValue = titleTMP;
            uiSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            uiSO.FindProperty("_goldText").objectReferenceValue = goldTMP;

            // Tab Buttons
            uiSO.FindProperty("_buyTabButton").objectReferenceValue = buyTabBtn;
            uiSO.FindProperty("_boothTabButton").objectReferenceValue = boothTabBtn;

            // Buy Panel
            uiSO.FindProperty("_buyPanel").objectReferenceValue = buyPanelGO;
            uiSO.FindProperty("_listingsContainer").objectReferenceValue = contentGO.transform;
            uiSO.FindProperty("_refreshTimerText").objectReferenceValue = refreshTimerTMP;

            // Listing Detail
            uiSO.FindProperty("_listingDetailPanel").objectReferenceValue = detailPanelGO;
            uiSO.FindProperty("_listingNameText").objectReferenceValue = listingNameTMP;
            uiSO.FindProperty("_listingTypeText").objectReferenceValue = listingTypeTMP;
            uiSO.FindProperty("_listingPriceText").objectReferenceValue = listingPriceTMP;
            uiSO.FindProperty("_buyButton").objectReferenceValue = buyButton;
            uiSO.FindProperty("_buyButtonText").objectReferenceValue = buyBtnTMP;
            uiSO.FindProperty("_closeListingButton").objectReferenceValue = closeListingBtn;

            // Booth Panel
            uiSO.FindProperty("_boothPanel").objectReferenceValue = boothPanelGO;
            uiSO.FindProperty("_boothRevenueText").objectReferenceValue = boothRevenueTMP;
            uiSO.FindProperty("_collectRevenueButton").objectReferenceValue = collectRevenueBtn;
            uiSO.FindProperty("_boothItemsText").objectReferenceValue = boothItemsTMP;
            uiSO.FindProperty("_boothCountText").objectReferenceValue = boothCountTMP;

            uiSO.ApplyModifiedPropertiesWithoutUndo();

            // Wire MarketController
            var controllerGO = new GameObject("MarketController");
            var controller = controllerGO.AddComponent<MarketController>();
            var cso = new SerializedObject(controller);
            cso.FindProperty("_marketUI").objectReferenceValue = marketSceneUI;
            cso.ApplyModifiedPropertiesWithoutUndo();

            // --- Finalize ---
            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("[Conquer Chronicles] Market scene setup complete! Hit Play to test.");
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
