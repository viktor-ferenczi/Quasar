# Quasar/Services/Analytics/AnalyticsMetrics.cs

**Module:** Quasar.Services.Analytics  **Kind:** record + class  **Tier:** 2

## Summary

Central catalogue of analytics metrics exposed by the `/analytics` dashboard and `/api/analytics/series` endpoint. Each entry defines the metric key, panel title/subtitle, sample selector, availability check, axis formatting, and fixed/dynamic Y-axis behaviour so browser charts and API responses cannot drift.

## Structure

Namespace: `Quasar.Services.Analytics`

**`AnalyticsMetric`** (`sealed record`)

Carries metric display and extraction metadata:
- `Key`, `Title`, `Subtitle`
- `Selector : Func<MetricSample,double>`
- `IsAvailable : Func<MetricSample,bool>`
- `RequiresZero`, `Decimals`, `Kilo`, `FixedMax`, `DynamicMaxStep5`

**`AnalyticsMetrics`** (`public static class`)

Members:
- `All : IReadOnlyList<AnalyticsMetric>` — default panel order and full supported metric set.
- `Find(string? key) : AnalyticsMetric?` — case-insensitive lookup.

Default metrics:
- `simspeed`, `cpu`, `memory`, `players`, `frametime`, `pcu`
- `grids`, `entities`, `blocks`, `floating`

## Dependencies

- [`Quasar/Services/Analytics/MetricSample.cs`](MetricSample.cs.md)
- [`Quasar/Services/Analytics/AnalyticsSeriesService.cs`](AnalyticsSeriesService.cs.md)

## Notes

`blocks` and `floating` are optional sample fields; their availability checks hide missing historical data instead of plotting zeroes for analytics files written before those fields existed.
