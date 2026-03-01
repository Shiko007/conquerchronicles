using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Equipment
{
    public class EquipmentInstance
    {
        public EquipmentData Data;
        public int UpgradeLevel;
        public GemData[] SocketedGems;

        private static readonly GemData EmptyGem = default;

        public EquipmentInstance(EquipmentData data)
        {
            Data = data;
            UpgradeLevel = 0;
            SocketedGems = new GemData[data.MaxSockets];
        }

        public CharacterStats ComputeStats()
        {
            var stats = Data.BaseStats;

            // Apply upgrade bonus: for each +level, ATK scales by +10%, DEF scales by +8%
            if (UpgradeLevel > 0)
            {
                var upgradeBonus = new CharacterStats
                {
                    ATK = (int)(Data.BaseStats.ATK * 0.1f * UpgradeLevel),
                    DEF = (int)(Data.BaseStats.DEF * 0.08f * UpgradeLevel)
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
