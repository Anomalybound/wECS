using System.Collections.Generic;
using AgentProcessor.Core;
using Assets.Dev.Scripts.AgentProcessor.Implementation;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameController))]
public class GameControllerInspector : Editor
{
    private GameSystemRunner systemRunner;

    private readonly List<ISystem> toggleList = new List<ISystem>();

    private void OnEnable()
    {
        systemRunner = target as GameSystemRunner;
    }

    public override void OnInspectorGUI()
    {
        var debuggerEnable = EditorGUILayout.ToggleLeft("Enable Visual Debugger", systemRunner.AgentDebugger);
        if (debuggerEnable != systemRunner.AgentDebugger)
        {
            systemRunner.AgentDebugger = debuggerEnable;
        }
        
        EditorGUILayout.Space();

        if (systemRunner.Processors.Count <= 0)
        {
            return;
        }

        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            foreach (var item in systemRunner.Processors)
            {
                var key = item.Key;
                var value = item.Value;

                var newValue = EditorGUILayout.ToggleLeft(key.ToString(), value);

                if (newValue != value)
                {
                    toggleList.Add(key);
                }
            }
        }

        for (var i = 0; i < toggleList.Count; i++)
        {
            var value = systemRunner.Processors[toggleList[i]];
            systemRunner.Processors[toggleList[i]] = !value;
        }

        toggleList.Clear();
    }
}