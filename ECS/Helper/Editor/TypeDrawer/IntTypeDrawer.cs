#if UNITY_EDITOR
using System;
using UnityEditor;

public class IntTypeDrawer : ITypeDrawer
{
    public bool HandlesType(Type type)
    {
        return type == typeof(int);
    }

    public object DrawAndGetNewValue(Type memberType, string memberName, object value, object target)
    {
        return EditorGUILayout.IntField(memberName, (int) value);
    }
}
#endif
