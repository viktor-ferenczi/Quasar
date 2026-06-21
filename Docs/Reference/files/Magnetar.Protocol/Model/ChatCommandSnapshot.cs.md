# Magnetar.Protocol/Model/ChatCommandSnapshot.cs

**Module:** Magnetar.Protocol  **Kind:** class  **Tier:** 1

## Summary
DTO describing one registered PluginSdk chat command reported by `Quasar.Agent` in `AgentSnapshot.ChatCommands`. Carries the full command text (`!prefix path`), generated syntax including arguments, help/description text, owner id, root title, minimum promote level, and path segments so Quasar can offer command autocomplete without referencing `PluginSdk`.

## Structure
Namespace: `Magnetar.Protocol.Model`

Class `ChatCommandSnapshot` (concrete, no base type):

| Property | Type | Description |
|---|---|---|
| `Text` | `string` | Command text to insert/send, e.g. `!ess save`. |
| `Syntax` | `string` | Generated PluginSdk usage string, including required/optional arguments. |
| `Prefix` | `string` | Root command prefix without `!`. |
| `Path` | `string` | Space-separated path under the prefix. Empty for root/default commands. |
| `Description` / `HelpText` | `string` | Short overview text and longer help text from the SDK attributes. |
| `Title` | `string` | Human-readable command root title. |
| `OwnerId` | `string` | Assembly/plugin owner id assigned by the Magnetar command registry. |
| `MinimumPromoteLevel` | `string` | Required Space Engineers promote level as text. |
| `PathSegments` | `List<string>` | Original command path split into tokens. |

## Dependencies
- [`Magnetar.Protocol/Model/AgentSnapshot.cs`](AgentSnapshot.cs.md) — embeds a list of registered chat command snapshots.

## Notes
The DTO deliberately stores plain strings instead of PluginSdk or VRage types so `Magnetar.Protocol` remains a version-neutral `netstandard2.0` wire contract.
