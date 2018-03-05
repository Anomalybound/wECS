using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgentProcessor.Core;
using UnityEngine;

namespace AgentProcessor.Helper
{
    public static class Extensions
    {
        public static bool IsNull(this IComponent comp)
        {
            return comp == null;
        }

        public static T AddOrGetComponent<T>(this GameObject obj) where T : Component
        {
            var result = obj.GetComponent<T>();
            if (result == null)
            {
                result = obj.AddComponent<T>();
            }

            return result;
        }

        public static bool CanWrite(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.All:
                    break;
                case MemberTypes.Constructor:
                    break;
                case MemberTypes.Custom:
                    break;
                case MemberTypes.Event:
                    break;
                case MemberTypes.Field:
                    break;
                case MemberTypes.Method:
                    break;
                case MemberTypes.NestedType:
                    break;
                case MemberTypes.Property:
                    break;
                case MemberTypes.TypeInfo:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        public static object GetValue(this MemberInfo memberInfo, object obj)
        {
            var fieldInfo = memberInfo as FieldInfo;
            var propertyInfo = memberInfo as PropertyInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.GetValue(obj);
            }

            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj, null);
            }

            return null;
        }

        public static void SetValue(this MemberInfo memberInfo, object obj, object value, object[] indice)
        {
            var fieldInfo = memberInfo as FieldInfo;
            var propertyInfo = memberInfo as PropertyInfo;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(obj, value);
            }

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value, indice);
            }
        }

        public static Type GetUnderlyingType(this MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Event:
                    return ((EventInfo) member).EventHandlerType;
                case MemberTypes.Field:
                    return ((FieldInfo) member).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo) member).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo) member).PropertyType;
                default:
                    throw new ArgumentException
                    (
                        "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                    );
            }
        }

        public static MemberInfo[] GetTargetMemberInfos(this Type type)
        {
            return type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(x => (x.MemberType & (MemberTypes.Property | MemberTypes.Field)) != 0).ToArray();
        }

        public static bool SortedListEquals<T>(this List<T> left, List<T> right)
        {
            if (left != null && right == null || left == null && right != null)
            {
                return false;
            }

            if (left == null)
            {
                return true;
            }
            
            if (left.Count == right.Count)
            {
                for (var i = 0; i < left.Count; i++)
                {
                    if(!left[i].Equals(right[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }
    }
}