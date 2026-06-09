# Quasar.Agent/AgentProfilerPatches.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 1

## Summary

Mode-aware Harmony patch registrar for Quasar's profiler telemetry. `DeepContinuous` patches known Space Engineers server methods and adds IL call-site transpilers for session components, replication simulation, entity update dispatch, parallel waits/callbacks, and physics stepping internals. `SafeContinuous` keeps continuous method-level timing without deep transpilers. `Off` skips profiler patches.

## Structure

Namespace: `Quasar.Agent`

**`AgentProfilerPatches`** (`internal static class`)

Key members:
- `Apply(AgentOptions)` — honors profiler mode, creates the Harmony instance, applies profiler patches, and logs the number of successfully patched targets.
- `Dispose()` — unpatches Quasar profiler patches by Harmony id.
- `PatchKnownMethods(bool deepMode)` — patches named game/server methods found by type/name and returns the success count; inherited methods are resolved to the actual declaring type before Harmony sees them.
- `PatchDeepCallSites()` — applies Magnetar-compatible Harmony transpilers and falls back to safe session/entity timing when full deep groups miss.
- `PatchEntityUpdateMethods()` — safe-mode fallback that patches update methods declared by entity-derived types.
- `PatchCallSites(...)` — delegates to `AgentProfilerTranspiler`.
- `Patch(...)` / `PatchDeclared(...)` / `PatchDeclaredByTypeName(...)` — isolate individual Harmony failures, log skipped targets, and keep patching the rest.
- `Prefix` / `InstancePostfix` / `StaticPostfix` — call `AgentProfiler.Begin/End`.

## Dependencies

- [`Quasar.Agent/AgentProfiler.cs`](AgentProfiler.cs.md)
- [`Quasar.Agent/AgentProfilerMode.cs`](AgentProfilerMode.cs.md)
- [`Quasar.Agent/AgentProfilerTranspiler.cs`](AgentProfilerTranspiler.cs.md)
- `Lib.Harmony` / `HarmonyLib`
- Space Engineers assemblies: `Sandbox`, `Sandbox.Game.Entities`, `Sandbox.Game.Entities.Blocks`

## Notes

Static and instance postfixes are separate so Harmony does not request `__instance` for static targets. Known-method lookup walks base types with `DeclaredOnly` binding flags so inherited API moves, such as `RunSingleFrame` living on `Sandbox.Engine.Platform.Game` instead of `MySandboxGame`, do not produce Harmony's "patch the declared method" failure. Missing target methods and individual patch failures are logged/skipped, allowing the agent to survive game-build differences while still profiling any targets that patched successfully.
