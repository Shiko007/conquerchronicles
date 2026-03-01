using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Enemy;

namespace ConquerChronicles.Core.Stage
{
    /// <summary>
    /// Hardcoded test stages for development. Will be replaced by ScriptableObjects.
    /// </summary>
    public static class TestStages
    {
        public static EnemyData GreenSlime => new()
        {
            ID = "slime_green",
            Name = "Green Slime",
            Stats = new CharacterStats { HP = 30, ATK = 5, DEF = 2, MATK = 0, MDEF = 1, AGI = 3 },
            MoveSpeed = 1.5f, AttackRange = 0.5f, AttackCooldown = 1.5f,
            XPReward = 10, GoldReward = 5, IsBoss = false
        };

        public static EnemyData RedSlime => new()
        {
            ID = "slime_red",
            Name = "Red Slime",
            Stats = new CharacterStats { HP = 50, ATK = 8, DEF = 4, MATK = 0, MDEF = 2, AGI = 4 },
            MoveSpeed = 1.8f, AttackRange = 0.5f, AttackCooldown = 1.2f,
            XPReward = 18, GoldReward = 8, IsBoss = false
        };

        public static EnemyData Skeleton => new()
        {
            ID = "skeleton",
            Name = "Skeleton",
            Stats = new CharacterStats { HP = 80, ATK = 12, DEF = 6, MATK = 0, MDEF = 3, AGI = 5 },
            MoveSpeed = 1.3f, AttackRange = 0.8f, AttackCooldown = 1.0f,
            XPReward = 25, GoldReward = 12, IsBoss = false
        };

        public static EnemyData SkeletonKnight => new()
        {
            ID = "skeleton_knight",
            Name = "Skeleton Knight",
            Stats = new CharacterStats { HP = 150, ATK = 18, DEF = 12, MATK = 0, MDEF = 5, AGI = 4 },
            MoveSpeed = 1.0f, AttackRange = 1.0f, AttackCooldown = 1.5f,
            XPReward = 40, GoldReward = 20, IsBoss = false
        };

        public static EnemyData SlimeKing => new()
        {
            ID = "boss_slime_king",
            Name = "Slime King",
            Stats = new CharacterStats { HP = 500, ATK = 15, DEF = 10, MATK = 5, MDEF = 8, AGI = 3 },
            MoveSpeed = 0.8f, AttackRange = 1.5f, AttackCooldown = 2.0f,
            XPReward = 200, GoldReward = 100, IsBoss = true
        };

        public static EnemyData SkeletonLord => new()
        {
            ID = "boss_skeleton_lord",
            Name = "Skeleton Lord",
            Stats = new CharacterStats { HP = 1000, ATK = 25, DEF = 18, MATK = 10, MDEF = 12, AGI = 5 },
            MoveSpeed = 0.6f, AttackRange = 2.0f, AttackCooldown = 1.8f,
            XPReward = 500, GoldReward = 250, IsBoss = true
        };

        public static EnemyData DarkWizard => new()
        {
            ID = "boss_dark_wizard",
            Name = "Dark Wizard",
            Stats = new CharacterStats { HP = 800, ATK = 10, DEF = 8, MATK = 35, MDEF = 20, AGI = 6 },
            MoveSpeed = 0.7f, AttackRange = 3.0f, AttackCooldown = 1.5f,
            XPReward = 600, GoldReward = 300, IsBoss = true
        };

        public static EnemyData[] AllEnemies => new[]
        {
            GreenSlime, RedSlime, Skeleton, SkeletonKnight, SlimeKing, SkeletonLord, DarkWizard
        };

        // --- Stage 1: Slime Fields (Lv 1) ---
        public static StageData Stage1 => new()
        {
            ID = "stage_01",
            Name = "Slime Fields",
            RecommendedLevel = 1,
            XPMultiplier = 1.0f,
            CompletionGold = 100,
            CompletionMetaCurrency = 10,
            Waves = new[]
            {
                MakeWave(1.5f, 0.5f, "slime_green", 5, SpawnEdge.Random, SpawnPattern.Stream),
                MakeWave(2f, 0.4f, "slime_green", 8, SpawnEdge.Random, SpawnPattern.Stream),
                MakeWave(2f, 0.3f, "slime_green", 6, SpawnEdge.North, SpawnPattern.Burst,
                                   "slime_red", 3, SpawnEdge.South, SpawnPattern.Stream),
                MakeWave(2f, 0.3f, "slime_red", 8, SpawnEdge.Random, SpawnPattern.Surround),
                MakeWave(2f, 0.2f, "slime_green", 10, SpawnEdge.Random, SpawnPattern.Burst,
                                   "slime_red", 5, SpawnEdge.Random, SpawnPattern.Stream),
            },
            BossWave = MakeWave(3f, 0f, "boss_slime_king", 1, SpawnEdge.North, SpawnPattern.Burst)
        };

        // --- Stage 2: Bone Yard (Lv 10) ---
        public static StageData Stage2 => new()
        {
            ID = "stage_02",
            Name = "Bone Yard",
            RecommendedLevel = 10,
            XPMultiplier = 1.5f,
            CompletionGold = 250,
            CompletionMetaCurrency = 20,
            Waves = new[]
            {
                MakeWave(2f, 0.4f, "skeleton", 6, SpawnEdge.Random, SpawnPattern.Stream),
                MakeWave(2f, 0.3f, "skeleton", 8, SpawnEdge.Random, SpawnPattern.Surround,
                                   "slime_red", 4, SpawnEdge.South, SpawnPattern.Burst),
                MakeWave(1.5f, 0.3f, "skeleton", 10, SpawnEdge.Random, SpawnPattern.Stream,
                                     "skeleton_knight", 2, SpawnEdge.North, SpawnPattern.Burst),
                MakeWave(2f, 0.2f, "skeleton_knight", 4, SpawnEdge.Random, SpawnPattern.Surround,
                                   "skeleton", 8, SpawnEdge.Random, SpawnPattern.Stream),
                MakeWave(1f, 0.2f, "skeleton_knight", 6, SpawnEdge.Random, SpawnPattern.Surround,
                                   "skeleton", 12, SpawnEdge.Random, SpawnPattern.Burst),
            },
            BossWave = MakeWave(3f, 0f, "boss_skeleton_lord", 1, SpawnEdge.North, SpawnPattern.Burst)
        };

        // --- Stage 3: Dark Tower (Lv 20) ---
        public static StageData Stage3 => new()
        {
            ID = "stage_03",
            Name = "Dark Tower",
            RecommendedLevel = 20,
            XPMultiplier = 2.0f,
            CompletionGold = 500,
            CompletionMetaCurrency = 35,
            Waves = new[]
            {
                MakeWave(2f, 0.3f, "skeleton_knight", 5, SpawnEdge.Random, SpawnPattern.Stream,
                                   "skeleton", 8, SpawnEdge.Random, SpawnPattern.Burst),
                MakeWave(1.5f, 0.2f, "skeleton_knight", 8, SpawnEdge.Random, SpawnPattern.Surround),
                MakeWave(1.5f, 0.2f, "skeleton_knight", 6, SpawnEdge.North, SpawnPattern.Burst,
                                     "slime_red", 10, SpawnEdge.South, SpawnPattern.Stream),
                MakeWave(1f, 0.2f, "skeleton_knight", 10, SpawnEdge.Random, SpawnPattern.Surround,
                                   "skeleton", 15, SpawnEdge.Random, SpawnPattern.Burst),
                MakeWave(1f, 0.15f, "skeleton_knight", 12, SpawnEdge.Random, SpawnPattern.Surround,
                                    "skeleton", 20, SpawnEdge.Random, SpawnPattern.Burst),
            },
            BossWave = MakeWave(3f, 0f, "boss_dark_wizard", 1, SpawnEdge.North, SpawnPattern.Burst,
                                        "skeleton_knight", 4, SpawnEdge.Random, SpawnPattern.Surround)
        };

        public static StageData[] AllStages => new[] { Stage1, Stage2, Stage3 };

        // --- Helpers ---

        private static WaveData MakeWave(float delay, float interval,
            string enemy1, int count1, SpawnEdge edge1, SpawnPattern pattern1)
        {
            return new WaveData
            {
                DelayBeforeWave = delay,
                SpawnInterval = interval,
                SpawnEntries = new[]
                {
                    new EnemySpawnEntry { EnemyID = enemy1, Count = count1, Edge = edge1, Pattern = pattern1 }
                }
            };
        }

        private static WaveData MakeWave(float delay, float interval,
            string enemy1, int count1, SpawnEdge edge1, SpawnPattern pattern1,
            string enemy2, int count2, SpawnEdge edge2, SpawnPattern pattern2)
        {
            return new WaveData
            {
                DelayBeforeWave = delay,
                SpawnInterval = interval,
                SpawnEntries = new[]
                {
                    new EnemySpawnEntry { EnemyID = enemy1, Count = count1, Edge = edge1, Pattern = pattern1 },
                    new EnemySpawnEntry { EnemyID = enemy2, Count = count2, Edge = edge2, Pattern = pattern2 }
                }
            };
        }
    }
}
