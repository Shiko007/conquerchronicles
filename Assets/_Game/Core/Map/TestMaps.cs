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

        // --- Rat Family ---

        public static EnemyData Rat => new()
        {
            ID = "rat",
            Name = "Rat",
            Stats = new CharacterStats { HP = 500, ATK = 5, DEF = 2 },
            MoveSpeed = 1.2f,
            AttackRange = 0.6f,
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

        public static EnemyData InfectedRat => new()
        {
            ID = "rat_infected",
            Name = "Infected Rat",
            TintR = 0.6f, TintG = 1f, TintB = 0.6f, TintA = 1f, Scale = 1.2f,
            Stats = new CharacterStats { HP = 600, ATK = 10, DEF = 4 },
            MoveSpeed = 1.4f,
            AttackRange = 0.6f,
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

        public static EnemyData BloodyRat => new()
        {
            ID = "rat_bloody",
            Name = "Bloody Rat",
            TintR = 1f, TintG = 0.5f, TintB = 0.5f, TintA = 1f, Scale = 1.5f,
            Stats = new CharacterStats { HP = 1000, ATK = 15, DEF = 8 },
            MoveSpeed = 1.6f,
            AttackRange = 0.6f,
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
            Stats = new CharacterStats { HP = 1500, ATK = 25, DEF = 12 },
            MoveSpeed = 1.3f,
            AttackRange = 0.6f,
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

        public static EnemyData Gladiator => new()
        {
            ID = "gladiator",
            Name = "Gladiator",
            Stats = new CharacterStats { HP = 1200, ATK = 30, DEF = 8 },
            MoveSpeed = 1.1f,
            AttackRange = 0.7f,
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
            Stats = new CharacterStats { HP = 2500, ATK = 40, DEF = 25 },
            MoveSpeed = 1.0f,
            AttackRange = 0.6f,
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

        // --- Volcano Family ---

        public static EnemyData FireImp => new()
        {
            ID = "fire_imp",
            Name = "Fire Imp",
            Stats = new CharacterStats { HP = 6000, ATK = 80, DEF = 50 },
            MoveSpeed = 1.5f,
            AttackRange = 0.6f,
            AttackCooldown = 1.2f,
            XPReward = 140,
            GoldReward = 45,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_phoenix", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_phoenix", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_phoenix", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_gold", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData LavaGolem => new()
        {
            ID = "lava_golem",
            Name = "Lava Golem",
            Stats = new CharacterStats { HP = 10000, ATK = 100, DEF = 70 },
            MoveSpeed = 0.8f,
            AttackRange = 0.6f,
            AttackCooldown = 1.8f,
            XPReward = 200,
            GoldReward = 60,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_gold", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData InfernoBeetle => new()
        {
            ID = "inferno_beetle",
            Name = "Inferno Beetle",
            Stats = new CharacterStats { HP = 15000, ATK = 120, DEF = 85 },
            MoveSpeed = 1.0f,
            AttackRange = 0.8f,
            AttackCooldown = 1.5f,
            XPReward = 280,
            GoldReward = 80,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_phoenix", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_gold", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_gold", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Frozen Family ---

        public static EnemyData FrostWolf => new()
        {
            ID = "frost_wolf",
            Name = "Frost Wolf",
            Stats = new CharacterStats { HP = 22000, ATK = 150, DEF = 100 },
            MoveSpeed = 1.6f,
            AttackRange = 0.6f,
            AttackCooldown = 1.0f,
            XPReward = 380,
            GoldReward = 100,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_phoenix", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_phoenix", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData IceWraith => new()
        {
            ID = "ice_wraith",
            Name = "Ice Wraith",
            Stats = new CharacterStats { HP = 32000, MATK = 180, DEF = 80, MDEF = 120 },
            MoveSpeed = 1.2f,
            AttackRange = 1.2f,
            AttackCooldown = 1.3f,
            XPReward = 500,
            GoldReward = 130,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_gold", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_gold", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.003f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData GlacierGiant => new()
        {
            ID = "glacier_giant",
            Name = "Glacier Giant",
            Stats = new CharacterStats { HP = 50000, ATK = 220, DEF = 150 },
            MoveSpeed = 0.7f,
            AttackRange = 0.8f,
            AttackCooldown = 2.0f,
            XPReward = 680,
            GoldReward = 170,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "boots_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Shadow Family ---

        public static EnemyData ShadowStalker => new()
        {
            ID = "shadow_stalker",
            Name = "Shadow Stalker",
            Stats = new CharacterStats { HP = 70000, ATK = 280, DEF = 180 },
            MoveSpeed = 1.4f,
            AttackRange = 0.6f,
            AttackCooldown = 0.9f,
            XPReward = 900,
            GoldReward = 220,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_conqueror", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData VoidWeaver => new()
        {
            ID = "void_weaver",
            Name = "Void Weaver",
            Stats = new CharacterStats { HP = 100000, MATK = 350, DEF = 150, MDEF = 200 },
            MoveSpeed = 1.1f,
            AttackRange = 1.4f,
            AttackCooldown = 1.3f,
            XPReward = 1200,
            GoldReward = 280,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData AbyssalHorror => new()
        {
            ID = "abyssal_horror",
            Name = "Abyssal Horror",
            Stats = new CharacterStats { HP = 150000, ATK = 420, DEF = 250 },
            MoveSpeed = 0.9f,
            AttackRange = 0.8f,
            AttackCooldown = 1.5f,
            XPReward = 1600,
            GoldReward = 360,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Dragon Family ---

        public static EnemyData Drake => new()
        {
            ID = "drake",
            Name = "Drake",
            Stats = new CharacterStats { HP = 200000, ATK = 520, DEF = 300 },
            MoveSpeed = 1.3f,
            AttackRange = 0.7f,
            AttackCooldown = 1.2f,
            XPReward = 2100,
            GoldReward = 450,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData ElderDragon => new()
        {
            ID = "elder_dragon",
            Name = "Elder Dragon",
            Stats = new CharacterStats { HP = 300000, ATK = 650, DEF = 380 },
            MoveSpeed = 1.0f,
            AttackRange = 1.0f,
            AttackCooldown = 1.5f,
            XPReward = 2800,
            GoldReward = 580,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        public static EnemyData DragonLord => new()
        {
            ID = "dragon_lord",
            Name = "Dragon Lord",
            Stats = new CharacterStats { HP = 500000, ATK = 800, DEF = 500, MDEF = 300 },
            MoveSpeed = 0.8f,
            AttackRange = 1.0f,
            AttackCooldown = 1.8f,
            XPReward = 3800,
            GoldReward = 750,
            IsBoss = false,
            DropTable = new DropTable
            {
                Entries = new[]
                {
                    new DropEntry { ItemID = "helm_conqueror", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "armor_conqueror", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "boots_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "neck_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                    new DropEntry { ItemID = "ring_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 }
                }
            }
        };

        // --- Dark Family ---

        public static EnemyData DarkAcolyte => new()
        {
            ID = "dark_acolyte",
            Name = "Dark Acolyte",
            Stats = new CharacterStats { HP = 2000, MATK = 45, DEF = 15, MDEF = 20 },
            MoveSpeed = 1.2f,
            AttackRange = 1.2f,
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
            Stats = new CharacterStats { HP = 4000, ATK = 60, DEF = 40, MDEF = 10 },
            MoveSpeed = 0.9f,
            AttackRange = 0.6f,
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
            Rat, InfectedRat, BloodyRat,
            Skeleton, Gladiator, SkeletonKnight,
            DarkAcolyte, DarkKnight,
            FireImp, LavaGolem, InfernoBeetle,
            FrostWolf, IceWraith, GlacierGiant,
            ShadowStalker, VoidWeaver, AbyssalHorror,
            Drake, ElderDragon, DragonLord
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
                        new EnemySpawnWeight { EnemyID = "rat", Weight = 1.0f }
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
                        new EnemySpawnWeight { EnemyID = "rat", Weight = 0.6f },
                        new EnemySpawnWeight { EnemyID = "rat_infected", Weight = 0.4f }
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
                        new EnemySpawnWeight { EnemyID = "rat_infected", Weight = 0.5f },
                        new EnemySpawnWeight { EnemyID = "rat_bloody", Weight = 0.3f },
                        new EnemySpawnWeight { EnemyID = "rat", Weight = 0.2f }
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
                        new EnemySpawnWeight { EnemyID = "gladiator", Weight = 0.3f }
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
                        new EnemySpawnWeight { EnemyID = "gladiator", Weight = 0.3f },
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
                        new EnemySpawnWeight { EnemyID = "gladiator", Weight = 0.5f }
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
        // Map 4: Volcano Depths (Lv 50-70)
        // =================================================================

        public static MapData VolcanoDepths => new()
        {
            ID = "map_volcano_depths",
            Name = "Volcano Depths",
            Description = "Molten caverns deep beneath an active volcano.",
            MinLevel = 50,
            MaxLevel = 70,
            BackgroundID = "bg_volcano_depths",
            Areas = new[]
            {
                // Area 1: Ember Caves (Lv 50-58)
                new AreaData
                {
                    ID = "area_volcano_ember_caves",
                    Name = "Ember Caves",
                    MinLevel = 50,
                    MaxLevel = 58,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 18,
                    SpawnInterval = 1.2f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "fire_imp", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_phoenix", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_phoenix", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_phoenix", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_gold", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_gold", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Magma Channels (Lv 58-65)
                new AreaData
                {
                    ID = "area_volcano_magma_channels",
                    Name = "Magma Channels",
                    MinLevel = 58,
                    MaxLevel = 65,
                    XPMultiplier = 1.4f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 20,
                    SpawnInterval = 1.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "fire_imp", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "lava_golem", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_gold", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_gold", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Caldera Core (Lv 65-70)
                new AreaData
                {
                    ID = "area_volcano_caldera_core",
                    Name = "Caldera Core",
                    MinLevel = 65,
                    MaxLevel = 70,
                    XPMultiplier = 1.8f,
                    GoldMultiplier = 1.5f,
                    MaxConcurrentEnemies = 24,
                    SpawnInterval = 0.9f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "lava_golem", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "inferno_beetle", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_gold", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_gold", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.005f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // Map 5: Frozen Wastes (Lv 70-90)
        // =================================================================

        public static MapData FrozenWastes => new()
        {
            ID = "map_frozen_wastes",
            Name = "Frozen Wastes",
            Description = "An endless tundra where blizzards never cease.",
            MinLevel = 70,
            MaxLevel = 90,
            BackgroundID = "bg_frozen_wastes",
            Areas = new[]
            {
                // Area 1: Frost Plains (Lv 70-78)
                new AreaData
                {
                    ID = "area_frozen_frost_plains",
                    Name = "Frost Plains",
                    MinLevel = 70,
                    MaxLevel = 78,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 20,
                    SpawnInterval = 1.1f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "frost_wolf", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_phoenix", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Howling Peaks (Lv 78-85)
                new AreaData
                {
                    ID = "area_frozen_howling_peaks",
                    Name = "Howling Peaks",
                    MinLevel = 78,
                    MaxLevel = 85,
                    XPMultiplier = 1.4f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 22,
                    SpawnInterval = 1.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "frost_wolf", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "ice_wraith", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "weapon_phoenix_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_phoenix_shield", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.01f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.008f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Glacial Abyss (Lv 85-90)
                new AreaData
                {
                    ID = "area_frozen_glacial_abyss",
                    Name = "Glacial Abyss",
                    MinLevel = 85,
                    MaxLevel = 90,
                    XPMultiplier = 1.8f,
                    GoldMultiplier = 1.5f,
                    MaxConcurrentEnemies = 24,
                    SpawnInterval = 0.9f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "ice_wraith", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "glacier_giant", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // Map 6: Shadow Realm (Lv 90-110)
        // =================================================================

        public static MapData ShadowRealm => new()
        {
            ID = "map_shadow_realm",
            Name = "Shadow Realm",
            Description = "A dimension of pure darkness where nightmares take form.",
            MinLevel = 90,
            MaxLevel = 110,
            BackgroundID = "bg_shadow_realm",
            Areas = new[]
            {
                // Area 1: Twilight Border (Lv 90-98)
                new AreaData
                {
                    ID = "area_shadow_twilight_border",
                    Name = "Twilight Border",
                    MinLevel = 90,
                    MaxLevel = 98,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 22,
                    SpawnInterval = 1.0f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "shadow_stalker", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.015f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Void Corridors (Lv 98-105)
                new AreaData
                {
                    ID = "area_shadow_void_corridors",
                    Name = "Void Corridors",
                    MinLevel = 98,
                    MaxLevel = 105,
                    XPMultiplier = 1.4f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 24,
                    SpawnInterval = 0.9f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "shadow_stalker", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "void_weaver", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.02f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Abyssal Throne (Lv 105-110)
                new AreaData
                {
                    ID = "area_shadow_abyssal_throne",
                    Name = "Abyssal Throne",
                    MinLevel = 105,
                    MaxLevel = 110,
                    XPMultiplier = 1.8f,
                    GoldMultiplier = 1.5f,
                    MaxConcurrentEnemies = 26,
                    SpawnInterval = 0.8f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "void_weaver", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "abyssal_horror", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // Map 7: Dragon's Domain (Lv 110-130)
        // =================================================================

        public static MapData DragonsDomain => new()
        {
            ID = "map_dragons_domain",
            Name = "Dragon's Domain",
            Description = "The ancient nesting grounds of the world's mightiest dragons.",
            MinLevel = 110,
            MaxLevel = 130,
            BackgroundID = "bg_dragons_domain",
            Areas = new[]
            {
                // Area 1: Scorched Foothills (Lv 110-118)
                new AreaData
                {
                    ID = "area_dragon_scorched_foothills",
                    Name = "Scorched Foothills",
                    MinLevel = 110,
                    MaxLevel = 118,
                    XPMultiplier = 1.0f,
                    GoldMultiplier = 1.0f,
                    MaxConcurrentEnemies = 24,
                    SpawnInterval = 0.9f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "drake", Weight = 1.0f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.025f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 2: Dragon Roost (Lv 118-125)
                new AreaData
                {
                    ID = "area_dragon_roost",
                    Name = "Dragon Roost",
                    MinLevel = 118,
                    MaxLevel = 125,
                    XPMultiplier = 1.4f,
                    GoldMultiplier = 1.3f,
                    MaxConcurrentEnemies = 26,
                    SpawnInterval = 0.8f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "drake", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "elder_dragon", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.03f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                },

                // Area 3: Sovereign's Lair (Lv 125-130)
                new AreaData
                {
                    ID = "area_dragon_sovereigns_lair",
                    Name = "Sovereign's Lair",
                    MinLevel = 125,
                    MaxLevel = 130,
                    XPMultiplier = 2.0f,
                    GoldMultiplier = 1.8f,
                    MaxConcurrentEnemies = 28,
                    SpawnInterval = 0.7f,
                    EnemyPool = new[]
                    {
                        new EnemySpawnWeight { EnemyID = "elder_dragon", Weight = 0.4f },
                        new EnemySpawnWeight { EnemyID = "dragon_lord", Weight = 0.6f }
                    },
                    AreaDropTable = new DropTable
                    {
                        Entries = new[]
                        {
                            new DropEntry { ItemID = "helm_conqueror", DropRate = 0.05f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "armor_conqueror", DropRate = 0.05f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "weapon_conqueror_blade", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "offhand_conqueror_shield", DropRate = 0.04f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "boots_conqueror", DropRate = 0.05f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "neck_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 },
                            new DropEntry { ItemID = "ring_conqueror", DropRate = 0.035f, MinQuantity = 1, MaxQuantity = 1 }
                        }
                    }
                }
            }
        };

        // =================================================================
        // All Maps
        // =================================================================

        public static MapData[] AllMaps => new[]
        {
            SlimeFields, BoneYard, DarkTower,
            VolcanoDepths, FrozenWastes, ShadowRealm, DragonsDomain
        };

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
        /// Finds the map and area for a given area ID. Returns (map, area) tuple.
        /// Both will be default if not found.
        /// </summary>
        public static (MapData Map, AreaData Area) FindAreaByID(string areaID)
        {
            var maps = AllMaps;
            for (int i = 0; i < maps.Length; i++)
            {
                var areas = maps[i].Areas;
                if (areas == null) continue;
                for (int j = 0; j < areas.Length; j++)
                {
                    if (areas[j].ID == areaID)
                        return (maps[i], areas[j]);
                }
            }
            return (default, default);
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
