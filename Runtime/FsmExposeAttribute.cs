using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SensenToolkit.StateMachine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FsmExposeAttribute : Attribute
    {
        public string Name;

        public FsmExposeAttribute(string name = null)
        {
            this.Name = name;
        }

        public static IEnumerable<(PropertyInfo, FsmExposeAttribute)> GetExposedProperties<TComponent>()
        where TComponent : Component
        {
            foreach (var property in typeof(TComponent).GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy))
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
