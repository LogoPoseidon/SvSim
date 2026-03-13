using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Statements;

public class DelayStatement(ulong delayTicks) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        yield return new SuspendDelay(delayTicks);
    }
}
