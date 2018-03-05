using UnityEditor;

namespace AgentProcessor.Core
{
    [CustomEditor(typeof(GameEntityDebugger))]
    public class GameAgentDebuggerEditor : AgentDebuggerEditor<GameEntityDebugger, GameEntity> { }
}