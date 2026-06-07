using System;
using System.Reflection;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;
using VRage.ModAPI;

namespace Quasar.Agent
{
    internal sealed class ProfilerDescriptor
    {
        private ProfilerDescriptor()
        {
        }

        public string AccumulatorKey { get; private set; }

        public string Key { get; private set; }

        public string Name { get; private set; }

        public string Category { get; private set; }

        public long? EntityId { get; private set; }

        public string GridName { get; private set; }

        public string BlockName { get; private set; }

        public string TypeName { get; private set; }

        public string MethodName { get; private set; }

        public static ProfilerDescriptor From(string category, object instance, MethodBase method)
        {
            var methodName = FormatMethod(method);
            switch (category)
            {
                case "script":
                    return Script(instance as MyProgrammableBlock, methodName);
                case "grid":
                    return Grid(instance as MyCubeGrid, methodName);
                case "entity":
                    return Entity(instance, methodName);
                default:
                    return Named(category, methodName, methodName, methodName);
            }
        }

        private static ProfilerDescriptor Script(MyProgrammableBlock block, string methodName)
        {
            var entityId = block?.EntityId;
            var blockName = block?.DisplayNameText ?? "<script>";
            var gridName = block?.CubeGrid?.DisplayName ?? string.Empty;
            var key = entityId.HasValue ? entityId.Value.ToString() : $"{gridName}/{blockName}";
            return new ProfilerDescriptor
            {
                AccumulatorKey = $"script:{key}",
                Key = key,
                Name = string.IsNullOrWhiteSpace(gridName) ? blockName : $"{gridName}/{blockName}",
                Category = "script",
                EntityId = entityId,
                GridName = gridName,
                BlockName = blockName,
                TypeName = block?.GetType().FullName ?? string.Empty,
                MethodName = methodName,
            };
        }

        private static ProfilerDescriptor Grid(MyCubeGrid grid, string methodName)
        {
            var entityId = grid?.EntityId;
            var gridName = grid?.DisplayName ?? "<grid>";
            var key = entityId.HasValue ? entityId.Value.ToString() : gridName;
            return new ProfilerDescriptor
            {
                AccumulatorKey = $"grid:{key}",
                Key = key,
                Name = gridName,
                Category = "grid",
                EntityId = entityId,
                GridName = gridName,
                BlockName = string.Empty,
                TypeName = grid?.GetType().FullName ?? string.Empty,
                MethodName = methodName,
            };
        }

        private static ProfilerDescriptor Entity(object instance, string methodName)
        {
            var entity = instance as IMyEntity;
            var typeName = instance?.GetType().FullName ?? "<null>";
            var entityId = entity?.EntityId;
            var displayName = entity?.DisplayName;
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = typeName;

            var key = entityId.HasValue ? entityId.Value.ToString() : typeName;
            return new ProfilerDescriptor
            {
                AccumulatorKey = $"entity:{key}",
                Key = key,
                Name = displayName,
                Category = "entity",
                EntityId = entityId,
                GridName = string.Empty,
                BlockName = string.Empty,
                TypeName = typeName,
                MethodName = methodName,
            };
        }

        private static ProfilerDescriptor Named(string category, string key, string name, string methodName)
        {
            return new ProfilerDescriptor
            {
                AccumulatorKey = $"{category}:{key}",
                Key = key,
                Name = name,
                Category = category,
                EntityId = null,
                GridName = string.Empty,
                BlockName = string.Empty,
                TypeName = string.Empty,
                MethodName = methodName,
            };
        }

        private static string FormatMethod(MethodBase method)
        {
            if (method == null)
                return "<unknown>";

            return $"{method.DeclaringType?.FullName ?? "<unknown>"}#{method.Name}";
        }
    }
}
