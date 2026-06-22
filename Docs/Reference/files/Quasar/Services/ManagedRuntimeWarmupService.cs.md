# Quasar/Services/ManagedRuntimeWarmupService.cs

**Module:** Quasar.Services.Core  **Kind:** class  **Tier:** 1

## Summary

`ManagedRuntimeWarmupService` is a `BackgroundService` that immediately checks and prepares the managed SteamCMD, Magnetar, and Space Engineers Dedicated Server installs at Quasar startup, so managed launches are blocked until prerequisites are ready. After startup it checks the managed Magnetar install for updates every hour, exposes manual Magnetar and Dedicated Server check methods for the Updates page, enriches snapshots with installed versions/paths, and fires `Changed` on every status update.

## Structure

Namespace: `Quasar.Services`

**`ManagedRuntimeWarmupService`** — sealed class extending `BackgroundService`.

| Member | Description |
|---|---|
| `event Action? Changed` | Raised on state transitions. |
| `GetSnapshot()` | Returns a copy of the current `ManagedRuntimeWarmupSnapshot`, enriched with installed runtime paths/versions from the resolver. |
| `bool IsReady` | True only after SteamCMD, Magnetar, and Dedicated Server readiness completes. Used by `DedicatedServerSupervisor` to gate managed server launches. |
| `BlockLaunchMessage` | User-facing reason shown when a launch is requested before managed runtime readiness. |
| `RetryAsync(ct)` | Reruns the same readiness workflow for dashboard retry after a failed Dedicated Server download. A semaphore prevents concurrent warmups. |
| `ExecuteAsync(ct)` | Transitions `Pending → Running`, calls `_runtimeResolver.EnsureManagedRuntimeReadyAsync` with a progress reporter, then transitions to `Complete` or `Failed`; after that, runs an hourly `PeriodicTimer` for managed Magnetar update checks. |
| `CheckMagnetarNowAsync(ct)` | Public UI hook that runs the managed Magnetar update check immediately. |
| `CheckDedicatedServerNowAsync(ct)` | Public UI hook that runs a managed Dedicated Server update/validate check immediately. |
| `RunMagnetarUpdateCheckAsync(ct)` | Uses the same semaphore as warmup/retry, calls `_runtimeResolver.EnsureManagedMagnetarCurrentAsync` with a progress reporter, and logs update-check failures without faulting the readiness snapshot. |
| `RunDedicatedServerUpdateCheckAsync(ct)` | Uses the same semaphore, calls `_runtimeResolver.EnsureManagedDedicatedServerCurrentAsync`, and records failures on the Dedicated Server component row. |
| `ApplyProgress(...)` | Maps resolver progress events into per-component snapshot rows, including version and last-check timestamps, and raises `Changed` for live UI refresh. |

**`ManagedRuntimeWarmupState`** — enum `{Pending, Running, Complete, Failed}`.

**`ManagedRuntimeWarmupSnapshot`** — sealed record `{State, Message, Components, UpdatedAtUtc}`. `CreateInitial()` seeds SteamCMD, Magnetar, and Dedicated Server rows; `Copy()` returns a detached component list; `WithComponent` / `WithComponents` update immutable row data.

**`ManagedRuntimeComponentState`** — enum `{Pending, Checking, Downloading, Installing, Ready, Failed}`.

**`ManagedRuntimeComponentSnapshot`** — sealed record carrying component id, display name, state, message, optional percent, path, installed version, and last check timestamp.

## Dependencies

- [`Quasar/Services/ManagedDedicatedServerRuntimeResolver.cs`](ManagedDedicatedServerRuntimeResolver.cs.md) — `EnsureManagedRuntimeReadyAsync`, `EnsureManagedMagnetarCurrentAsync`, `EnsureManagedDedicatedServerCurrentAsync`, `GetInstalledVersions`, progress DTOs
