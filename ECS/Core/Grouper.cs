using System;
using System.Collections.Generic;
using AgentProcessor.Helper;

namespace AgentProcessor.Core
{
    public class Grouper
    {
        public readonly List<Type> Include;
        public readonly List<Type> Exclude;

        private static readonly List<Grouper> groups = new List<Grouper>();
        private static readonly TypeCompare comparer = new TypeCompare();

        public static Grouper Get(List<Type> include, List<Type> exclude = null)
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

            var newGroup = new Grouper(include, exclude);
            groups.Add(newGroup);

            return newGroup;
        }

        public Grouper(List<Type> include, List<Type> exclude)
        {
            Include = include;
            Exclude = exclude;
        }

        public override bool Equals(object obj)
        {
            var group = obj as Grouper;
            if (group != null)
            {
                if (group.Include.SortedListEquals(Include) &&
                    group.Exclude.SortedListEquals(Exclude))
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}