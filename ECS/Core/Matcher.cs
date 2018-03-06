using System;
using System.Collections.Generic;
using wECS.Helper;

namespace wECS.Core
{
    public class Matcher<T> : IMatcher where T : IEntity, new()
    {
        public readonly List<Type> Include;
        public readonly List<Type> Exclude;

        private static readonly List<Matcher<T>> groups = new List<Matcher<T>>();
        private static readonly TypeCompare comparer = new TypeCompare();

        public static Matcher<T> Get(params Type[] types)
        {
            return Get(new List<Type>(types));
        }

        public static Matcher<T> Get(List<Type> include, List<Type> exclude = null)
        {
            if (include != null)
            {
                include.Sort(comparer);
            }

            if (exclude != null)
            {
                exclude.Sort(comparer);
            }

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                if (group.Include.SortedListEquals(include) &&
                    group.Exclude.SortedListEquals(exclude))
                {
                    return group;
                }
            }

            var newGroup = new Matcher<T>(include, exclude);
            groups.Add(newGroup);

            return newGroup;
        }

        private T first;

        public readonly List<T> All = new List<T>();

        public T First
        {
            get
            {
                if (first == null)
                {
                    if (All.Count > 0)
                    {
                        first = All[0];
                    }
                }

                return first;
            }
        }

        protected Matcher(List<Type> include, List<Type> exclude)
        {
            Include = include;
            Exclude = exclude;

            SystemRunner<T>.OnAgentAdded += OnAgentCreated;
            SystemRunner<T>.OnAgentRemoved += OnAgentReleased;
            SystemRunner<T>.OnAddComponent += ProcessAddedComponent;
            SystemRunner<T>.OnRemoveComponent += ProcessRemovedComponent;
        }

        public override bool Equals(object obj)
        {
            var matcher = obj as Matcher<T>;
            if (matcher != null)
            {
                if (matcher.Include.SortedListEquals(Include) &&
                    matcher.Exclude.SortedListEquals(Exclude))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        private void OnAgentCreated(T agent)
        {
            if (agent.Has(Include))
            {
                if (agent.Any(Exclude))
                {
                    return;
                }

                All.Add(agent);
            }
        }

        private void OnAgentReleased(T agent)
        {
            if (!All.Contains(agent))
            {
                return;
            }

            if (agent.Any(Include))
            {
                All.Remove(agent);
            }
        }

        private void ProcessRemovedComponent(Type type, T agent)
        {
            if (Exclude != null)
            {
                if (Exclude.Contains(type))
                {
                    return;
                }
            }

            if (Include.Contains(type))
            {
                All.Remove(agent);
            }
        }

        private void ProcessAddedComponent(Type type, T agent)
        {
            if (Exclude != null)
            {
                if (Exclude.Contains(type))
                {
                    return;
                }
            }

            if (Include.Contains(type))
            {
                for (var i = 0; i < Include.Count; i++)
                {
                    if (!agent.Has(Include[i]))
                    {
                        return;
                    }
                }

                All.Add(agent);
            }
        }
    }
}