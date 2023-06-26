using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace SensenToolkit.StateMachine
{
    public readonly struct VisualScriptingStateData
    {
        public int Depth { get; }
        public string Title { get; }
        public string ParentTitle { get; }
        public string RootTitle { get; }
        public Guid ParentGuid { get; }
        public IEnumerable<Guid> AncestorGuids { get; }

        public VisualScriptingStateData(int depth, string title, string parentTitle, string rootTitle, Guid parentGuid, IEnumerable<Guid> ancestorGuids)
        {
            Depth = depth;
            Title = title;
            ParentTitle = parentTitle;
            RootTitle = rootTitle;
            ParentGuid = parentGuid;
            AncestorGuids = ancestorGuids;
        }
    }
    public abstract class FsmStateUnit<TMachine, TState> : Unit
    where TMachine : FsmMachine
    where TState : FsmState
    {
        private ControlInput _enter;
        private TMachine _machine;

        protected override void Definition()
        {
            _enter = ControlInput(nameof(_enter), Enter);
        }

        private ControlOutput Enter(Flow flow)
        {
            FsmMachineFetcher.GetFromFlow(flow, ref _machine);
            IGraph parentGraph = flow.stack.parentElement.graph;
            IGraph rootGraph = flow.stack.rootGraph;
            System.Guid parentGuid = parentGraph == rootGraph ? System.Guid.Empty : flow.stack.parentElementGuids.SkipLast(1).Last();
            VisualScriptingStateData data = new(
                depth: flow.stack.depth - 2,
                title: flow.stack.graph.title,
                parentTitle: parentGraph.title,
                rootTitle: rootGraph.title,
                parentGuid: parentGuid,
                ancestorGuids: flow.stack.parentElementGuids.SkipLast(1)
            );
            _machine.TransitToState<TState>(data);
            return null;
        }
    }
}
