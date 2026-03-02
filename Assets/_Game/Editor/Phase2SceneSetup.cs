using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using ConquerChronicles.Gameplay.Bootstrap;
using ConquerChronicles.Gameplay.Camera;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Combat;
using ConquerChronicles.Gameplay.Enemy;
using ConquerChronicles.Gameplay.Loot;
using ConquerChronicles.Gameplay.Map;
using ConquerChronicles.Gameplay.Stage;
using ConquerChronicles.Gameplay.Audio;
using ConquerChronicles.Gameplay.UI.HUD;
using ConquerChronicles.Gameplay.UI.Tutorial;
using ConquerChronicles.Gameplay.Animation;

namespace ConquerChronicles.Editor
{
    public static class Phase2SceneSetup
    {
        [MenuItem("Conquer Chronicles/Setup Phase 2 Scene")]
        public static void Setup()
        {
            SetupScene(0);
        }

        [MenuItem("Conquer Chronicles/Setup Phase 3 Combat Scene")]
        public static void SetupCombat()
        {
            SetupScene(1);
        }

        [MenuItem("Conquer Chronicles/Setup Map Scene")]
        public static void SetupMap()
        {
            SetupScene(2);
        }

        // phase: 0=Phase2, 1=Phase3, 2=Phase4
        private static void SetupScene(int phase)
        {
            bool withCombat = phase >= 1;
            bool withStage = phase >= 2;
            // --- Create Prefabs ---
            var enemyPrefab = CreateEnemyPrefab();
            DamageNumberView dmgNumPrefabView = null;
            HitEffectView hitFxPrefabView = null;
            GoldCoinView goldCoinPrefabView = null;
            EquipmentDropView equipDropPrefabView = null;
            if (withCombat)
            {
                dmgNumPrefabView = CreateDamageNumberPrefab();
                hitFxPrefabView = CreateHitEffectPrefab();
            }
            if (withStage)
            {
                goldCoinPrefabView = CreateGoldCoinPrefab();
                equipDropPrefabView = CreateEquipmentDropPrefab();
            }

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
                cam.orthographicSize = 8f;
                cameraGO.AddComponent<AudioListener>();
                cameraGO.tag = "MainCamera";
            }
            cameraGO.transform.position = new Vector3(0, 0, -10);
            var camera = cameraGO.GetComponent<UnityEngine.Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 8f;
            camera.backgroundColor = new Color(0.15f, 0.15f, 0.2f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            var isoCam = cameraGO.GetComponent<IsometricCamera>();
            if (isoCam == null) isoCam = cameraGO.AddComponent<IsometricCamera>();

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

            // --- Player ---
            var playerGO = new GameObject("Player");
            playerGO.transform.position = Vector3.zero;
            var playerSR = playerGO.AddComponent<SpriteRenderer>();

            // Try to load first male idle frame from atlas, fallback to generated circle
            Sprite playerDefaultSprite = null;
            var playerAtlasAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Atlases/GameAtlas.png");
            foreach (var asset in playerAtlasAssets)
            {
                if (asset is Sprite s && s.name.StartsWith("Male_Base_SIdle_"))
                {
                    playerDefaultSprite = s;
                    break;
                }
            }
            playerSR.sprite = playerDefaultSprite != null ? playerDefaultSprite : CreateCircleSprite("PlayerSprite", new Color(0.2f, 0.5f, 1f, 1f), 32);
            playerSR.sortingLayerName = "Default";
            playerGO.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            var characterView = playerGO.AddComponent<CharacterView>();
            playerGO.AddComponent<SpriteAnimator>();
            playerGO.AddComponent<IsometricYSort>();
            var cvSO = new SerializedObject(characterView);
            cvSO.FindProperty("_spriteRenderer").objectReferenceValue = playerSR;
            cvSO.ApplyModifiedPropertiesWithoutUndo();

            // --- EnemyPool ---
            var poolGO = new GameObject("EnemyPool");
            var enemyPool = poolGO.AddComponent<EnemyPool>();
            var poolSO = new SerializedObject(enemyPool);
            poolSO.FindProperty("_prefab").objectReferenceValue = enemyPrefab.GetComponent<EnemyView>();
            poolSO.FindProperty("_warmupCount").intValue = 64;
            poolSO.ApplyModifiedPropertiesWithoutUndo();

            // --- EnemySpawner ---
            var spawnerGO = new GameObject("EnemySpawner");
            var enemySpawner = spawnerGO.AddComponent<EnemySpawner>();

            // --- MapBoundsProvider ---
            var boundsGO = new GameObject("MapBoundsProvider");
            var mapBounds = boundsGO.AddComponent<MapBoundsProvider>();

            // --- IsometricGrid ---
            var gridGO = new GameObject("IsometricGrid");
            gridGO.AddComponent<IsometricGrid>();

            // --- Combat objects (Phase 3) ---
            CombatManager combatManager = null;
            DamageNumberPool damageNumberPool = null;
            HitEffectPool hitEffectPool = null;

            if (withCombat)
            {
                // DamageNumberPool
                var dmgPoolGO = new GameObject("DamageNumberPool");
                damageNumberPool = dmgPoolGO.AddComponent<DamageNumberPool>();
                if (dmgNumPrefabView != null)
                {
                    var dmgSO = new SerializedObject(damageNumberPool);
                    dmgSO.FindProperty("_prefab").objectReferenceValue = dmgNumPrefabView;
                    dmgSO.FindProperty("_warmupCount").intValue = 32;
                    dmgSO.ApplyModifiedPropertiesWithoutUndo();
                }

                // HitEffectPool
                var hitPoolGO = new GameObject("HitEffectPool");
                hitEffectPool = hitPoolGO.AddComponent<HitEffectPool>();
                if (hitFxPrefabView != null)
                {
                    var hitSO = new SerializedObject(hitEffectPool);
                    hitSO.FindProperty("_prefab").objectReferenceValue = hitFxPrefabView;
                    hitSO.FindProperty("_warmupCount").intValue = 32;
                    hitSO.ApplyModifiedPropertiesWithoutUndo();
                }

                // CombatManager
                var combatGO = new GameObject("CombatManager");
                combatManager = combatGO.AddComponent<CombatManager>();
            }

            // --- HUD Canvas ---
            PlayerHUD playerHUD = null;
            if (withCombat)
            {
                playerHUD = CreateHUDCanvas();
            }

            // --- Map system ---
            MapManager mapManager = null;
            WaveAnnouncerUI waveAnnouncer = null;
            RunSummaryUI runSummary = null;
            GoldCoinPool goldCoinPool = null;
            EquipmentDropPool equipmentDropPool = null;
            LootVisualManager lootVisualManager = null;

            if (withStage)
            {
                // MapManager
                var mapGO = new GameObject("MapManager");
                mapManager = mapGO.AddComponent<MapManager>();

                // Area announcer (reusing WaveAnnouncerUI)
                waveAnnouncer = CreateWaveAnnouncerUI();

                // Session summary
                runSummary = CreateRunSummaryUI();

                // GoldCoinPool
                var goldPoolGO = new GameObject("GoldCoinPool");
                goldCoinPool = goldPoolGO.AddComponent<GoldCoinPool>();
                if (goldCoinPrefabView != null)
                {
                    var gcSO = new SerializedObject(goldCoinPool);
                    gcSO.FindProperty("_prefab").objectReferenceValue = goldCoinPrefabView;
                    gcSO.FindProperty("_warmupCount").intValue = 32;
                    gcSO.ApplyModifiedPropertiesWithoutUndo();
                }

                // EquipmentDropPool
                var equipPoolGO = new GameObject("EquipmentDropPool");
                equipmentDropPool = equipPoolGO.AddComponent<EquipmentDropPool>();
                if (equipDropPrefabView != null)
                {
                    var edSO = new SerializedObject(equipmentDropPool);
                    edSO.FindProperty("_prefab").objectReferenceValue = equipDropPrefabView;
                    edSO.FindProperty("_warmupCount").intValue = 16;
                    edSO.ApplyModifiedPropertiesWithoutUndo();
                }

                // LootVisualManager
                var lootGO = new GameObject("LootVisualManager");
                lootVisualManager = lootGO.AddComponent<LootVisualManager>();
                var lvmSO = new SerializedObject(lootVisualManager);
                lvmSO.FindProperty("_goldCoinPool").objectReferenceValue = goldCoinPool;
                lvmSO.FindProperty("_equipmentDropPool").objectReferenceValue = equipmentDropPool;
                lvmSO.FindProperty("_damageNumberPool").objectReferenceValue = damageNumberPool;
                lvmSO.FindProperty("_playerTransform").objectReferenceValue = characterView.transform;
                lvmSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // --- Audio Manager ---
            var audioManagerGO = new GameObject("AudioManager");
            var audioManager = audioManagerGO.AddComponent<AudioManager>();

            // --- Tutorial Overlay ---
            TutorialOverlay tutorialOverlay = null;
            {
                var tutCanvasGO = new GameObject("TutorialCanvas");
                var tutCanvas = tutCanvasGO.AddComponent<Canvas>();
                tutCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                tutCanvas.sortingOrder = 999;
                var tutScaler = tutCanvasGO.AddComponent<CanvasScaler>();
                tutScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                tutScaler.referenceResolution = new Vector2(1080, 1920);
                tutScaler.matchWidthOrHeight = 1.0f;
                tutCanvasGO.AddComponent<GraphicRaycaster>();

                var overlay = new GameObject("TutorialOverlay");
                overlay.AddComponent<RectTransform>();
                overlay.transform.SetParent(tutCanvasGO.transform, false);
                var overlayParentRT = overlay.GetComponent<RectTransform>();
                overlayParentRT.anchorMin = Vector2.zero;
                overlayParentRT.anchorMax = Vector2.one;
                overlayParentRT.offsetMin = Vector2.zero;
                overlayParentRT.offsetMax = Vector2.zero;
                tutorialOverlay = overlay.AddComponent<TutorialOverlay>();

                // Overlay root (full screen dark backdrop)
                var overlayRoot = new GameObject("OverlayRoot");
                overlayRoot.AddComponent<RectTransform>();
                overlayRoot.transform.SetParent(overlay.transform, false);
                var overlayRT = overlayRoot.GetComponent<RectTransform>();
                overlayRT.anchorMin = Vector2.zero;
                overlayRT.anchorMax = Vector2.one;
                overlayRT.offsetMin = Vector2.zero;
                overlayRT.offsetMax = Vector2.zero;

                var backdrop = overlayRoot.AddComponent<Image>();
                backdrop.color = new Color(0, 0, 0, 0.7f);
                backdrop.raycastTarget = true;

                // Tooltip text (centered)
                var tooltipGO = new GameObject("TooltipText");
                tooltipGO.AddComponent<RectTransform>();
                tooltipGO.transform.SetParent(overlayRoot.transform, false);
                var tooltipRT = tooltipGO.GetComponent<RectTransform>();
                tooltipRT.anchorMin = new Vector2(0.1f, 0.4f);
                tooltipRT.anchorMax = new Vector2(0.9f, 0.6f);
                tooltipRT.offsetMin = Vector2.zero;
                tooltipRT.offsetMax = Vector2.zero;
                var tooltipTMP = tooltipGO.AddComponent<TextMeshProUGUI>();
                tooltipTMP.alignment = TextAlignmentOptions.Center;
                tooltipTMP.fontSize = 36;
                tooltipTMP.color = Color.white;

                // Dismiss button
                var dismissGO = new GameObject("DismissButton");
                dismissGO.AddComponent<RectTransform>();
                dismissGO.transform.SetParent(overlayRoot.transform, false);
                var dismissRT = dismissGO.GetComponent<RectTransform>();
                dismissRT.anchorMin = new Vector2(0.3f, 0.2f);
                dismissRT.anchorMax = new Vector2(0.7f, 0.3f);
                dismissRT.offsetMin = Vector2.zero;
                dismissRT.offsetMax = Vector2.zero;
                var dismissImg = dismissGO.AddComponent<Image>();
                dismissImg.color = new Color(0.2f, 0.6f, 1f, 1f);
                var dismissBtn = dismissGO.AddComponent<Button>();
                dismissBtn.targetGraphic = dismissImg;

                var dismissTextGO = new GameObject("Text");
                dismissTextGO.AddComponent<RectTransform>();
                dismissTextGO.transform.SetParent(dismissGO.transform, false);
                var dismissTextRT = dismissTextGO.GetComponent<RectTransform>();
                dismissTextRT.anchorMin = Vector2.zero;
                dismissTextRT.anchorMax = Vector2.one;
                dismissTextRT.offsetMin = Vector2.zero;
                dismissTextRT.offsetMax = Vector2.zero;
                var dismissTMP = dismissTextGO.AddComponent<TextMeshProUGUI>();
                dismissTMP.text = "Next";
                dismissTMP.alignment = TextAlignmentOptions.Center;
                dismissTMP.fontSize = 28;
                dismissTMP.color = Color.white;

                // Wire SerializedObject
                var tutSO = new SerializedObject(tutorialOverlay);
                tutSO.FindProperty("_overlayRoot").objectReferenceValue = overlayRoot;
                tutSO.FindProperty("_backdrop").objectReferenceValue = backdrop;
                tutSO.FindProperty("_tooltipText").objectReferenceValue = tooltipTMP;
                tutSO.FindProperty("_dismissButton").objectReferenceValue = dismissBtn;
                tutSO.FindProperty("_dismissButtonText").objectReferenceValue = dismissTMP;
                tutSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // --- GameManager (Test Setup) ---
            var managerGO = new GameObject("GameManager");
            var testSetup = managerGO.AddComponent<GameplayTestSetup>();
            var tsSO = new SerializedObject(testSetup);
            tsSO.FindProperty("_isometricCamera").objectReferenceValue = isoCam;
            tsSO.FindProperty("_characterView").objectReferenceValue = characterView;
            tsSO.FindProperty("_enemyPool").objectReferenceValue = enemyPool;
            tsSO.FindProperty("_enemySpawner").objectReferenceValue = enemySpawner;
            tsSO.FindProperty("_mapBoundsProvider").objectReferenceValue = mapBounds;
            if (withCombat)
            {
                tsSO.FindProperty("_combatManager").objectReferenceValue = combatManager;
                tsSO.FindProperty("_damageNumberPool").objectReferenceValue = damageNumberPool;
                tsSO.FindProperty("_hitEffectPool").objectReferenceValue = hitEffectPool;
                tsSO.FindProperty("_playerHUD").objectReferenceValue = playerHUD;
            }
            if (withStage)
            {
                tsSO.FindProperty("_mapManager").objectReferenceValue = mapManager;
                tsSO.FindProperty("_waveAnnouncer").objectReferenceValue = waveAnnouncer;
                tsSO.FindProperty("_runSummary").objectReferenceValue = runSummary;
                tsSO.FindProperty("_testMapIndex").intValue = 0;
                tsSO.FindProperty("_testAreaIndex").intValue = 0;
                tsSO.FindProperty("_lootVisualManager").objectReferenceValue = lootVisualManager;
                tsSO.FindProperty("_goldCoinPool").objectReferenceValue = goldCoinPool;
                tsSO.FindProperty("_equipmentDropPool").objectReferenceValue = equipmentDropPool;
            }
            tsSO.FindProperty("_audioManager").objectReferenceValue = audioManager;
            tsSO.FindProperty("_tutorialOverlay").objectReferenceValue = tutorialOverlay;
            tsSO.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);

            string phaseName = withStage ? "Map (Combat + Areas)" : withCombat ? "Phase 3 (Combat)" : "Phase 2";
            Debug.Log($"[Conquer Chronicles] {phaseName} scene setup complete! Hit Play to test.");
        }

        // --- Prefab Creators ---

        private static GameObject CreateEnemyPrefab()
        {
            EnsureFolder("Assets/_Game/Data/Prefabs");

            // Always regenerate to pick up new components
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");
            if (existing != null)
                AssetDatabase.DeleteAsset("Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");

            var go = new GameObject("Enemy_Slime");
            var sr = go.AddComponent<SpriteRenderer>();

            // Try to load first rat walk frame from atlas, fallback to generated circle
            var atlasSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Atlases/GameAtlas.png");
            Sprite defaultSprite = null;
            foreach (var asset in atlasSprites)
            {
                if (asset is Sprite s && s.name.StartsWith("Rat_LWalk_"))
                {
                    defaultSprite = s;
                    break;
                }
            }
            sr.sprite = defaultSprite != null ? defaultSprite : CreateCircleSprite("EnemySprite", new Color(0.3f, 0.9f, 0.3f, 1f), 24);
            sr.sortingLayerName = "Default";

            var view = go.AddComponent<EnemyView>();
            var so = new SerializedObject(view);
            so.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<EnemyMovement>();
            go.AddComponent<SpriteAnimator>();
            go.AddComponent<IsometricYSort>();

            // --- Health Bar ---
            var healthBarRoot = new GameObject("HealthBar");
            healthBarRoot.transform.SetParent(go.transform, false);
            healthBarRoot.transform.localPosition = new Vector3(0f, 0.8f, 0f); // above the enemy sprite
            healthBarRoot.SetActive(true); // always visible

            // Background (dark bar)
            var bgGo = new GameObject("BG");
            bgGo.transform.SetParent(healthBarRoot.transform, false);
            var bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sprite = CreateRectSprite("HealthBarBG_Thin", new Color(0.15f, 0.15f, 0.15f, 0.8f), 24, 2);
            bgSr.sortingLayerName = "Default";
            bgSr.sortingOrder = 10;

            // Fill (green bar)
            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(healthBarRoot.transform, false);
            var fillSr = fillGo.AddComponent<SpriteRenderer>();
            fillSr.sprite = CreateRectSprite("HealthBarFill_Thin", new Color(0.2f, 0.9f, 0.2f, 1f), 24, 2);
            fillSr.sortingLayerName = "Default";
            fillSr.sortingOrder = 11;

            // Assign health bar references to EnemyView
            so = new SerializedObject(view);
            so.FindProperty("_healthBarRoot").objectReferenceValue = healthBarRoot;
            so.FindProperty("_healthBarFill").objectReferenceValue = fillGo.transform;
            so.ApplyModifiedPropertiesWithoutUndo();

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/_Game/Data/Prefabs/Enemy_Slime.prefab");
            Object.DestroyImmediate(go);
            return prefab;
        }

        private static DamageNumberView CreateDamageNumberPrefab()
        {
            EnsureFolder("Assets/_Game/Data/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/DamageNumber.prefab");
            if (existing != null) return existing.GetComponent<DamageNumberView>();

            var go = new GameObject("DamageNumber");
            var tmp = go.AddComponent<TextMeshPro>();
            tmp.fontSize = 4f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.sortingOrder = 100;
            tmp.color = Color.white;

            var view = go.AddComponent<DamageNumberView>();
            go.SetActive(false);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/_Game/Data/Prefabs/DamageNumber.prefab");
            Object.DestroyImmediate(go);
            return prefab.GetComponent<DamageNumberView>();
        }

        private static HitEffectView CreateHitEffectPrefab()
        {
            EnsureFolder("Assets/_Game/Data/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/HitEffect.prefab");
            if (existing != null) return existing.GetComponent<HitEffectView>();

            var go = new GameObject("HitEffect");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite("HitEffectSprite", new Color(1f, 1f, 1f, 0.8f), 16);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 90;

            var view = go.AddComponent<HitEffectView>();
            var so = new SerializedObject(view);
            so.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.SetActive(false);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/_Game/Data/Prefabs/HitEffect.prefab");
            Object.DestroyImmediate(go);
            return prefab.GetComponent<HitEffectView>();
        }

        private static GoldCoinView CreateGoldCoinPrefab()
        {
            EnsureFolder("Assets/_Game/Data/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/GoldCoin.prefab");
            if (existing != null) return existing.GetComponent<GoldCoinView>();

            var go = new GameObject("GoldCoin");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite("GoldCoinSprite", new Color(1f, 0.85f, 0.2f, 1f), 12);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 80;

            var view = go.AddComponent<GoldCoinView>();
            var so = new SerializedObject(view);
            so.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.SetActive(false);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/_Game/Data/Prefabs/GoldCoin.prefab");
            Object.DestroyImmediate(go);
            return prefab.GetComponent<GoldCoinView>();
        }

        private static EquipmentDropView CreateEquipmentDropPrefab()
        {
            EnsureFolder("Assets/_Game/Data/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Data/Prefabs/EquipmentDrop.prefab");
            if (existing != null) return existing.GetComponent<EquipmentDropView>();

            var go = new GameObject("EquipmentDrop");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = CreateCircleSprite("EquipmentDropSprite", new Color(0.7f, 0.3f, 0.9f, 1f), 16);
            sr.sortingLayerName = "Default";
            sr.sortingOrder = 75;

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.5f;

            var view = go.AddComponent<EquipmentDropView>();
            var so = new SerializedObject(view);
            so.FindProperty("_spriteRenderer").objectReferenceValue = sr;
            so.ApplyModifiedPropertiesWithoutUndo();

            go.SetActive(false);

            var prefab = PrefabUtility.SaveAsPrefabAsset(go, "Assets/_Game/Data/Prefabs/EquipmentDrop.prefab");
            Object.DestroyImmediate(go);
            return prefab.GetComponent<EquipmentDropView>();
        }

        // --- HUD Canvas ---

        private static PlayerHUD CreateHUDCanvas()
        {
            // Canvas
            var canvasGO = new GameObject("HUD_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            var scaler = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f;
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // SafeArea container
            var safeAreaGO = new GameObject("SafeArea", typeof(RectTransform));
            safeAreaGO.transform.SetParent(canvasGO.transform, false);
            var safeAreaRT = safeAreaGO.GetComponent<RectTransform>();
            safeAreaRT.anchorMin = Vector2.zero;
            safeAreaRT.anchorMax = Vector2.one;
            safeAreaRT.offsetMin = Vector2.zero;
            safeAreaRT.offsetMax = Vector2.zero;
            safeAreaGO.AddComponent<ConquerChronicles.Gameplay.UI.SafeAreaHandler>();

            // =============================================
            // XP Bar — full width across top
            // =============================================
            var xpBarBG = new GameObject("XP_Bar_BG", typeof(RectTransform));
            xpBarBG.transform.SetParent(safeAreaGO.transform, false);
            var xpBgRT = xpBarBG.GetComponent<RectTransform>();
            xpBgRT.anchorMin = new Vector2(0, 1);
            xpBgRT.anchorMax = new Vector2(1, 1);
            xpBgRT.pivot = new Vector2(0.5f, 1);
            xpBgRT.anchoredPosition = new Vector2(0, 0);
            xpBgRT.sizeDelta = new Vector2(0, 22);
            var xpBgImg = xpBarBG.AddComponent<Image>();
            xpBgImg.color = new Color(0.08f, 0.08f, 0.12f, 0.9f);

            var xpFill = new GameObject("XP_Fill", typeof(RectTransform));
            xpFill.transform.SetParent(xpBarBG.transform, false);
            var xpFillRT = xpFill.GetComponent<RectTransform>();
            xpFillRT.anchorMin = Vector2.zero;
            xpFillRT.anchorMax = Vector2.one;
            xpFillRT.offsetMin = new Vector2(1, 1);
            xpFillRT.offsetMax = new Vector2(-1, -1);
            var xpFillImg = xpFill.AddComponent<Image>();
            xpFillImg.color = new Color(1f, 0.84f, 0f, 1f); // gold
            xpFillImg.type = UnityEngine.UI.Image.Type.Filled;
            xpFillImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

            // XP text overlay (centered on bar)
            var xpTextGO = CreateUIText(xpBarBG.transform, "XP_Text", "0/100",
                Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, 14);
            var xpTMP = xpTextGO.GetComponent<TextMeshProUGUI>();
            xpTMP.alignment = TextAlignmentOptions.Center;
            xpTMP.color = Color.white;
            xpTMP.fontStyle = FontStyles.Bold;

            // =============================================
            // Level badge — left side, below XP bar
            // =============================================
            var levelBadge = new GameObject("Level_Badge", typeof(RectTransform));
            levelBadge.transform.SetParent(safeAreaGO.transform, false);
            var lvBadgeRT = levelBadge.GetComponent<RectTransform>();
            lvBadgeRT.anchorMin = new Vector2(0, 1);
            lvBadgeRT.anchorMax = new Vector2(0, 1);
            lvBadgeRT.pivot = new Vector2(0, 1);
            lvBadgeRT.anchoredPosition = new Vector2(12, -26);
            lvBadgeRT.sizeDelta = new Vector2(80, 32);
            var lvBadgeImg = levelBadge.AddComponent<Image>();
            lvBadgeImg.color = new Color(0.15f, 0.12f, 0.25f, 0.9f);

            var levelTextGO = new GameObject("Level_Text", typeof(RectTransform));
            levelTextGO.transform.SetParent(levelBadge.transform, false);
            var levelTextRT = levelTextGO.GetComponent<RectTransform>();
            levelTextRT.anchorMin = Vector2.zero;
            levelTextRT.anchorMax = Vector2.one;
            levelTextRT.offsetMin = Vector2.zero;
            levelTextRT.offsetMax = Vector2.zero;
            var levelTMP = levelTextGO.AddComponent<TextMeshProUGUI>();
            levelTMP.text = "Lv.1";
            levelTMP.fontSize = 22;
            levelTMP.color = new Color(1f, 0.84f, 0f, 1f); // gold text
            levelTMP.fontStyle = FontStyles.Bold;
            levelTMP.alignment = TextAlignmentOptions.Center;

            // Class name — right of level badge
            var classTextGO = new GameObject("Class_Text", typeof(RectTransform));
            classTextGO.transform.SetParent(safeAreaGO.transform, false);
            var classTextRT = classTextGO.GetComponent<RectTransform>();
            classTextRT.anchorMin = new Vector2(0, 1);
            classTextRT.anchorMax = new Vector2(0, 1);
            classTextRT.pivot = new Vector2(0, 1);
            classTextRT.anchoredPosition = new Vector2(98, -28);
            classTextRT.sizeDelta = new Vector2(200, 28);
            var classTMP = classTextGO.AddComponent<TextMeshProUGUI>();
            classTMP.text = "Trojan";
            classTMP.fontSize = 18;
            classTMP.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            classTMP.alignment = TextAlignmentOptions.Left;

            // =============================================
            // Stats panel — HP and MP bars below level
            // =============================================
            var statsPanel = new GameObject("Stats_Panel", typeof(RectTransform));
            statsPanel.transform.SetParent(safeAreaGO.transform, false);
            var statsPanelRT = statsPanel.GetComponent<RectTransform>();
            statsPanelRT.anchorMin = new Vector2(0, 1);
            statsPanelRT.anchorMax = new Vector2(0, 1);
            statsPanelRT.pivot = new Vector2(0, 1);
            statsPanelRT.anchoredPosition = new Vector2(12, -62);
            statsPanelRT.sizeDelta = new Vector2(340, 80);

            // --- HP Bar ---
            // HP label
            var hpLabelGO = new GameObject("HP_Label", typeof(RectTransform));
            hpLabelGO.transform.SetParent(statsPanel.transform, false);
            var hpLabelRT = hpLabelGO.GetComponent<RectTransform>();
            hpLabelRT.anchorMin = new Vector2(0, 1);
            hpLabelRT.anchorMax = new Vector2(0, 1);
            hpLabelRT.pivot = new Vector2(0, 1);
            hpLabelRT.anchoredPosition = new Vector2(0, 0);
            hpLabelRT.sizeDelta = new Vector2(36, 34);
            var hpLabelTMP = hpLabelGO.AddComponent<TextMeshProUGUI>();
            hpLabelTMP.text = "HP";
            hpLabelTMP.fontSize = 18;
            hpLabelTMP.color = new Color(1f, 0.35f, 0.35f, 1f);
            hpLabelTMP.fontStyle = FontStyles.Bold;
            hpLabelTMP.alignment = TextAlignmentOptions.MidlineLeft;

            // HP bar background
            var hpBarBG = new GameObject("HP_Bar_BG", typeof(RectTransform));
            hpBarBG.transform.SetParent(statsPanel.transform, false);
            var hpBgRT = hpBarBG.GetComponent<RectTransform>();
            hpBgRT.anchorMin = new Vector2(0, 1);
            hpBgRT.anchorMax = new Vector2(0, 1);
            hpBgRT.pivot = new Vector2(0, 1);
            hpBgRT.anchoredPosition = new Vector2(38, -2);
            hpBgRT.sizeDelta = new Vector2(290, 30);
            var hpBgImg = hpBarBG.AddComponent<Image>();
            hpBgImg.color = new Color(0.12f, 0.04f, 0.04f, 0.9f);

            // HP bar outline
            var hpOutline = hpBarBG.AddComponent<Outline>();
            hpOutline.effectColor = new Color(0.5f, 0.15f, 0.15f, 0.8f);
            hpOutline.effectDistance = new Vector2(1, -1);

            // HP fill
            var hpFill = new GameObject("HP_Fill", typeof(RectTransform));
            hpFill.transform.SetParent(hpBarBG.transform, false);
            var hpFillRT = hpFill.GetComponent<RectTransform>();
            hpFillRT.anchorMin = Vector2.zero;
            hpFillRT.anchorMax = Vector2.one;
            hpFillRT.offsetMin = new Vector2(2, 2);
            hpFillRT.offsetMax = new Vector2(-2, -2);
            var hpFillImg = hpFill.AddComponent<Image>();
            hpFillImg.color = new Color(0.85f, 0.15f, 0.15f, 1f);
            hpFillImg.type = UnityEngine.UI.Image.Type.Filled;
            hpFillImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

            // HP text (on top of bar)
            var hpTextGO = CreateUIText(hpBarBG.transform, "HP_Text", "100/100",
                Vector2.zero, Vector2.one, new Vector2(4, 0), new Vector2(-4, 0), 16);
            var hpTMP = hpTextGO.GetComponent<TextMeshProUGUI>();
            hpTMP.alignment = TextAlignmentOptions.Center;
            hpTMP.fontStyle = FontStyles.Bold;

            // --- MP Bar ---
            // MP label
            var mpLabelGO = new GameObject("MP_Label", typeof(RectTransform));
            mpLabelGO.transform.SetParent(statsPanel.transform, false);
            var mpLabelRT = mpLabelGO.GetComponent<RectTransform>();
            mpLabelRT.anchorMin = new Vector2(0, 1);
            mpLabelRT.anchorMax = new Vector2(0, 1);
            mpLabelRT.pivot = new Vector2(0, 1);
            mpLabelRT.anchoredPosition = new Vector2(0, -38);
            mpLabelRT.sizeDelta = new Vector2(36, 34);
            var mpLabelTMP = mpLabelGO.AddComponent<TextMeshProUGUI>();
            mpLabelTMP.text = "MP";
            mpLabelTMP.fontSize = 18;
            mpLabelTMP.color = new Color(0.3f, 0.5f, 1f, 1f);
            mpLabelTMP.fontStyle = FontStyles.Bold;
            mpLabelTMP.alignment = TextAlignmentOptions.MidlineLeft;

            // MP bar background
            var mpBarBG = new GameObject("MP_Bar_BG", typeof(RectTransform));
            mpBarBG.transform.SetParent(statsPanel.transform, false);
            var mpBgRT = mpBarBG.GetComponent<RectTransform>();
            mpBgRT.anchorMin = new Vector2(0, 1);
            mpBgRT.anchorMax = new Vector2(0, 1);
            mpBgRT.pivot = new Vector2(0, 1);
            mpBgRT.anchoredPosition = new Vector2(38, -40);
            mpBgRT.sizeDelta = new Vector2(290, 26);
            var mpBgImg = mpBarBG.AddComponent<Image>();
            mpBgImg.color = new Color(0.04f, 0.04f, 0.14f, 0.9f);

            // MP bar outline
            var mpOutline = mpBarBG.AddComponent<Outline>();
            mpOutline.effectColor = new Color(0.15f, 0.2f, 0.5f, 0.8f);
            mpOutline.effectDistance = new Vector2(1, -1);

            // MP fill
            var mpFill = new GameObject("MP_Fill", typeof(RectTransform));
            mpFill.transform.SetParent(mpBarBG.transform, false);
            var mpFillRT = mpFill.GetComponent<RectTransform>();
            mpFillRT.anchorMin = Vector2.zero;
            mpFillRT.anchorMax = Vector2.one;
            mpFillRT.offsetMin = new Vector2(2, 2);
            mpFillRT.offsetMax = new Vector2(-2, -2);
            var mpFillImg = mpFill.AddComponent<Image>();
            mpFillImg.color = new Color(0.15f, 0.35f, 0.95f, 1f);
            mpFillImg.type = UnityEngine.UI.Image.Type.Filled;
            mpFillImg.fillMethod = UnityEngine.UI.Image.FillMethod.Horizontal;

            // MP text (on top of bar)
            var mpTextGO = CreateUIText(mpBarBG.transform, "MP_Text", "50/50",
                Vector2.zero, Vector2.one, new Vector2(4, 0), new Vector2(-4, 0), 14);
            var mpTMP = mpTextGO.GetComponent<TextMeshProUGUI>();
            mpTMP.alignment = TextAlignmentOptions.Center;
            mpTMP.fontStyle = FontStyles.Bold;

            // =============================================
            // Kill counter — below stats panel
            // =============================================
            var killText = new GameObject("Kill_Text", typeof(RectTransform));
            killText.transform.SetParent(safeAreaGO.transform, false);
            var killTextRT = killText.GetComponent<RectTransform>();
            killTextRT.anchorMin = new Vector2(0, 1);
            killTextRT.anchorMax = new Vector2(0, 1);
            killTextRT.pivot = new Vector2(0, 1);
            killTextRT.anchoredPosition = new Vector2(16, -148);
            killTextRT.sizeDelta = new Vector2(200, 28);
            var killTMP = killText.AddComponent<TextMeshProUGUI>();
            killTMP.text = "Kills: 0";
            killTMP.fontSize = 20;
            killTMP.color = new Color(0.9f, 0.9f, 0.9f, 0.8f);
            killTMP.alignment = TextAlignmentOptions.Left;

            // =============================================
            // Back button — top-right
            // =============================================
            var backBtnGO = new GameObject("BackButton", typeof(RectTransform));
            backBtnGO.transform.SetParent(safeAreaGO.transform, false);
            var backBtnRT = backBtnGO.GetComponent<RectTransform>();
            backBtnRT.anchorMin = new Vector2(1, 1);
            backBtnRT.anchorMax = new Vector2(1, 1);
            backBtnRT.pivot = new Vector2(1, 1);
            backBtnRT.anchoredPosition = new Vector2(-16, -28);
            backBtnRT.sizeDelta = new Vector2(120, 44);
            var backBtnImg = backBtnGO.AddComponent<Image>();
            backBtnImg.color = new Color(0.5f, 0.12f, 0.12f, 0.85f);
            var backBtn = backBtnGO.AddComponent<Button>();
            backBtn.targetGraphic = backBtnImg;

            // Back button outline
            var backOutline = backBtnGO.AddComponent<Outline>();
            backOutline.effectColor = new Color(0.7f, 0.2f, 0.2f, 0.6f);
            backOutline.effectDistance = new Vector2(1, -1);

            var backBtnTextGO = new GameObject("Text", typeof(RectTransform));
            backBtnTextGO.transform.SetParent(backBtnGO.transform, false);
            var backBtnTextRT = backBtnTextGO.GetComponent<RectTransform>();
            backBtnTextRT.anchorMin = Vector2.zero;
            backBtnTextRT.anchorMax = Vector2.one;
            backBtnTextRT.offsetMin = Vector2.zero;
            backBtnTextRT.offsetMax = Vector2.zero;
            var backBtnTMP = backBtnTextGO.AddComponent<TextMeshProUGUI>();
            backBtnTMP.text = "Leave";
            backBtnTMP.fontSize = 22;
            backBtnTMP.color = Color.white;
            backBtnTMP.fontStyle = FontStyles.Bold;
            backBtnTMP.alignment = TextAlignmentOptions.Center;

            // =============================================
            // Wire PlayerHUD
            // =============================================
            var hud = canvasGO.AddComponent<PlayerHUD>();
            var hudSO = new SerializedObject(hud);
            hudSO.FindProperty("_hpFill").objectReferenceValue = hpFillImg;
            hudSO.FindProperty("_hpText").objectReferenceValue = hpTMP;
            hudSO.FindProperty("_mpFill").objectReferenceValue = mpFillImg;
            hudSO.FindProperty("_mpText").objectReferenceValue = mpTMP;
            hudSO.FindProperty("_xpFill").objectReferenceValue = xpFillImg;
            hudSO.FindProperty("_xpText").objectReferenceValue = xpTMP;
            hudSO.FindProperty("_levelText").objectReferenceValue = levelTMP;
            hudSO.FindProperty("_classText").objectReferenceValue = classTMP;
            hudSO.FindProperty("_killCountText").objectReferenceValue = killTMP;
            hudSO.FindProperty("_backButton").objectReferenceValue = backBtn;
            hudSO.ApplyModifiedPropertiesWithoutUndo();

            return hud;
        }

        // --- Stage UI Creators ---

        private static WaveAnnouncerUI CreateWaveAnnouncerUI()
        {
            var canvasGO = new GameObject("WaveAnnouncer_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 20;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f;

            // Center text container with CanvasGroup for fading
            var containerGO = new GameObject("AnnouncerContainer", typeof(RectTransform));
            containerGO.transform.SetParent(canvasGO.transform, false);
            var containerRT = containerGO.GetComponent<RectTransform>();
            containerRT.anchorMin = new Vector2(0.1f, 0.4f);
            containerRT.anchorMax = new Vector2(0.9f, 0.6f);
            containerRT.offsetMin = Vector2.zero;
            containerRT.offsetMax = Vector2.zero;
            var canvasGroup = containerGO.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            // Background
            var bgImg = containerGO.AddComponent<Image>();
            bgImg.color = new Color(0f, 0f, 0f, 0.6f);

            // Text
            var textGO = new GameObject("AnnouncerText", typeof(RectTransform));
            textGO.transform.SetParent(containerGO.transform, false);
            var textRT = textGO.GetComponent<RectTransform>();
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = Vector2.zero;
            textRT.offsetMax = Vector2.zero;
            var tmp = textGO.AddComponent<TextMeshProUGUI>();
            tmp.text = "";
            tmp.fontSize = 48;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;

            var announcer = canvasGO.AddComponent<WaveAnnouncerUI>();
            var so = new SerializedObject(announcer);
            so.FindProperty("_text").objectReferenceValue = tmp;
            so.FindProperty("_canvasGroup").objectReferenceValue = canvasGroup;
            so.ApplyModifiedPropertiesWithoutUndo();

            return announcer;
        }

        private static RunSummaryUI CreateRunSummaryUI()
        {
            var canvasGO = new GameObject("RunSummary_Canvas");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 30;
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1.0f;
            canvasGO.AddComponent<GraphicRaycaster>();

            // Dark overlay panel
            var panelGO = new GameObject("SummaryPanel", typeof(RectTransform));
            panelGO.transform.SetParent(canvasGO.transform, false);
            var panelRT = panelGO.GetComponent<RectTransform>();
            panelRT.anchorMin = new Vector2(0.1f, 0.15f);
            panelRT.anchorMax = new Vector2(0.9f, 0.85f);
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
            var panelImg = panelGO.AddComponent<Image>();
            panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.92f);

            // Title (VICTORY / DEFEATED)
            var titleGO = CreateUIText(panelGO.transform, "TitleText", "VICTORY!",
                new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(0, -20), new Vector2(0, 80), 56);
            var titleTMP = titleGO.GetComponent<TextMeshProUGUI>();
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
            var titleRT = titleGO.GetComponent<RectTransform>();
            titleRT.anchorMin = new Vector2(0, 1);
            titleRT.anchorMax = new Vector2(1, 1);
            titleRT.pivot = new Vector2(0.5f, 1);
            titleRT.anchoredPosition = new Vector2(0, -20);
            titleRT.sizeDelta = new Vector2(0, 80);

            // Stats texts (kills, time, gold, xp, stars)
            float yOffset = -120f;
            float lineHeight = 50f;

            var killsGO = CreateStatText(panelGO.transform, "KillsText", "Enemies Killed: 0", yOffset);
            yOffset -= lineHeight;
            var timeGO = CreateStatText(panelGO.transform, "TimeText", "Time: 00:00", yOffset);
            yOffset -= lineHeight;
            var goldGO = CreateStatText(panelGO.transform, "GoldText", "Gold: +0", yOffset);
            yOffset -= lineHeight;
            var xpGO = CreateStatText(panelGO.transform, "XPText", "XP: +0", yOffset);
            yOffset -= lineHeight;
            var starsGO = CreateStatText(panelGO.transform, "StarsText", "Rating: [---]", yOffset);

            // Continue button
            var btnGO = new GameObject("ContinueButton", typeof(RectTransform));
            btnGO.transform.SetParent(panelGO.transform, false);
            var btnRT = btnGO.GetComponent<RectTransform>();
            btnRT.anchorMin = new Vector2(0.2f, 0);
            btnRT.anchorMax = new Vector2(0.8f, 0);
            btnRT.pivot = new Vector2(0.5f, 0);
            btnRT.anchoredPosition = new Vector2(0, 30);
            btnRT.sizeDelta = new Vector2(0, 70);
            var btnImg = btnGO.AddComponent<Image>();
            btnImg.color = new Color(0.2f, 0.6f, 0.2f, 1f);
            var btn = btnGO.AddComponent<Button>();
            btn.targetGraphic = btnImg;

            var btnTextGO = new GameObject("ButtonText", typeof(RectTransform));
            btnTextGO.transform.SetParent(btnGO.transform, false);
            var btnTextRT = btnTextGO.GetComponent<RectTransform>();
            btnTextRT.anchorMin = Vector2.zero;
            btnTextRT.anchorMax = Vector2.one;
            btnTextRT.offsetMin = Vector2.zero;
            btnTextRT.offsetMax = Vector2.zero;
            var btnTMP = btnTextGO.AddComponent<TextMeshProUGUI>();
            btnTMP.text = "Continue";
            btnTMP.fontSize = 32;
            btnTMP.color = Color.white;
            btnTMP.alignment = TextAlignmentOptions.Center;

            panelGO.SetActive(false);

            // Wire RunSummaryUI
            var summary = canvasGO.AddComponent<RunSummaryUI>();
            var sso = new SerializedObject(summary);
            sso.FindProperty("_panel").objectReferenceValue = panelGO;
            sso.FindProperty("_titleText").objectReferenceValue = titleTMP;
            sso.FindProperty("_killsText").objectReferenceValue = killsGO.GetComponent<TextMeshProUGUI>();
            sso.FindProperty("_timeText").objectReferenceValue = timeGO.GetComponent<TextMeshProUGUI>();
            sso.FindProperty("_goldText").objectReferenceValue = goldGO.GetComponent<TextMeshProUGUI>();
            sso.FindProperty("_xpText").objectReferenceValue = xpGO.GetComponent<TextMeshProUGUI>();
            sso.FindProperty("_starsText").objectReferenceValue = starsGO.GetComponent<TextMeshProUGUI>();
            sso.FindProperty("_continueButton").objectReferenceValue = btn;
            sso.ApplyModifiedPropertiesWithoutUndo();

            return summary;
        }

        private static GameObject CreateStatText(Transform parent, string name, string text, float yOffset)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, yOffset);
            rt.sizeDelta = new Vector2(-60, 45);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 28;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Left;
            return go;
        }

        // --- UI Helpers ---

        private static GameObject CreateUIPanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.pivot = anchorMin;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
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
            var img = go.AddComponent<UnityEngine.UI.Image>();
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

        // --- Asset Helpers ---

        private static void EnsureFolder(string path)
        {
            var parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        private static Sprite CreateCircleSprite(string name, Color color, int size)
        {
            string path = $"Assets/Visual/Sprites/{name}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            float center = size / 2f;
            float radius = size / 2f - 1f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (dist <= radius)
                    {
                        float shade = 1f - (dist / radius) * 0.3f;
                        float highlight = Mathf.Max(0, 1f - Vector2.Distance(
                            new Vector2(x, y),
                            new Vector2(center - radius * 0.3f, center + radius * 0.3f)) / radius);
                        Color c = color * shade + Color.white * highlight * 0.2f;
                        c.a = 1f;
                        tex.SetPixel(x, y, c);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Sprite CreateRectSprite(string name, Color color, int width, int height)
        {
            string path = $"Assets/Visual/Sprites/{name}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
            tex.Apply();

            byte[] png = tex.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, png);
            AssetDatabase.Refresh();

            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
