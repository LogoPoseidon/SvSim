using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.Simulation.Statements;

public class WaitEventStatement(ISimLogicSignal signal, SvEdgeKind edgeType) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        yield return new SuspendEdge(signal, edgeType);
    }
}