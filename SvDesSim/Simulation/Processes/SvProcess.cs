using System.Numerics;
using SvAstParser.AstTree.SvEnums;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Processes;

public class SvProcess(IEnumerator<YieldInstruction> routine, EventScheduler scheduler)
    : ISimEvent
{
    public void Trigger() => scheduler.Schedule(EventRegion.Active, this);
    public void Start() => Trigger();

    public void Execute()
    {
        if (!routine.MoveNext()) return;

        var instruction = routine.Current;
        switch (instruction)
        {
            case SuspendDelay delay:
                scheduler.ScheduleFuture(delay.Ticks, EventRegion.Active, this);
                break;

            case SuspendEventControl eventControl:
                foreach (var sig in eventControl.Signals)
                    sig.Subscribe(this);
                break;

            case SuspendEdge edge:
                var trigger = new EdgeTriggerBridge(this, edge.Signal, edge.EdgeType, scheduler);
                edge.Signal.Subscribe(trigger);
                break;

            case SuspendEdgeList edgeList:
                var compositeTrigger = new CompositeEdgeTriggerBridge(this, edgeList.Edges, scheduler);
                foreach (var e in edgeList.Edges)
                {
                    e.sig.Subscribe(compositeTrigger);
                }

                break;
        }
    }

    private class EdgeTriggerBridge(SvProcess parent, ISimLogicSignal sig, SvEdgeKind edge, EventScheduler scheduler)
        : ISimEvent
    {
        private BigInteger _lastVal = sig.GetValueAsBigInt();

        public void Trigger()
        {
            var newVal = sig.GetValueAsBigInt();

            var fired = edge switch
            {
                SvEdgeKind.PosEdge => (_lastVal == 0 && newVal != 0),
                SvEdgeKind.NegEdge => (_lastVal != 0 && newVal == 0),
                _ => (newVal != _lastVal)
            };

            _lastVal = newVal;

            if (!fired) return;
            sig.Unsubscribe(this);
            scheduler.Schedule(EventRegion.Active, parent);
        }

        public void Execute()
        {
        }
    }

    private class CompositeEdgeTriggerBridge : ISimEvent
    {
        private readonly SvProcess _parent;
        private readonly List<(ISimLogicSignal sig, SvEdgeKind edge)> _edges;
        private readonly EventScheduler _scheduler;
        private readonly BigInteger[] _lastVals;
        private bool _fired;

        public CompositeEdgeTriggerBridge(SvProcess parent, List<(ISimLogicSignal sig, SvEdgeKind edge)> edges,
            EventScheduler scheduler)
        {
            _parent = parent;
            _edges = edges;
            _scheduler = scheduler;
            _lastVals = new BigInteger[edges.Count];
            for (var i = 0; i < edges.Count; i++)
            {
                _lastVals[i] = edges[i].sig.GetValueAsBigInt();
            }
        }

        public void Trigger()
        {
            if (_fired) return;

            var triggered = false;
            for (var i = 0; i < _edges.Count; i++)
            {
                var newVal = _edges[i].sig.GetValueAsBigInt();
                var edge = _edges[i].edge;
                var lastVal = _lastVals[i];

                var firedEdge = edge switch
                {
                    SvEdgeKind.PosEdge => (lastVal == 0 && newVal != 0),
                    SvEdgeKind.NegEdge => (lastVal != 0 && newVal == 0),
                    _ => (newVal != lastVal)
                };

                _lastVals[i] = newVal;
                if (firedEdge) triggered = true;
            }

            if (!triggered) return;

            _fired = true;
            foreach (var edge in _edges) edge.sig.Unsubscribe(this);
            _scheduler.Schedule(EventRegion.Active, _parent);
        }

        public void Execute()
        {
        }
    }
}