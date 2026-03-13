using SvSim.Simulation.Engine;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Processes;

public class SvProcess : ISimEvent
{
    private readonly IEnumerator<YieldInstruction> _routine;
    private readonly EventScheduler _scheduler;

    public SvProcess(IEnumerator<YieldInstruction> routine, EventScheduler scheduler)
    {
        _routine = routine;
        _scheduler = scheduler;
    }

    public void Trigger() => _scheduler.Schedule(EventRegion.Active, this);

    public void Start() => Trigger();

    public void Execute()
    {
        if (!_routine.MoveNext()) return;

        var instruction = _routine.Current;
        switch (instruction)
        {
            case SuspendDelay delay:
                _scheduler.ScheduleFuture(delay.Ticks, EventRegion.Active, this);
                break;
            case SuspendEventControl eventControl:
                foreach (var sig in eventControl.Signals)
                    sig.Subscribe(this);
                break;
        }
    }
}