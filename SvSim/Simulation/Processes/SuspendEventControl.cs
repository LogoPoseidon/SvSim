using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Processes;

public class SuspendEventControl(params ISimEventSource[] signals) : YieldInstruction
{
    public ISimEventSource[] Signals { get; } = signals;
}