using System;
using System.Collections.Generic;
using System.Reflection;
using AgentProcessor.Core;
using AgentProcessor.Helper;
using UnityEditor;
using UnityEngine;

public class AgentDebuggerEditor<T, W> : Editor where T : EntityDebugger<W> where W : IEntity, new()
{
    private T AgentDebugger;

    private Dictionary<IComponent, IEnumerable<PropertyInfo>> props =
        new Dictionary<IComponent, IEnumerable<PropertyInfo>>();

    private void OnEnable()
    {
        AgentDebugger = target as T;
        SystemRunner<W>.OnUpdateComponent += OnComponentUpdate;
    }

    private void OnDisable()
    {
        SystemRunner<W>.OnUpdateComponent -= OnComponentUpdate;
    }

    private void OnComponentUpdate(Type arg1, W arg2)
    {
        if (Equals(AgentDebugger.Agent, arg2))
        {
            Repaint();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField(string.Format("Agent has {0} components.", AgentDebugger.Agent.Components.Count));
        EditorGUILayout.Space();

        AgentDebugger.Agent.Components.ForEach(x =>
        {
            var type = x.Key.UnderlyingSystemType;
            var allMembers = GameDebugger.GetMembers(type);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField(x.Key.ToString());
                EditorGUI.indentLevel++;
                serializedObject.Update();

                for (var i = 0; i < allMembers.Length; i++)
                {
                    var member = type.GetMember(allMembers[i])[0];
                    var memberType = member.GetUnderlyingType();
                    var drawer = GameDebugger.GetDrawer(memberType);
                    var rootValue = member.GetValue(x.Value);

                    if (drawer == null)
                    {
                        var childMembers =
                            memberType.GetTargetMemberInfos();

                        EditorGUI.indentLevel++;
                        for (var j = 0; j < childMembers.Length; j++)
                        {
                            var childMember = childMembers[j];
                            var childMemberType = childMember.GetUnderlyingType();
                            var childDrawer = GameDebugger.GetDrawer(childMemberType);
                            if (childDrawer != null)
                            {
                                var value = childMember.GetValue(rootValue);
                                if (childMember.CanWrite())
                                {
                                    using (new EditorGUI.DisabledScope())
                                    {
                                        childDrawer.DrawAndGetNewValue(childMemberType, childMembers[j].Name,
                                            value, target);
                                    }
                                }
                                else
                                {
                                    var newValue = childDrawer.DrawAndGetNewValue(childMemberType, childMembers[j].Name,
                                        value, target);
                                    childMember.SetValue(x.Value, newValue, null);
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField(string.Format("{0} not implemented.", childMemberType));
                            }
                        }

                        if (childMembers.Length <= 0)
                        {
                            EditorGUILayout.LabelField(string.Format("{0} not implemented.", memberType));
                        }

                        EditorGUI.indentLevel--;
                    }
                    else
                    {
                        if (member.CanWrite())
                        {
                            using (new EditorGUI.DisabledScope())
                            {
                                drawer.DrawAndGetNewValue(memberType, allMembers[i], rootValue, target);
                            }
                        }
                        else
                        {
                            var newValue = drawer.DrawAndGetNewValue(memberType, allMembers[i], rootValue, target);
                            member.SetValue(x.Value, newValue, null);
                        }
                    }
                }
            }

            EditorGUI.indentLevel--;
        });
    }
}