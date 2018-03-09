#if UNITY_EDITOR
using UnityEditor;

namespace wECS.Core
{
    [CustomEditor(typeof(GameEntityDebugger))]
    public class GameAgentDebuggerEditor : AgentDebuggerEditor<GameEntityDebugger, GameEntity> { }
}
#endif
