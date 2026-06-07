# Quasar.Agent/ProfilerDescriptor.cs

**Module:** Quasar.Agent  **Kind:** class  **Tier:** 2

## Summary

Maps patched method context into stable profiler keys and display labels. It turns programmable block instances into per-script entries, cube grids into per-grid entries, other entities into per-entity entries when an entity id exists, and named system methods into category entries.

## Structure

Namespace: `Quasar.Agent`

**`ProfilerDescriptor`** (`internal sealed class`)

Fields/properties describe the accumulator key, public key/name/category, optional entity id, grid/block labels, type name, and method name.

Factory:
- `From(category, instance, method)` — dispatches to script/grid/entity/named descriptor builders based on patch category.

## Dependencies

- `Sandbox.Game.Entities.MyCubeGrid`
- `Sandbox.Game.Entities.Blocks.MyProgrammableBlock`
- `VRage.ModAPI.IMyEntity`

## Notes

Accumulator keys are category-prefixed so identical entity ids or method names in different profiler buckets cannot collide.
