using System;
using System.Collections.Generic;

namespace AgentProcessor.Helper
{
    public class TypeCompare : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return x.GetHashCode() - y.GetHashCode();
        }
    }
}