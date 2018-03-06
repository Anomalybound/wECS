using wECS.Core;
using wECS.Helper;
using UnityEngine;

public abstract class GameSystemRunner : SystemRunner<GameEntity>
{
#if UNITY_EDITOR

    private Transform DebuggerRoot;

    protected override void Awake()
    {
        if (AgentDebugger)
        {
            DebuggerRoot = new GameObject("Game Entities").transform;
        }
        base.Awake();
    }

    protected override void OnDebugAgentAdded(GameEntity entity)
    {
        GameObject container;
        if (GameObjectPool.Count > 0)
        {
            container = GameObjectPool[0];
            container.SetActive(true);
            GameObjectPool.RemoveAt(0);
        }
        else
        {
            container = new GameObject();
        }

        container.name = entity.ToString();
        container.transform.SetParent(DebuggerRoot.transform, false);

        var debugger = container.AddOrGetComponent<GameEntityDebugger>();
        debugger.Agent = entity;

        DebuggerLinks.Add(entity, debugger);
    }

    protected override void OnDebugAgentRemoved(GameEntity entity)
    {
        if (!DebuggerLinks.ContainsKey(entity))
        {
            return;
        }

        var container = DebuggerLinks[entity];
        container.gameObject.SetActive(false);
        container.transform.SetParent(transform, false);

        GameObjectPool.Add(container.gameObject);
        DebuggerLinks.Remove(entity);
    }
#endif
}