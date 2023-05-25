using Unity.VisualScripting;
using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using SensenToolkit.StateMachine;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmParametersUnit<T> : Unit
    where T : FsmMachine<T>
    {
        private T _machine;
        protected override void Definition()
        {
            Type dataType = typeof(T);
            foreach (var (property, attr) in GetExposedProperties())
            {
                ValueOutput(property.PropertyType, attr.Name, (flow) => {
                    FsmMachine.GetFromFlow(flow, ref _machine);
                    return property.GetValue(_machine);
                });
            }
        }

        public IEnumerable<(PropertyInfo, FsmExposeAttribute)> GetExposedProperties()
        {
            foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
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
