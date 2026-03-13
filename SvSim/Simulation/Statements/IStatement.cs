using SvSim.Simulation.Processes;

namespace SvSim.Simulation.Statements;

public interface IStatement
{
    IEnumerable<YieldInstruction> Execute();
}