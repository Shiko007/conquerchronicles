using System.Collections.Generic;

namespace ConquerChronicles.Core.Combat
{
    public class AutoAttackResolver
    {
        // Find nearest alive enemy within range
        public int FindNearestTarget(IReadOnlyList<EnemyTarget> enemies, CombatPosition playerPos, float range)
        {
            int bestIndex = -1;
            float bestDist = float.MaxValue;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsDead) continue;
                float dist = playerPos.DistanceTo(enemies[i].Position);
                if (dist <= range && dist < bestDist)
                {
                    bestDist = dist;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }

        // Find all enemies within radius (for AoE)
        public void FindTargetsInRadius(IReadOnlyList<EnemyTarget> enemies, CombatPosition center, float radius, List<int> results)
        {
            results.Clear();
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].IsDead) continue;
                if (center.DistanceTo(enemies[i].Position) <= radius)
                    results.Add(i);
            }
        }

        // Find N nearest targets (for chain/multi-target)
        public void FindNNearestTargets(IReadOnlyList<EnemyTarget> enemies, CombatPosition center, float range, int count, List<int> results)
        {
            results.Clear();
            // Simple O(n*k) selection — fine for < 300 enemies
            var used = new HashSet<int>();
            for (int k = 0; k < count; k++)
            {
                int bestIdx = -1;
                float bestDist = float.MaxValue;
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].IsDead || used.Contains(i)) continue;
                    float dist = center.DistanceTo(enemies[i].Position);
                    if (dist <= range && dist < bestDist)
                    {
                        bestDist = dist;
                        bestIdx = i;
                    }
                }
                if (bestIdx < 0) break;
                results.Add(bestIdx);
                used.Add(bestIdx);
            }
        }
    }
}
