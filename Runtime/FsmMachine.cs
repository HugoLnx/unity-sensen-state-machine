using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SensenToolkit;
using static SensenToolkit.Componentsx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmMachine<TMachine> : MonoBehaviour
    where TMachine : FsmMachine<TMachine>
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

        private FsmState GetState<TState>()
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

        internal MonoBehaviour GetDataComponent(Type type)
        {
            if (_dataComponents.TryGetValue(type, out MonoBehaviour dataComponent))
            {
                return dataComponent;
            }
            dataComponent = GetComponentInChildren(type, includeInactive: true) as MonoBehaviour;
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
    public static class FsmMachine
    {
        private const string VariableKey = "Fsm_stateMachine";
        public static T GetFromFlow<T>(Flow flow)
        where T : FsmMachine<T>
        {
            if (flow == null)
            {
                throw new ArgumentNullException(nameof(flow));
            }
            if (flow.variables.IsDefined(VariableKey))
            {
                return flow.variables.Get<T>(VariableKey);
            }
            GameObject gameObject = flow.stack.gameObject;
            T machine = gameObject.GetComponentInChildren<T>(includeInactive: true);
            Assert.IsNotNull(machine, $"No {typeof(T).Name} found in gameObject {gameObject.name}");
            flow.variables.Set(VariableKey, machine);
            return machine;
        }

        public static T GetFromFlow<T>(Flow flow, ref T machine)
        where T : FsmMachine<T>
        {
            if (machine != null) return machine;
            machine = GetFromFlow<T>(flow);
            return machine;
        }
    }
}
