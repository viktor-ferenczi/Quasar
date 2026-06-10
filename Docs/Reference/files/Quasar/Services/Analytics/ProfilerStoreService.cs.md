# Quasar/Services/Analytics/ProfilerStoreService.cs

**Module:** Quasar.Services.Analytics  **Kind:** class + records  **Tier:** 2

## Summary

Bounded in-memory store for recent profiler snapshots per server. `AgentRegistry` enqueues validated profiler windows as agent snapshots arrive; `AnalyticsSeriesService` reads those windows when profiler metric panels are requested through the existing `/api/analytics/series` endpoint, and the Analytics page can probe it for profiler-only chart visibility.

## Structure

Namespace: `Quasar.Services.Analytics`

**`ProfilerStoreService`** (`public sealed class`)

Members:
- `Enqueue(string uniqueName, ProfilerSnapshot snapshot)` — validates/normalizes then appends new completed windows to the per-server queue, dropping repeated copies of the same already-published window.
- `Build(long fromUnix, long toUnix, IReadOnlyList<string> servers) : ProfilerSeriesResponse` — returns matching snapshots grouped by server.
- `HasSamples(long fromUnix, long toUnix, IReadOnlyList<string> servers) : bool` — quickly reports whether any selected server has profiler samples in the requested window.
- `ClampTopCount(int count)` — internal helper shared by the validator.

Records:
- `ProfilerSeriesResponse`
- `ProfilerServerSeries`

## Dependencies

- [`Quasar/Services/Analytics/ProfilerSnapshotValidator.cs`](ProfilerSnapshotValidator.cs.md)
- [`Magnetar.Protocol/Model/ProfilerSnapshot.cs`](../../../Magnetar.Protocol/Model/ProfilerSnapshot.cs.md)

## Notes

Retention is deliberately small and in-memory (720 snapshots per server, roughly 12 minutes at the current one-snapshot-per-second profiler cadence). Full profiler top lists are not persisted into the scalar RRD path.
