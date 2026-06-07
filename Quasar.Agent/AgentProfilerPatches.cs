using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Sandbox;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Blocks;

namespace Quasar.Agent
{
    internal static class AgentProfilerPatches
    {
        private static Harmony _harmony;
        private static bool _applied;
        private static readonly ConcurrentDictionary<MethodBase, string> Categories = new ConcurrentDictionary<MethodBase, string>();

        public static void Apply()
        {
            if (_applied)
                return;

            _applied = true;
            try
            {
                _harmony = new Harmony("quasar.agent.profiler");
                PatchKnownMethods();
                PatchEntityUpdateMethods();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Quasar profiler patches failed: {exception.Message}");
            }
        }

        public static void Dispose()
        {
            try
            {
                _harmony?.UnpatchAll("quasar.agent.profiler");
            }
            catch
            {
            }
            finally
            {
                _harmony = null;
                _applied = false;
            }
        }

        private static void PatchKnownMethods()
        {
            Patch(AccessTools.Method(typeof(MySandboxGame), "RunSingleFrame"), "frame");
            Patch(AccessTools.Method(typeof(MySandboxGame), "UpdateInternal"), "update");
            Patch(AccessTools.Method(typeof(MyProgrammableBlock), "RunSandboxedProgramAction"), "script");

            PatchByTypeName("Sandbox.Engine.Physics.MyPhysics", "Simulate", "physics");
            PatchByTypeName("Sandbox.Engine.Physics.MyPhysics", "StepWorldsInternal", "physics");
            PatchByTypeName("Sandbox.Game.Replication.MyReplicationServer", "UpdateBefore", "replication");
            PatchByTypeName("Sandbox.Game.Replication.MyReplicationServer", "UpdateAfter", "replication");
            PatchByTypeName("Sandbox.Game.Replication.MyReplicationServer", "SendUpdate", "replication");
            PatchByTypeName("VRage.Network.MyTransportLayer", "Tick", "network");
            PatchByTypeName("VRage.Network.MyNetworkReader", "Process", "network");
            PatchByTypeName("Sandbox.Game.World.MySession", "UpdateComponents", "session");
        }

        private static void PatchEntityUpdateMethods()
        {
            var entityBase = typeof(MyCubeGrid).BaseType;
            if (entityBase == null)
                return;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var type in assemblies.SelectMany(GetTypesSafely))
            {
                if (type == null || type.IsAbstract || !entityBase.IsAssignableFrom(type))
                    continue;

                foreach (var method in FindDeclaredUpdateMethods(type))
                {
                    var category = typeof(MyCubeGrid).IsAssignableFrom(type) ? "grid" : "entity";
                    Patch(method, category);
                }
            }
        }

        private static IEnumerable<MethodInfo> FindDeclaredUpdateMethods(Type type)
        {
            var names = new[]
            {
                "UpdateBeforeSimulation",
                "UpdateBeforeSimulation10",
                "UpdateBeforeSimulation100",
                "UpdateAfterSimulation",
                "UpdateAfterSimulation10",
                "UpdateAfterSimulation100",
                "UpdateOnceBeforeFrame",
                "Simulate",
            };

            foreach (var name in names)
            {
                var method = AccessTools.Method(type, name, Type.EmptyTypes);
                if (method != null && method.DeclaringType == type)
                    yield return method;
            }
        }

        private static void PatchByTypeName(string typeName, string methodName, string category)
        {
            var type = AccessTools.TypeByName(typeName);
            if (type == null)
                return;

            Patch(AccessTools.Method(type, methodName), category);
        }

        private static void Patch(MethodBase method, string category)
        {
            if (method == null || _harmony == null)
                return;

            Categories[method] = category;
            var prefix = new HarmonyMethod(typeof(AgentProfilerPatches).GetMethod(nameof(Prefix), BindingFlags.Static | BindingFlags.NonPublic));
            var postfixMethod = method.IsStatic
                ? nameof(StaticPostfix)
                : nameof(InstancePostfix);
            var postfix = new HarmonyMethod(typeof(AgentProfilerPatches).GetMethod(postfixMethod, BindingFlags.Static | BindingFlags.NonPublic));
            _harmony.Patch(method, prefix, postfix);
        }

        private static void Prefix(out long __state)
        {
            __state = AgentProfiler.Begin();
        }

        private static void InstancePostfix(long __state, object __instance, MethodBase __originalMethod)
        {
            var category = Categories.TryGetValue(__originalMethod, out var value) ? value : "method";
            AgentProfiler.End(__state, category, __instance, __originalMethod);
        }

        private static void StaticPostfix(long __state, MethodBase __originalMethod)
        {
            var category = Categories.TryGetValue(__originalMethod, out var value) ? value : "method";
            AgentProfiler.End(__state, category, null, __originalMethod);
        }

        private static IEnumerable<Type> GetTypesSafely(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.Where(type => type != null);
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }
    }
}
