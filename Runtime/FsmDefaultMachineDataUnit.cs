using Unity.VisualScripting;

namespace SensenToolkit.StateMachine
{
    [UnitCategory("SensenFSM")]
    public class FsmDefaultMachineDataUnit : FsmMachineDataUnit<FsmMachine>
    {
        [FsmExpose]
        public bool CurrentStateHasAskedToExit => _machine.CurrentStateHasAskedToExit;
    }
}
