# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A Unity 6 (6.0.2.13f1) demo/sandbox for **Context Steering** — a circular directional-map AI steering algorithm. Agents evaluate multiple interest and danger behaviors each frame and move toward the best available direction.

## Building & Running

This is a Unity project — there are no shell build commands. Open in Unity Editor and use the Editor Play button or `File > Build Settings` for standalone builds. The solution file `ContextSteering.sln` is for IDE integration (Rider or Visual Studio).

## Architecture

All game logic lives in `Assets/Game/Scripts/`. The system is split into four layers:

### 1. Context Map (`ContextSteering/ContextMap.cs`)
A circular buffer of N directional slots (default 16). Behaviors write influence values into slots with falloff. Supports blur (smooths adjacent slots) and temporal blending (lerps toward previous frame's values). This is the core data structure shared across the whole pipeline.

### 2. Brain (`ContextSteering/ContextSteeringBrain.cs`)
Aggregates all behaviors into two `ContextMap` instances — **interest** and **danger**. Each frame it clears both maps, runs every `IInterestBehavior` and `IDangerBehavior`, merges results with `MergeMax`, then applies blur + temporal blend.

### 3. Resolver (`ContextSteering/ContextSteeringResolver.cs`)
Takes the combined interest + danger maps and produces a final direction + speed:
- Masks out interest slots where danger exceeds a threshold
- Finds the highest-remaining interest slot
- Refines direction via sub-slot interpolation (weighted blend of neighboring slots)
- Scales speed by interest strength, reduced when facing danger

### 4. Agent (`ContextSteering/ContextSteeringAgent.cs`)
The root `MonoBehaviour`. Wires up brain, resolver, and `MoveComponent`. Each `Update`: Brain evaluates → Resolver picks direction → MoveComponent moves.

### Behaviors (pluggable via `IInterestBehavior` / `IDangerBehavior`)
| Behavior | Type | What it does |
|---|---|---|
| `ChaseBehavior` | Interest | Writes interest toward a single target Transform |
| `AttractionPointBehavior` | Interest | Writes interest toward all GameObjects tagged `"Attraction"` |
| `AvoidBehavior` | Danger | Writes danger away from all `ObstacleComponent` objects |

### Movement (`Components/MoveComponent.cs`)
Applies acceleration/deceleration smoothing, translates the agent, and slerps rotation toward the target direction.

### Configuration
`ContextSteeringConfig` is a `ScriptableObject` (`Assets/Game/Configs/`) with tunable parameters: `MapResolution`, `TemporalBlend`, `MoveSpeed`, `RotationSmoothing`, `MinimumSpeedThreshold`, `DangerThreshold`.

## Adding New Behaviors

1. Create a class implementing `IInterestBehavior` or `IDangerBehavior`
2. Implement `Evaluate(ContextMap map, Transform agentTransform)` — write values into `map` using `map.WriteValue(directionIndex, value)`
3. Add an instance to the `ContextSteeringBrain`'s behavior list (via Inspector or code)

## Key Data Flow

```
ContextSteeringAgent.Update()
  └─ ContextSteeringBrain.Evaluate()        // fills interest + danger maps
       ├─ IInterestBehavior[].Evaluate()
       └─ IDangerBehavior[].Evaluate()
  └─ ContextSteeringResolver.Resolve()      // returns direction + speed
  └─ MoveComponent.Move(direction, speed)   // applies movement
```
