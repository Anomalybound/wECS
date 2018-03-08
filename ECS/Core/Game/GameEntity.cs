using System;
using System.Collections.Generic;
using wECS.Core;
using UnityEngine;

[Serializable]
public class GameEntity : IEntity
{
    public long EntityId { get; private set; }

    public Dictionary<Type, IComponent> Components { get; private set; }

    private string entityName;

    public GameEntity()
    {
        Components = new Dictionary<Type, IComponent>();
    }

    public void SetAgentId(long entityId)
    {
        EntityId = entityId;
        entityName = "Entity - " + entityId;
    }

    public override string ToString()
    {
        return entityName;
    }

    public T Add<T>(T data) where T : IComponent
    {
        var type = typeof(T);
        if (Components.ContainsKey(type))
        {
            Debug.LogErrorFormat("Adding duplicate component {0}", type);
            return Get<T>();
        }

        Components.Add(type, data);
        if (GameSystemRunner.ComponentsLookup.ContainsKey(type))
        {
            GameSystemRunner.ComponentsLookup[type].Add(this);
        }
        else
        {
            GameSystemRunner.ComponentsLookup.Add(type, new List<GameEntity> {this});
        }

        if (GameSystemRunner.OnAddComponent != null)
        {
            GameSystemRunner.OnAddComponent.Invoke(type, this);
        }

        if (GameSystemRunner.OnUpdateComponent != null)
        {
            GameSystemRunner.OnUpdateComponent.Invoke(type, this);
        }

        return data;
    }

    public T Add<T>() where T : IComponent, new()
    {
        var type = typeof(T);
        if (Components.ContainsKey(type))
        {
            Debug.LogErrorFormat("Adding duplicate component {0}", type);
            return Get<T>();
        }

        var component = new T();
        Components.Add(type, component);
        if (GameSystemRunner.ComponentsLookup.ContainsKey(type))
        {
            GameSystemRunner.ComponentsLookup[type].Add(this);
        }
        else
        {
            GameSystemRunner.ComponentsLookup.Add(type, new List<GameEntity> {this});
        }

        if (GameSystemRunner.OnAddComponent != null)
        {
            GameSystemRunner.OnAddComponent.Invoke(type, this);
        }

        if (GameSystemRunner.OnUpdateComponent != null)
        {
            GameSystemRunner.OnUpdateComponent.Invoke(type, this);
        }

        return component;
    }

    public void Remove<T>() where T : IComponent
    {
        var type = typeof(T);
        if (!Components.ContainsKey(type))
        {
            Debug.LogErrorFormat("Component {0} not exists", type);
        }

        Components.Remove(type);

        if (GameSystemRunner.ComponentsLookup.ContainsKey(type))
        {
            GameSystemRunner.ComponentsLookup[type].Add(this);
        }
        else
        {
            GameSystemRunner.ComponentsLookup.Add(type, new List<GameEntity> {this});
        }

        if (GameSystemRunner.OnRemoveComponent != null)
        {
            GameSystemRunner.OnRemoveComponent.Invoke(type, this);
        }

        if (GameSystemRunner.OnUpdateComponent != null)
        {
            GameSystemRunner.OnUpdateComponent.Invoke(type, this);
        }
    }

    public void Set<T>(T data) where T : IComponent, new()
    {
        if (Has<T>())
        {
            Remove<T>();
        }

        Add(data);
    }

    public void Update()
    {
        foreach (var x in Components)
        {
            if (GameSystemRunner.OnUpdateComponent != null)
            {
                GameSystemRunner.OnUpdateComponent.Invoke(x.Key, this);
            }
        }
    }

    public void Update<T>() where T : IComponent
    {
        var type = typeof(T);
        if (GameSystemRunner.OnUpdateComponent != null)
        {
            GameSystemRunner.OnUpdateComponent.Invoke(type, this);
        }
    }

    public bool Has<T>() where T : IComponent
    {
        return Components.ContainsKey(typeof(T));
    }

    public bool Has(Type type)
    {
        return Components.ContainsKey(type);
    }

    public bool Has(List<Type> types)
    {
        if (types == null)
        {
            return false;
        }

        for (var i = 0; i < types.Count; i++)
        {
            if (!Components.ContainsKey(types[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool Any(List<Type> types)
    {
        if (types == null)
        {
            return false;
        }

        for (var i = 0; i < types.Count; i++)
        {
            if (Components.ContainsKey(types[i]))
            {
                return true;
            }
        }

        return false;
    }

    public T Get<T>() where T : IComponent
    {
        if (!Components.ContainsKey(typeof(T)))
        {
            // TODO Better solution
            return (T) GameSystemRunner.NullComponent;
        }

        return (T) Components[typeof(T)];
    }
}