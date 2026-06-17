using SvAstParser.AstTree.SvEnums;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Processes;

public class SuspendEdgeList(List<(ISimLogicSignal sig, SvEdgeKind edge)> edges) : YieldInstruction
{
    public List<(ISimLogicSignal sig, SvEdgeKind edge)> Edges { get; } = edges;
}