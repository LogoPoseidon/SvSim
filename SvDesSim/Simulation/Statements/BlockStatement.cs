using SvDesSim.Simulation.Processes;

namespace SvDesSim.Simulation.Statements;

public class BlockStatement(List<IStatement> statements) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        return statements.SelectMany(stmt => stmt.Execute());
    }
}