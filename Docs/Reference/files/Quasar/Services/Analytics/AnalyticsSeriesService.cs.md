# Quasar/Services/Analytics/AnalyticsSeriesService.cs

**Module:** Quasar.Services.Analytics  **Kind:** class + records  **Tier:** 2

## Summary

Builds compact chart payloads for the analytics HTTP endpoint. It reads metric samples from `MetricsStoreService`, chooses the correct RRD tier for the requested time span, buckets each server onto one shared timeline, and returns aligned series arrays for browser-side rendering.

## Structure

Namespace: `Quasar.Services.Analytics`

**`AnalyticsSeriesService`** (`sealed class`)

Constants:
- `MaxPointsCeiling = 1000`, `MaxPointsFloor = 10`

Constructor:
- `AnalyticsSeriesService(MetricsStoreService store)`

Methods:
- `Build(long fromUnix, long toUnix, IReadOnlyList<string> servers, IReadOnlyList<string> metricKeys, int maxPoints) : AnalyticsSeriesResponse` — validates range/inputs, resolves metrics via `AnalyticsMetrics.Find`, reads samples, buckets values, drops empty buckets, and returns one chart DTO per metric with available data.

Private helpers:
- `ResolveMax(AnalyticsMetric, double)` — applies fixed or dynamic Y-axis max.
- `ReadSamplesForRange(ServerMetricsStore, long, long, long)` — raw samples for <=2h, 1-minute rollups for <=24h, 1-hour rollups beyond.
- `ServerBuckets` — per-server bucket sums/counts for every requested metric.

DTO records:
- `AnalyticsSeriesResponse`
- `AnalyticsChartDto`
- `AnalyticsAxisDto`
- `AnalyticsSeriesDto`

## Dependencies

- [`Quasar/Services/Analytics/MetricsStoreService.cs`](MetricsStoreService.cs.md)
- [`Quasar/Services/Analytics/ServerMetricsStore.cs`](ServerMetricsStore.cs.md)
- [`Quasar/Services/Analytics/MetricSample.cs`](MetricSample.cs.md)
- [`Quasar/Services/Analytics/AnalyticsMetrics.cs`](AnalyticsMetrics.cs.md)

## Notes

Each metric uses its own `IsAvailable` predicate, so optional telemetry fields such as block count and floating-object count do not create false zeroes when absent.
