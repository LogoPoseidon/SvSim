using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public class ContinueStatement : IStatement
{
    public IEnumerable<YieldInstruction> Execute() { throw new ContinueException(); }
}
