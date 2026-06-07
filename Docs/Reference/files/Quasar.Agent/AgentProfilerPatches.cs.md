# Quasar.Agent/AgentProfilerPatches.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 1

## Summary

Harmony patch registrar for Quasar's profiler telemetry. It patches known Space Engineers server methods for frame/update, programmable block scripts, physics, replication, network, and session components, then scans loaded entity types for declared simulation update methods so grids and other entities can be timed.

## Structure

Namespace: `Quasar.Agent`

**`AgentProfilerPatches`** (`internal static class`)

Key members:
- `Apply()` — creates the Harmony instance and applies all profiler patches.
- `Dispose()` — unpatches Quasar profiler patches by Harmony id.
- `PatchKnownMethods()` — patches named game/server methods found by type/name.
- `PatchEntityUpdateMethods()` — patches update methods declared by entity-derived types.
- `Prefix` / `InstancePostfix` / `StaticPostfix` — call `AgentProfiler.Begin/End`.

## Dependencies

- [`Quasar.Agent/AgentProfiler.cs`](AgentProfiler.cs.md)
- `Lib.Harmony` / `HarmonyLib`
- Space Engineers assemblies: `Sandbox`, `Sandbox.Game.Entities`, `Sandbox.Game.Entities.Blocks`

## Notes

Static and instance postfixes are separate so Harmony does not request `__instance` for static targets. Missing target methods are ignored, allowing the agent to survive minor game-build differences.
