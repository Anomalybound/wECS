using System;
using System.Collections.Generic;

namespace AgentProcessor.Core
{
    public class Matcher<T> : IMatcher where T : IEntity, new()
    {
        private static readonly Dictionary<Grouper, Matcher<T>> MatchCaches = new Dictionary<Grouper, Matcher<T>>();

        public static Matcher<T> GetMatcher(Grouper grouper)
        {
            Matcher<T> cacheMatcher;
            if (MatchCaches.ContainsKey(grouper))
            {
                cacheMatcher = MatchCaches[grouper];
            }
            else
            {
                cacheMatcher = new Matcher<T>(grouper);
                MatchCaches.Add(grouper, cacheMatcher);
            }

            return cacheMatcher;
        }

        private readonly Grouper grouper;

        public readonly List<T> All = new List<T>();

        protected Matcher(Grouper grouper)
        {
            this.grouper = grouper;

            SystemRunner<T>.OnAgentAdded += OnAgentCreated;
            SystemRunner<T>.OnAgentRemoved += OnAgentReleased;
            SystemRunner<T>.OnAddComponent += ProcessAddedComponent;
            SystemRunner<T>.OnRemoveComponent += ProcessRemovedComponent;
        }

        private void OnAgentCreated(T agent)
        {
            if (agent.Has(grouper.Include))
            {
                if (agent.Any(grouper.Exclude))
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

            if (agent.Any(grouper.Include))
            {
                All.Remove(agent);
            }
        }

        private void ProcessRemovedComponent(Type type, T agent)
        {
            if (grouper.Exclude != null)
            {
                if (grouper.Exclude.Contains(type))
                {
                    return;
                }
            }

            if (grouper.Include.Contains(type))
            {
                All.Remove(agent);
            }
        }

        private void ProcessAddedComponent(Type type, T agent)
        {
            if (grouper.Exclude != null)
            {
                if (grouper.Exclude.Contains(type))
                {
                    return;
                }
            }

            if (grouper.Include.Contains(type))
            {
                for (var i = 0; i < grouper.Include.Count; i++)
                {
                    if (!agent.Has(grouper.Include[i]))
                    {
                        return;
                    }
                }

                All.Add(agent);
            }
        }
    }
}