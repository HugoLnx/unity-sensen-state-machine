using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SensenToolkit;
using static SensenToolkit.Componentsx;
using UnityEngine;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmMachine : MonoBehaviour
    {
        public FsmState CurrentState { get; private set; }
        private Dictionary<Type, FsmState> _states;
        private Dictionary<Type, FsmState> States
            => _states ??= GetStates();
        private readonly Dictionary<Type, MonoBehaviour> _dataComponents = new();

        protected void Awake()
        {
            foreach (FsmState state in States.Values)
            {
                state.Boot();
            }
        }

        public void OnEnterState<TState>()
        where TState : FsmState
        {
            FsmState state = GetState<TState>();
            state.Enter();
            this.CurrentState = state;
        }

        public void OnExitState<TState>()
        where TState : FsmState
        {
            States[typeof(TState)].Exit();
        }

        internal FsmState GetState<TState>()
        where TState : FsmState
        {
            if (States.TryGetValue(typeof(TState), out FsmState state))
            {
                return state;
            }
            state = EnsureComponent<TState>(this, searchChildren: true);
            state.Boot();
            States.Add(typeof(TState), state);
            return state;
        }

        internal TData GetDataComponent<TData>()
        where TData : MonoBehaviour
        {
            Type type = typeof(TData);
            if (_dataComponents.TryGetValue(type, out MonoBehaviour component))
            {
                return (TData) component;
            }
            TData dataComponent = EnsureComponent<TData>(this, searchChildren: true);
            Assertx.IsNotNull(dataComponent, $"DataComponent {type.Name} wasn't found in StateMachine {GetType().Name}/{gameObject.name}");
            _dataComponents.Add(type, dataComponent);
            return dataComponent;
        }

        private Dictionary<Type, FsmState> GetStates()
        {
            return GetComponentsInChildren<FsmState>(includeInactive: true)
            .ToDictionary(s => s.GetType(), s => s);
        }
    }
}
