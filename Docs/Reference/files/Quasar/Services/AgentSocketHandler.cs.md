# Quasar/Services/AgentSocketHandler.cs

**Module:** Quasar.Services.Core  **Kind:** class  **Tier:** 1

## Summary

`AgentSocketHandler` is the HTTP/WebSocket entry point for incoming Quasar.Agent connections. It accepts the `quasar.agent.v1` sub-protocol, drives the per-connection read loop, dispatches each `AgentWireMessage` to the appropriate service, handles agent-originated admin stop/restart lifecycle signals, and marks the connection disconnected in the registry on teardown.

## Structure

Namespace: `Quasar.Services`

**`AgentSocketHandler`** — sealed class. ctor injects `AgentRegistry`, `PluginConfigService`, `DedicatedServerSupervisor`, `PluginLogStream`, `IHostApplicationLifetime`, `ILogger`.

| Member | Description |
|---|---|
| `HandleAsync(HttpContext)` | Rejects non-WebSocket requests (400); upgrades to WebSocket, assigns a GUID connection id, runs the read loop, calls `_registry.MarkDisconnected` + closes the socket in `finally`. Loop token is a linked CTS of `RequestAborted` + `_lifetime.ApplicationStopping` so an in-flight `ReceiveAsync` cannot stall graceful shutdown ~30s. |
| `ProcessMessageAsync(message, connectionId, socket, ct)` | Dispatches on `WireMessageKind`: `Hello` → `UpsertHello` (wires a per-socket send callback); `Snapshot` → `UpdateSnapshot`; `CommandResult` → `UpdateCommandResult`; `PluginConfigSnapshot` → `_pluginConfigService.IngestSnapshot`; `PluginLogs` → `IngestPluginLogs`; `AdminStop` → `_supervisor.SetGoalStateAsync(Off)` using `_lifetime.ApplicationStopping`; `AdminRestart` → `_supervisor.BeginAdminRestartAsync(...)`; `Ping` → `Pong` reply; default → debug-log and ignore. |
| `IngestPluginLogs(PluginLogBatch, connectionId)` | Resolves unique name from the connection's Hello; parses each line with `PluginLogStream.TryParseSinkLine` and appends to the live buffer. |
| `ReceiveAsync(WebSocket, ct)` | Reads fragmented text frames (16 KB buffer) into a `MemoryStream`; deserialises as `AgentWireMessage`. |
| `SendAsync(WebSocket, AgentWireMessage, ct)` | Serialises and sends a single text frame. |

JSON options: `JsonSerializerDefaults.Web`, `WhenWritingNull`.

## Dependencies

- `Quasar/Services/AgentRegistry.cs` — target of `Hello`/`Snapshot`/`CommandResult`, unique-name resolution, disconnect marking
- [`Quasar/Services/DedicatedServerSupervisor.cs`](DedicatedServerSupervisor.cs.md) — `SetGoalStateAsync` on `AdminStop`, `BeginAdminRestartAsync` on `AdminRestart`
- `Quasar/Services/PluginSdk/PluginConfigService.cs` — `IngestSnapshot`
- [`Quasar/Services/PluginSdk/PluginLogStream.cs`](PluginSdk/PluginLogStream.cs.md) — `TryParseSinkLine` / `Append` for `PluginLogs`
- `Quasar/Models/` — `DedicatedServerGoalState`
- `Magnetar.Protocol.Model`, `Magnetar.Protocol.Transport` — `AgentWireMessage`, `WireMessageKind`, `PluginLogBatch`
- ASP.NET Core — `HttpContext`, `IHostApplicationLifetime`, `WebSocket`; `System.Text.Json`

## Notes

Critical cancellation design: read/reply paths use `context.RequestAborted`, but persistent lifecycle mutations (`AdminStop` and `AdminRestart`) use `_lifetime.ApplicationStopping`. The agent closes its socket soon after sending a signal (its process is exiting); using the request token would cancel the write mid-flight and let the exit be misread. Plugin log lines arrive over this agent channel (not the supervisor's stdout pump) precisely so they keep flowing after Quasar restarts and reconnects to a detached, still-running server daemon.
