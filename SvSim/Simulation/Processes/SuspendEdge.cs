using SvSim.Simulation.Signal;
using SvSim.SlangAstParser.AstTree.SvEnums;

namespace SvSim.Simulation.Processes;

public class SuspendEdge(ISimLogicSignal signal, SvEdgeKind edgeType) : YieldInstruction
{
    public ISimLogicSignal Signal { get; } = signal;
    public SvEdgeKind EdgeType { get; } = edgeType;
}