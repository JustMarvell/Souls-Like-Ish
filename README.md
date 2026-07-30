# Souls-Like-Ish

A single-player soulslike action RPG built in Unity, with an open-world-capable design and future potential for drop-in multiplayer (Genshin-style world visiting). Combat blends **Sekiro** (parry timing, posture), **Zenless Zone Zero** (responsive combos, flashy feedback), and **Elden Ring** (stamina, weight, punishing enemies).

## Status

Vertical slice in progress. Character, Combat, and a basic Enemy are implemented and playable end-to-end (player can move, attack, dodge, block, parry, and counter; a NavMesh-driven enemy shares the same combat core and can chase, attack, get staggered, and die). World streaming, inventory, save systems, and multiplayer remain deferred.

## Tech Stack

| Component | Detail |
|---|---|
| Engine | Unity 6000.3, URP |
| Input | Unity New Input System |
| Character Model | PicoChan (Humanoid rig) |
| Animation Source | Free 32 RPG Animations + RPG Character Mecanim Animation Pack FREE, retargeted onto PicoChan via Humanoid Avatar |
| Current Hardware | Ryzen 5 6600H / Radeon 660M (iGPU) / 16GB RAM (+12GB swap) |

## Combat Identity

Fast, punishing, parry-centric combat with weight:
- **Attack** — data-driven via `AttackData` ScriptableObjects (damage, stamina cost, active-frame window); chains into a 3-hit combo via `NextCombo` references, with a buffered-input combo window per hit
- **Dodge** — timed i-frame-style evasion (stamina-gated, root-motion driven)
- **Parry** — tight binary timing window (no posture meter yet); a successful parry negates the hit, staggers the attacker (via `IStaggerable`), and opens a short counter window. A whiffed parry has a punishable recovery.
- **Defend/Block** — held guard; fully negates damage but drains stamina per hit taken
- **Counterattack** — press Attack again within the post-parry window to land a bonus-damage, stamina-free counter using a separate `AttackData`

Player and enemies share the same underlying combat core (`Hitbox`/`Hurtbox`/`IDamageable`/`AttackData`/`StaminaComponent`) so the parry system stays fair and readable in both directions — the enemy's attack is just another `AttackData` asset running through the exact same hit-detection code as the player's.

## Project Structure

```
Assets/
  _Project/
    Input/
      PlayerControls.inputactions        Player + UI action maps
    Scripts/
      Core/                 SoulsLikeIsh.Core            IState, StateMachine (shared by player & enemy)
      Input/                SoulsLikeIsh.Input            PlayerInputReader (ScriptableObject), InputBuffer, PlayerAction enum, generated wrapper
      Combat/                SoulsLikeIsh.Combat           Hitbox, Hurtbox, IDamageable, IStaggerable, DamageInfo, AttackData
      Character/
        Shared/              SoulsLikeIsh.Character.Shared  StaminaComponent, HealthComponent
        Player/               SoulsLikeIsh.Character.Player  PlayerController + States/ (Idle, Move, Attack, Dodge, Block, Parry)
        Enemy/                SoulsLikeIsh.Character.Enemy   EnemyController (NavMeshAgent, detection, IDamageable/IStaggerable)
      AI/                    SoulsLikeIsh.AI                Enemy behavior States/ (Idle, Chase, Attack, Stagger, Dead)
      Camera/               SoulsLikeIsh.Camera       CameraShaker, CameraZoom, LockOnController
    Animations/
      Player/                PlayerAnimator controller
    ScriptableObjects/       AttackData instances (player default/counter, enemy attacks)
    Prefabs/
```

Namespace root: `SoulsLikeIsh`.

## Core Systems (Vertical Slice Scope)

1. **State Machine** — ✅ shared `Core.StateMachine`/`IState` base, used identically by `PlayerController` and `EnemyController`
2. **Stamina + Health** — ✅ implemented (`StaminaComponent`, `HealthComponent`); **Posture** not yet implemented — parry is currently a binary timing window rather than a multi-hit stagger meter
3. **Input Buffering** — ✅ `InputBuffer` timestamps buffered actions (Attack/Dodge/Parry); `PlayerController.ProcessInputBuffer()` consumes them once the player returns to an actionable state, instead of dropping inputs during recovery windows
4. **Hitbox/Hurtbox + Parry Window** — ✅ frame-window-based hit detection (`Hitbox`/`Hurtbox`/`AttackData`), binary parry window with punishable whiff recovery
5. **Hit Feedback** — ✅ masked-layer upper-body flinch (`HitReact`) on normal hits, full-body `Stagger` state reserved for parry-punish CC
6. **Data-Driven Attacks** — ✅ `AttackData` ScriptableObjects for move timing/damage/stamina cost, no code changes needed to add new attacks
7. **Locomotion Blending** — ✅ 2D Freeform Cartesian blend tree (`MoveX`/`MoveY`) on `PlayerAnimator`; local-space velocity drives directional strafe clips, collapses to forward-only blending automatically when not locked on (no separate strafe mode/bool needed)

## Build Order (completed so far)

1. ✅ Input wrapper (New Input System actions + `PlayerInputReader`)
2. ✅ Player state machine skeleton (Idle/Move/Attack/Dodge)
3. ✅ Combat core (hitboxes + stamina)
4. ✅ Parry/Block/Counter layer (+ Health, `IStaggerable`)
5. ✅ Basic enemy sharing the same combat core (NavMesh chase/attack/stagger/death)
6. ✅ Attack combo chaining (`AttackData.NextCombo`, combo window buffering) + centralized input buffer (Attack/Dodge/Parry)
7. ✅ A lightweight soft target lock (stap to target on attack + line of sight during swing, rotate only the character model not the camera)
8. ✅ Hit reaction feedback — masked upper-body Animator layer (`HitReact`) driven from `PlayerController`/`EnemyController.TakeDamage`, separate from the full-body `Stagger` interrupt used for parry punishes
9. ✅ Implemented Lock-On camera system and target detection and player-facing/strafe movement based on the lock on target
10. ✅ 2D directional strafe locomotion (`MoveX`/`MoveY` local-space blend tree, 8-directional clips) replacing the single forward-run animation during lock-on strafing

## Known Gaps / Candidates for Next Steps

- Guard-break isn't handled when stamina can't cover a blocked hit (currently just blocks anyway — see `TODO` in `PlayerController.TakeDamage`)
- Enemy death has no loot/despawn/respawn hookup yet
- No posture/stagger meter — parry and stagger are binary/instant rather than accumulating
- Combo chain is currently linear (3 hits, no branching); no directional/charged variants yet
- Input buffering covers Attack/Dodge/Parry only — Jump/Interact/LockOn remain direct event-driven calls
- Open-world streaming, inventory, save/load, and multiplayer world-visiting remain out of scope for the current vertical slice