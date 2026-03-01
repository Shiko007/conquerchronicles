using System;
using System.Collections.Generic;
using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Combat
{
    public class SkillResolver
    {
        private readonly AutoAttackResolver _targetResolver = new();
        private readonly List<int> _hitBuffer = new(64);
        private int _seedCounter;

        public SkillResolver(int initialSeed = 42)
        {
            _seedCounter = initialSeed;
        }

        public void UpdateCooldowns(float deltaTime, IReadOnlyList<SkillState> skills)
        {
            for (int i = 0; i < skills.Count; i++)
                skills[i].UpdateCooldown(deltaTime);
        }

        public void ResolveSkills(
            CharacterStats playerStats,
            CombatPosition playerPos,
            IReadOnlyList<SkillState> skills,
            IReadOnlyList<EnemyTarget> enemies,
            List<CombatEvent> events)
        {
            for (int i = 0; i < skills.Count; i++)
            {
                var skill = skills[i];
                if (!skill.IsReady) continue;

                bool fired = ResolveSkill(playerStats, playerPos, skill, enemies, events);
                if (fired)
                    skill.TriggerCooldown();
            }
        }

        private bool ResolveSkill(
            CharacterStats playerStats,
            CombatPosition playerPos,
            SkillState skillState,
            IReadOnlyList<EnemyTarget> enemies,
            List<CombatEvent> events)
        {
            var skill = skillState.Data;

            // Self-buff skills
            if (skill.IsSelfBuff)
            {
                events.Add(new CombatEvent
                {
                    Type = CombatEvent.EventType.Buff,
                    SourceIndex = -1,
                    TargetIndex = -1,
                    Value = (int)(skill.BuffMultiplier * 100),
                    SkillID = skill.ID,
                    Pattern = skill.Pattern
                });
                return true;
            }

            switch (skill.Pattern)
            {
                case AttackPattern.MeleeSwing:
                case AttackPattern.AoECircle:
                case AttackPattern.Nova:
                    return ResolveAoE(playerStats, playerPos, skill, enemies, events);

                case AttackPattern.RangedSingle:
                    return ResolveSingleTarget(playerStats, playerPos, skill, enemies, events);

                case AttackPattern.RangedPiercing:
                case AttackPattern.MultiProjectile:
                    return ResolveMultiTarget(playerStats, playerPos, skill, enemies, events);

                case AttackPattern.Chain:
                    return ResolveChain(playerStats, playerPos, skill, enemies, events);

                case AttackPattern.AoECone:
                    return ResolveAoE(playerStats, playerPos, skill, enemies, events);

                case AttackPattern.Orbiting:
                case AttackPattern.SummonZone:
                    return ResolveAoE(playerStats, playerPos, skill, enemies, events);

                default:
                    return false;
            }
        }

        private bool ResolveSingleTarget(CharacterStats playerStats, CombatPosition playerPos, SkillData skill, IReadOnlyList<EnemyTarget> enemies, List<CombatEvent> events)
        {
            int targetIdx = _targetResolver.FindNearestTarget(enemies, playerPos, skill.Range);
            if (targetIdx < 0) return false;

            var result = DamageFormula.Calculate(playerStats, enemies[targetIdx].Stats, skill, _seedCounter++);
            events.Add(new CombatEvent
            {
                Type = CombatEvent.EventType.Damage,
                SourceIndex = -1,
                TargetIndex = targetIdx,
                Value = result.Damage,
                IsCritical = result.IsCritical,
                DmgType = skill.DamageType,
                Pattern = skill.Pattern,
                AppliedEffect = skill.AppliedEffect,
                SkillID = skill.ID
            });
            return true;
        }

        private bool ResolveAoE(CharacterStats playerStats, CombatPosition playerPos, SkillData skill, IReadOnlyList<EnemyTarget> enemies, List<CombatEvent> events)
        {
            float radius = skill.AoERadius > 0 ? skill.AoERadius : skill.Range;
            _targetResolver.FindTargetsInRadius(enemies, playerPos, radius, _hitBuffer);
            if (_hitBuffer.Count == 0) return false;

            foreach (int idx in _hitBuffer)
            {
                var result = DamageFormula.Calculate(playerStats, enemies[idx].Stats, skill, _seedCounter++);
                events.Add(new CombatEvent
                {
                    Type = CombatEvent.EventType.Damage,
                    SourceIndex = -1,
                    TargetIndex = idx,
                    Value = result.Damage,
                    IsCritical = result.IsCritical,
                    DmgType = skill.DamageType,
                    Pattern = skill.Pattern,
                    AppliedEffect = skill.AppliedEffect,
                    SkillID = skill.ID
                });
            }
            return true;
        }

        private bool ResolveMultiTarget(CharacterStats playerStats, CombatPosition playerPos, SkillData skill, IReadOnlyList<EnemyTarget> enemies, List<CombatEvent> events)
        {
            int count = Math.Max(1, skill.ProjectileCount);
            _targetResolver.FindNNearestTargets(enemies, playerPos, skill.Range, count, _hitBuffer);
            if (_hitBuffer.Count == 0) return false;

            foreach (int idx in _hitBuffer)
            {
                var result = DamageFormula.Calculate(playerStats, enemies[idx].Stats, skill, _seedCounter++);
                events.Add(new CombatEvent
                {
                    Type = CombatEvent.EventType.Damage,
                    SourceIndex = -1,
                    TargetIndex = idx,
                    Value = result.Damage,
                    IsCritical = result.IsCritical,
                    DmgType = skill.DamageType,
                    Pattern = skill.Pattern,
                    AppliedEffect = skill.AppliedEffect,
                    SkillID = skill.ID
                });
            }
            return true;
        }

        private bool ResolveChain(CharacterStats playerStats, CombatPosition playerPos, SkillData skill, IReadOnlyList<EnemyTarget> enemies, List<CombatEvent> events)
        {
            int count = Math.Max(1, skill.ProjectileCount);
            _targetResolver.FindNNearestTargets(enemies, playerPos, skill.Range, count, _hitBuffer);
            if (_hitBuffer.Count == 0) return false;

            float damageMultiplier = 1f;
            foreach (int idx in _hitBuffer)
            {
                var modifiedSkill = skill;
                modifiedSkill.DamageMultiplier *= damageMultiplier;
                var result = DamageFormula.Calculate(playerStats, enemies[idx].Stats, modifiedSkill, _seedCounter++);
                events.Add(new CombatEvent
                {
                    Type = CombatEvent.EventType.Damage,
                    SourceIndex = -1,
                    TargetIndex = idx,
                    Value = result.Damage,
                    IsCritical = result.IsCritical,
                    DmgType = skill.DamageType,
                    Pattern = skill.Pattern,
                    AppliedEffect = skill.AppliedEffect,
                    SkillID = skill.ID
                });
                damageMultiplier *= 0.8f; // 20% less per bounce
            }
            return true;
        }
    }
}
