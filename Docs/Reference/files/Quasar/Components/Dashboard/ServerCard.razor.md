# Quasar/Components/Dashboard/ServerCard.razor

**Module:** Quasar.Components  **Kind:** Blazor component  **Tier:** 2

## Summary
Card component for a single managed server shown on the Dashboard card layout. Displays the server display name, status chip (OFF / STARTING / CONNECTING / OPEN / STOPPING / RESTARTING / CRASHED / FAULTED), host/world caption, last message or health summary, port/direct-connect chip, config-profile chip, management icon buttons (console, clone, template, edit, delete), lifecycle action buttons, and `ServerDetailPanel` body content. The Start button can be disabled by the dashboard while managed runtime prerequisites are still preparing.

## Structure
No `@page` route — used as a child component.

**Injection:** `ServerManagementActions ServerActions` for clone/edit/delete/template/console flows; `QuasarConfigProfileCatalog` for config-chip labels; `IJSRuntime`, `ISnackbar`, and `NavigationManager` for direct-connect copy feedback.

**Parameters:**
| Parameter | Type | Notes |
|---|---|---|
| `Server` | `DedicatedServerDefinition` | Required. Static server config. |
| `Runtime` | `DedicatedServerRuntimeSnapshot?` | Live process state snapshot. |
| `Agent` | `AgentRuntimeState?` | Live agent/game state. |
| `LaunchBlocked` | `bool` | Disables Start while the dashboard waits for managed runtime readiness. |
| `StartRequested` | `EventCallback<string>` | Fires with `UniqueName` when Start clicked. |
| `StopRequested` | `EventCallback<string>` | Fires with `UniqueName` when Stop clicked. |
| `KillStartingRequested` | `EventCallback<string>` | Fires with `UniqueName` when Kill clicked during `Starting`/`Restarting`. |
| `RestartRequested` | `EventCallback<string>` | Fires with `UniqueName` when Restart clicked. |
| `ConfigProfileSelected` | `EventCallback<string>` | Fires with the assigned config-profile id when the config chip is clicked. |

**Key MudBlazor components:** `MudCard`, `MudCardHeader`, `MudCardContent`, `MudStack`, `MudChip`, `MudButton`, `MudIconButton`, `MudTooltip`, `MudText`.

**Private helpers:**
- `ProcessState` — derives `DedicatedServerProcessState` from `Runtime?.State`.
- `IsProcessActive`, `CanStart`, `CanStop`, `CanKillStarting`, `CanRestart` — lifecycle button visibility logic. Start is shown only for `Stopped`, `Crashed`, and `Faulted`; Stop is shown only for stable `Running`; Kill is shown for `Starting`/`Restarting`; Restart is shown only for `Running`. No lifecycle button is shown during `Stopping`; Delete is disabled while the process is active.
- `CanCreateTemplate` — delegates to `ServerManagementActions.CanCreateWorldTemplate`.
- `CanOpenConfigProfile`, `OpenConfigProfileAsync`, `GetConfigProfileName()` — resolve and invoke the assigned config profile chip.
- `CopyDirectConnectAsync`, `GetDirectConnectAddress`, `ResolveDirectConnectHost` — copy `host:port` for Space Engineers direct connect; wildcard/any-address bindings fall back to the browser host and IPv6 hosts are bracketed.
- `OpenConsoleAsync`, `CloneAsync`, `CreateTemplateAsync`, `EditAsync`, `DeleteAsync` — delegate to `ServerManagementActions`.
- `GetDisplayName()` — prefers `Server.DisplayName`, falls back to `Agent.ServerDisplayName`, then `UniqueName`.
- `GetHostLabel()` — shows `Agent.HostDisplayName` or "Local host".
- `GetWorldLabel()` — shows `Agent.WorldDisplayName`, else last path segment of `Server.WorldPath`, else "World pending".
- `GetStatusLabel()` / `GetStatusColor()` — status chip text and `Color` enum. Shows `CONNECTING` (Info) when the process is `Running` but the agent link is not yet established — e.g. just after a Quasar restart that adopted a running process, or a transient agent drop — versus `STARTING` (Warning) for a genuine boot.

## Dependencies
- [`Quasar/Components/Dashboard/ServerDetailPanel.razor`](ServerDetailPanel.razor.md) — embedded in card body
- [`Quasar/Services/ServerManagementActions.cs`](../../Services/ServerManagementActions.cs.md) — clone/edit/delete/template/console dialog flows
- `Quasar/Services/QuasarConfigProfileCatalog.cs` — config profile lookup
- `Magnetar.Protocol.Model.DedicatedServerDefinition` — static server config type
- `Magnetar.Protocol.Model.DedicatedServerRuntimeSnapshot` — runtime state parameter
- `Magnetar.Protocol.Model.DedicatedServerProcessState` — process state enum
- `Magnetar.Protocol.Model.AgentRuntimeState` — agent connection state parameter
- MudBlazor
