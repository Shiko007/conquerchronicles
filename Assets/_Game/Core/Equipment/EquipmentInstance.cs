using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Equipment
{
    public class EquipmentInstance
    {
        public EquipmentData Data;
        public int UpgradeLevel;
        public GemData[] SocketedGems;

        private static readonly GemData EmptyGem = default;

        // GDD upgrade bonus percentages: index 0 = +0 (no bonus), index 12 = +12 (155%)
        private static readonly float[] UpgradeBonusPercent = new float[]
        {
            0f,     // +0: no bonus
            0.05f,  // +1: +5%
            0.10f,  // +2: +10%
            0.16f,  // +3: +16%
            0.22f,  // +4: +22%
            0.30f,  // +5: +30%
            0.40f,  // +6: +40%
            0.52f,  // +7: +52%
            0.66f,  // +8: +66%
            0.82f,  // +9: +82%
            1.00f,  // +10: +100%
            1.25f,  // +11: +125%
            1.55f   // +12: +155%
        };

        public EquipmentInstance(EquipmentData data)
        {
            Data = data;
            UpgradeLevel = 0;
            SocketedGems = new GemData[data.MaxSockets];
        }

        public CharacterStats ComputeStats()
        {
            var stats = Data.BaseStats;

            // Apply GDD upgrade bonus: scales ALL base stats by the percentage table
            if (UpgradeLevel > 0 && UpgradeLevel <= 12)
            {
                float bonus = UpgradeBonusPercent[UpgradeLevel];
                var upgradeBonus = new CharacterStats
                {
                    HP = (int)(Data.BaseStats.HP * bonus),
                    MP = (int)(Data.BaseStats.MP * bonus),
                    ATK = (int)(Data.BaseStats.ATK * bonus),
                    DEF = (int)(Data.BaseStats.DEF * bonus),
                    MATK = (int)(Data.BaseStats.MATK * bonus),
                    MDEF = (int)(Data.BaseStats.MDEF * bonus),
                    AGI = (int)(Data.BaseStats.AGI * bonus),
                    CritRate = Data.BaseStats.CritRate * bonus,
                    CritDmg = Data.BaseStats.CritDmg * bonus,
                    AttackSpeed = Data.BaseStats.AttackSpeed * bonus
                };

                stats = stats + upgradeBonus;
            }

            // Add gem stat bonuses from all socketed gems
            for (int i = 0; i < SocketedGems.Length; i++)
            {
                if (IsGemSocketed(i))
                {
                    stats = stats + SocketedGems[i].GetBonus();
                }
            }

            return stats;
        }

        public bool CanSocket(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SocketedGems.Length)
                return false;

            return !IsGemSocketed(slotIndex);
        }

        public void Socket(int slotIndex, GemData gem)
        {
            if (!CanSocket(slotIndex))
                return;

            SocketedGems[slotIndex] = gem;
        }

        public GemData Unsocket(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= SocketedGems.Length)
                return EmptyGem;

            if (!IsGemSocketed(slotIndex))
                return EmptyGem;

            var gem = SocketedGems[slotIndex];
            SocketedGems[slotIndex] = default;
            return gem;
        }

        private bool IsGemSocketed(int slotIndex)
        {
            // A gem is considered socketed if its Tier is greater than 0
            // (default GemData has Tier = 0, which is invalid since valid tiers are 1-9)
            return SocketedGems[slotIndex].Tier > 0;
        }
    }
}
