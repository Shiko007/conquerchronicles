using NUnit.Framework;
using ConquerChronicles.Core.Character;
using ConquerChronicles.Core.Combat;

namespace ConquerChronicles.Tests
{
    [TestFixture]
    public class DamageFormulaTests
    {
        private CharacterStats MakeAttacker(int atk = 50, int matk = 30, float critRate = 0f, float critDmg = 1.5f)
        {
            return new CharacterStats
            {
                HP = 100, MP = 50, ATK = atk, DEF = 10, MATK = matk, MDEF = 5,
                AGI = 10, CritRate = critRate, CritDmg = critDmg, AttackSpeed = 1f
            };
        }

        private CharacterStats MakeDefender(int def = 10, int mdef = 5)
        {
            return new CharacterStats
            {
                HP = 50, MP = 0, ATK = 5, DEF = def, MATK = 0, MDEF = mdef,
                AGI = 3, CritRate = 0f, CritDmg = 1f, AttackSpeed = 0.5f
            };
        }

        private SkillData MakePhysicalSkill(float multiplier = 1.0f)
        {
            return new SkillData
            {
                ID = "test_phys",
                DamageType = DamageType.Physical,
                DamageMultiplier = multiplier,
                Pattern = AttackPattern.MeleeSwing,
                Range = 2f,
                AoERadius = 2f
            };
        }

        private SkillData MakeMagicalSkill(float multiplier = 1.0f)
        {
            return new SkillData
            {
                ID = "test_magic",
                DamageType = DamageType.Magical,
                DamageMultiplier = multiplier,
                Pattern = AttackPattern.AoECircle,
                Range = 3f,
                AoERadius = 3f
            };
        }

        [Test]
        public void PhysicalDamage_PositiveResult()
        {
            var attacker = MakeAttacker(atk: 50);
            var defender = MakeDefender(def: 10);
            var skill = MakePhysicalSkill(1.0f);

            var result = DamageFormula.Calculate(attacker, defender, skill, 42);

            // ATK(50) - DEF(10)*0.5 = 45, * 1.0 multiplier, * ~1.0 variance
            Assert.That(result.Damage, Is.GreaterThan(0));
            Assert.That(result.Damage, Is.InRange(36, 54)); // 45 * 0.9 to 45 * 1.1 roughly
        }

        [Test]
        public void MagicalDamage_UsesMAtkAndMDef()
        {
            var attacker = MakeAttacker(matk: 60);
            var defender = MakeDefender(mdef: 10);
            var skill = MakeMagicalSkill(1.0f);

            var result = DamageFormula.Calculate(attacker, defender, skill, 42);

            // MATK(60) - MDEF(10)*0.5 = 55, * 1.0 multiplier
            Assert.That(result.Damage, Is.GreaterThan(0));
            Assert.That(result.Damage, Is.InRange(44, 66)); // 55 * 0.9 to 55 * 1.1
        }

        [Test]
        public void MinimumDamage_IsAtLeastOne()
        {
            var attacker = MakeAttacker(atk: 1);
            var defender = MakeDefender(def: 200);
            var skill = MakePhysicalSkill(1.0f);

            var result = DamageFormula.Calculate(attacker, defender, skill, 42);

            // ATK(1) - DEF(200)*0.5 = -99, clamped to 1
            Assert.That(result.Damage, Is.EqualTo(1));
        }

        [Test]
        public void DamageMultiplier_ScalesDamage()
        {
            var attacker = MakeAttacker(atk: 50);
            var defender = MakeDefender(def: 10);
            var skill1x = MakePhysicalSkill(1.0f);
            var skill2x = MakePhysicalSkill(2.0f);

            // Use same seed for deterministic comparison
            var result1 = DamageFormula.Calculate(attacker, defender, skill1x, 100);
            var result2 = DamageFormula.Calculate(attacker, defender, skill2x, 100);

            Assert.That(result2.Damage, Is.GreaterThan(result1.Damage));
        }

        [Test]
        public void CriticalHit_IncreasesDamage()
        {
            // Use 100% crit rate to guarantee crit
            var attacker = MakeAttacker(atk: 50, critRate: 1.0f, critDmg: 2.0f);
            var defender = MakeDefender(def: 10);
            var skill = MakePhysicalSkill(1.0f);

            var result = DamageFormula.Calculate(attacker, defender, skill, 42);

            Assert.That(result.IsCritical, Is.True);
            // Base ~45, * 2.0 crit = ~90
            Assert.That(result.Damage, Is.GreaterThan(70));
        }

        [Test]
        public void ZeroCritRate_NeverCrits()
        {
            var attacker = MakeAttacker(critRate: 0f);
            var defender = MakeDefender();
            var skill = MakePhysicalSkill();

            // Test multiple seeds — none should crit
            for (int seed = 0; seed < 50; seed++)
            {
                var result = DamageFormula.Calculate(attacker, defender, skill, seed);
                Assert.That(result.IsCritical, Is.False, $"Crit on seed {seed} with 0% crit rate");
            }
        }

        [Test]
        public void Deterministic_SameSeedSameResult()
        {
            var attacker = MakeAttacker(atk: 50, critRate: 0.5f);
            var defender = MakeDefender();
            var skill = MakePhysicalSkill();

            var result1 = DamageFormula.Calculate(attacker, defender, skill, 777);
            var result2 = DamageFormula.Calculate(attacker, defender, skill, 777);

            Assert.That(result1.Damage, Is.EqualTo(result2.Damage));
            Assert.That(result1.IsCritical, Is.EqualTo(result2.IsCritical));
        }
    }
}
