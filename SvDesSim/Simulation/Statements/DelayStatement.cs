using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public class DelayStatement(ulong delayTicks) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        yield return new SuspendDelay(delayTicks);
    }
}
