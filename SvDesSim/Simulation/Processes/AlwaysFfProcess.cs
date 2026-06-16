using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Signal;
using SvDesSim.Simulation.Statements;

namespace SvDesSim.Simulation.Processes;

public class AlwaysFfProcess<T> : ISimEvent where T : IEquatable<T>
{
    private readonly IStatement _block;
    private readonly EventScheduler _scheduler;
    private readonly SimVar<T> _clockSignal;
    
    private T _lastClockValue;
    private readonly bool _isPosedge;

    public AlwaysFfProcess(IStatement block, SimVar<T> clockSignal, bool isPosedge, EventScheduler scheduler)
    {
        _block = block;
        _clockSignal = clockSignal;
        _isPosedge = isPosedge;
        _scheduler = scheduler;
        _lastClockValue = clockSignal.Value;

        _clockSignal.Subscribe(this);
    }

    public void Trigger()
    {
        var newValue = _clockSignal.Value;
        var wasLow = EqualityComparer<T>.Default.Equals(_lastClockValue, default);
        var isHigh = !EqualityComparer<T>.Default.Equals(newValue, default);
        var triggered = _isPosedge ? (wasLow && isHigh) : (!wasLow && !isHigh);

        _lastClockValue = newValue;

        if (triggered)
            _scheduler.Schedule(EventRegion.Active, this);
    }

    public void Execute()
    {
        using var enumerator = _block.Execute().GetEnumerator();
        while (enumerator.MoveNext()) { } 
    }
}