using System;
using System.Collections.Generic;

namespace wECS.Core
{
    public interface IEntity
    {
        long EntityId { get; }

        void SetAgentId(long entityId);

        Dictionary<Type, IComponent> Components { get; }

        T Add<T>(T data) where T : IComponent;
        T Add<T>() where T : IComponent, new();
        T Get<T>() where T : IComponent;
        void Remove<T>() where T : IComponent;
        void Set<T>(T data) where T : IComponent, new();
        void Update();
        void Update<T>() where T : IComponent;
        bool Has<T>() where T : IComponent;
        bool Has(Type type);
        bool Has(List<Type> types);
        bool Any(List<Type> types);
    }
}