using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Processes;

public class ContinuousAssignProcess<T> : ISimEvent where T : IEquatable<T>
{
    private readonly SimVar<T> _lhs;
    private readonly IExpression<T> _rhs;
    private readonly EventScheduler _scheduler;

    public ContinuousAssignProcess(SimVar<T> lhs, IExpression<T> rhs, IEnumerable<ISimEventSource> dependencies, EventScheduler scheduler)
    {
        _lhs = lhs;
        _rhs = rhs;
        _scheduler = scheduler;

        foreach (var dep in dependencies)
            dep.Subscribe(this);
            
        Trigger();
    }

    public void Trigger() => _scheduler.Schedule(EventRegion.Active, this);

    public void Execute() => _lhs.Assign(_rhs.Evaluate());
}