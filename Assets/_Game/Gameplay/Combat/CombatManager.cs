using System.Collections.Generic;
using UnityEngine;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;
using ConquerChronicles.Gameplay.Character;
using ConquerChronicles.Gameplay.Enemy;

namespace ConquerChronicles.Gameplay.Combat
{
    public class CombatManager : MonoBehaviour
    {
        [SerializeField] private CharacterView _player;
        [SerializeField] private EnemySpawner _enemySpawner;
        [SerializeField] private DamageNumberPool _damageNumberPool;
        [SerializeField] private HitEffectPool _hitEffectPool;

        private SkillResolver _skillResolver;
        private List<SkillState> _activeSkills;
        private readonly List<CombatEvent> _eventBuffer = new(64);
        private readonly List<EnemyTarget> _targetSnapshot = new(256);

        private int _killCount;
        public int KillCount => _killCount;

        // Event for HUD updates
        public System.Action<int> OnKillCountChanged;
        public System.Action<int, bool> OnPlayerXPGained;

        public void Initialize(
            CharacterView player,
            EnemySpawner spawner,
            DamageNumberPool dmgPool,
            HitEffectPool hitPool,
            List<SkillState> skills)
        {
            _player = player;
            _enemySpawner = spawner;
            _damageNumberPool = dmgPool;
            _hitEffectPool = hitPool;
            _activeSkills = skills;
            _skillResolver = new SkillResolver(System.Environment.TickCount);
            _killCount = 0;
        }

        private void Update()
        {
            if (_player == null || _player.State == null || _player.State.IsDead) return;
            if (_activeSkills == null || _activeSkills.Count == 0) return;

            float dt = Time.deltaTime;
            var playerStats = _player.GetComputedStats();
            var playerPos = new CombatPosition(_player.transform.position.x, _player.transform.position.y);

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

            // Clean up dead enemies
            _enemySpawner.RemoveDeadEnemies(OnEnemyDied);
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

                // Spawn damage number
                if (_damageNumberPool != null)
                {
                    var dmgNum = _damageNumberPool.Get();
                    dmgNum.Play(evt.Value, evt.IsCritical, enemy.transform.position + Vector3.up * 0.5f);
                }

                // Spawn hit effect
                if (_hitEffectPool != null)
                {
                    var hitFx = _hitEffectPool.Get();
                    hitFx.Play(enemy.transform.position);
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
        }
    }
}
