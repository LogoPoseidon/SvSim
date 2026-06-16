using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Processes;

public class SuspendEventControl(params ISimEventSource[] signals) : YieldInstruction
{
    public ISimEventSource[] Signals { get; } = signals;
}