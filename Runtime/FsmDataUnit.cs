using Unity.VisualScripting;
using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using SensenToolkit.StateMachine;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmDataUnit<TMachine, TDataComponent> : Unit
    where TMachine : FsmMachine<TMachine>
    where TDataComponent : MonoBehaviour
    {
        private TMachine _machine;
        protected override void Definition()
        {
            Type dataType = typeof(TMachine);
            foreach (var (property, attr) in GetExposedProperties())
            {
                ValueOutput(property.PropertyType, attr.Name, (flow) => {
                    FsmMachine.GetFromFlow(flow, ref _machine);
                    MonoBehaviour dataComponent = _machine.GetDataComponent<TDataComponent>();
                    return property.GetValue(dataComponent);
                });
            }
        }

        public IEnumerable<(PropertyInfo, FsmExposeAttribute)> GetExposedProperties()
        {
            foreach (var property in typeof(TMachine).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
            {
                FsmExposeAttribute attr = property.GetCustomAttribute<FsmExposeAttribute>();
                if (attr != null)
                {
                    yield return (property, attr);
                }
            }
        }
    }
}
