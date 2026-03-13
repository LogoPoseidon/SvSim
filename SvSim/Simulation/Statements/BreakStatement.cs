using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Statements;

public class BreakStatement : IStatement
{
    public IEnumerable<YieldInstruction> Execute() { throw new BreakException(); }
}