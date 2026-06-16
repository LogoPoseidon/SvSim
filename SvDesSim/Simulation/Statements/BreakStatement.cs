using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public class BreakStatement : IStatement
{
    public IEnumerable<YieldInstruction> Execute() { throw new BreakException(); }
}