using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Enemy;

namespace ConquerChronicles.Core.Map
{
    /// <summary>
    /// Hardcoded test maps for development. Will be replaced by ScriptableObjects.
    /// Contains all enemy definitions, 3 maps with 3 areas each, and utility methods.
    /// </summary>
    public static class TestMaps
    {
        // =================================================================
        // Enemy Definitions
        // =================================================================

        // --- Slime Family ---

        public static EnemyData GreenSlime => new()
        {
            ID = "slime_green",
            Name = "Green Slime",
            Stats = new CharacterStats { HP = 50, ATK = 5, DEF = 2 },
            MoveSpeed = 1.2f,
            AttackRange = 0.5f,
            AttackCooldown = 1.5f,
            XPReward = 8,
            GoldReward = 3,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_iron", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_iron", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData RedSlime => new()
        {
            ID = "slime_red",
            Name = "Red Slime",
            Stats = new CharacterStats { HP = 60, ATK = 10, DEF = 4 },
            MoveSpeed = 1.4f,
            AttackRange = 0.5f,
            AttackCooldown = 1.3f,
            XPReward = 15,
            GoldReward = 5,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "armor_iron", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_iron_blade", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_copper", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData BlueSlime => new()
        {
            ID = "slime_blue",
            Name = "Blue Slime",
            Stats = new CharacterStats { HP = 100, ATK = 15, DEF = 8 },
            MoveSpeed = 1.6f,
            AttackRange = 0.5f,
            AttackCooldown = 1.2f,
            XPReward = 25,
            GoldReward = 8,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "weapon_iron_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_wooden_shield", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_copper", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "helm_steel", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Skeleton Family ---

        public static EnemyData Skeleton => new()
        {
            ID = "skeleton",
            Name = "Skeleton",
            Stats = new CharacterStats { HP = 150, ATK = 25, DEF = 12 },
            MoveSpeed = 1.3f,
            AttackRange = 0.8f,
            AttackCooldown = 1.0f,
            XPReward = 35,
            GoldReward = 12,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_silver", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData SkeletonArcher => new()
        {
            ID = "skeleton_archer",
            Name = "Skeleton Archer",
            Stats = new CharacterStats { HP = 120, ATK = 30, DEF = 8 },
            MoveSpeed = 1.1f,
            AttackRange = 3f,
            AttackCooldown = 1.2f,
            XPReward = 45,
            GoldReward = 15,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "weapon_steel_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_steel_shield", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_silver", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData SkeletonKnight => new()
        {
            ID = "skeleton_knight",
            Name = "Skeleton Knight",
            Stats = new CharacterStats { HP = 250, ATK = 40, DEF = 25 },
            MoveSpeed = 1.0f,
            AttackRange = 1.0f,
            AttackCooldown = 1.5f,
            XPReward = 60,
            GoldReward = 20,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_steel", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_steel", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_steel_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "helm_dragon", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_dragon", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Dark Family ---

        public static EnemyData DarkAcolyte => new()
        {
            ID = "dark_acolyte",
            Name = "Dark Acolyte",
            Stats = new CharacterStats { HP = 200, MATK = 45, DEF = 15, MDEF = 20 },
            MoveSpeed = 1.2f,
            AttackRange = 2.0f,
            AttackCooldown = 1.3f,
            XPReward = 75,
            GoldReward = 25,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_jade", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_jade", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData DarkKnight => new()
        {
            ID = "dark_knight",
            Name = "Dark Knight",
            Stats = new CharacterStats { HP = 400, ATK = 60, DEF = 40, MDEF = 10 },
            MoveSpeed = 0.9f,
            AttackRange = 1.0f,
            AttackCooldown = 1.5f,
            XPReward = 100,
            GoldReward = 35,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "helm_phoenix", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_phoenix", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // =================================================================
        // All Enemies
        // =================================================================

        public static EnemyData[] AllEnemies => new[]
        {
            GreenSlime, RedSlime, BlueSlime,
            Skeleton, SkeletonArcher, SkeletonKnight,
            DarkAcolyte, DarkKnight
        };

        // =================================================================
        // Map 1: Slime Fields (Lv 1-15)
        // =================================================================

        public static MapData SlimeFields => new()
        {
            ID = "map_slime_fields",
            Name = "Slime Fields",
            Description = "Rolling green plains infested with slimes of every hue.",
            MinLevel = 1,
            MaxLevel = 15,
            BackgroundID = "bg_slime_fields",
            Areas = new[]
            {
                // Area 1: Outskirts (Lv 1-5)
                new AreaData
                {
                    ID = "area_slime_outskirts",
                    Name = "Outskirts",
                    MinLevel = 1,
                    MaxLevel = 5,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 8,
                    SpawnInterval = 2.5f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "slime_green", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_iron", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_iron", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_iron_blade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_iron", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Heartlands (Lv 5-10)
                new AreaData
                {
                    ID = "area_slime_heartlands",
                    Name = "Heartlands",
                    MinLevel = 5,
                    MaxLevel = 10,
                    XPMultiplier = 1.2f,
                    GoldMultiplier = 1.1f,
                    MaxConcurrentEnemies = 10,
                    SpawnInterval = 2.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "slime_green", Weight = 0.6f },
                        new EnemySpawnWeight { EnemyID = "slime_red", Weight = 0.4f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_iron_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_wooden_shield", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_copper", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_copper", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Deep Marshes (Lv 10-15)
                new AreaData
                {
                    ID = "area_slime_deep_marshes",
                    Name = "Deep Marshes",
                    MinLevel = 10,
                    MaxLevel = 15,
                    XPMultiplier = 1.5f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 12,
                    SpawnInterval = 1.8f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "slime_red", Weight = 0.5f },
                        new EnemySpawnWeight { EnemyID = "slime_blue", Weight = 0.3f },
                        new EnemySpawnWeight { EnemyID = "slime_green", Weight = 0.2f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_iron_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_iron", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_steel", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_steel", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_steel_blade", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // Map 2: Bone Yard (Lv 15-30)
        // =================================================================

        public static MapData BoneYard => new()
        {
            ID = "map_bone_yard",
            Name = "Bone Yard",
            Description = "An ancient graveyard where the dead refuse to rest.",
            MinLevel = 15,
            MaxLevel = 30,
            BackgroundID = "bg_bone_yard",
            Areas = new[]
            {
                // Area 1: Outer Graves (Lv 15-20)
                new AreaData
                {
                    ID = "area_bone_outer_graves",
                    Name = "Outer Graves",
                    MinLevel = 15,
                    MaxLevel = 20,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 14,
                    SpawnInterval = 1.8f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "skeleton", Weight = 0.7f },
                        new EnemySpawnWeight { EnemyID = "skeleton_archer", Weight = 0.3f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_steel", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_steel", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_steel_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_steel", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_steel_shield", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_silver", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_silver", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Crypt Halls (Lv 20-25)
                new AreaData
                {
                    ID = "area_bone_crypt_halls",
                    Name = "Crypt Halls",
                    MinLevel = 20,
                    MaxLevel = 25,
                    XPMultiplier = 1.3f,
                    GoldMultiplier = 1.2f,
                    MaxConcurrentEnemies = 16,
                    SpawnInterval = 1.5f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "skeleton", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "skeleton_archer", Weight = 0.3f },
                        new EnemySpawnWeight { EnemyID = "skeleton_knight", Weight = 0.3f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_steel_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_steel", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_dragon", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_dragon", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Inner Sanctum (Lv 25-30)
                new AreaData
                {
                    ID = "area_bone_inner_sanctum",
                    Name = "Inner Sanctum",
                    MinLevel = 25,
                    MaxLevel = 30,
                    XPMultiplier = 1.6f,
                    GoldMultiplier = 1.4f,
                    MaxConcurrentEnemies = 18,
                    SpawnInterval = 1.3f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "skeleton_knight", Weight = 0.5f },
                        new EnemySpawnWeight { EnemyID = "skeleton_archer", Weight = 0.5f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_jade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_jade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // Map 3: Dark Tower (Lv 30-50)
        // =================================================================

        public static MapData DarkTower => new()
        {
            ID = "map_dark_tower",
            Name = "Dark Tower",
            Description = "A towering spire of shadow where dark forces gather.",
            MinLevel = 30,
            MaxLevel = 50,
            BackgroundID = "bg_dark_tower",
            Areas = new[]
            {
                // Area 1: Lower Floors (Lv 30-35)
                new AreaData
                {
                    ID = "area_tower_lower_floors",
                    Name = "Lower Floors",
                    MinLevel = 30,
                    MaxLevel = 35,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 18,
                    SpawnInterval = 1.2f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "dark_acolyte", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_dragon", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Mid Halls (Lv 35-45)
                new AreaData
                {
                    ID = "area_tower_mid_halls",
                    Name = "Mid Halls",
                    MinLevel = 35,
                    MaxLevel = 45,
                    XPMultiplier = 1.4f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 20,
                    SpawnInterval = 1.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "dark_acolyte", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "dark_knight", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_dragon_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_dragon", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_phoenix", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_phoenix", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Apex (Lv 45-50)
                new AreaData
                {
                    ID = "area_tower_apex",
                    Name = "Apex",
                    MinLevel = 45,
                    MaxLevel = 50,
                    XPMultiplier = 1.8f,
                    GoldMultiplier = 1.5f,
                    MaxConcurrentEnemies = 24,
                    SpawnInterval = 1.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "dark_knight", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // All Maps
        // =================================================================

        public static MapData[] AllMaps => new[] { SlimeFields, BoneYard, DarkTower };

        // =================================================================
        // Utility Methods
        // =================================================================

        /// <summary>
        /// Looks up a map by its ID. Returns default if not found.
        /// </summary>
        public static MapData GetByID(string id)
        {
            var maps = AllMaps;
            for (int i = 0; i < maps.Length; i++)
            {
                if (maps[i].ID == id)
                    return maps[i];
            }
            return default;
        }

        /// <summary>
        /// Searches all maps for an area with the given ID. Returns default if not found.
        /// </summary>
        public static AreaData GetAreaByID(string areaID)
        {
            var maps = AllMaps;
            for (int i = 0; i < maps.Length; i++)
            {
                var areas = maps[i].Areas;
                if (areas == null) continue;
                for (int j = 0; j < areas.Length; j++)
                {
                    if (areas[j].ID == areaID)
                        return areas[j];
                }
            }
            return default;
        }

        /// <summary>
        /// Selects a random enemy ID from a weighted pool using the provided RNG.
        /// Weights do not need to sum to 1.0; they are normalized internally.
        /// </summary>
        public static string PickRandomEnemy(EnemySpawnWeight[] pool, System.Random rng)
        {
            if (pool == null || pool.Length == 0)
                return null;

            float totalWeight = 0f;
            for (int i = 0; i < pool.Length; i++)
                totalWeight += pool[i].Weight;

            float roll = (float)rng.NextDouble() * totalWeight;
            float cumulative = 0f;

            for (int i = 0; i < pool.Length; i++)
            {
                cumulative += pool[i].Weight;
                if (roll <= cumulative)
                    return pool[i].EnemyID;
            }

            // Fallback: return the last entry
            return pool[pool.Length - 1].EnemyID;
        }
    }
}
