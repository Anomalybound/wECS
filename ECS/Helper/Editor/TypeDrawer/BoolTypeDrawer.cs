#if UNITY_EDITOR
using System;
using UnityEditor;

public class BoolTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type == typeof(bool);
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.Toggle(memberName, (bool) value);
    }
}
#endif
