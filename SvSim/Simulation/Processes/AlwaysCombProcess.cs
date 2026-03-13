using SvSim.Simulation.Engine;
using SvSim.Simulation.Signal;
using SvSim.Simulation.Statements;

namespace SvSim.Simulation.Processes;

public class AlwaysCombProcess : ISimEvent
{
    private readonly IStatement _block;
    private readonly EventScheduler _scheduler;

    public AlwaysCombProcess(IStatement block, IEnumerable<ISimEventSource> sensitivityList, EventScheduler scheduler)
    {
        _block = block;
        _scheduler = scheduler;

        foreach (var sig in sensitivityList)
        {
            sig.Subscribe(this); 
        }
        Trigger();
    }

    public void Trigger() => _scheduler.Schedule(EventRegion.Active, this);

    public void Execute() 
    {
        using var e = _block.Execute().GetEnumerator();
        while (e.MoveNext()) { }
    }
}