# [Project Name TBD]

A single-player soulslike action RPG built in Unity, with an open-world-capable design and future potential for drop-in multiplayer (Genshin-style world visiting). Combat blends **Sekiro** (parry timing, posture), **Zenless Zone Zero** (responsive combos, flashy feedback), and **Elden Ring** (stamina, weight, punishing enemies).

* Don't know anymore what to write here. hope that summarize it.

## Status

Vertical slice in progress. Current focus: **Character, Combat, Enemy/Boss** systems only. World streaming, inventory, save systems, and multiplayer are deferred.

## Tech Stack

| Component | Detail |
|---|---|
| Engine | Unity 6000.3 HDRP |
| Input | Unity New Input System |
| Target Hardware | Ryzen 5 6600H / Radeon 660M (iGPU) / 16GB RAM |

Performance is a first-class constraint, not an afterthought — the Radeon 660M is an entry-level iGPU, so systems are designed with draw call count, shader complexity, LOD, and culling in mind from the start.

## Combat Identity

Fast, punishing, parry-centric combat with weight:
- **Attack** — data-driven combo strings
- **Dodge** — i-frame windowed evasion
- **Parry** — tight timing window, rewards aggression (Sekiro-inspired)
- **Defend/Block** — stamina-gated guard
- **Counterattack** — triggered off successful parries / posture breaks

Player and enemies share the same underlying combat core so the parry system stays fair and readable in both directions.

## Project Structure

```
Assets/
  _Project/
    Scripts/
      Core/              Game.Core            State machines, event bus, base utilities
      Input/             Game.Input           Input System wrapper/actions
      Character/
        Player/          Game.Character.Player
        Enemy/           Game.Character.Enemy
        Shared/          Game.Character.Shared  Stats (HP/Stamina/Posture), hit detection
      Combat/             Game.Combat          Attack data, hitboxes, parry/block/counter logic
      AI/                 Game.AI              Enemy/boss behavior
      Camera/             Game.Camera          Lock-on, follow camera
    Animations/
    Prefabs/
    ScriptableObjects/    Attack data, enemy stats, weapon data (designer-friendly, code-decoupled)
```

Namespace root: `Game` (placeholder — update once project is named).

## Core Systems (Vertical Slice Scope)

1. **State Machine** — shared base for Player and Enemy (Idle, Attack, Dodge, Block, Parry, Stagger, Counter)
2. **Stamina + Posture** — stamina gates actions; posture break opens counterattack windows
3. **Input Buffering** — enables responsive combos and tight parry timing
4. **Hitbox/Hurtbox + Parry Window** — frame-window-based parry detection, not simple block toggling
5. **Data-Driven Attacks** — ScriptableObjects for moves/combos, extendable without code changes

## Build Order

1. Input wrapper (New Input System actions)
2. Player state machine skeleton (Idle/Move/Attack/Dodge)
3. Combat core (hitboxes + stamina)
4. Parry/Block/Counter layer
5. Basic enemy sharing the same combat core

## Deferred / Future Considerations

- Open-world streaming and level design
- Inventory, save/load systems
- Multiplayer world-visiting (Genshin-style) — architecture kept in mind but not implemented yet

## Development Conventions

- Comments kept minimal — only where logic isn't self-explanatory
- Code organized by category into dedicated folders with matching namespaces
- Code changes delivered as targeted snippets with file/line references, not full-file rewrites, unless a full rewrite is requested