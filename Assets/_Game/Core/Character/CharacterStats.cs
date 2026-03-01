namespace ConquerChronicles.Core.Character
{
    [System.Serializable]
    public struct CharacterStats
    {
        public int HP;
        public int MP;
        public int ATK;
        public int DEF;
        public int MATK;
        public int MDEF;
        public int AGI;
        public float CritRate;
        public float CritDmg;
        public float AttackSpeed;

        public static CharacterStats operator +(CharacterStats a, CharacterStats b)
        {
            return new CharacterStats
            {
                HP = a.HP + b.HP,
                MP = a.MP + b.MP,
                ATK = a.ATK + b.ATK,
                DEF = a.DEF + b.DEF,
                MATK = a.MATK + b.MATK,
                MDEF = a.MDEF + b.MDEF,
                AGI = a.AGI + b.AGI,
                CritRate = a.CritRate + b.CritRate,
                CritDmg = a.CritDmg + b.CritDmg,
                AttackSpeed = a.AttackSpeed + b.AttackSpeed
            };
        }

        public static CharacterStats operator *(CharacterStats stats, float multiplier)
        {
            return new CharacterStats
            {
                HP = (int)(stats.HP * multiplier),
                MP = (int)(stats.MP * multiplier),
                ATK = (int)(stats.ATK * multiplier),
                DEF = (int)(stats.DEF * multiplier),
                MATK = (int)(stats.MATK * multiplier),
                MDEF = (int)(stats.MDEF * multiplier),
                AGI = (int)(stats.AGI * multiplier),
                CritRate = stats.CritRate * multiplier,
                CritDmg = stats.CritDmg * multiplier,
                AttackSpeed = stats.AttackSpeed * multiplier
            };
        }

        public static CharacterStats operator *(CharacterStats stats, int multiplier)
        {
            return new CharacterStats
            {
                HP = stats.HP * multiplier,
                MP = stats.MP * multiplier,
                ATK = stats.ATK * multiplier,
                DEF = stats.DEF * multiplier,
                MATK = stats.MATK * multiplier,
                MDEF = stats.MDEF * multiplier,
                AGI = stats.AGI * multiplier,
                CritRate = stats.CritRate * multiplier,
                CritDmg = stats.CritDmg * multiplier,
                AttackSpeed = stats.AttackSpeed * multiplier
            };
        }
    }
}
