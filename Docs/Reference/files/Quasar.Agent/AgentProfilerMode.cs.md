# Quasar.Agent/AgentProfilerMode.cs

**Module:** Quasar.Agent  **Kind:** enum  **Tier:** 1

## Summary

Public enum describing the in-DS profiler patch depth selected through `AgentOptions.ProfilerMode`.

## Structure

Namespace: `Quasar.Agent`

**`AgentProfilerMode`** (`public enum`)

Values:
- `Off` - disables profiler collection and Harmony profiler patches.
- `SafeContinuous` - enables continuous method-level profiler patches without deep IL call-site transpilers.
- `DeepContinuous` - default; enables continuous method patches plus deep call-site IL transpilers.

## Dependencies

None.

## Notes

`AgentOptions.FromEnvironment()` reads this mode from `QUASAR_AGENT_PROFILER_MODE`; Quasar passes the value from `WebServiceOptions.AgentProfilerMode` to each managed server process.
