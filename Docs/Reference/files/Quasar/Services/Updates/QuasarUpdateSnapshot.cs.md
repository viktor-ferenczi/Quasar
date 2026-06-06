# Quasar/Services/Updates/QuasarUpdateSnapshot.cs

**Module:** Quasar.Services.Core  **Kind:** class (DTO)  **Tier:** 2

## Summary

DTO records and status enum used by the update service and Updates page. `QuasarUpdateSnapshot` describes the current checker state, current UI worker and Bootstrap versions, last check/change timestamps, and optional web/Bootstrap candidates; `QuasarUpdateCandidate` describes one release asset and whether it is staged or requires privileged installation.

## Structure

Namespace: `Quasar.Services.Updates`

**`QuasarUpdateSnapshot`** — sealed record.

| Property | Description |
|---|---|
| `Enabled` / `SupportedPlatform` | Whether update checks are configured and whether this platform can stage/activate updates. |
| `CurrentVersion` | Running Quasar UI worker version from `WebServiceOptions`. |
| `CurrentBootstrapVersion` | Bootstrap launcher version passed through `QUASAR_BOOTSTRAP_VERSION` when the worker is managed by Bootstrap. |
| `Status` / `Message` | Current `QuasarUpdateStatus` and display message. |
| `LastCheckedUtc` / `LastChangedUtc` | Timestamps for release checks and snapshot changes. |
| `Web` / `Bootstrap` | Optional update candidates. |

**`QuasarUpdateCandidate`** — sealed record with release version, asset name/download URL, expected SHA-256, size, publish time, staged directory, availability/staged flags, and privileged-install flag.

**`QuasarUpdateStatus`** — enum: `Idle`, `Checking`, `UpdateQueued`, `Staging`, `Staged`, `Activating`, `Failed`.

## Dependencies

- [`Quasar/Services/Updates/QuasarUpdateService.cs`](QuasarUpdateService.cs.md) — produces snapshots and candidates
- [`Quasar/Components/Pages/Updates.razor`](../../Components/Pages/Updates.razor.md) — displays snapshots and maps status to UI state
