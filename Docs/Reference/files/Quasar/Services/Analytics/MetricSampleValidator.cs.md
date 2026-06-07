# Quasar/Services/Analytics/MetricSampleValidator.cs

**Module:** Quasar.Services.Analytics  **Kind:** class  **Tier:** 2

## Summary

Internal analytics data-quality gate used before samples enter memory or are restored from disk. It rejects structurally invalid samples and normalizes bounded values so corrupt agent payloads or hand-edited JSONL lines cannot poison the RRD buffers or chart output.

## Structure

Namespace: `Quasar.Services.Analytics`

**`MetricSampleValidator`** (`internal static class`)

Constants:
- `MaxSimSpeed = 5`
- `MaxMemoryMb = 1 TiB`
- `MaxFrameTimeMs = 60,000`
- `MaxReasonableCount = 100,000,000`

Methods:
- `TryNormalize(in MetricSample sample, out MetricSample normalized) : bool` — rejects non-positive timestamps and non-finite float fields; clamps simspeed, process CPU, memory, frame time, PCU/player counts, and optional world-size counts. Recomputes frame time from simspeed when it is missing but simspeed is available.
- Private helpers normalize required counts to non-negative values and optional counts to either `-1` (unavailable) or a bounded value.

## Dependencies

- [`Quasar/Services/Analytics/MetricSample.cs`](MetricSample.cs.md)
- [`Quasar/Services/Analytics/MetricsStoreService.cs`](MetricsStoreService.cs.md)

## Notes

Process CPU is capped to `100 * Environment.ProcessorCount`, matching TorchMonitor-style process CPU accounting where a busy multi-core process can exceed 100%.
