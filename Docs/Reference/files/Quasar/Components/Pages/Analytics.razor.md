# Quasar/Components/Pages/Analytics.razor

**Module:** Quasar.Components  **Kind:** Blazor component  **Tier:** 2

## Summary
Interactive `/analytics` dashboard that renders rolling Space Engineers server metrics as client-side uPlot time-series charts. It reads scalar metrics from `MetricsStoreService`, fetches chart series over `/api/analytics/series`, reads recent calculation timing windows from `ProfilerStoreService`, supports per-server selection, a configurable CSS-grid panel layout, persisted view config (localStorage), auto-refresh, custom time ranges, theme-aware chart styling, and a per-panel settings dialog.

## Structure
- `@page "/analytics"`, `@implements IDisposable`.
- **`[Inject]`ed services:** `MetricsStoreService MetricsStore`, `ProfilerStoreService ProfilerStore`, `DedicatedServerCatalog ServerCatalog`, `AgentRegistry Registry`, `ISnackbar Snackbar`, `ILocalStorageService LocalStorage`, `IDialogService DialogService`, `ThemePreferenceService ThemePreference`, `IJSRuntime JS`. No `[Parameter]`s.
- **Toolbar:** time-range `MudSelect` (30s..30d + Custom), auto-refresh `MudSelect` (Off/5/15/30/60s), Refresh / Export / Reset-layout buttons, and an "Add panel" `MudMenu` listing hidden panels. Custom range shows date/time pickers.
- **Filters paper:** server checkbox grid with "Select all"; grid controls (`Columns`, `Rows`, `Row height`, `Max visible lines`, "Show all selected server lines") plus "Reset panels".
- **Summary chips row:** Servers, SimSpeed, CPU, Memory, Players, PCU, Grids, Entities, Blocks, Floating, Range Avg Sim.
- **Chart grid:** CSS-grid (`analytics-chart-grid`) of `MudPaper` cards, each containing a stable `div` target rendered by `quasar-charts.js` with uPlot; panel settings use a "Tune" `MudIconButton` opening `AnalyticsPanelDialog`.
- **Profiler section:** latest sampled windows per selected server with game-loop chips (frame/update/physics/scripts/network/other) plus top grids, scripts, entities, and system methods tables.
- **Metrics:** sourced from `AnalyticsMetrics.All`: simspeed, cpu, memory (GB), players, frametime, pcu, grids, entities, blocks, floating — each with selector/availability/axis metadata.
- **Refresh pipeline:** `RefreshView` resolves range/servers, reads in-memory samples only for summary chips, builds visible chart descriptors, and builds profiler cards. `SyncChartsAsync` sends a compact descriptor to JS; the browser fetches bulk series data directly. Source-change events are coalesced through `ProcessQueuedRefreshAsync`; auto-refresh runs a `PeriodicTimer` loop.
- **Persistence/config:** `AnalyticsViewConfig` (storage key `quasar.analytics.view.v2`) with `NormalizeConfig`, `CreateDefaultPanels`, panel visibility/order/spans.
- **Helper records/classes:** `ServerOption`, `SummaryChipModel`, `ChartCard`, `ProfilerSummaryCard`, `ChartSyncResult`.

## Dependencies
- `Quasar/Components/Pages/AnalyticsPanelDialog.razor` (panel settings dialog)
- `Quasar/Services/Analytics/MetricsStoreService.cs` (and `ServerMetricsStore` / `MetricSample` / `AnalyticsMetrics`)
- [`Quasar/Services/Analytics/ProfilerStoreService.cs`](../../Services/Analytics/ProfilerStoreService.cs.md)
- `Quasar/Services/DedicatedServerCatalog.cs`
- `Quasar/Services/AgentRegistry.cs`
- [`Quasar/Services/ThemePreferenceService.cs`](../../Services/ThemePreferenceService.cs.md)
- [`Quasar/Components/Pages/Analytics.razor.css`](Analytics.razor.css.md) (scoped styles)
- External: **MudBlazor**, **Blazored.LocalStorage** (`ILocalStorageService`), `Microsoft.JSInterop`, uPlot via `quasar-charts.js`.

## Notes
- Bulk chart points are fetched by the browser instead of pushed through the Blazor SignalR circuit; the server chooses the rollup tier by span (<=2h raw, <=24h 1-minute, else 1-hour).
- All JS interop and localStorage access guard against `InvalidOperationException`/`JSDisconnectedException` (prerender / circuit-disconnected) and silently degrade.
- `Dispose` cancels the auto-refresh `CancellationTokenSource` and detaches all source `Changed` / theme events.
- The server-name map (`BuildServerOptions`) labels each server by its configured `DedicatedServerDefinition.DisplayName` (falling back to the unique name only when blank), so the filter checkboxes and the chart series legends show the operator-chosen name rather than the unique name / id.
