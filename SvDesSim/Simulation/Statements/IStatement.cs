using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public interface IStatement
{
    IEnumerable<YieldInstruction> Execute();
}