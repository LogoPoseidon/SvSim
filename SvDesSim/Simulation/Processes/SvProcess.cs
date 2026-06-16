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
        }
    }

    private class EdgeTriggerBridge : ISimEvent
    {
        private readonly SvProcess _parent;
        private readonly ISimLogicSignal _sig;
        private readonly SvEdgeKind _edge;
        private readonly EventScheduler _scheduler;
        private BigInteger _lastVal;

        public EdgeTriggerBridge(SvProcess parent, ISimLogicSignal sig, SvEdgeKind edge, EventScheduler scheduler)
        {
            _parent = parent;
            _sig = sig;
            _edge = edge;
            _scheduler = scheduler;
            _lastVal = sig.GetValueAsBigInt();
        }

        public void Trigger()
        {
            var newVal = _sig.GetValueAsBigInt();

            var fired = _edge switch
            {
                SvEdgeKind.PosEdge => (_lastVal == 0 && newVal != 0),
                SvEdgeKind.NegEdge => (_lastVal != 0 && newVal == 0),
                _ => (newVal != _lastVal)
            };

            _lastVal = newVal;

            if (!fired) return;
            _sig.Unsubscribe(this);
            _scheduler.Schedule(EventRegion.Active, _parent);
        }
        public void Execute() { }
    }
}