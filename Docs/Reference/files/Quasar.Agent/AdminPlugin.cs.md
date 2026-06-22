# Quasar.Agent/AdminPlugin.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 1

## Summary
`AdminPlugin` is the Magnetar `IPlugin` entry point for the Quasar agent that runs inside the Space Engineers dedicated server. On `Init` it logs Magnetar/Quasar.Agent versions, reads `AgentOptions`, applies profiler Harmony patches, registers Quasar's root admin chat commands (`!stop`, `!restart`, `!quit`), builds the `GameBridge`, starts `PluginLogOutbox`, wires command hooks to early `AdminStop` / `AdminRestart` signals, and starts `AgentConnection`. It drives snapshot/profiler refresh on each `Update`, refreshes death subscriptions, and reports fallback admin shutdown intent when the server terminates outside a Quasar-requested stop.

## Structure
**Namespace:** `Quasar.Agent`  **Base:** `IPlugin` (VRage.Plugins)  **Modifiers:** public, concrete

Fields: `_bridge` (`GameBridge`), `_connection` (`AgentConnection`), `_outbox` (`PluginLogOutbox`), `_adminStopSync`, `_adminStopReported`, `_adminRestartSync`, `_adminRestartRequested`, `_adminRestartReported`, `_deathSubscriptionsByIdentityId`, `_recentDeathsByIdentityId`.

| Member | Description |
|---|---|
| `Init(object gameServer)` | Logs startup versions, reads `AgentOptions.FromEnvironment()`, calls `AgentProfiler.Configure(options)` and `AgentProfilerPatches.Apply(options)`, registers `StopCommand`, `RestartCommand`, and `QuitCommand` through PluginSdk `ServerCommands`, builds `GameBridge`; creates and `Start()`s `PluginLogOutbox`; constructs `AgentConnection(bridge, WebServiceLocator, options, outbox)`, assigns command hooks to `ReportAdminStop` / `ReportAdminRestart`, and starts the connection; subscribes `MyVisualScriptLogicProvider.PlayerDied` as a fallback and `ServerControl.Terminating`. |
| `Update()` | Delegates to `GameBridge.Update()` each game tick, then periodically scans online human players to hook their current `IMyCharacter.CharacterDied` event. |
| `Dispose()` | Unsubscribes process/session events and character death handlers, clears all command hooks, stops the connection, disposes the outbox and bridge, unpatches profiler hooks, nulls references. |
| `OnServerTerminating(ServerTerminationKind kind)` | If `kind == Shutdown`, the bridge was not asked by Quasar to stop, and an admin restart was not already requested, calls `ReportAdminStop()` as a fallback for admin/console shutdowns outside the Quasar command paths. |
| `ReportAdminStop()` | Locks `_adminStopSync` and sends `AgentConnection.TrySendAdminStop()` until one attempt succeeds; suppressed once `!restart` has reported an admin restart. |
| `ReportAdminRestart()` | Marks an admin restart request and sends `AgentConnection.TrySendAdminRestart()` once so Quasar can enter `Restarting` before the process exits. |
| `LogStartupVersions()` | Resolves Magnetar and Quasar.Agent versions and writes them to the game/Magnetar log plus console output. |
| `RefreshDeathSubscriptions()` | Once per second, scans `MySession.Static.Players.GetOnlinePlayers()`, skips bots/NPC identities, and hooks each player's current character. |
| `HookCharacterDeath(IMyCharacter, long, string)` | Replaces stale per-identity character subscriptions when the character entity changes after respawn. |
| `OnCharacterDied(IMyCharacter, long, string)` / `OnPlayerDied(long)` | Resolve the victim name and record a `DeathEventSnapshot` (`DeathType = "Accident"`) via `GameBridge.RecordDeath`; duplicate character/visual-script notifications are suppressed per identity for a short window. |

## Dependencies
- `Quasar.Agent/GameBridge.cs`
- [`Quasar.Agent/StopCommand.cs`](StopCommand.cs.md)
- [`Quasar.Agent/AgentProfilerPatches.cs`](AgentProfilerPatches.cs.md)
- [`Quasar.Agent/AgentProfiler.cs`](AgentProfiler.cs.md)
- [`Quasar.Agent/AgentConnection.cs`](AgentConnection.cs.md)
- [`Quasar.Agent/PluginLogOutbox.cs`](PluginLogOutbox.cs.md)
- `Quasar.Agent/WebServiceLocator.cs`
- `Quasar.Agent/AgentOptions.cs`
- `Magnetar.Protocol/Model/DeathEventSnapshot.cs`
- `PluginSdk` — `IPlugin`, `ServerControl.Terminating`, `ServerTerminationKind`; `PluginSdk.Commands` — `ServerCommands`
- `Sandbox.Game` — `MyVisualScriptLogicProvider.PlayerDied`; `Sandbox.Game.World` — `MySession`; `VRage.Game.ModAPI` — `IMyCharacter.CharacterDied`
- `VRage.Plugins` — `IPlugin`

## Notes
The `PluginLogOutbox` is created and started before the `AgentConnection` so plugin log lines emitted during startup are buffered and shipped once connected. Death capture hooks the live character and re-hooks after respawn; `MyVisualScriptLogicProvider.PlayerDied` remains subscribed only as a fallback. `ReportAdminStop` is shared by `!stop`, `!quit`, and the termination fallback so Quasar sees shutdown intent early while preserving coverage for other admin-triggered stops. `ReportAdminRestart` gates the termination fallback so `!restart` is not misreported as a stop.
