namespace ConquerChronicles.Core.Combat
{
    public class SkillState
    {
        public SkillData Data;
        public float CooldownRemaining;
        public bool IsReady => CooldownRemaining <= 0f;

        public SkillState(SkillData data)
        {
            Data = data;
            CooldownRemaining = 0f;
        }

        public void UpdateCooldown(float deltaTime)
        {
            if (CooldownRemaining > 0f)
                CooldownRemaining -= deltaTime;
        }

        public void TriggerCooldown()
        {
            CooldownRemaining = Data.Cooldown;
        }
    }
}
