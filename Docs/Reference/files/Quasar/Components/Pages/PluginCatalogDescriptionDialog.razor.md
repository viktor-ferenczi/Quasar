# Quasar/Components/Pages/PluginCatalogDescriptionDialog.razor

**Module:** Quasar.Components  **Kind:** Blazor component  **Tier:** 2

## Summary
Small MudBlazor dialog opened from `Configs.razor` when the user expands a plugin catalog description. Shows the plugin display name, plugin ID, and full description text with preserved line breaks, then closes via a single primary action.

## Structure
- **Dialog:** `MudDialog` with `TitleContent`, `DialogContent`, and `DialogActions`.
- **Parameters:**
  - `DisplayName` (string) — friendly plugin name shown as the dialog title.
  - `PluginId` (string) — catalog plugin ID shown as a caption when present.
  - `Description` (string) — full catalog description rendered with `white-space: pre-wrap`.
- **Cascading parameters:** `IMudDialogInstance` for closing the dialog.
- **Key methods:** `Close`.

## Dependencies
- [`Quasar/Components/Pages/Configs.razor`](Configs.razor.md) — opens this dialog from the plugin catalog table.
- MudBlazor (`MudDialog`, `MudStack`, `MudText`, `MudButton`, `IMudDialogInstance`)
