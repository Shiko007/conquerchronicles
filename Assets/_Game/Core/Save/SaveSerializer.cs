using System.Collections.Generic;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Equipment;
using ConquerChronicles.Core.Inventory;
using ConquerChronicles.Core.MetaProgression;

namespace ConquerChronicles.Core.Save
{
    /// <summary>
    /// Static helper to convert between live game state objects and the flat SaveData structure.
    /// No JSON serialization happens here — this is purely an object mapper.
    /// The actual JSON encode/decode is handled by the Gameplay layer via SaveManager delegates.
    /// </summary>
    public static class SaveSerializer
    {
        /// <summary>
        /// Captures the current live game state into a SaveData snapshot.
        /// </summary>
        public static SaveData CaptureState(
            CharacterState character,
            InventoryState inventory,
            MetaProgressionState meta,
            string[] completedStages,
            int[] stageStars)
        {
            var data = new SaveData
            {
                Version = 1,

                // Character
                SelectedClass = character.Class,
                CharacterLevel = character.Level,
                CharacterXP = character.XP,
                StatPointsAvailable = character.StatPointsAvailable,
                Vitality = character.Vitality,
                Mana = character.Mana,
                Strength = character.Strength,
                Agility = character.Agility,
                Spirit = character.Spirit,

                // Inventory
                Gold = inventory.Gold,
                EquippedItems = SerializeEquippedItems(inventory.EquippedItems),
                BagItems = SerializeBagItems(inventory.Bag),
                GemBag = SerializeGemBag(inventory.GemBag),

                // Stages
                CompletedStageIDs = completedStages ?? System.Array.Empty<string>(),
                CompletedStageStars = stageStars ?? System.Array.Empty<int>(),

                // Meta
                MetaCurrency = meta.MetaCurrency,
                MetaUpgradeLevels = (int[])meta.UpgradeLevels.Clone(),

                // Mining (default — set externally if needed)
                MiningStartTimestamp = 0,
                ActiveMineID = string.Empty,

                // Market (default — set externally if needed)
                PlayerBoothItemIDs = System.Array.Empty<string>(),
                PlayerBoothPrices = System.Array.Empty<int>(),
                BoothRevenue = 0
            };

            return data;
        }

        /// <summary>
        /// Rebuilds live game state objects from a SaveData snapshot.
        /// Equipment instances are reconstructed by looking up DataIDs in the provided catalog.
        /// Items with unknown DataIDs are silently skipped.
        /// </summary>
        public static void RestoreState(
            SaveData data,
            Dictionary<string, EquipmentData> equipmentCatalog,
            CharacterStats baseStats,
            CharacterStats perLevelGrowth,
            out CharacterState character,
            out InventoryState inventory,
            out MetaProgressionState meta)
        {
            // Restore character
            character = new CharacterState(data.SelectedClass, baseStats, perLevelGrowth);
            character.Level = data.CharacterLevel;
            character.XP = data.CharacterXP;
            character.StatPointsAvailable = data.StatPointsAvailable;
            character.Vitality = data.Vitality;
            character.Mana = data.Mana;
            character.Strength = data.Strength;
            character.Agility = data.Agility;
            character.Spirit = data.Spirit;

            // Recompute HP/MP from restored stats
            var computed = character.ComputeStats();
            character.CurrentHP = computed.HP;
            character.CurrentMP = computed.MP;

            // Restore inventory
            inventory = new InventoryState();
            inventory.Gold = data.Gold;

            // Restore equipped items
            if (data.EquippedItems != null)
            {
                for (int i = 0; i < data.EquippedItems.Length && i < InventoryState.EquipmentSlotCount; i++)
                {
                    var serialized = data.EquippedItems[i];
                    if (!serialized.IsEmpty)
                    {
                        var instance = DeserializeEquipment(serialized, equipmentCatalog);
                        if (instance != null)
                        {
                            inventory.EquippedItems[i] = instance;
                        }
                    }
                }
            }

            // Restore bag items
            if (data.BagItems != null)
            {
                for (int i = 0; i < data.BagItems.Length; i++)
                {
                    var instance = DeserializeEquipment(data.BagItems[i], equipmentCatalog);
                    if (instance != null)
                    {
                        inventory.AddToBag(instance);
                    }
                }
            }

            // Restore gem bag
            if (data.GemBag != null)
            {
                for (int i = 0; i < data.GemBag.Length; i++)
                {
                    var gem = new GemData((GemType)data.GemBag[i].Type, data.GemBag[i].Tier);
                    inventory.AddGem(gem);
                }
            }

            // Restore meta progression
            meta = new MetaProgressionState();
            meta.MetaCurrency = data.MetaCurrency;

            if (data.MetaUpgradeLevels != null)
            {
                int count = System.Math.Min(data.MetaUpgradeLevels.Length, MetaProgressionState.UpgradeTypeCount);
                for (int i = 0; i < count; i++)
                {
                    meta.UpgradeLevels[i] = data.MetaUpgradeLevels[i];
                }
            }
        }

        // --- Serialization Helpers ---

        private static SerializedEquipment[] SerializeEquippedItems(EquipmentInstance[] equipped)
        {
            var result = new SerializedEquipment[equipped.Length];

            for (int i = 0; i < equipped.Length; i++)
            {
                if (equipped[i] != null)
                {
                    result[i] = SerializeEquipmentInstance(equipped[i]);
                }
                else
                {
                    result[i] = new SerializedEquipment
                    {
                        DataID = string.Empty,
                        UpgradeLevel = 0,
                        Gems = System.Array.Empty<SerializedGem>()
                    };
                }
            }

            return result;
        }

        private static SerializedEquipment[] SerializeBagItems(List<EquipmentInstance> bag)
        {
            var result = new SerializedEquipment[bag.Count];

            for (int i = 0; i < bag.Count; i++)
            {
                result[i] = SerializeEquipmentInstance(bag[i]);
            }

            return result;
        }

        private static SerializedEquipment SerializeEquipmentInstance(EquipmentInstance instance)
        {
            var gems = System.Array.Empty<SerializedGem>();

            if (instance.SocketedGems != null && instance.SocketedGems.Length > 0)
            {
                gems = new SerializedGem[instance.SocketedGems.Length];
                for (int i = 0; i < instance.SocketedGems.Length; i++)
                {
                    gems[i] = new SerializedGem
                    {
                        Type = (int)instance.SocketedGems[i].Type,
                        Tier = instance.SocketedGems[i].Tier
                    };
                }
            }

            return new SerializedEquipment
            {
                DataID = instance.Data.ID,
                UpgradeLevel = instance.UpgradeLevel,
                Gems = gems
            };
        }

        private static SerializedGem[] SerializeGemBag(List<GemData> gemBag)
        {
            var result = new SerializedGem[gemBag.Count];

            for (int i = 0; i < gemBag.Count; i++)
            {
                result[i] = new SerializedGem
                {
                    Type = (int)gemBag[i].Type,
                    Tier = gemBag[i].Tier
                };
            }

            return result;
        }

        // --- Deserialization Helpers ---

        private static EquipmentInstance DeserializeEquipment(
            SerializedEquipment serialized,
            Dictionary<string, EquipmentData> catalog)
        {
            if (serialized.IsEmpty)
                return null;

            if (!catalog.TryGetValue(serialized.DataID, out var equipmentData))
                return null;

            var instance = new EquipmentInstance(equipmentData);
            instance.UpgradeLevel = System.Math.Clamp(serialized.UpgradeLevel, 0, 12);

            // Restore socketed gems
            if (serialized.Gems != null)
            {
                for (int i = 0; i < serialized.Gems.Length && i < instance.SocketedGems.Length; i++)
                {
                    if (serialized.Gems[i].Tier > 0)
                    {
                        var gem = new GemData((GemType)serialized.Gems[i].Type, serialized.Gems[i].Tier);
                        instance.SocketedGems[i] = gem;
                    }
                }
            }

            return instance;
        }
    }
}
