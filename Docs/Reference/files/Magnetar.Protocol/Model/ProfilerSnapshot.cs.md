# Magnetar.Protocol/Model/ProfilerSnapshot.cs

**Module:** Magnetar.Protocol  **Kind:** DTO classes  **Tier:** 1

## Summary

Wire DTOs for Quasar's low-duty profiler telemetry. `ProfilerSnapshot` is embedded in `AgentSnapshot` and carries one sampled calculation window: frame range, frame count, per-frame game-loop timing buckets, and bounded top lists for grids, programmable blocks, entities, system methods, physics, and network/replication/session work.

## Structure

Namespace: `Magnetar.Protocol.Model`

Classes:
- `ProfilerSnapshot` — capture metadata plus top-entry lists.
- `ProfilerTimingBreakdown` — per-frame timing buckets: frame, update, network, replication, session components, scripts, physics, parallel wait, other.
- `ProfilerEntrySnapshot` — one measured entry with stable key/name/category, optional entity id, grid/block/type/method labels, main/off-thread totals, per-frame totals, and call count.

## Dependencies

- [`Magnetar.Protocol/Model/AgentSnapshot.cs`](AgentSnapshot.cs.md)

## Notes

All values are plain BCL types so the contract stays loadable by both Quasar and the in-process Space Engineers plugin.
