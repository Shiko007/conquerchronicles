using UnityEngine;

namespace ConquerChronicles.Gameplay.Audio
{
    [CreateAssetMenu(menuName = "Conquer Chronicles/SFX Library", fileName = "SFXLibrary")]
    public class SFXLibrary : ScriptableObject
    {
        [Header("Combat")]
        public AudioClip Hit;
        public AudioClip CriticalHit;
        public AudioClip EnemyDeath;
        public AudioClip SkillCast;
        public AudioClip PlayerHit;

        [Header("Progression")]
        public AudioClip LevelUp;
        public AudioClip LootCollect;
        public AudioClip GoldCollect;

        [Header("Equipment")]
        public AudioClip UpgradeSuccess;
        public AudioClip UpgradeFail;
        public AudioClip UpgradeDestroy;
        public AudioClip EquipItem;

        [Header("UI")]
        public AudioClip ButtonClick;
        public AudioClip MenuOpen;
        public AudioClip MenuClose;

        [Header("Mining")]
        public AudioClip MineCollect;
        public AudioClip Teleport;

        [Header("Music")]
        public AudioClip CombatMusic;
        public AudioClip MenuMusic;
    }
}
