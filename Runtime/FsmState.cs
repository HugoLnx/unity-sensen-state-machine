using System;
using Unity.VisualScripting;
using UnityEngine;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmState : MonoBehaviour
    {
        public bool IsEntering { get; private set; }
        public bool IsExiting { get; private set; }
        public bool IsCurrent { get; private set; }

        public void Boot()
        {
            this.enabled = false;
        }
        public void Enter()
        {
            this.IsCurrent = true;
            this.IsEntering = true;
            this.enabled = true;
            OnEnter();
            this.IsEntering = false;
        }
        public void Exit()
        {
            this.IsExiting = true;
            OnExit();
            this.enabled = false;
            this.IsExiting = false;
            this.IsCurrent = false;
        }
        protected virtual void OnEnter()
        {}

        protected virtual void OnExit()
        {}

    }
}
