using Unity.VisualScripting;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmStateUnit<TMachine, TState> : Unit
    where TMachine : FsmMachine<TMachine>
    where TState : FsmState
    {
        private ControlInput _enter;
        private ControlInput _exit;
        private TMachine _machine;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), Enter);
            _exit = ControlInput(nameof(_exit), Exit);
        }

        private ControlOutput Enter(Flow flow)
        {
            FsmMachine.GetFromFlow(flow, ref _machine);
            _machine.OnEnterState<TState>();
            return null;
        }

        private ControlOutput Exit(Flow flow)
        {
            _machine.OnExitState<TState>();
            return null;
        }
    }
}
