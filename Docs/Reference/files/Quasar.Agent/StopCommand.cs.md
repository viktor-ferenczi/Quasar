# Quasar.Agent/StopCommand.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 1

## Summary
Quasar-owned PluginSdk command modules for root in-game admin lifecycle commands. `StopCommand` handles `!stop` by reporting `AdminStop` and calling `ServerControl.SaveAndQuit()`. `RestartCommand` handles `!restart` by reporting `AdminRestart` and then using save-and-quit so Quasar tracks `Restarting` and performs the relaunch. `QuitCommand` handles `!quit` by reporting `AdminStop` and calling `ServerControl.QuitWithoutSaving()` for immediate no-save shutdown.

## Structure
**Namespace:** `Quasar.Agent`  **Base:** `CommandModule` (PluginSdk.Commands)  **Modifiers:** public, sealed command classes

| Member | Description |
|---|---|
| `StopCommand.AdminStopRequested` | Static hook assigned by `AdminPlugin`; lets `!stop` send Quasar's `AdminStop` signal before shutdown starts. |
| `StopCommand.Stop()` | Handles bare `!stop`, responds that save/shutdown has started, then queues admin-stop notification plus `ServerControl.SaveAndQuit()` via `Task.Run`. |
| `RestartCommand.AdminRestartRequested` | Static hook assigned by `AdminPlugin`; lets `!restart` send Quasar's `AdminRestart` signal while the socket is still alive. |
| `RestartCommand.Restart()` | Handles bare `!restart`, responds that save/restart has started, then queues admin-restart notification plus `ServerControl.SaveAndQuit()`. |
| `QuitCommand.AdminStopRequested` | Static hook assigned by `AdminPlugin`; lets `!quit` send Quasar's `AdminStop` signal before the process exits. |
| `QuitCommand.Quit()` | Handles bare `!quit`, responds that no-save quit has started, then queues admin-stop notification plus `ServerControl.QuitWithoutSaving()`. |
| `TryNotify*Requested()` | Best-effort wrappers around the static hooks so notification failures cannot block server shutdown. |

## Dependencies
- `PluginSdk` — `ServerControl.SaveAndQuit`, `ServerControl.QuitWithoutSaving`
- `PluginSdk.Commands` — `CommandRootAttribute`, `CommandAttribute`, `CommandModule`
- `System` — `Action`
- `System.Threading.Tasks` — `Task.Run`

## Notes
`[Permission]` is intentionally absent, so PluginSdk applies its fail-safe default of admin-only access. Each command uses `[Command("")]` so the bare root (`!stop`, `!restart`, or `!quit`) runs the action instead of showing the generated command overview. `!restart` deliberately avoids Magnetar self-restart and exits after saving so Quasar, not Magnetar, owns the restart state and relaunch.
