using ConquerChronicles.Core.Enemy;

namespace ConquerChronicles.Core.Combat
{
    public static class StatusEffectSystem
    {
        /// <summary>
        /// Apply a new effect to an enemy. Refreshes duration if the same type already exists.
        /// </summary>
        public static void ApplyEffect(EnemyState enemy, StatusEffect effect)
        {
            if (effect.Type == StatusEffectType.None) return;

            // Check if same type already active — refresh it
            for (int i = 0; i < enemy.ActiveEffects.Count; i++)
            {
                if (enemy.ActiveEffects[i].Type == effect.Type)
                {
                    var existing = enemy.ActiveEffects[i];
                    existing.RemainingDuration = effect.Duration;
                    existing.TickTimer = 0f;
                    enemy.ActiveEffects[i] = existing;
                    UpdateDerivedState(enemy);
                    return;
                }
            }

            // New effect
            enemy.ActiveEffects.Add(new ActiveStatusEffect
            {
                Type = effect.Type,
                RemainingDuration = effect.Duration,
                TickTimer = 0f,
                TickDamage = effect.TickDamage,
                SlowPercent = effect.SlowPercent
            });
            UpdateDerivedState(enemy);
        }

        /// <summary>
        /// Tick all active effects. Returns total DoT damage dealt this frame.
        /// </summary>
        public static int TickEffects(EnemyState enemy, float deltaTime)
        {
            int totalDotDamage = 0;

            for (int i = enemy.ActiveEffects.Count - 1; i >= 0; i--)
            {
                var fx = enemy.ActiveEffects[i];
                fx.RemainingDuration -= deltaTime;

                if (fx.RemainingDuration <= 0f)
                {
                    enemy.ActiveEffects.RemoveAt(i);
                    continue;
                }

                // Tick DoT (Poison, Burn, Bleed)
                if (fx.TickDamage > 0 && fx.TickTimer > 0f)
                {
                    fx.TickTimer -= deltaTime;
                    if (fx.TickTimer <= 0f)
                    {
                        totalDotDamage += fx.TickDamage;
                        fx.TickTimer += 1.0f; // tick every 1 second
                    }
                }
                else if (fx.TickDamage > 0)
                {
                    fx.TickTimer += 1.0f - deltaTime; // init tick timer
                }

                enemy.ActiveEffects[i] = fx;
            }

            if (totalDotDamage > 0)
            {
                enemy.TakeDamage(totalDotDamage);
            }

            UpdateDerivedState(enemy);
            return totalDotDamage;
        }

        /// <summary>
        /// Recompute MoveSpeedMultiplier and IsStunned from active effects.
        /// </summary>
        private static void UpdateDerivedState(EnemyState enemy)
        {
            enemy.MoveSpeedMultiplier = 1.0f;
            enemy.IsStunned = false;

            for (int i = 0; i < enemy.ActiveEffects.Count; i++)
            {
                var fx = enemy.ActiveEffects[i];
                if (fx.Type == StatusEffectType.Slow)
                    enemy.MoveSpeedMultiplier *= (1.0f - fx.SlowPercent);
                else if (fx.Type == StatusEffectType.Stun)
                    enemy.IsStunned = true;
            }
        }

        /// <summary>
        /// Remove all active effects and reset derived state.
        /// </summary>
        public static void ClearAll(EnemyState enemy)
        {
            enemy.ActiveEffects.Clear();
            enemy.MoveSpeedMultiplier = 1.0f;
            enemy.IsStunned = false;
        }
    }
}
