using SvAstParser.AstTree.SvEnums;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Processes;

public class SuspendEdge(ISimLogicSignal signal, SvEdgeKind edgeType) : YieldInstruction
{
    public ISimLogicSignal Signal { get; } = signal;
    public SvEdgeKind EdgeType { get; } = edgeType;
}