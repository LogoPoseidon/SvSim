using SvAstParser.AstTree.SvEnums;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class WaitEventStatement(ISimLogicSignal signal, SvEdgeKind edgeType) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        yield return new SuspendEdge(signal, edgeType);
    }
}