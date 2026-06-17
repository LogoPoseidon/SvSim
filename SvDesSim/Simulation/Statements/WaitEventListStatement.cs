using SvAstParser.AstTree.SvEnums;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class WaitEventListStatement(List<(ISimLogicSignal sig, SvEdgeKind edge)> events) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        yield return new SuspendEdgeList(events);
    }
}