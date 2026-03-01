# CLAUDE.md — Conquer Chronicles

This file provides guidance to Claude Code when working on this project.

---

## Project: Conquer Chronicles

A Unity-based mobile idle-survivor game combining Conquer Online MMO mechanics (classes, +12 equipment upgrades, gem socketing, idle mining, booth market) with Vampire Survivors-style auto-combat. The player is static in the center; enemies swarm toward them.

- **Engine**: Unity 6 LTS (6000.3.9f1), 2D URP
- **Platform**: Mobile (iOS first), offline/single-player first
- **Art Style**: Pixel art, isometric perspective
- **Art Tools**: Aseprite (sprites), TexturePacker (atlas optimization)

---

## Architecture

### Assembly Definitions

```
ConquerChronicles.Core        (pure C#, noEngineReferences: true, zero Unity deps)
        │
        ▼
ConquerChronicles.Gameplay    (MonoBehaviours, VContainer, DOTween, URP, InputSystem)
        │
        ▼
ConquerChronicles.Editor      (Editor-only tools)

ConquerChronicles.Tests.EditMode  ──> Core only
ConquerChronicles.Tests.PlayMode  ──> Core + Gameplay
```

### Key Rules

1. **Core/ has zero Unity dependencies** — all game logic is pure C#, testable without Unity
2. **No singletons or FindObjectOfType** — use VContainer for DI
3. **All balance data in ScriptableObjects** — no hardcoded numbers in code
4. **Object pool everything** that spawns frequently (enemies, projectiles, damage numbers, effects)
5. **Structs for combat events** — zero heap allocation in the combat loop
6. **Offline-compatible idle systems** — use timestamp deltas, not Update() ticking

### Packages

| Package | Purpose |
|---------|---------|
| VContainer (OpenUPM) | Dependency injection |
| DOTween | Animation/tweening (import from Asset Store) |
| 2D Aseprite (3.0.1) | Direct .aseprite file import |
| 2D Tilemap Extras | Isometric tilemap support |
| Input System (1.18.0) | Touch input |
| URP (17.3.0) | Render pipeline |

### Scenes

- **Boot** — VContainer root, save loading
- **MainMenu** — Title, character select, navigation
- **Gameplay** — Combat arena (per stage)
- **Equipment** — Gear management (additive load)
- **Mining** — Idle mining (additive load)

---

## Game Systems Reference

Full details in `GameDesignDocument.md`. Key systems:

- **6 Character Classes**: Trojan, Warrior, Archer, Water Taoist, Fire Taoist, Ninja
- **Combat**: Player is static at center, auto-attacks, skills fire on cooldown
- **Stages**: 5-10 waves + boss per stage, stage-based progression
- **Equipment**: 7 slots, quality tiers (Normal→Super), +1 to +12 upgrade with risk
- **Gem Socketing**: 8 gem types, tiers 1-9, combinable
- **Mid-Run Progression**: VS-style "pick 1 of 3" on level up, weapon evolution
- **Mining**: Idle timestamp-based, works offline
- **Market**: AI-driven in single-player, player-driven in future multiplayer
- **Meta Progression**: Permanent upgrades with Chronicle Coins

---

## Folder Structure

```
Assets/_Game/Core/       # Pure C# game logic
Assets/_Game/Gameplay/   # MonoBehaviours binding Core to Unity
Assets/_Game/Editor/     # Editor tools
Assets/_Game/Tests/      # EditMode + PlayMode tests
Assets/_Game/Data/       # ScriptableObject instances
Assets/Visual/           # Sprites, Atlases, Animations, Shaders
Assets/Scenes/           # Unity scenes
ArtSource/               # Aseprite source files (not imported by Unity)
```
