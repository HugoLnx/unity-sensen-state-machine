using Unity.VisualScripting;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmStateUnit<TM, TS> : Unit
    where TM : FsmMachine<TM>
    where TS : FsmState
    {
        private ControlInput _enter;
        private ControlInput _exit;
        private TM _machine;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), Enter);
            _exit = ControlInput(nameof(_exit), Exit);
        }

        private ControlOutput Enter(Flow flow)
        {
            FsmMachine.GetFromFlow(flow, ref _machine);
            _machine.OnEnterState<TS>();
            return null;
        }

        private ControlOutput Exit(Flow flow)
        {
            _machine.OnExitState<TS>();
            return null;
        }
    }
}
