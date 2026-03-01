using System.Collections.Generic;
using ConquerChronicles.Core.Character;

namespace ConquerChronicles.Core.Equipment
{
    /// <summary>
    /// Hardcoded test equipment for development. Will be replaced by ScriptableObjects.
    /// RequiredClass = CharacterClass.None means any class can equip it.
    /// </summary>
    public static class TestEquipment
    {
        // =====================================================================
        // Level 1-10 — Starter Gear (Normal quality, 0 sockets)
        // =====================================================================

        public static EquipmentData IronHelmet => new()
        {
            ID = "helm_iron",
            Name = "Iron Helmet",
            Slot = EquipmentSlot.Headgear,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 2, DEF = 5 },
            MaxSockets = 0,
            Description = "A plain iron helmet. Better than nothing."
        };

        public static EquipmentData IronArmor => new()
        {
            ID = "armor_iron",
            Name = "Iron Armor",
            Slot = EquipmentSlot.Armor,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 8, HP = 20 },
            MaxSockets = 0,
            Description = "Standard-issue iron armor for new adventurers."
        };

        public static EquipmentData IronBlade => new()
        {
            ID = "weapon_iron_blade",
            Name = "Iron Blade",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 10 },
            MaxSockets = 0,
            Description = "A simple iron blade, dull but dependable."
        };

        public static EquipmentData WoodenShield => new()
        {
            ID = "offhand_wooden_shield",
            Name = "Wooden Shield",
            Slot = EquipmentSlot.OffHand,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 6 },
            MaxSockets = 0,
            Description = "A sturdy wooden shield. Blocks minor blows."
        };

        public static EquipmentData IronBoots => new()
        {
            ID = "boots_iron",
            Name = "Iron Boots",
            Slot = EquipmentSlot.Boots,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { AGI = 2, DEF = 3 },
            MaxSockets = 0,
            Description = "Heavy iron boots. Slow but protective."
        };

        public static EquipmentData CopperNecklace => new()
        {
            ID = "neck_copper",
            Name = "Copper Necklace",
            Slot = EquipmentSlot.Necklace,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { HP = 15 },
            MaxSockets = 0,
            Description = "A simple copper necklace with a faint warmth."
        };

        public static EquipmentData CopperRing => new()
        {
            ID = "ring_copper",
            Name = "Copper Ring",
            Slot = EquipmentSlot.Ring,
            Quality = EquipmentQuality.Normal,
            RequiredLevel = 1,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 3 },
            MaxSockets = 0,
            Description = "A thin copper ring. Grants a sliver of strength."
        };

        // =====================================================================
        // Level 15-25 — Steel Tier (Refined quality, 1 socket)
        // =====================================================================

        public static EquipmentData SteelHelmet => new()
        {
            ID = "helm_steel",
            Name = "Steel Helmet",
            Slot = EquipmentSlot.Headgear,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 15,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 5, DEF = 12 },
            MaxSockets = 1,
            Description = "Forged from tempered steel. Trusted by soldiers."
        };

        public static EquipmentData SteelArmor => new()
        {
            ID = "armor_steel",
            Name = "Steel Armor",
            Slot = EquipmentSlot.Armor,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 15,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 18, HP = 50 },
            MaxSockets = 1,
            Description = "Heavy steel plates. A warrior's best friend."
        };

        public static EquipmentData SteelBlade => new()
        {
            ID = "weapon_steel_blade",
            Name = "Steel Blade",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 15,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 22 },
            MaxSockets = 1,
            Description = "A well-honed steel blade. Cuts through bone."
        };

        public static EquipmentData SteelShield => new()
        {
            ID = "offhand_steel_shield",
            Name = "Steel Shield",
            Slot = EquipmentSlot.OffHand,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 15,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 14, HP = 30 },
            MaxSockets = 1,
            Description = "A solid steel shield. Deflects arrows with ease."
        };

        public static EquipmentData SteelBoots => new()
        {
            ID = "boots_steel",
            Name = "Steel Boots",
            Slot = EquipmentSlot.Boots,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 15,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { AGI = 4, DEF = 8 },
            MaxSockets = 1,
            Description = "Reinforced steel greaves. Balanced weight."
        };

        public static EquipmentData SilverNecklace => new()
        {
            ID = "neck_silver",
            Name = "Silver Necklace",
            Slot = EquipmentSlot.Necklace,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 18,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { HP = 40, MP = 20 },
            MaxSockets = 1,
            Description = "A silver chain imbued with minor healing magic."
        };

        public static EquipmentData SilverRing => new()
        {
            ID = "ring_silver",
            Name = "Silver Ring",
            Slot = EquipmentSlot.Ring,
            Quality = EquipmentQuality.Refined,
            RequiredLevel = 18,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 8, CritRate = 0.02f },
            MaxSockets = 1,
            Description = "A polished silver ring. Sharpens the wearer's focus."
        };

        // =====================================================================
        // Level 30-45 — Dragon / Jade Tier (Unique quality, 1 socket)
        // =====================================================================

        public static EquipmentData DragonHelmet => new()
        {
            ID = "helm_dragon",
            Name = "Dragon Helmet",
            Slot = EquipmentSlot.Headgear,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 30,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 10, DEF = 20, HP = 80 },
            MaxSockets = 1,
            Description = "Forged from dragon scales. Radiates ancient power."
        };

        public static EquipmentData DragonArmor => new()
        {
            ID = "armor_dragon",
            Name = "Dragon Armor",
            Slot = EquipmentSlot.Armor,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 32,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 35, HP = 120, MDEF = 10 },
            MaxSockets = 1,
            Description = "Scales of an ancient dragon woven into plate armor."
        };

        public static EquipmentData DragonBlade => new()
        {
            ID = "weapon_dragon_blade",
            Name = "Dragon Blade",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 35,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 45, CritDmg = 0.1f },
            MaxSockets = 1,
            Description = "A blade quenched in dragon fire. Burns on contact."
        };

        public static EquipmentData DragonShield => new()
        {
            ID = "offhand_dragon_shield",
            Name = "Dragon Shield",
            Slot = EquipmentSlot.OffHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 30,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 25, HP = 60, MDEF = 15 },
            MaxSockets = 1,
            Description = "A shield of dragon bone. Resists magic and steel alike."
        };

        public static EquipmentData DragonBoots => new()
        {
            ID = "boots_dragon",
            Name = "Dragon Boots",
            Slot = EquipmentSlot.Boots,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 30,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { AGI = 8, DEF = 15 },
            MaxSockets = 1,
            Description = "Boots lined with dragon hide. Light as wind."
        };

        public static EquipmentData JadeNecklace => new()
        {
            ID = "neck_jade",
            Name = "Jade Necklace",
            Slot = EquipmentSlot.Necklace,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 35,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { HP = 80, MP = 40, MDEF = 8 },
            MaxSockets = 1,
            Description = "A jade pendant said to ward off dark curses."
        };

        public static EquipmentData JadeRing => new()
        {
            ID = "ring_jade",
            Name = "Jade Ring",
            Slot = EquipmentSlot.Ring,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 35,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 15, MATK = 10, CritRate = 0.03f },
            MaxSockets = 1,
            Description = "A jade ring pulsing with mystical energy."
        };

        // =====================================================================
        // Level 50-70 — Phoenix / Gold Tier (Elite quality, 2 sockets)
        // =====================================================================

        public static EquipmentData PhoenixHelmet => new()
        {
            ID = "helm_phoenix",
            Name = "Phoenix Helmet",
            Slot = EquipmentSlot.Headgear,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 50,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 18, DEF = 30, HP = 150 },
            MaxSockets = 2,
            Description = "A helmet wreathed in phoenix feathers. Blazes with inner fire."
        };

        public static EquipmentData PhoenixArmor => new()
        {
            ID = "armor_phoenix",
            Name = "Phoenix Armor",
            Slot = EquipmentSlot.Armor,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 55,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 55, HP = 250, MDEF = 20 },
            MaxSockets = 2,
            Description = "Armor bathed in phoenix flame. Warms the soul, scorches the foe."
        };

        public static EquipmentData PhoenixBlade => new()
        {
            ID = "weapon_phoenix_blade",
            Name = "Phoenix Blade",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 55,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 80, CritRate = 0.05f, CritDmg = 0.2f },
            MaxSockets = 2,
            Description = "A blade reborn from ashes. Each strike ignites the air."
        };

        public static EquipmentData PhoenixShield => new()
        {
            ID = "offhand_phoenix_shield",
            Name = "Phoenix Shield",
            Slot = EquipmentSlot.OffHand,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 50,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 40, HP = 120, MDEF = 25 },
            MaxSockets = 2,
            Description = "A shield forged in rebirth fire. Absorbs flame and fury."
        };

        public static EquipmentData PhoenixBoots => new()
        {
            ID = "boots_phoenix",
            Name = "Phoenix Boots",
            Slot = EquipmentSlot.Boots,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 50,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { AGI = 15, DEF = 22, AttackSpeed = 0.1f },
            MaxSockets = 2,
            Description = "Boots that leave trails of ember. Swift as rising flame."
        };

        public static EquipmentData GoldNecklace => new()
        {
            ID = "neck_gold",
            Name = "Gold Necklace",
            Slot = EquipmentSlot.Necklace,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 55,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { HP = 150, MP = 80, MDEF = 15 },
            MaxSockets = 2,
            Description = "A necklace of pure gold enchanted by a grand taoist."
        };

        public static EquipmentData GoldRing => new()
        {
            ID = "ring_gold",
            Name = "Gold Ring",
            Slot = EquipmentSlot.Ring,
            Quality = EquipmentQuality.Elite,
            RequiredLevel = 55,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 25, MATK = 20, CritRate = 0.05f },
            MaxSockets = 2,
            Description = "A gold ring inscribed with runes of devastation."
        };

        // =====================================================================
        // Level 80+ — Conqueror Tier (Super quality, 2 sockets)
        // =====================================================================

        public static EquipmentData ConquerorBlade => new()
        {
            ID = "weapon_conqueror_blade",
            Name = "Conqueror Blade",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 80,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 130, CritRate = 0.08f, CritDmg = 0.35f },
            MaxSockets = 2,
            Description = "The legendary blade of the Conqueror. Nations trembled at its gleam."
        };

        public static EquipmentData ConquerorArmor => new()
        {
            ID = "armor_conqueror",
            Name = "Conqueror Armor",
            Slot = EquipmentSlot.Armor,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 80,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 85, HP = 400, MDEF = 35 },
            MaxSockets = 2,
            Description = "Armor worn by the one who conquered all. Indomitable."
        };

        public static EquipmentData ConquerorHelmet => new()
        {
            ID = "helm_conqueror",
            Name = "Conqueror Helmet",
            Slot = EquipmentSlot.Headgear,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 82,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 25, DEF = 45, HP = 200 },
            MaxSockets = 2,
            Description = "A crown of conquest. Its wearer commands fear itself."
        };

        public static EquipmentData ConquerorShield => new()
        {
            ID = "offhand_conqueror_shield",
            Name = "Conqueror Shield",
            Slot = EquipmentSlot.OffHand,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 80,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { DEF = 60, HP = 180, MDEF = 40 },
            MaxSockets = 2,
            Description = "The unbreakable shield. No force has ever shattered it."
        };

        public static EquipmentData ConquerorBoots => new()
        {
            ID = "boots_conqueror",
            Name = "Conqueror Boots",
            Slot = EquipmentSlot.Boots,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 80,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { AGI = 22, DEF = 35, AttackSpeed = 0.15f },
            MaxSockets = 2,
            Description = "Boots of the Conqueror. Each step echoes through the battlefield."
        };

        public static EquipmentData ConquerorNecklace => new()
        {
            ID = "neck_conqueror",
            Name = "Conqueror Necklace",
            Slot = EquipmentSlot.Necklace,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 85,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { HP = 220, MP = 120, MDEF = 25, CritRate = 0.03f },
            MaxSockets = 2,
            Description = "A necklace imbued with the soul of a fallen deity."
        };

        public static EquipmentData ConquerorRing => new()
        {
            ID = "ring_conqueror",
            Name = "Conqueror Ring",
            Slot = EquipmentSlot.Ring,
            Quality = EquipmentQuality.Super,
            RequiredLevel = 85,
            RequiredClass = CharacterClass.None,
            BaseStats = new CharacterStats { ATK = 35, MATK = 30, CritRate = 0.07f, CritDmg = 0.2f },
            MaxSockets = 2,
            Description = "The Conqueror's signet ring. Its power is absolute."
        };

        // =====================================================================
        // Class-Specific Weapons (Unique quality, level 40)
        // =====================================================================

        public static EquipmentData TrojanCleaver => new()
        {
            ID = "weapon_trojan_cleaver",
            Name = "Trojan Cleaver",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.Trojan,
            BaseStats = new CharacterStats { ATK = 55, CritRate = 0.04f, CritDmg = 0.15f },
            MaxSockets = 1,
            Description = "A massive cleaver wielded only by Trojans. Strikes with brutal force."
        };

        public static EquipmentData WarriorHalberd => new()
        {
            ID = "weapon_warrior_halberd",
            Name = "Warrior Halberd",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.Warrior,
            BaseStats = new CharacterStats { ATK = 48, DEF = 10, HP = 50 },
            MaxSockets = 1,
            Description = "A halberd favored by Warriors. Reach and resilience in one weapon."
        };

        public static EquipmentData ArcherLongbow => new()
        {
            ID = "weapon_archer_longbow",
            Name = "Archer Longbow",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.Archer,
            BaseStats = new CharacterStats { ATK = 42, AGI = 10, CritRate = 0.06f },
            MaxSockets = 1,
            Description = "A longbow crafted for Archers. Strikes from beyond sight."
        };

        public static EquipmentData WaterTaoistStaff => new()
        {
            ID = "weapon_water_taoist_staff",
            Name = "Tidal Staff",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.WaterTaoist,
            BaseStats = new CharacterStats { MATK = 45, MP = 60, HP = 40 },
            MaxSockets = 1,
            Description = "A staff that commands the tides. Water Taoists channel its flow."
        };

        public static EquipmentData FireTaoistWand => new()
        {
            ID = "weapon_fire_taoist_wand",
            Name = "Inferno Wand",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.FireTaoist,
            BaseStats = new CharacterStats { MATK = 50, ATK = 10, CritDmg = 0.12f },
            MaxSockets = 1,
            Description = "A wand of concentrated flame. Fire Taoists wield its fury."
        };

        public static EquipmentData NinjaKatana => new()
        {
            ID = "weapon_ninja_katana",
            Name = "Shadow Katana",
            Slot = EquipmentSlot.MainHand,
            Quality = EquipmentQuality.Unique,
            RequiredLevel = 40,
            RequiredClass = CharacterClass.Ninja,
            BaseStats = new CharacterStats { ATK = 40, AGI = 12, AttackSpeed = 0.15f, CritRate = 0.05f },
            MaxSockets = 1,
            Description = "A katana forged in darkness. Only Ninjas can master its speed."
        };

        // =====================================================================
        // Utility Methods
        // =====================================================================

        /// <summary>
        /// Returns all test equipment items.
        /// </summary>
        public static EquipmentData[] GetAll()
        {
            return new[]
            {
                // Level 1-10: Starter (Normal)
                IronHelmet, IronArmor, IronBlade, WoodenShield, IronBoots,
                CopperNecklace, CopperRing,

                // Level 15-25: Steel (Refined)
                SteelHelmet, SteelArmor, SteelBlade, SteelShield, SteelBoots,
                SilverNecklace, SilverRing,

                // Level 30-45: Dragon / Jade (Unique)
                DragonHelmet, DragonArmor, DragonBlade, DragonShield, DragonBoots,
                JadeNecklace, JadeRing,

                // Level 50-70: Phoenix / Gold (Elite)
                PhoenixHelmet, PhoenixArmor, PhoenixBlade, PhoenixShield, PhoenixBoots,
                GoldNecklace, GoldRing,

                // Level 80+: Conqueror (Super)
                ConquerorBlade, ConquerorArmor, ConquerorHelmet, ConquerorShield,
                ConquerorBoots, ConquerorNecklace, ConquerorRing,

                // Class-Specific Weapons (Unique)
                TrojanCleaver, WarriorHalberd, ArcherLongbow,
                WaterTaoistStaff, FireTaoistWand, NinjaKatana
            };
        }

        /// <summary>
        /// Returns all test equipment items that match the specified slot.
        /// </summary>
        public static EquipmentData[] GetBySlot(EquipmentSlot slot)
        {
            var all = GetAll();
            var results = new List<EquipmentData>();

            for (int i = 0; i < all.Length; i++)
            {
                if (all[i].Slot == slot)
                    results.Add(all[i]);
            }

            return results.ToArray();
        }
    }
}
