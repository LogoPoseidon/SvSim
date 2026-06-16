namespace SvDesSim.Simulation.Processes;

public class SuspendDelay(ulong ticks) : YieldInstruction
{
    public ulong Ticks { get; } = ticks;
}
