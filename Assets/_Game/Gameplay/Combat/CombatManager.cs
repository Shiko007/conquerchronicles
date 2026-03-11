using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Audio;
using ConquerChronicles.Gameplay.Enemy;

namespace ConquerChronicles.Gameplay.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private CharacterView _player;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private DamageNumberPool _damageNumberPool;
        [SerializeField] private HitEffectPool _hitEffectPool;

        private AudioManager _audioManager;
        private SkillResolver _skillResolver;
        private List<SkillState> _activeSkills;
        private readonly List<CombatEvent> _eventBuffer = new(64);
        private readonly List<EnemyTarget> _targetSnapshot = new(256);

        private int _killCount;
        public int KillCount => _killCount;

        // Event for HUD updates
        public System.Action<int> OnKillCountChanged;
        public System.Action<long, bool> OnPlayerXPGained;
        public System.Action<EnemyView> OnEnemyKilled;
        public System.Action<int> OnPlayerDamaged; // damage amount

        public void Initialize(
            CharacterView player,
            EnemySpawner spawner,
            DamageNumberPool dmgPool,
            HitEffectPool hitPool,
            List<SkillState> skills,
            AudioManager audioManager = null)
        {
            _player = player;
            _enemySpawner = spawner;
            _damageNumberPool = dmgPool;
            _hitEffectPool = hitPool;
            _activeSkills = skills;
            _audioManager = audioManager;
            _skillResolver = new SkillResolver(System.Environment.TickCount);
            _killCount = 0;
        }

        public void RefreshSkills(SkillData[] newSkills)
        {
            _activeSkills = new List<SkillState>();
            for (int i = 0; i < newSkills.Length; i++)
                _activeSkills.Add(new SkillState(newSkills[i]));
        }

        private void Update()
        {
            if (_player == null || _player.State == null || _player.State.IsDead) return;
            if (_activeSkills == null || _activeSkills.Count == 0) return;

            float dt = Time.deltaTime;
            var playerStats = _player.GetComputedStats();
            var playerPos = new CombatPosition(_player.transform.position.x, _player.transform.position.y);

            // Face nearest enemy
            FaceNearestEnemy();

            // Build target snapshot from active enemies
            BuildTargetSnapshot();

            // Update cooldowns and resolve skills
            _skillResolver.UpdateCooldowns(dt, _activeSkills);
            _eventBuffer.Clear();
            _skillResolver.ResolveSkills(playerStats, playerPos, _activeSkills, _targetSnapshot, _eventBuffer);

            // Apply combat events
            foreach (var evt in _eventBuffer)
            {
                ApplyCombatEvent(evt);
            }

            // Tick status effects on all enemies
            TickStatusEffects(dt);

            // Process enemy attacks on player
            ProcessEnemyAttacks(playerStats);

            // Clean up dead enemies
            _enemySpawner.RemoveDeadEnemies(OnEnemyDied);
        }

        private void FaceNearestEnemy()
        {
            var enemies = _enemySpawner.ActiveEnemies;
            if (enemies.Count == 0) return;

            float closestDist = float.MaxValue;
            Vector3 closestPos = Vector3.zero;
            var playerPos = _player.transform.position;

            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].State.IsDead) continue;
                float dist = Vector3.SqrMagnitude(enemies[i].transform.position - playerPos);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestPos = enemies[i].transform.position;
                }
            }

            if (closestDist < float.MaxValue)
                _player.FaceToward(closestPos);
        }

        private void BuildTargetSnapshot()
        {
            _targetSnapshot.Clear();
            var enemies = _enemySpawner.ActiveEnemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                var e = enemies[i];
                _targetSnapshot.Add(new EnemyTarget
                {
                    Index = i,
                    Position = new CombatPosition(e.transform.position.x, e.transform.position.y),
                    Stats = e.State.Data.Stats,
                    CurrentHP = e.State.CurrentHP,
                    IsDead = e.State.IsDead
                });
            }
        }

        private void ApplyCombatEvent(CombatEvent evt)
        {
            if (evt.Type == CombatEvent.EventType.Damage)
            {
                var enemies = _enemySpawner.ActiveEnemies;
                if (evt.TargetIndex < 0 || evt.TargetIndex >= enemies.Count) return;

                var enemy = enemies[evt.TargetIndex];
                enemy.TakeDamage(evt.Value);

                // Player attack animation — only when enemy is visually close
                float distToEnemy = Vector3.Distance(enemy.transform.position, _player.transform.position);
                if (distToEnemy <= 2.0f)
                {
                    bool faceLeft = enemy.transform.position.x < _player.transform.position.x;
                    _player.PlayAttack(faceLeft);
                }

                // Spawn damage number
                if (_damageNumberPool != null)
                {
                    var dmgNum = _damageNumberPool.Get();
                    dmgNum.Play(evt.Value, evt.IsCritical, enemy.transform.position + Vector3.up * 0.5f);
                }

                // Play hit SFX
                if (_audioManager != null && _audioManager.Library != null)
                {
                    _audioManager.PlaySFX(evt.IsCritical
                        ? _audioManager.Library.CriticalHit
                        : _audioManager.Library.Hit);
                }

                // Apply status effect
                if (evt.AppliedEffect.Type != StatusEffectType.None)
                {
                    StatusEffectSystem.ApplyEffect(enemy.State, evt.AppliedEffect);
                }
            }
        }

        private void TickStatusEffects(float dt)
        {
            var enemies = _enemySpawner.ActiveEnemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.State.IsDead) continue;
                StatusEffectSystem.TickEffects(enemy.State, dt);
            }
        }

        private void ProcessEnemyAttacks(CharacterStats playerStats)
        {
            var enemies = _enemySpawner.ActiveEnemies;
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                if (enemy.State.IsDead) continue;

                var movement = enemy.Movement;
                if (movement == null || !movement.ReadyToAttack) continue;

                movement.ConsumeAttack();

                int rawDamage = enemy.State.Data.Stats.ATK;
                int damage = Mathf.Max(1, rawDamage - playerStats.DEF);
                _player.TakeDamage(damage);
                OnPlayerDamaged?.Invoke(damage);

                // Play player hit SFX
                if (_audioManager != null && _audioManager.Library != null)
                    _audioManager.PlaySFX(_audioManager.Library.PlayerHit);

                // Spawn damage number on player
                if (_damageNumberPool != null)
                {
                    var dmgNum = _damageNumberPool.Get();
                    dmgNum.Play(damage, false, _player.transform.position + Vector3.up * 0.5f, Color.red);
                }
            }
        }

        private void OnEnemyDied(EnemyView enemy)
        {
            _killCount++;
            OnKillCountChanged?.Invoke(_killCount);

            int xp = enemy.State.Data.XPReward;
            _player.GainXP(xp);
            OnPlayerXPGained?.Invoke(xp, false);
            OnEnemyKilled?.Invoke(enemy);

            // Play enemy death SFX
            if (_audioManager != null && _audioManager.Library != null)
                _audioManager.PlaySFX(_audioManager.Library.EnemyDeath);
        }
    }
}
