using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace SensenToolkit.StateMachine
{
    public static class FsmMachineFetcher
    {
        private const string VariableKey = "Fsm_stateMachine";
        public static T GetFromFlow<T>(Flow flow)
        where T : FsmMachine
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
            Assertx.IsNotNull(machine, $"No {typeof(T).Name} found in gameObject {gameObject.name}");
            flow.variables.Set(VariableKey, machine);
            return machine;
        }

        public static T GetFromFlow<T>(Flow flow, ref T machine)
        where T : FsmMachine
        {
            if (machine != null) return machine;
            machine = GetFromFlow<T>(flow);
            return machine;
        }
    }
}
