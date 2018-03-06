using System;
using System.Collections.Generic;

namespace wECS.Core
{
    public class GameMatcher : Matcher<GameEntity>
    {
        protected GameMatcher(List<Type> include, List<Type> exclude) : base(include, exclude) { }
    }
}