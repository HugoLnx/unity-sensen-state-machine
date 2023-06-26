using Unity.VisualScripting;
using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmDataUnit<TMachine, TDataComponent> : Unit
    where TMachine : FsmMachine
    where TDataComponent : MonoBehaviour
    {
        protected TMachine _machine;
        protected override void Definition()
        {
            Type dataType = typeof(TDataComponent);
            foreach (var (property, attr) in FsmExposeAttribute.GetExposedProperties<TDataComponent>())
            {
                ValueOutput(property.PropertyType, attr.Name ?? property.Name, (flow) => {
                    FsmMachineFetcher.GetFromFlow(flow, ref _machine);
                    MonoBehaviour dataComponent = _machine.GetDataComponent<TDataComponent>();
                    return property.GetValue(dataComponent);
                });
            }
        }
    }
}
