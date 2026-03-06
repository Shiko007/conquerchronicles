using UnityEngine;
using UnityEditor;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Core.Save;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Save;

namespace ConquerChronicles.Editor
{
    public class DebugXPWindow : EditorWindow
    {
        private long _xpAmount = 1000;
        private int _goldAmount = 1000;
        private int _upgradeLevel;
        private int _gemTier = 1;
        private GemType _gemType;
        private Vector2 _scrollPos;

        // Equipment catalog (built once per play session)
        private EquipmentData[] _allEquipment;
        private string[] _equipmentNames;

        [MenuItem("Conquer Chronicles/Debug Tools")]
        private static void Open()
        {
            GetWindow<DebugXPWindow>("Debug Tools");
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play mode first.", MessageType.Info);
                return;
            }

            var player = Object.FindAnyObjectByType<CharacterView>();
            if (player == null || player.State == null)
            {
                EditorGUILayout.HelpBox("No CharacterView found in scene.", MessageType.Warning);
                return;
            }

            // Build equipment catalog once
            if (_allEquipment == null)
            {
                _allEquipment = TestEquipment.GetAll();
                _equipmentNames = new string[_allEquipment.Length];
                for (int i = 0; i < _allEquipment.Length; i++)
                    _equipmentNames[i] = $"{_allEquipment[i].Name} (Lv{_allEquipment[i].RequiredLevel} {_allEquipment[i].Quality})";
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            var state = player.State;

            // === XP Section ===
            EditorGUILayout.LabelField("XP & Level", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Level", state.Level.ToString());
            EditorGUILayout.LabelField("XP", $"{state.XP} / {LevelUpTable.GetRequiredXP(state.Level)}");
            EditorGUILayout.LabelField("Stat Points", state.StatPointsAvailable.ToString());

            _xpAmount = EditorGUILayout.LongField("XP to Add", _xpAmount);
            if (GUILayout.Button($"Add {_xpAmount} XP"))
                player.GainXP(_xpAmount);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 1 Level"))
            {
                long needed = LevelUpTable.GetRequiredXP(state.Level) - state.XP;
                if (needed > 0) player.GainXP(needed);
            }
            if (GUILayout.Button("Add 10 Levels"))
            {
                for (int i = 0; i < 10; i++)
                {
                    long needed = LevelUpTable.GetRequiredXP(state.Level) - state.XP;
                    if (needed > 0) player.GainXP(needed);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // === Inventory Section ===
            var saveManager = SaveSystemBridge.GetOrCreate();
            var saveData = saveManager.LoadGame();
            if (saveData == null)
            {
                EditorGUILayout.HelpBox("No save data found.", MessageType.Warning);
                EditorGUILayout.EndScrollView();
                return;
            }

            // Rebuild inventory from save to show bag count
            int bagCount = saveData.BagItems != null ? saveData.BagItems.Length : 0;

            EditorGUILayout.LabelField("Inventory", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Bag", $"{bagCount} / {InventoryState.BagCapacity}");
            EditorGUILayout.LabelField("Gold", saveData.Gold.ToString());

            // Gold
            _goldAmount = EditorGUILayout.IntField("Gold to Add", _goldAmount);
            if (GUILayout.Button($"Add {_goldAmount} Gold"))
            {
                saveData.Gold += _goldAmount;
                saveManager.SaveGame(saveData);
            }

            EditorGUILayout.Space(10);

            // === Add Equipment ===
            EditorGUILayout.LabelField("Add Equipment to Bag", EditorStyles.boldLabel);

            _upgradeLevel = EditorGUILayout.IntSlider("Upgrade Level", _upgradeLevel, 0, 12);

            for (int i = 0; i < _allEquipment.Length; i++)
            {
                var eq = _allEquipment[i];
                if (GUILayout.Button($"+ {eq.Name}  (Lv{eq.RequiredLevel} {eq.Quality} {eq.Slot})"))
                {
                    AddEquipmentToSave(saveManager, eq, _upgradeLevel);
                }
            }

            EditorGUILayout.Space(10);

            // === Add Gems ===
            EditorGUILayout.LabelField("Add Gems to Bag", EditorStyles.boldLabel);

            _gemType = (GemType)EditorGUILayout.EnumPopup("Gem Type", _gemType);
            _gemTier = EditorGUILayout.IntSlider("Gem Tier", _gemTier, 1, 9);

            if (GUILayout.Button($"+ {_gemType} Gem (Tier {_gemTier})"))
            {
                AddGemToSave(saveManager, _gemType, _gemTier);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add All Gem Types (Tier 1)"))
            {
                var types = System.Enum.GetValues(typeof(GemType));
                foreach (GemType t in types)
                    AddGemToSave(saveManager, t, 1);
            }
            if (GUILayout.Button($"Add All Gem Types (Tier {_gemTier})"))
            {
                var types = System.Enum.GetValues(typeof(GemType));
                foreach (GemType t in types)
                    AddGemToSave(saveManager, t, _gemTier);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // === Clear Bag ===
            EditorGUILayout.LabelField("Danger Zone", EditorStyles.boldLabel);
            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("Clear Entire Bag"))
            {
                var save = saveManager.LoadGame();
                if (save != null)
                {
                    save.BagItems = new SerializedBagItem[0];
                    saveManager.SaveGame(save);
                    Debug.Log("[Debug] Bag cleared.");
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndScrollView();
            Repaint();
        }

        private static void AddEquipmentToSave(SaveManager saveManager, EquipmentData data, int upgradeLevel)
        {
            var save = saveManager.LoadGame();
            if (save == null) return;

            int currentCount = save.BagItems != null ? save.BagItems.Length : 0;
            if (currentCount >= InventoryState.BagCapacity)
            {
                Debug.LogWarning("[Debug] Bag is full!");
                return;
            }

            var serialized = new SerializedEquipment
            {
                DataID = data.ID,
                UpgradeLevel = upgradeLevel,
                Gems = new SerializedGem[data.MaxSockets]
            };

            var newBag = new SerializedBagItem[currentCount + 1];
            if (save.BagItems != null)
                System.Array.Copy(save.BagItems, newBag, currentCount);
            newBag[currentCount] = SerializedBagItem.FromEquipment(serialized);
            save.BagItems = newBag;

            saveManager.SaveGame(save);
            Debug.Log($"[Debug] Added {data.Name} +{upgradeLevel} to bag.");
        }

        private static void AddGemToSave(SaveManager saveManager, GemType type, int tier)
        {
            var save = saveManager.LoadGame();
            if (save == null) return;

            int currentCount = save.BagItems != null ? save.BagItems.Length : 0;
            if (currentCount >= InventoryState.BagCapacity)
            {
                Debug.LogWarning("[Debug] Bag is full!");
                return;
            }

            var gem = new SerializedGem { Type = (int)type, Tier = tier };
            var newBag = new SerializedBagItem[currentCount + 1];
            if (save.BagItems != null)
                System.Array.Copy(save.BagItems, newBag, currentCount);
            newBag[currentCount] = SerializedBagItem.FromGem(gem);
            save.BagItems = newBag;

            saveManager.SaveGame(save);
            Debug.Log($"[Debug] Added {type} Gem (Tier {tier}) to bag.");
        }
    }
}
