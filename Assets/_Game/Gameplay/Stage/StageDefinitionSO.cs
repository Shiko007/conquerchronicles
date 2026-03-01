using UnityEngine;
using ConquerChronicles.Core.Stage;

namespace ConquerChronicles.Gameplay.Stage
{
    [CreateAssetMenu(menuName = "Conquer Chronicles/Stage Definition")]
    public class StageDefinitionSO : ScriptableObject
    {
        public string stageID;
        public string displayName;
        public int recommendedLevel;
        public float xpMultiplier = 1f;
        public int completionGold = 100;
        public int completionMetaCurrency = 10;

        [Header("Waves")]
        public WaveDefinitionEntry[] waves;

        [Header("Boss Wave")]
        public WaveDefinitionEntry bossWave;

        public StageData ToStageData()
        {
            var waveArray = new WaveData[waves.Length];
            for (int i = 0; i < waves.Length; i++)
                waveArray[i] = waves[i].ToWaveData();

            return new StageData
            {
                ID = stageID,
                Name = displayName,
                RecommendedLevel = recommendedLevel,
                Waves = waveArray,
                BossWave = bossWave.ToWaveData(),
                XPMultiplier = xpMultiplier,
                CompletionGold = completionGold,
                CompletionMetaCurrency = completionMetaCurrency
            };
        }
    }

    [System.Serializable]
    public class WaveDefinitionEntry
    {
        public float delayBeforeWave = 2f;
        public float spawnInterval = 0.5f;
        public SpawnEntry[] spawnEntries;

        public WaveData ToWaveData()
        {
            var entries = new EnemySpawnEntry[spawnEntries != null ? spawnEntries.Length : 0];
            for (int i = 0; i < entries.Length; i++)
            {
                entries[i] = new EnemySpawnEntry
                {
                    EnemyID = spawnEntries[i].enemyID,
                    Count = spawnEntries[i].count,
                    Edge = spawnEntries[i].edge,
                    Pattern = spawnEntries[i].pattern
                };
            }
            return new WaveData
            {
                DelayBeforeWave = delayBeforeWave,
                SpawnInterval = spawnInterval,
                SpawnEntries = entries
            };
        }
    }

    [System.Serializable]
    public class SpawnEntry
    {
        public string enemyID;
        public int count = 5;
        public ConquerChronicles.Core.Stage.SpawnEdge edge = ConquerChronicles.Core.Stage.SpawnEdge.Random;
        public SpawnPattern pattern = SpawnPattern.Stream;
    }
}
