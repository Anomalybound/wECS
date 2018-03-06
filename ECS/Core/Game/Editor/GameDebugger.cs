using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using wECS.Helper;
using UnityEditor;

namespace wECS.Core
{
    public static class GameDebugger
    {
        private static readonly List<ITypeDrawer> Drawers = new List<ITypeDrawer>();
        private static readonly Dictionary<Type, ITypeDrawer> Caches = new Dictionary<Type, ITypeDrawer>();

        private static readonly List<Type> Components = new List<Type>();
        private static readonly Dictionary<Type, string[]> ComponentMembers = new Dictionary<Type, string[]>();

        static GameDebugger()
        {
            if (!EditorPrefs.GetBool("EnableDebugger"))
            {
                return;
            }

            var drawerType = typeof(ITypeDrawer);
            var componentType = typeof(IComponent);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//            var assemblies = new[] {Assembly.GetAssembly(typeof(GameDebugger))};
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var types = assembly.GetTypes();
                for (var j = 0; j < types.Length; j++)
                {
                    var type = types[j];
                    if (type.IsAbstract)
                    {
                        continue;
                    }

                    if (drawerType.IsAssignableFrom(type))
                    {
                        var drawer = Activator.CreateInstance(type) as ITypeDrawer;
                        Drawers.Add(drawer);
                    }

                    if (componentType.IsAssignableFrom(type))
                    {
                        Components.Add(type);
                        var members = type.GetTargetMemberInfos().Select(x => x.Name).ToArray();
                        ComponentMembers.Add(type, members);
                    }
                }
            }
        }

        public static string[] GetMembers(Type type)
        {
            if (ComponentMembers.ContainsKey(type))
            {
                return ComponentMembers[type];
            }

            return null;
        }

        public static ITypeDrawer GetDrawer(Type type)
        {
            if (Caches.ContainsKey(type))
            {
                return Caches[type];
            }

            for (var i = 0; i < Drawers.Count; i++)
            {
                var drawer = Drawers[i];
                if (drawer.HandlesType(type))
                {
                    Caches.Add(type, drawer);
                    return drawer;
                }
            }

            return null;
        }
    }
}