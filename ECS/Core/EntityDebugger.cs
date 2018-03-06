using wECS.Core;
using UnityEngine;

public class EntityDebugger<T> : MonoBehaviour where T : IEntity
{
    public T Agent;
}