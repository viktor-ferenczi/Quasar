# Quasar.Agent/AgentProfiler.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 1

## Summary

Continuous in-process profiler accumulator used by Harmony method patches and IL call-site transpilers. It records elapsed ticks into numeric call-site accumulators, splits main-thread vs off-thread time, and publishes the latest one-second `ProfilerSnapshot` window for inclusion in the next agent snapshot.

## Structure

Namespace: `Quasar.Agent`

**`AgentProfiler`** (`internal static class`)

Key members:
- `Enabled` / `Mode` — expose whether profiler collection is active and which `AgentProfilerMode` is configured.
- `Configure(AgentOptions)` — applies the configured profiler mode and resets the publish window.
- `MarkGameThread()` — records current game-thread id.
- `Update()` — publishes a completed rolling window from the game tick when the one-second interval elapses.
- `Begin()` / `End(...)` — lightweight patch hooks using `Stopwatch.GetTimestamp`.
- `RegisterCallSite(...)` — allocates numeric ids for patched methods or transpiled call sites.
- `BeginCallSite(...)` / `EndCallSite(...)` — token-based hooks emitted around deep IL call sites.
- `GetLatestSnapshot()` — returns latest completed profiler window.

Private helpers project raw call-site timing into game-loop breakdowns and top lists for grids, scripts, entity types, physics, network/replication/session, and other system methods.

## Dependencies

- [`Quasar.Agent/AgentOptions.cs`](AgentOptions.cs.md)
- [`Quasar.Agent/AgentProfilerMode.cs`](AgentProfilerMode.cs.md)
- [`Magnetar.Protocol/Model/ProfilerSnapshot.cs`](../Magnetar.Protocol/Model/ProfilerSnapshot.cs.md)
- `Sandbox.MySandboxGame` — simulation frame counter

## Notes

Hot-path work avoids string formatting: patches pass numeric call-site ids, accumulators use struct keys, and display labels are built only when a snapshot is published. Idle accumulators are pruned after empty windows so obsolete grids/scripts/types do not stay in memory forever.
