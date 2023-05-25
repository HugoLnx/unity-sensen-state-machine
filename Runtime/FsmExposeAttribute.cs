using System;

namespace SensenToolkit.StateMachine
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FsmExposeAttribute : Attribute
    {
        public string Name;

        public FsmExposeAttribute(string name)
        {
            this.Name = name;
        }
    }
}
