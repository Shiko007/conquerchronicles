# Conquer Chronicles — Game Design Document

> **Version**: 1.0
> **Last Updated**: 2026-03-01
> **Engine**: Unity 6 LTS (6000.3.9f1), 2D URP
> **Platform**: Mobile (iOS first)
> **Art Style**: Pixel art, isometric perspective

---

## 1. Game Overview

**Conquer Chronicles** is a mobile idle-survivor game that combines **Conquer Online** MMO mechanics with **Vampire Survivors**-style auto-combat.

**Core Fantasy**: You are a warrior standing your ground against endless hordes. You don't run — they come to you. Between battles, you mine for gems, upgrade your gear to +12, socket powerful gems, and trade in the market.

**The Twist**: The player is **static** in the center of the screen. Enemies swarm from all edges. Combat is fully automatic — your character attacks based on equipped weapons, skills, and class abilities.

### Core Pillars
1. **Satisfying Auto-Combat** — Watch your character obliterate waves of enemies with escalating power
2. **Deep Gear Progression** — CO-style +12 upgrades with risk/reward, gem socketing for stat customization
3. **Idle-Friendly** — Mining runs offline, market refreshes passively, quick session-friendly stage runs
4. **Class Identity** — 6 distinct classes with unique playstyles and skill sets

---

## 2. Game Loop

```
┌─────────────────────────────────────────────────────┐
│                    MAIN MENU                         │
│  [Play Stage]  [Equipment]  [Mining]  [Market]      │
└──────┬──────────────┬───────────┬──────────┬────────┘
       │              │           │          │
       v              v           v          v
  ┌─────────┐   ┌──────────┐  ┌──────┐  ┌────────┐
  │ COMBAT  │   │EQUIPMENT │  │MINING│  │ MARKET │
  │ Stage   │   │ Upgrade  │  │ Idle │  │ Browse │
  │ Waves   │   │ Socket   │  │ AFK  │  │  Buy   │
  │ Boss    │   │ Equip    │  │      │  │  Sell  │
  └────┬────┘   └──────────┘  └──────┘  └────────┘
       │
       v
  ┌──────────┐
  │ REWARDS  │
  │ Gold/XP  │──> Level Up ──> Stat Points
  │ Loot     │──> Equipment / Gems
  │ MetaCoin │──> Permanent Upgrades
  └──────────┘
```

---

## 3. Character System

### 3.1 Classes

| Class | Role | Primary Stat | Weapon Type | Attack Style |
|-------|------|-------------|-------------|--------------|
| **Trojan** | Melee DPS | ATK | Dual Blades | Fast melee swings, AoE arcs |
| **Warrior** | Tank | HP/DEF | Sword + Shield | Slower hits, AoE knockback |
| **Archer** | Ranged DPS | ATK/AGI | Bow | Piercing projectiles, multi-shot |
| **Water Taoist** | Support | MATK/MP | Backsword | Heals, slows, defensive buffs |
| **Fire Taoist** | Magic DPS | MATK | Backsword | Burn DoT, AoE explosions, novas |
| **Ninja** | Crit/Speed | AGI/CritRate | Katana + Shuriken | Fast attacks, crit bursts, poison |

### 3.2 Stats

```csharp
// Core/Character/CharacterStats.cs
public struct CharacterStats
{
    public int HP;            // Health points
    public int MP;            // Mana points
    public int ATK;           // Physical attack power
    public int DEF;           // Physical defense
    public int MATK;          // Magic attack power
    public int MDEF;          // Magic defense
    public int AGI;           // Agility (attack speed + dodge)
    public float CritRate;    // Critical hit chance (0.0 - 1.0)
    public float CritDmg;     // Critical damage multiplier (e.g. 1.5 = 150%)
    public float AttackSpeed; // Attacks per second
}
```

### 3.3 Class Enum

```csharp
// Core/Character/CharacterClass.cs
public enum CharacterClass
{
    Trojan,
    Warrior,
    Archer,
    WaterTaoist,
    FireTaoist,
    Ninja
}
```

### 3.4 Base Stats Per Class (Level 1)

| Stat | Trojan | Warrior | Archer | Water Taoist | Fire Taoist | Ninja |
|------|--------|---------|--------|-------------|-------------|-------|
| HP | 120 | 180 | 90 | 100 | 80 | 95 |
| MP | 30 | 20 | 40 | 120 | 100 | 50 |
| ATK | 25 | 18 | 22 | 8 | 10 | 20 |
| DEF | 12 | 22 | 8 | 10 | 6 | 10 |
| MATK | 5 | 3 | 5 | 25 | 28 | 8 |
| MDEF | 5 | 8 | 5 | 18 | 15 | 8 |
| AGI | 15 | 8 | 18 | 10 | 10 | 22 |
| CritRate | 0.05 | 0.02 | 0.08 | 0.03 | 0.03 | 0.12 |
| CritDmg | 1.5 | 1.3 | 1.6 | 1.4 | 1.5 | 1.8 |
| AttackSpeed | 1.2 | 0.8 | 1.0 | 0.9 | 0.9 | 1.4 |

### 3.5 Leveling

- XP gained from killing enemies during combat stages
- Each level grants **3 stat points** to allocate freely
- XP curve: `requiredXP = 100 * level^1.5` (soft exponential)
- Max level: 130 (matching Conquer Online's classic cap)

### 3.6 Stat Point Allocation

Per stat point spent:

| Stat Target | Bonus |
|-------------|-------|
| Vitality | +10 HP |
| Mana | +8 MP |
| Strength | +3 ATK |
| Agility | +2 AGI, +0.002 CritRate |
| Spirit | +3 MATK |

---

## 4. Combat System

### 4.1 Overview

- Player character is **fixed at the center** of the isometric arena
- Enemies spawn from screen edges and walk toward the player
- Character **auto-attacks** the nearest enemy using their equipped weapon/skills
- Skills fire automatically when off cooldown, prioritizing the best target
- Stage ends when all waves + boss are defeated, or the player dies

### 4.2 Damage Formula

```
Physical: damage = (ATK - DEF * 0.5) * skillMultiplier * variance * critBonus
Magical:  damage = (MATK - MDEF * 0.5) * skillMultiplier * variance * critBonus

variance = random(0.9, 1.1)
critBonus = isCrit ? CritDmg : 1.0
minimum damage = 1
```

### 4.3 Attack Patterns

| Pattern | Description | Used By |
|---------|-------------|---------|
| `MeleeSwing` | Arc damage in front of character | Trojan, Warrior |
| `RangedSingle` | Single projectile to nearest | Archer, Ninja |
| `RangedPiercing` | Projectile passes through enemies | Archer |
| `AoECircle` | Damage in radius around player | Warrior, Fire Taoist |
| `AoECone` | Cone-shaped area in a direction | Trojan |
| `MultiProjectile` | N projectiles in a spread | Archer, Ninja |
| `Orbiting` | Orbs circling the player | Fire Taoist |
| `Chain` | Jumps between N enemies | Water Taoist |
| `Nova` | Expanding ring from player | Fire Taoist |
| `SummonZone` | Persistent damage zone on ground | Water Taoist |

### 4.4 Status Effects

| Effect | Description | Duration |
|--------|-------------|----------|
| Poison | Tick damage every 0.5s | 3s |
| Burn | Tick damage every 1s, higher per tick | 4s |
| Slow | Reduce enemy move speed by 40% | 3s |
| Stun | Enemy stops completely | 1.5s |
| Bleed | Tick damage that stacks | 5s |

### 4.5 Class Skills

#### Trojan
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Blade Fury | MeleeSwing | 1.5s | 1.5x | Wide arc melee slash |
| Cyclone | AoECircle | 8s | 2.0x | Spinning attack hitting all nearby |
| Hercules | Self-buff | 15s | — | +50% ATK for 5 seconds |

#### Warrior
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Shield Bash | MeleeSwing | 2s | 1.2x | Knockback + Stun 1s |
| Superman | AoECircle | 10s | 1.8x | AoE slam, taunts enemies |
| Defensive Stance | Self-buff | 20s | — | +80% DEF for 6 seconds |

#### Archer
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Scatter Shot | MultiProjectile (5) | 3s | 0.8x | 5 arrows in a fan |
| Arrow Rain | SummonZone | 10s | 0.5x/tick | Rain arrows in area for 3s |
| Fly | Self-buff | 18s | — | +60% AttackSpeed for 4s |

#### Water Taoist
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Healing Wave | Self-heal | 8s | — | Restore 20% max HP |
| Stigma | AoECircle | 6s | 1.0x | Slow all enemies in range |
| Celestial Chain | Chain (4) | 5s | 1.3x | Bounces between 4 enemies |

#### Fire Taoist
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Fire Ring | Orbiting | 12s | 0.6x/hit | 3 fire orbs orbit for 6s |
| Meteor | AoECircle | 10s | 3.0x | Massive single AoE, Burn |
| Fire Circle | Nova | 7s | 1.5x | Expanding fire ring |

#### Ninja
| Skill | Pattern | Cooldown | Multiplier | Description |
|-------|---------|----------|-----------|-------------|
| Shuriken Spread | MultiProjectile (8) | 3s | 0.6x | 8 shurikens in all directions |
| Shadow Step | Self-buff | 12s | — | +100% CritRate for 3s |
| Toxic Fog | SummonZone | 9s | 0.4x/tick | Poison cloud for 4s |

---

## 5. Equipment System

### 5.1 Equipment Slots

| Slot | Stat Focus |
|------|-----------|
| Headgear | DEF, HP |
| Armor | DEF, HP, MDEF |
| Weapon | ATK or MATK (class-specific) |
| Shield | DEF, MDEF (Warrior only; others get offhand) |
| Necklace | MATK, MDEF |
| Ring | ATK, CritRate |
| Boots | AGI, MoveSpeed |

### 5.2 Equipment Quality Tiers

| Quality | Color | Max Gem Slots | Stat Multiplier |
|---------|-------|--------------|-----------------|
| Normal | White | 0 | 1.0x |
| Refined | Blue | 1 | 1.2x |
| Unique | Purple | 1 | 1.5x |
| Elite | Orange | 2 | 1.8x |
| Super | Gold | 3 | 2.2x |

### 5.3 Equipment Upgrade System (+1 to +12)

The core addictive loop from Conquer Online.

| Level | Success Rate | On Failure | Stat Bonus |
|-------|-------------|------------|-----------|
| +1 | 100% | — | +5% |
| +2 | 100% | — | +10% |
| +3 | 95% | Downgrade -1 | +16% |
| +4 | 85% | Downgrade -1 | +22% |
| +5 | 70% | Downgrade -1 | +30% |
| +6 | 55% | Downgrade -1 | +40% |
| +7 | 40% | Reset to +0 | +52% |
| +8 | 25% | Reset to +0 | +66% |
| +9 | 15% | Reset to +0 | +82% |
| +10 | 8% | DESTROYED | +100% |
| +11 | 4% | DESTROYED | +125% |
| +12 | 2% | DESTROYED | +155% |

**Protection Items**:
- **Meteor** — Required crafting material for any upgrade attempt
- **DragonBall** — Prevents destruction on failure at +10/+11/+12 (item resets to +0 instead)

```csharp
// Core/Equipment/UpgradeCalculator.cs
public struct UpgradeResult
{
    public bool Success;
    public int NewLevel;
    public bool ItemDestroyed;
}
```

### 5.4 Gem Socketing System

Equipment with gem slots can have gems inserted for permanent stat bonuses.

| Gem Type | Stat Bonus (per tier) | Visual Color |
|----------|----------------------|-------------|
| Dragon Gem | +HP (50/100/200/400/800/1600/3200/6400/12800) | Red |
| Phoenix Gem | +ATK (5/10/20/40/80/160/320/640/1280) | Orange |
| Moon Gem | +MP (30/60/120/240/480/960/1920/3840/7680) | Blue |
| Tortoise Gem | +DEF (5/10/20/40/80/160/320/640/1280) | Green |
| Thunder Gem | +MATK (5/10/20/40/80/160/320/640/1280) | Yellow |
| Fury Gem | +CritRate (0.01 per tier) | Purple |
| Violet Gem | +XP% (5% per tier) | Violet |
| Rainbow Gem | +All stats (small, 2 per tier) | Rainbow |

- Gems come in **tiers 1-9**
- **Two gems of the same type and tier** can be combined into the next tier
- Gems are obtained from **mining** and **enemy drops**

---

## 6. Stage System

### 6.1 Structure

Each stage consists of:
- **5-10 enemy waves** with increasing enemy count and variety
- **A boss wave** at the end
- **Completion rewards**: Gold, XP, equipment drops, gems, meta currency

### 6.2 Stage Progression

- Stages unlock linearly (complete Stage 1 to unlock Stage 2)
- Each stage has a **recommended level** displayed
- Stages can be **replayed** for grinding
- Star rating based on: HP remaining, time taken, no-hit bonus

### 6.3 Enemy Spawning

```
Wave start → Delay → Spawn groups from edges → Enemies walk to center → Wave cleared → Next wave
```

Spawn edges: North, South, East, West, Random, All (surround)

Spawn patterns:
- **Burst**: All enemies appear at once
- **Stream**: Enemies trickle in one by one
- **Surround**: Enemies appear from all 4 edges simultaneously

### 6.4 Boss Mechanics

Bosses are large enemies with:
- 10-50x normal enemy HP
- Special attack patterns (telegraphed AoE zones)
- Phase transitions at HP thresholds (50%, 25%)
- Guaranteed loot drops on kill

---

## 7. Mid-Run Progression

### 7.1 During Combat

- Killing enemies grants **mid-run XP** (separate from character XP)
- On mid-run level up, game **pauses** and offers **3 random upgrades**
- Pick one upgrade per level
- Upgrades stack and compound throughout the run

### 7.2 Upgrade Categories

| Category | Examples |
|----------|---------|
| Stat Boost | +10% ATK, +15% HP, +20% CritDmg |
| Skill Enhancement | -20% Cooldown, +1 Projectile Count, +30% AoE Radius |
| New Ability | Orbiting Shield, Thorns Aura, Life Steal |
| Weapon Evolution | Base weapon + catalyst = evolved weapon |

### 7.3 Weapon Evolution (examples)

| Base Upgrade | Catalyst Upgrade | Evolved Weapon |
|-------------|-----------------|---------------|
| "Flame Blade" buff | "Dragon Heart" buff | "Inferno Blade" — 3x fire AoE on every 5th hit |
| "Ice Arrow" buff | "Blizzard Ring" buff | "Absolute Zero" — arrows freeze all enemies in path |
| "Shadow Kunai" buff | "Venom Coat" buff | "Death Lotus" — poison shuriken spiral |

---

## 8. Mining System (Idle)

### 8.1 Overview

An idle/AFK system where the player sends their character to mine and collects rewards later.

### 8.2 Mines

| Mine | Level Req | Duration | Possible Drops |
|------|----------|----------|---------------|
| Copper Mine | 1 | 1 hour | Tier 1-2 gems, meteors, gold |
| Silver Mine | 20 | 2 hours | Tier 2-4 gems, meteors, rare materials |
| Gold Mine | 50 | 4 hours | Tier 3-6 gems, DragonBalls (rare), gold |
| Crystal Mine | 80 | 8 hours | Tier 5-8 gems, DragonBalls, elite materials |
| Dragon Mine | 110 | 12 hours | Tier 7-9 gems, DragonBalls, super materials |

### 8.3 Mechanics

- Player selects a mine and starts mining
- Mining runs on **timestamps** — works even when app is closed
- On return, player collects all mined resources
- Local push notification when mining is complete
- Only one mining operation at a time

```csharp
// Core/Mining/MiningResolver.cs — uses timestamp delta, no Update() needed
public static MiningYield CalculateYield(MineData mine, long startTimestamp, long currentTimestamp, int seed)
```

---

## 9. Market System

### 9.1 Overview (Single Player — AI-Driven)

Since the game starts as offline/single-player, the market is populated by AI vendors with rotating stock.

### 9.2 AI Market

- Market refreshes every **4 hours** (real-time)
- 20-30 listings per refresh cycle
- Items include: equipment, gems, upgrade materials
- Prices based on item quality + random variance
- Player can browse, buy, and sell items from their inventory

### 9.3 Player Booth

- Player can set up their own stall with items for sale
- In single-player mode, AI buyers purchase items over time
- Revenue collected when returning to the market screen

### 9.4 Future Multiplayer

When multiplayer is added:
- Market becomes fully player-driven
- Real players browse each other's booths
- Economy emerges from supply/demand

---

## 10. Meta Progression (Between Runs)

### 10.1 Meta Currency

- **Chronicle Coins** earned from completing stages (first-time bonus + repeatable)
- Spent on permanent upgrades that persist across all runs

### 10.2 Permanent Upgrades

| Upgrade | Max Level | Bonus Per Level | Cost Per Level |
|---------|----------|----------------|---------------|
| Vitality | 50 | +2% max HP | 10 * level |
| Power | 50 | +2% ATK | 10 * level |
| Arcane | 50 | +2% MATK | 10 * level |
| Fortitude | 50 | +2% DEF | 10 * level |
| Swiftness | 30 | +1% AttackSpeed | 15 * level |
| Fortune | 30 | +3% Gold earned | 15 * level |
| Wisdom | 30 | +3% XP earned | 15 * level |
| Luck | 20 | +2% Drop rate | 25 * level |

---

## 11. Art Pipeline

### 11.1 Tools
- **Aseprite** — All pixel art creation and animation
- **TexturePacker** — Optimized sprite atlas generation (for release builds)
- **Unity Aseprite Importer** (com.unity.2d.aseprite 3.0.1) — Direct .aseprite import during development

### 11.2 Sprite Specifications

| Asset Type | Size | Directions |
|-----------|------|-----------|
| Player character | 64x64 px per frame | 4-directional (flip for remaining 4) |
| Normal enemy | 32x32 to 48x48 px | 4-directional |
| Boss enemy | 96x96 to 128x128 px | 4-directional |
| Equipment icons | 32x32 px | N/A (static) |
| Gem icons | 16x16 px | N/A (static) |
| Isometric ground tile | 64x32 px | N/A |
| UI elements | Variable | N/A |

### 11.3 Animation States Per Character

| State | Frames | Loop |
|-------|--------|------|
| Idle | 4-6 | Yes |
| Attack | 4-6 | No |
| Cast | 4-6 | No |
| Hit | 2-3 | No |
| Die | 4-6 | No |

### 11.4 File Organization

**Source files** (outside Unity — not imported):
```
ArtSource/
├── Characters/     # .aseprite files, one per class
├── Enemies/        # .aseprite files, one per enemy type
├── Equipment/      # .aseprite icon sheets
├── Gems/           # .aseprite icon sheet
├── UI/             # .aseprite UI elements
├── Effects/        # .aseprite VFX frames
└── Tiles/          # .aseprite isometric tiles
```

**In Unity** (imported assets):
```
Assets/Visual/
├── Sprites/        # Exported PNGs or .aseprite files
├── Atlases/        # TexturePacker .tpsheet + .png output
├── Animations/     # AnimationClip + AnimatorController
├── Shaders/        # ShaderGraph assets
├── Materials/      # Material instances
└── PostProcess/    # URP Volume profiles
```

### 11.5 Sprite Sorting Layers (back to front)

1. **Background** — Parallax far background
2. **Tiles** — Isometric ground tiles
3. **Shadows** — Entity shadows
4. **Entities** — Enemies + Player (Y-sorted within this layer)
5. **Effects** — Particles, projectiles, hit effects
6. **UI** — HUD overlay

### 11.6 Isometric Y-Sorting

Entities on the "Entities" sorting layer use `SpriteRenderer.sortingOrder` set dynamically based on Y position. Lower Y = rendered in front (closer to camera).

---

## 12. Architecture

### 12.1 Assembly Definitions

```
ConquerChronicles.Core        (pure C#, noEngineReferences: true)
        │
        ▼
ConquerChronicles.Gameplay    (MonoBehaviours, VContainer, DOTween, URP)
        │
        ▼
ConquerChronicles.Editor      (Editor-only tools, custom inspectors)

ConquerChronicles.Tests.EditMode  ──> Core (pure logic tests)
ConquerChronicles.Tests.PlayMode  ──> Core + Gameplay (integration tests)
```

### 12.2 Key Architectural Rules

1. **Core has zero Unity dependencies** — All combat math, stats, inventory, save data are pure C#
2. **No singletons or FindObjectOfType** — All dependencies wired through VContainer
3. **All game data is ScriptableObject-driven** — No hardcoded balance numbers
4. **Object pooling for all frequently spawned objects** — Enemies, projectiles, damage numbers, effects
5. **Struct-based combat events** — No heap allocations in the combat hot loop
6. **Isometric Y-sorting** — Entities sorted by Y position every frame
7. **Offline-compatible idle systems** — Mining uses timestamp deltas, not Update() ticking

### 12.3 Scene Structure

| Scene | Purpose |
|-------|---------|
| Boot | VContainer root, save data loading, splash |
| MainMenu | Title, character select, navigation |
| Gameplay | Combat arena (loaded per stage) |
| Equipment | Gear management (additive load) |
| Mining | Idle mining screen (additive load) |

Scene flow: `Boot → MainMenu → [Equipment|Mining] (additive) or Gameplay → MainMenu`

### 12.4 Dependency Injection (VContainer)

Each scene has a `LifetimeScope` that registers:
- Core services (resolvers, calculators)
- Object pools (enemies, projectiles, damage numbers)
- MonoBehaviour entry points (managers)
- Scene component bindings

### 12.5 Save System

```csharp
// Core/Save/SaveData.cs — JSON serializable
public class SaveData
{
    public int Version;
    public CharacterClass SelectedClass;
    public int Level;
    public int XP;
    public int[] StatPoints;
    public SerializedEquipment[] Equipped;
    public SerializedEquipment[] Bag;
    public int Gold;
    public SerializedGem[] Gems;
    public string[] CompletedStages;
    public SerializedMetaUpgrade[] PermanentUpgrades;
    public int MetaCurrency;
    public long MiningStartTimestamp;
    public string ActiveMineID;
}
```

Saved via `PlayerPrefs` as JSON. Auto-saves on stage complete, equipment change, upgrade.

---

## 13. Isometric Coordinate System

### 13.1 Grid to World Conversion

```csharp
// Standard isometric projection
// Tile size: 64x32 pixels, 32 pixels per unit
worldX = (gridX - gridY) * (tileWidth / 2) / pixelsPerUnit
worldY = (gridX + gridY) * (tileHeight / 2) / pixelsPerUnit
```

### 13.2 Camera Setup

- Orthographic camera
- Size adjusted for mobile aspect ratios (portrait mode)
- Camera centered on the player (who is at grid center)

---

## 14. Mobile Performance Targets

| Metric | Target |
|--------|--------|
| Frame rate | 60 FPS stable |
| Max enemies on screen | 200-300 |
| Draw calls | < 50 per frame |
| Memory | < 300 MB |
| Atlas size | 2048x2048 max per atlas |
| GC allocations in combat | 0 per frame (struct-based) |

### 14.1 Performance Strategies

- **Object Pooling**: Pre-warm 64+ enemies, 128 projectiles, 64 damage numbers
- **Sprite Atlasing**: All sprites in category-based 2048x2048 atlases
- **SRP Batcher**: Same material for all sprites = automatic batching
- **MaterialPropertyBlock**: Per-enemy tinting without breaking batches
- **Enemy Cap**: Max 300 active; despawn farthest, spawn closer replacements
- **Off-screen Simplification**: Off-screen enemies use direction vector only (no pathfinding)
- **No mipmaps**: Pixel art should not use mipmaps
- **Struct combat events**: Zero allocations in the combat update loop

---

## 15. Required Packages

| Package | Version | Purpose | Install Method |
|---------|---------|---------|---------------|
| VContainer | 1.17.0 | Dependency Injection | OpenUPM (already in manifest) |
| DOTween | Latest | Animation/Tweening | Asset Store import in Unity Editor |
| 2D Aseprite | 3.0.1 | Aseprite file import | Already installed |
| 2D Tilemap Extras | 6.0.1 | Isometric tilemap | Already installed |
| Input System | 1.18.0 | Touch input | Already installed |
| URP | 17.3.0 | Render pipeline | Already installed |
| Test Framework | 1.6.0 | Unit testing | Already installed |
| TextMeshPro | via URP | UI text | Already installed |

**Manual Import Required**:
- **DOTween**: Download from Asset Store, import into project via Unity Editor

---

## 16. Phased Build Order

### Phase 1: Foundation
Player sprite on isometric map, one enemy type walks toward it.

### Phase 2: Combat Core
Auto-attacks, damage formula, damage numbers, enemy death, Trojan class.

### Phase 3: Stage System
Multi-wave stages, boss fights, stage completion + rewards.

### Phase 4: Mid-Run Progression
VS-style level up during combat, 3-choice upgrades, weapon evolution.

### Phase 5: All Classes
All 6 classes with unique skills, animations, and character select screen.

### Phase 6: Equipment System
Full gear with +12 upgrades, gem socketing, inventory management.

### Phase 7: Save System + Meta
Persistence, permanent upgrades, full game loop.

### Phase 8: Mining + Market
Offline mining, AI-driven market.

### Phase 9: Polish
Full art, audio, balance, performance, tutorial.

### Phase 10: Multiplayer (Future)
Player-driven market, leaderboards, co-op.

---

## 17. Folder Structure Reference

```
Assets/
├── _Game/
│   ├── Core/                    # Pure C# (no Unity)
│   │   ├── Character/           # CharacterClass, CharacterStats, StatCalculator, LevelUpTable
│   │   ├── Equipment/           # EquipmentData, UpgradeCalculator, GemSocketing
│   │   ├── Combat/              # DamageFormula, SkillData, SkillResolver, StatusEffect
│   │   ├── Stage/               # StageData, WaveData, StageState
│   │   ├── Enemy/               # EnemyData, DropTable
│   │   ├── Inventory/           # InventoryState, ItemData
│   │   ├── Mining/              # MiningState, MineData, MiningResolver
│   │   ├── Market/              # MarketState, MarketListing, MarketResolver
│   │   ├── MetaProgression/     # PermanentUpgrade, MetaProgressionState
│   │   ├── MidRunProgression/   # UpgradePool, WeaponEvolution
│   │   └── Save/                # SaveData, ISaveProvider
│   ├── Gameplay/                # MonoBehaviours
│   │   ├── Bootstrap/           # VContainer LifetimeScopes
│   │   ├── Character/           # CharacterView, CharacterAnimator
│   │   ├── Combat/              # CombatManager, Pools (Enemy, Projectile, DamageNumber)
│   │   ├── Enemy/               # EnemyView, EnemyMovement, EnemySpawner
│   │   ├── Stage/               # StageManager, WaveAnnouncerUI
│   │   ├── Equipment/           # EquipmentUI, UpgradeUI, GemSocketUI
│   │   ├── Inventory/           # InventoryUI, ItemTooltipUI
│   │   ├── Mining/              # MiningUI, MiningTimerView
│   │   ├── Market/              # MarketUI, BoothSetupUI
│   │   ├── MidRun/              # MidRunLevelUpUI, RunSummaryUI
│   │   ├── Camera/              # IsometricCamera, CameraShake
│   │   ├── Map/                 # IsometricGrid, TilemapManager, MapBoundsProvider
│   │   ├── UI/                  # HUD, MainMenu, MetaUpgrade, Common
│   │   ├── Audio/               # AudioManager, HapticFeedback
│   │   ├── Save/                # PlayerPrefsSaveProvider
│   │   └── Effects/             # VFXPool, LootDropEffect
│   ├── Editor/                  # Custom inspectors
│   ├── Tests/                   # EditMode + PlayMode tests
│   └── Data/                    # ScriptableObject assets
├── Visual/                      # Sprites, Atlases, Animations, Shaders, Materials
├── Scenes/                      # Boot, MainMenu, Gameplay, Equipment, Mining
└── Settings/                    # URP pipeline assets
```
