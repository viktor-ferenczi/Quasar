# Quasar/Components/Pages/Chat.razor

**Module:** Quasar.Components  **Kind:** Blazor component  **Tier:** 2

## Summary
Routable page (`/chat`) that gives admins a full-width chat and command console for managed servers. It combines a server dropdown, live recent-chat feed from the selected agent snapshot, a chat/command input that sends text through `ServerCommandType.SendChat`, command-mode autocomplete sourced from registered PluginSdk commands, quick Refresh/Save/Restart actions, and recent command-result feedback. Server-authored chat (`IsServerMessage`, SteamId 0, `Good.bot`, or `Server`) is displayed as `Server`.

## Structure
- **Route:** `@page "/chat"`
- **Implements:** `IDisposable`
- **Injected services:** `AgentRegistry`, `DedicatedServerCatalog`, `DedicatedServerSupervisor`, `IJSRuntime`, `ISnackbar`
- **Key UI sections:**
  - Header with connected-agent count.
  - Server selector built from configured server definitions plus connected unmanaged agents.
  - Quick actions: Refresh and Save dispatch agent commands; Save is disabled while the selected server is `Starting`/`Stopping`/`Restarting`; Restart delegates to `DedicatedServerSupervisor.RestartServerAsync` and is also disabled during those unstable states.
  - Status chips for connected selected agents (players, world, agent connected).
  - Scrollable `.admin-chat-list` showing `AgentSnapshot.RecentChat` oldest-to-newest.
  - Chat/command input: normal chat uses a multiline `MudTextField`; command mode uses `MudAutocomplete<string>` over `AgentSnapshot.ChatCommands` but still sends the selected/full typed text as chat so plugin/game chat-command handlers receive the original prefix.
  - Recent command results from `AgentRuntimeState.CommandResults`.
- **Key state:** `_selectedUniqueName`, `_inputText`, `_commandMode`, `_lastRenderedLatestMessageTicks`, `_scrollPending`; computed `SelectedRuntime`, `IsSelectedUnstable`, `CanSaveSelected`, and `CanRestartSelected` gate unstable lifecycle actions.
- **Key methods:** `BuildServerOptions`, `EnsureSelectedServer`, `SearchCommandSuggestionsAsync`, `SendAgentCommandAsync`, `RestartSelectedServerAsync`, `HandleInputKeyDownAsync`, `ScrollToBottomAsync`, `FormatAuthor` / `IsServerMessage`, `FormatTimestamp`.
- **Event subscriptions:** `Registry.Changed` and `ServerCatalog.Changed` refresh the dropdown and selected-agent view.
- **Private type:** `ServerOption` record (UniqueName, DisplayName).

## Dependencies
- [`Quasar/Services/AgentRegistry.cs`](../../Services/AgentRegistry.cs.md) — connected agents, snapshots, command dispatch
- [`Quasar/Services/DedicatedServerCatalog.cs`](../../Services/DedicatedServerCatalog.cs.md) — configured server list and display names
- [`Quasar/Services/DedicatedServerSupervisor.cs`](../../Services/DedicatedServerSupervisor.cs.md) — restart action
- `Magnetar.Protocol/Model/ChatMessageSnapshot.cs`
- [`Magnetar.Protocol/Model/ChatCommandSnapshot.cs`](../../../Magnetar.Protocol/Model/ChatCommandSnapshot.cs.md)
- `Magnetar.Protocol/Transport/ServerCommandEnvelope.cs`
- `Magnetar.Protocol/Transport/ServerCommandType.cs`
- [`Quasar/Services/TextSanitizer.cs`](../../Services/TextSanitizer.cs.md)
- MudBlazor and JS interop (`quasarConfigs.scrollToBottom`)

## Notes
- A connected agent is required for chat, command text, Refresh, and Save because those actions are transported over the agent WebSocket.
- Restart is enabled only for selected configured servers because it runs through the supervisor, not the agent WebSocket.
- The page treats commands as chat-command text rather than inventing a separate arbitrary-console-command protocol.
