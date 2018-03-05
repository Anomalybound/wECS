using System;
using System.Collections.Generic;
using AgentProcessor.Helper;
using UnityEditor;
using UnityEngine;

namespace AgentProcessor.Core
{
    public abstract class SystemRunner<T> : MonoBehaviour where T : IEntity, new()
    {
        public bool AgentDebugger
        {
            get { return EditorPrefs.GetBool("EnableDebugger"); }
            set { EditorPrefs.SetBool("EnableDebugger", value); }
        }

        #region Agents

        private static long AgentId;

        public static readonly List<T> Agents = new List<T>();
        public static readonly List<T> AgentPool = new List<T>();

        public static Action<T> OnAgentAdded;
        public static Action<T> OnAgentRemoved;

        public static readonly Dictionary<Type, List<T>> ComponentsLookup = new Dictionary<Type, List<T>>();

        public static T CreateAgent()
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

        public readonly Processors Processors = new Processors();

        protected abstract void SetupProcessors();

        #endregion

        private void Awake()
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
            Processors.Execute();
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