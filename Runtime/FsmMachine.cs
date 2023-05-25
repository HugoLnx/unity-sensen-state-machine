using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmMachine<T> : MonoBehaviour
    where T : FsmMachine<T>
    {
        public FsmState CurrentState { get; private set; }
        private Dictionary<Type, FsmState> _states;
        private Dictionary<Type, FsmState> States
            => _states ??= GetStates();

        protected void Awake()
        {
            foreach (FsmState state in States.Values)
            {
                state.Boot();
            }
        }

        public void OnEnterState<TB>()
        where TB : FsmState
        {
            FsmState state = States[typeof(TB)];
            state.Enter();
            this.CurrentState = state;
        }

        public void OnExitState<TB>()
        where TB : FsmState
        {
            States[typeof(TB)].Exit();
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
