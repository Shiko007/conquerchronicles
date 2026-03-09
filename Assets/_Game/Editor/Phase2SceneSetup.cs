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
                tutScaler.matchWidthOrHeight = 0f;
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

            // --- Name Label (above health bar) ---
            var nameLabelGO = new GameObject("NameLabel");
            nameLabelGO.transform.SetParent(go.transform, false);
            nameLabelGO.transform.localPosition = new Vector3(0f, 1.0f, 0f);
            var nameLabel = nameLabelGO.AddComponent<TextMesh>();
            nameLabel.text = "";
            nameLabel.fontSize = 32;
            nameLabel.characterSize = 0.06f;
            nameLabel.anchor = TextAnchor.LowerCenter;
            nameLabel.alignment = TextAlignment.Center;
            nameLabel.color = Color.white;
            var nameLabelRenderer = nameLabelGO.GetComponent<MeshRenderer>();
            nameLabelRenderer.sortingLayerName = "Default";
            nameLabelRenderer.sortingOrder = 12;

            // --- Health Bar ---
            var healthBarRoot = new GameObject("HealthBar");
            healthBarRoot.transform.SetParent(go.transform, false);
            healthBarRoot.transform.localPosition = new Vector3(0f, 0.8f, 0f); // above the enemy sprite
            healthBarRoot.SetActive(true); // always visible

            // Background (light grey bar)
            var bgGo = new GameObject("BG");
            bgGo.transform.SetParent(healthBarRoot.transform, false);
            var bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sprite = CreateRectSprite("HealthBarBG_Thin", new Color(0.75f, 0.75f, 0.75f, 0.9f), 24, 2);
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
            so.FindProperty("_nameLabel").objectReferenceValue = nameLabel;
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
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;
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

            // =============================================
            // Compute element sizes to fit within reference width
            // Bottom row: margin + HP orb + gap + 4 skill slots + gap + EXP orb + margin
            // Nav row: 4 nav buttons directly above the skill slots
            // =============================================
            float refWidth = 1080f;
            float margin = 5f;
            float orbToSlotGap = 5f;
            float orbRatio = 1.8f;
            // 2 orbs (orbRatio each) + 4 skill slots (nav buttons moved to right edge)
            float denom = 2f * orbRatio + 4f;
            int slotSize = Mathf.FloorToInt((refWidth - 2f * margin - 2f * orbToSlotGap) / denom);
            int orbSize = Mathf.RoundToInt(orbRatio * slotSize);
            int iconInset = Mathf.RoundToInt(slotSize * 0.09f);
            // No inset — circle mask fills full container to match orb sprite size.
            // The orb sprite on top visually defines the circular boundary.
            int orbInset = 0;

            // =============================================
            // Orb — bottom-left, Conquer Online style
            // =============================================

            // Orb container (anchored bottom-left)
            var orbContainer = new GameObject("Orb_Container", typeof(RectTransform));
            orbContainer.transform.SetParent(safeAreaGO.transform, false);
            var orbContRT = orbContainer.GetComponent<RectTransform>();
            orbContRT.anchorMin = new Vector2(0, 0);
            orbContRT.anchorMax = new Vector2(0, 0);
            orbContRT.pivot = new Vector2(0, 0);
            orbContRT.anchoredPosition = new Vector2(margin, 20);
            orbContRT.sizeDelta = new Vector2(orbSize, orbSize);

            // Circular mask (solid circle) to clip fills — inset to match orb sprite size
            var orbMask = new GameObject("Orb_Mask", typeof(RectTransform));
            orbMask.transform.SetParent(orbContainer.transform, false);
            var orbMaskRT = orbMask.GetComponent<RectTransform>();
            orbMaskRT.anchorMin = Vector2.zero;
            orbMaskRT.anchorMax = Vector2.one;
            orbMaskRT.offsetMin = new Vector2(orbInset, orbInset);
            orbMaskRT.offsetMax = new Vector2(-orbInset, -orbInset);
            var maskImg = orbMask.AddComponent<Image>();
            maskImg.sprite = CreateCircleSprite("OrbMaskCircle", Color.white, 64);
            var mask = orbMask.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            // Dark background fill (visible when HP/MP is depleted)
            var orbDarkBG = new GameObject("Orb_DarkBG", typeof(RectTransform));
            orbDarkBG.transform.SetParent(orbMask.transform, false);
            var orbDarkBGRT = orbDarkBG.GetComponent<RectTransform>();
            orbDarkBGRT.anchorMin = Vector2.zero;
            orbDarkBGRT.anchorMax = Vector2.one;
            orbDarkBGRT.offsetMin = Vector2.zero;
            orbDarkBGRT.offsetMax = Vector2.zero;
            var orbDarkBGImg = orbDarkBG.AddComponent<Image>();
            orbDarkBGImg.color = new Color(0.1f, 0.05f, 0.05f, 1f);

            // HP fill (left half) — anchor-driven height, clipped by circle mask
            var hpFillGO = new GameObject("HP_Fill", typeof(RectTransform));
            hpFillGO.transform.SetParent(orbMask.transform, false);
            var hpFillRT = hpFillGO.GetComponent<RectTransform>();
            hpFillRT.anchorMin = new Vector2(0, 0);
            hpFillRT.anchorMax = new Vector2(0.5f, 1);
            hpFillRT.offsetMin = Vector2.zero;
            hpFillRT.offsetMax = Vector2.zero;
            var hpFillImg = hpFillGO.AddComponent<Image>();
            hpFillImg.color = new Color(0.75f, 0.08f, 0.08f, 1f);

            // MP fill (right half) — anchor-driven height, clipped by circle mask
            var mpFillGO = new GameObject("MP_Fill", typeof(RectTransform));
            mpFillGO.transform.SetParent(orbMask.transform, false);
            var mpFillRT = mpFillGO.GetComponent<RectTransform>();
            mpFillRT.anchorMin = new Vector2(0.5f, 0);
            mpFillRT.anchorMax = new Vector2(1, 1);
            mpFillRT.offsetMin = Vector2.zero;
            mpFillRT.offsetMax = Vector2.zero;
            var mpFillImg = mpFillGO.AddComponent<Image>();
            mpFillImg.color = new Color(0.08f, 0.15f, 0.75f, 1f);

            // Vertical divider line (thin center line)
            var divider = new GameObject("Divider", typeof(RectTransform));
            divider.transform.SetParent(orbMask.transform, false);
            var divRT = divider.GetComponent<RectTransform>();
            divRT.anchorMin = new Vector2(0.5f, 0);
            divRT.anchorMax = new Vector2(0.5f, 1);
            divRT.pivot = new Vector2(0.5f, 0.5f);
            divRT.sizeDelta = new Vector2(2, 0);
            divRT.anchoredPosition = Vector2.zero;
            var divImg = divider.AddComponent<Image>();
            divImg.color = new Color(0.15f, 0.12f, 0.1f, 0.8f);

            // Orb sprite on top — untinted, transparent areas reveal fills behind
            var orbSprite = new GameObject("Orb_Sprite", typeof(RectTransform));
            orbSprite.transform.SetParent(orbContainer.transform, false);
            var orbSpriteRT = orbSprite.GetComponent<RectTransform>();
            orbSpriteRT.anchorMin = Vector2.zero;
            orbSpriteRT.anchorMax = Vector2.one;
            orbSpriteRT.offsetMin = Vector2.zero;
            orbSpriteRT.offsetMax = Vector2.zero;
            var orbSpriteImg = orbSprite.AddComponent<Image>();
            UIAtlasHelper.SetSimpleSprite(orbSpriteImg, "HP_MP_Orb");
            orbSpriteImg.raycastTarget = false;

            // Orb click button (transparent, covers orb for tap detection)
            var orbBtnImg = orbContainer.AddComponent<Image>();
            orbBtnImg.color = new Color(0, 0, 0, 0);
            var orbBtn = orbContainer.AddComponent<Button>();
            orbBtn.targetGraphic = orbBtnImg;

            // HP/MP text above the orb (hidden by default, shown on click)
            var orbTextGO = new GameObject("Orb_Text", typeof(RectTransform));
            orbTextGO.transform.SetParent(orbContainer.transform, false);
            var orbTextRT = orbTextGO.GetComponent<RectTransform>();
            orbTextRT.anchorMin = new Vector2(0.5f, 1f);
            orbTextRT.anchorMax = new Vector2(0.5f, 1f);
            orbTextRT.pivot = new Vector2(0.5f, 0f);
            orbTextRT.anchoredPosition = new Vector2(0, 4);
            orbTextRT.sizeDelta = new Vector2(120, 40);
            var orbTMP = orbTextGO.AddComponent<TextMeshProUGUI>();
            orbTMP.text = "100";
            orbTMP.fontSize = 20;
            orbTMP.color = Color.white;
            orbTMP.fontStyle = FontStyles.Bold;
            orbTMP.alignment = TextAlignmentOptions.Center;
            orbTMP.enableWordWrapping = false;
            orbTMP.overflowMode = TMPro.TextOverflowModes.Overflow;
            orbTextGO.SetActive(false);

            // =============================================
            // Skill Slots — 4 slots right of the orb
            // =============================================
            int slotSpacing = 0;
            float slotStartX = margin + orbSize + orbToSlotGap;
            float slotY = 20;
            Image[] skillIcons = new Image[4];

            for (int i = 0; i < 4; i++)
            {
                // Slot background
                var slotGO = new GameObject($"SkillSlot_{i}", typeof(RectTransform));
                slotGO.transform.SetParent(safeAreaGO.transform, false);
                var slotRT = slotGO.GetComponent<RectTransform>();
                slotRT.anchorMin = new Vector2(0, 0);
                slotRT.anchorMax = new Vector2(0, 0);
                slotRT.pivot = new Vector2(0, 0);
                slotRT.anchoredPosition = new Vector2(slotStartX + i * (slotSize + slotSpacing), slotY);
                slotRT.sizeDelta = new Vector2(slotSize, slotSize);
                var slotImg = slotGO.AddComponent<Image>();
                UIAtlasHelper.SetSimpleSprite(slotImg, "Blank_Slot");

                // Skill icon image (stretch to fill with proportional padding)
                var iconGO = new GameObject("SkillIcon", typeof(RectTransform));
                iconGO.transform.SetParent(slotGO.transform, false);
                var iconRT = iconGO.GetComponent<RectTransform>();
                iconRT.anchorMin = Vector2.zero;
                iconRT.anchorMax = Vector2.one;
                iconRT.offsetMin = new Vector2(iconInset, iconInset);
                iconRT.offsetMax = new Vector2(-iconInset, -iconInset);
                var iconImg = iconGO.AddComponent<Image>();
                iconImg.color = new Color(1f, 1f, 1f, 0f); // invisible until skill assigned
                iconImg.raycastTarget = false;

                skillIcons[i] = iconImg;
            }

            // =============================================
            // Navigation Buttons — above skill slots, same horizontal span
            // =============================================
            int navBtnSize = slotSize;
            int navBtnSpacing = 0;
            float navY = slotY + slotSize + 4; // just above skill slots

            string[] navNames = { "EquipmentButton", "InventoryButton", "MineButton", "MarketButton" };
            string[] navIconNames = { "Equipment_Closed", "Inventory_Closed", "Mining_Closed", "Market_Closed" };

            Button[] navBtns = new Button[4];
            Image[] navIcons = new Image[4];

            for (int i = 0; i < 4; i++)
            {
                var btnGO = new GameObject(navNames[i], typeof(RectTransform));
                btnGO.transform.SetParent(safeAreaGO.transform, false);
                var btnRT = btnGO.GetComponent<RectTransform>();
                btnRT.anchorMin = new Vector2(0, 0);
                btnRT.anchorMax = new Vector2(0, 0);
                btnRT.pivot = new Vector2(0, 0);
                btnRT.anchoredPosition = new Vector2(slotStartX + i * (navBtnSize + navBtnSpacing), navY);
                btnRT.sizeDelta = new Vector2(navBtnSize, navBtnSize);
                var btnImg = btnGO.AddComponent<Image>();
                btnImg.color = new Color(0, 0, 0, 0);
                var btn = btnGO.AddComponent<Button>();
                btn.targetGraphic = btnImg;

                // Icon child
                var iconGO = new GameObject("Icon", typeof(RectTransform));
                iconGO.transform.SetParent(btnGO.transform, false);
                var iconRT = iconGO.GetComponent<RectTransform>();
                iconRT.anchorMin = Vector2.zero;
                iconRT.anchorMax = Vector2.one;
                iconRT.offsetMin = new Vector2(iconInset, iconInset);
                iconRT.offsetMax = new Vector2(-iconInset, -iconInset);
                var iconImg = iconGO.AddComponent<Image>();
                UIAtlasHelper.SetSimpleSprite(iconImg, navIconNames[i]);
                iconImg.raycastTarget = false;

                navBtns[i] = btn;
                navIcons[i] = iconImg;
            }

            // navBtns[0]=Equipment, [1]=Inventory, [2]=Mine, [3]=Market

            // =============================================
            // EXP Orb — right of skill slots, same size as HP orb
            // =============================================
            int expOrbSize = orbSize;
            float expOrbX = slotStartX + 4 * slotSize + orbToSlotGap;
            float expOrbY = slotY; // same baseline as HP orb

            // Container
            var expOrbContainer = new GameObject("EXP_Orb", typeof(RectTransform));
            expOrbContainer.transform.SetParent(safeAreaGO.transform, false);
            var expOrbContRT = expOrbContainer.GetComponent<RectTransform>();
            expOrbContRT.anchorMin = new Vector2(0, 0);
            expOrbContRT.anchorMax = new Vector2(0, 0);
            expOrbContRT.pivot = new Vector2(0, 0);
            expOrbContRT.anchoredPosition = new Vector2(expOrbX, expOrbY);
            expOrbContRT.sizeDelta = new Vector2(expOrbSize, expOrbSize);

            // Circle mask to clip fill — inset to match orb sprite size
            var expOrbMask = new GameObject("EXP_Mask", typeof(RectTransform));
            expOrbMask.transform.SetParent(expOrbContainer.transform, false);
            var expMaskRT = expOrbMask.GetComponent<RectTransform>();
            expMaskRT.anchorMin = Vector2.zero;
            expMaskRT.anchorMax = Vector2.one;
            expMaskRT.offsetMin = new Vector2(orbInset, orbInset);
            expMaskRT.offsetMax = new Vector2(-orbInset, -orbInset);
            var expMaskImg = expOrbMask.AddComponent<Image>();
            expMaskImg.sprite = CreateCircleSprite("OrbMaskCircle", Color.white, 64);
            expOrbMask.AddComponent<UnityEngine.UI.Mask>().showMaskGraphic = false;

            // Dark background (visible when XP is empty)
            var expDarkBG = new GameObject("EXP_DarkBG", typeof(RectTransform));
            expDarkBG.transform.SetParent(expOrbMask.transform, false);
            var expDarkBGRT = expDarkBG.GetComponent<RectTransform>();
            expDarkBGRT.anchorMin = Vector2.zero;
            expDarkBGRT.anchorMax = Vector2.one;
            expDarkBGRT.offsetMin = Vector2.zero;
            expDarkBGRT.offsetMax = Vector2.zero;
            var expDarkBGImg = expDarkBG.AddComponent<Image>();
            expDarkBGImg.color = new Color(0.1f, 0.08f, 0.02f, 1f);

            // Yellow fill — anchor-driven height (bottom-up)
            var expFillGO = new GameObject("XP_Fill", typeof(RectTransform));
            expFillGO.transform.SetParent(expOrbMask.transform, false);
            var expFillRT = expFillGO.GetComponent<RectTransform>();
            expFillRT.anchorMin = Vector2.zero;
            expFillRT.anchorMax = new Vector2(1, 0); // starts empty
            expFillRT.offsetMin = Vector2.zero;
            expFillRT.offsetMax = Vector2.zero;
            var xpFillImg = expFillGO.AddComponent<Image>();
            xpFillImg.color = new Color(1f, 0.84f, 0f, 1f); // yellow

            // Orb sprite on top — untinted, transparent areas reveal yellow fill
            var expOrbSprite = new GameObject("EXP_Sprite", typeof(RectTransform));
            expOrbSprite.transform.SetParent(expOrbContainer.transform, false);
            var expSpriteRT = expOrbSprite.GetComponent<RectTransform>();
            expSpriteRT.anchorMin = Vector2.zero;
            expSpriteRT.anchorMax = Vector2.one;
            expSpriteRT.offsetMin = Vector2.zero;
            expSpriteRT.offsetMax = Vector2.zero;
            var expSpriteImg = expOrbSprite.AddComponent<Image>();
            UIAtlasHelper.SetSimpleSprite(expSpriteImg, "Exp_Orb");
            expSpriteImg.raycastTarget = false;

            // EXP orb click button (transparent, covers orb for tap detection)
            var expBtnImg = expOrbContainer.AddComponent<Image>();
            expBtnImg.color = new Color(0, 0, 0, 0);
            var expBtn = expOrbContainer.AddComponent<Button>();
            expBtn.targetGraphic = expBtnImg;

            // XP text above the EXP orb (hidden by default, shown on click)
            var expXpTextGO = new GameObject("XP_Text", typeof(RectTransform));
            expXpTextGO.transform.SetParent(expOrbContainer.transform, false);
            var expXpTextRT = expXpTextGO.GetComponent<RectTransform>();
            expXpTextRT.anchorMin = new Vector2(0.5f, 1f);
            expXpTextRT.anchorMax = new Vector2(0.5f, 1f);
            expXpTextRT.pivot = new Vector2(0.5f, 0f);
            expXpTextRT.anchoredPosition = new Vector2(0, 4);
            expXpTextRT.sizeDelta = new Vector2(120, 28);
            var expXpTMP = expXpTextGO.AddComponent<TextMeshProUGUI>();
            expXpTMP.text = "0";
            expXpTMP.fontSize = 20;
            expXpTMP.color = Color.white;
            expXpTMP.fontStyle = FontStyles.Bold;
            expXpTMP.alignment = TextAlignmentOptions.Center;
            expXpTMP.enableWordWrapping = false;
            expXpTextGO.SetActive(false);

            // Level badge above the EXP orb (hidden by default, shown on click)
            var expLvBadge = new GameObject("Level_Badge", typeof(RectTransform));
            expLvBadge.transform.SetParent(expOrbContainer.transform, false);
            var expLvBadgeRT = expLvBadge.GetComponent<RectTransform>();
            expLvBadgeRT.anchorMin = new Vector2(0.5f, 1f);
            expLvBadgeRT.anchorMax = new Vector2(0.5f, 1f);
            expLvBadgeRT.pivot = new Vector2(0.5f, 0f);
            expLvBadgeRT.anchoredPosition = new Vector2(0, 32);
            expLvBadgeRT.sizeDelta = new Vector2(80, 28);
            var expLvTMP = expLvBadge.AddComponent<TextMeshProUGUI>();
            expLvTMP.text = "Lv.1";
            expLvTMP.fontSize = 20;
            expLvTMP.color = new Color(1f, 0.84f, 0f, 1f);
            expLvTMP.fontStyle = FontStyles.Bold;
            expLvTMP.alignment = TextAlignmentOptions.Center;
            expLvBadge.SetActive(false);

            // XP gain popup anchor — sits above level badge
            var xpGainAnchor = new GameObject("XP_Gain_Anchor", typeof(RectTransform));
            xpGainAnchor.transform.SetParent(expOrbContainer.transform, false);
            var xpGainAnchorRT = xpGainAnchor.GetComponent<RectTransform>();
            xpGainAnchorRT.anchorMin = new Vector2(0.5f, 1f);
            xpGainAnchorRT.anchorMax = new Vector2(0.5f, 1f);
            xpGainAnchorRT.pivot = new Vector2(0.5f, 0f);
            xpGainAnchorRT.anchoredPosition = new Vector2(0, 64);
            xpGainAnchorRT.sizeDelta = new Vector2(200, 30);

            // =============================================
            // Revive Overlay — centered banner rectangle, hidden by default
            // =============================================
            var reviveOverlay = new GameObject("ReviveOverlay", typeof(RectTransform));
            reviveOverlay.transform.SetParent(canvasGO.transform, false);
            var reviveRT = reviveOverlay.GetComponent<RectTransform>();
            reviveRT.anchorMin = new Vector2(0.1f, 0.44f);
            reviveRT.anchorMax = new Vector2(0.9f, 0.56f);
            reviveRT.offsetMin = Vector2.zero;
            reviveRT.offsetMax = Vector2.zero;
            var reviveBgImg = reviveOverlay.AddComponent<Image>();
            reviveBgImg.color = new Color(0f, 0f, 0f, 0.85f);
            reviveBgImg.raycastTarget = false;

            var reviveTextGO = new GameObject("ReviveTimerText", typeof(RectTransform));
            reviveTextGO.transform.SetParent(reviveOverlay.transform, false);
            var reviveTextRT = reviveTextGO.GetComponent<RectTransform>();
            reviveTextRT.anchorMin = Vector2.zero;
            reviveTextRT.anchorMax = Vector2.one;
            reviveTextRT.offsetMin = Vector2.zero;
            reviveTextRT.offsetMax = Vector2.zero;
            var reviveTMP = reviveTextGO.AddComponent<TextMeshProUGUI>();
            reviveTMP.text = "Reviving in 2:00";
            reviveTMP.fontSize = 38;
            reviveTMP.color = new Color(1f, 0.3f, 0.3f, 1f);
            reviveTMP.fontStyle = FontStyles.Bold;
            reviveTMP.alignment = TextAlignmentOptions.Center;
            reviveTMP.raycastTarget = false;

            reviveOverlay.SetActive(false);

            // =============================================
            // Wire PlayerHUD
            // =============================================
            var hud = canvasGO.AddComponent<PlayerHUD>();
            var hudSO = new SerializedObject(hud);
            hudSO.FindProperty("_hpFill").objectReferenceValue = hpFillImg;
            hudSO.FindProperty("_mpFill").objectReferenceValue = mpFillImg;
            hudSO.FindProperty("_orbText").objectReferenceValue = orbTMP;
            hudSO.FindProperty("_xpFill").objectReferenceValue = xpFillImg;
            hudSO.FindProperty("_xpText").objectReferenceValue = expXpTMP;
            hudSO.FindProperty("_levelText").objectReferenceValue = expLvTMP;

            hudSO.FindProperty("_reviveOverlay").objectReferenceValue = reviveOverlay;
            hudSO.FindProperty("_reviveTimerText").objectReferenceValue = reviveTMP;
            var skillIconsProp = hudSO.FindProperty("_skillSlotIcons");
            skillIconsProp.arraySize = 4;
            for (int i = 0; i < 4; i++)
                skillIconsProp.GetArrayElementAtIndex(i).objectReferenceValue = skillIcons[i];
            hudSO.FindProperty("_orbButton").objectReferenceValue = orbBtn;
            hudSO.FindProperty("_expOrbButton").objectReferenceValue = expBtn;
            hudSO.FindProperty("_orbTextGO").objectReferenceValue = orbTextGO;
            hudSO.FindProperty("_expTextGO").objectReferenceValue = expXpTextGO;
            hudSO.FindProperty("_levelTextGO").objectReferenceValue = expLvBadge;
            hudSO.FindProperty("_xpGainAnchor").objectReferenceValue = xpGainAnchorRT;
            hudSO.FindProperty("_equipmentButton").objectReferenceValue = navBtns[0];
            hudSO.FindProperty("_inventoryButton").objectReferenceValue = navBtns[1];
            hudSO.FindProperty("_mineButton").objectReferenceValue = navBtns[2];
            hudSO.FindProperty("_marketButton").objectReferenceValue = navBtns[3];
            hudSO.FindProperty("_equipmentIcon").objectReferenceValue = navIcons[0];
            hudSO.FindProperty("_inventoryIcon").objectReferenceValue = navIcons[1];
            hudSO.FindProperty("_mineIcon").objectReferenceValue = navIcons[2];
            hudSO.FindProperty("_marketIcon").objectReferenceValue = navIcons[3];
            hudSO.ApplyModifiedPropertiesWithoutUndo();

            return hud;
        }

        /// <summary>
        /// Creates a ring (hollow circle) sprite for the orb frame, saved to disk.
        /// </summary>
        private static Sprite CreateRingSprite(string name, Color color, int size, int thickness)
        {
            string path = $"Assets/Visual/Sprites/{name}.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (existing != null) return existing;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.filterMode = FilterMode.Point;
            var pixels = new Color[size * size];
            float center = size * 0.5f;
            float outerR = center;
            float innerR = center - thickness;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = x - center + 0.5f;
                    float dy = y - center + 0.5f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    if (dist <= outerR && dist >= innerR)
                        pixels[y * size + x] = color;
                    else
                        pixels[y * size + x] = Color.clear;
                }
            }

            tex.SetPixels(pixels);
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
            scaler.matchWidthOrHeight = 0f;

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
            scaler.matchWidthOrHeight = 0f;
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

            // Always recreate to pick up color changes
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
