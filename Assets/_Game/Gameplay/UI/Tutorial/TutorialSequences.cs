namespace ConquerChronicles.Gameplay.UI.Tutorial
{
    public static class TutorialSequences
    {
        public static readonly string[] MainMenu = new[]
        {
            "Welcome to Conquer Chronicles!",
            "Tap 'Battle' to enter combat.",
            "Visit Equipment to gear up your hero.",
            "Mine resources while you're away!"
        };

        public static readonly string[] Gameplay = new[]
        {
            "Your hero fights automatically!",
            "Skills fire on cooldown — no tapping needed.",
            "Watch your HP bar — if it runs out, you lose gold and items!",
            "Defeated enemies drop gold, XP, and loot."
        };

        public static readonly string[] Equipment = new[]
        {
            "Equip gear to boost your stats.",
            "Upgrade equipment up to +12 — but beware of failure!",
            "Socket gems for extra stat bonuses.",
            "A DragonBall protects gear from destruction at +10 and above."
        };

        public static readonly string[] Mining = new[]
        {
            "Select a mine and teleport there.",
            "Mining works while you're away!",
            "Come back later to collect your ores."
        };
    }
}
