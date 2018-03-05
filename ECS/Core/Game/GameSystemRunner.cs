using AgentProcessor.Core;
using AgentProcessor.Helper;
using UnityEngine;

public abstract class GameSystemRunner : SystemRunner<GameEntity>
{
#if UNITY_EDITOR

    private Transform DebuggerRoot;

    private void Start()
    {
        if (AgentDebugger)
        {
            DebuggerRoot = new GameObject("Game Agents").transform;
        }
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