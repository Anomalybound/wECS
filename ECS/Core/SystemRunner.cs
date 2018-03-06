using System;
using System.Collections.Generic;
using wECS.Helper;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace wECS.Core
{
    public abstract class SystemRunner<T> : MonoBehaviour where T : IEntity, new()
    {
#if UNITY_EDITOR
        public bool AgentDebugger
        {
            get { return EditorPrefs.GetBool("EnableDebugger"); }
            set { EditorPrefs.SetBool("EnableDebugger", value); }
        }
#endif

        #region Entities

        private static long AgentId;

        public static readonly List<T> Agents = new List<T>();
        public static readonly List<T> AgentPool = new List<T>();

        public static Action<T> OnAgentAdded;
        public static Action<T> OnAgentRemoved;

        public static readonly Dictionary<Type, List<T>> ComponentsLookup = new Dictionary<Type, List<T>>();

        public static T CreateEntity()
        {
            T agent;
            if (AgentPool.Count > 0)
            {
                agent = AgentPool[0];
                AgentPool.RemoveAt(0);
            }
            else
            {
                agent = new T();
            }

            agent.SetAgentId(AgentId);
            AgentId++;

            Agents.Add(agent);
            if (OnAgentAdded != null)
            {
                OnAgentAdded.Invoke(agent);
            }

            return agent;
        }

        public static void ReleaseAgent(T agent)
        {
            if (Agents.Contains(agent))
            {
                Agents.Remove(agent);

                if (OnAgentRemoved != null)
                {
                    OnAgentRemoved.Invoke(agent);
                }

                AgentPool.Add(agent);

                agent.Components.Clear();
            }
        }

        #endregion

        #region Components

        public static readonly IComponent NullComponent = null;

        public static Action<Type, T> OnAddComponent;
        public static Action<Type, T> OnRemoveComponent;
        public static Action<Type, T> OnUpdateComponent;

        #endregion


        #region Processor

        public readonly Systems Systems = new Systems();

        protected abstract void SetupProcessors();

        #endregion

        protected virtual void Awake()
        {
            SetupProcessors();
#if UNITY_EDITOR
            if (AgentDebugger)
            {
                OnAgentAdded += OnDebugAgentAdded;
                OnAgentRemoved += OnDebugAgentRemoved;
            }
#endif
        }

        private void Update()
        {
            Systems.Execute();
        }

        private void FixedUpdate()
        {
            Systems.FixedExecute();
        }

#if UNITY_EDITOR

        protected readonly Dictionary<T, EntityDebugger<T>> DebuggerLinks = new Dictionary<T, EntityDebugger<T>>();

        protected readonly List<GameObject> GameObjectPool = new List<GameObject>();

        private void OnDestroy()
        {
            if (AgentDebugger)
            {
                OnAgentAdded -= OnDebugAgentAdded;
                OnAgentRemoved -= OnDebugAgentRemoved;
            }
        }

        protected abstract void OnDebugAgentRemoved(T agent);

        protected abstract void OnDebugAgentAdded(T agent);

#endif
    }
}