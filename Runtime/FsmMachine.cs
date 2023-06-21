using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SensenToolkit;
using static SensenToolkit.Componentsx;
using UnityEngine;

namespace SensenToolkit.StateMachine
{
    public abstract class FsmMachine : MonoBehaviour
    {
        private readonly struct SuperStateRecord
        {
            public FsmSuperState SuperState { get; }
            public VisualScriptingStateData Data { get; }
            public static readonly SuperStateRecord None = new(null, default);

            public SuperStateRecord(FsmSuperState superState, VisualScriptingStateData data)
            {
                SuperState = superState;
                Data = data;
            }
        }

        private Dictionary<Type, FsmState> _states;
        private bool _isInSuperStateEntry;
        private readonly Dictionary<Type, MonoBehaviour> _dataComponents = new();
        private readonly List<Guid> _superStateGuidStack = new();
        private readonly Dictionary<Guid, SuperStateRecord> _superStates = new();

        public FsmState CurrentState { get; private set; }
        public bool CurrentStateHasAskedToExit => CurrentState?.HasAskedToExit == true;
        private Dictionary<Type, FsmState> States
            => _states ??= GetStates();

        protected void Awake()
        {
            foreach (FsmState state in States.Values)
            {
                state.Boot();
            }
        }

        public void TransitToState<TState>(VisualScriptingStateData data)
        where TState : FsmState
        {
            // Debug.Log($"Transition {CurrentState?.GetType()?.Name} -> {typeof(TState).Name} Title:{data.Title} ParentTitle:{data.ParentTitle} ParentGuid:{data.ParentGuid} Ancestors:{string.Join(" | ", data.AncestorGuids)}");
            FsmState state = GetState<TState>();
            bool isEnteringSuperStateEntry = state is FsmSuperState;
            if (!_isInSuperStateEntry
                && CurrentState != null
                && CurrentState is not FsmSuperState)
            {
                CurrentState.ExitTo(state);
            }

            ResolveSuperStates(state, data);
            if (!isEnteringSuperStateEntry)
            {
                state.EnterFrom(CurrentState);
                this.CurrentState = state;
            }
            _isInSuperStateEntry = isEnteringSuperStateEntry;
        }

        private void ResolveSuperStates(FsmState nextState, VisualScriptingStateData data)
        {
            int currentDepth = _superStateGuidStack.Count;
            Guid currentSuperStateGuid = currentDepth > 0 ? _superStateGuidStack[^1] : Guid.Empty;
            if (data.ParentGuid == currentSuperStateGuid) return;
            Guid convergenceGuid = FindConvergenceGuid(data);
            // Debug.Log($"Convergence: {convergenceGuid}");
            ExitSuperStatesUntil(convergenceGuid, nextState);

            FsmSuperState superState = nextState as FsmSuperState;
            IEnumerable<Guid> guidsToPush = data.AncestorGuids;
            if (convergenceGuid != Guid.Empty)
            {
                guidsToPush = guidsToPush
                .SkipWhile(g => g != convergenceGuid)
                .Skip(1);
            }
            PushSuperStatesStack(guidsToPush);
            if (_superStateGuidStack.Count > 0)
            {
                _superStates[_superStateGuidStack[^1]] = new SuperStateRecord(superState, data);
                EnterTopSuperState();
                // Debug.Log($"Set super state {_superStateGuidStack[^1]} to {superState}");
            }
        }

        private void PushSuperStatesStack(IEnumerable<Guid> guids)
        {
            foreach (Guid guid in guids)
            {
                // Debug.Log($"Push SuperState {guid}");
                _superStateGuidStack.Add(guid);
                _superStates.Add(guid, SuperStateRecord.None);
            }
        }

        private Guid FindConvergenceGuid(VisualScriptingStateData data)
        {
            if (_superStates.ContainsKey(data.ParentGuid))
            {
                return data.ParentGuid;
            }
            foreach (Guid ancestorGuid in data.AncestorGuids.Reverse())
            {
                if (_superStates.ContainsKey(ancestorGuid))
                {
                    return ancestorGuid;
                }
            }
            return Guid.Empty;
        }

        private void ExitSuperStatesUntil(Guid targetGuid, FsmState nextState)
        {
            while (_superStateGuidStack.Count > 0 && _superStateGuidStack[^1] != targetGuid)
            {
                ExitTopSuperState(nextState);
            }
        }

        private void EnterTopSuperState()
        {
            Guid guid = _superStateGuidStack[^1];
            SuperStateRecord stateRecord = _superStates[guid];
            if (stateRecord.SuperState != null)
            {
                // Debug.Log($"Entering SuperState Guid:{guid} Title:{stateRecord.Data.Title} ParentTitle:{stateRecord.Data.ParentTitle}");
                stateRecord.SuperState?.EnterFrom(CurrentState);
            }
        }

        private void ExitTopSuperState(FsmState nextState)
        {
            Guid guid = _superStateGuidStack[^1];
            SuperStateRecord stateRecord = _superStates[guid];
            // Debug.Log($"Exiting SuperState Guid:{guid} Title:{stateRecord.Data.Title} ParentTitle:{stateRecord.Data.ParentTitle}");
            _superStateGuidStack.RemoveAt(_superStateGuidStack.Count - 1);
            stateRecord.SuperState?.ExitTo(nextState);
            _superStates.Remove(guid);
        }

        internal FsmState GetState<TState>()
        where TState : FsmState
        {
            if (States.TryGetValue(typeof(TState), out FsmState state))
            {
                return state;
            }
            state = EnsureComponent<TState>(this, searchChildren: true);
            state.Boot();
            States.Add(typeof(TState), state);
            return state;
        }

        internal TData GetDataComponent<TData>()
        where TData : MonoBehaviour
        {
            Type type = typeof(TData);
            if (_dataComponents.TryGetValue(type, out MonoBehaviour component))
            {
                return (TData) component;
            }
            TData dataComponent = EnsureComponent<TData>(this, searchChildren: true);
            Assertx.IsNotNull(dataComponent, $"DataComponent {type.Name} wasn't found in StateMachine {GetType().Name}/{gameObject.name}");
            _dataComponents.Add(type, dataComponent);
            return dataComponent;
        }

        private Dictionary<Type, FsmState> GetStates()
        {
            return GetComponentsInChildren<FsmState>(includeInactive: true)
            .ToDictionary(s => s.GetType(), s => s);
        }
    }
}
