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
        [FsmExpose] public bool HasAskedToExit { get; private set; }

        public void Boot()
        {
            this.enabled = false;
        }
        public void EnterFrom(FsmState previousState)
        {
            this.IsCurrent = true;
            this.IsEntering = true;
            this.enabled = true;
            OnEnter();
            OnEnterFrom(previousState);
            this.IsEntering = false;
        }
        public void ExitTo(FsmState nextState)
        {
            HasAskedToExit = false;
            this.IsExiting = true;
            OnExit();
            OnExitTo(nextState);
            this.enabled = false;
            this.IsExiting = false;
            this.IsCurrent = false;
        }
        protected void AskToExit()
        {
            HasAskedToExit = true;
        }

        protected virtual void OnEnter()
        {}

        protected virtual void OnExit()
        {}
        protected virtual void OnEnterFrom(FsmState previousState)
        {}

        protected virtual void OnExitTo(FsmState nextState)
        {}

    }
}
