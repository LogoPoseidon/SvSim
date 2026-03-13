using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaConcatAssignStatement(
    ISimLogicSignal[] lhsSignals, 
    IExpression<SimLogic<BigInteger>> rhs, 
    EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var evaluatedValue = rhs.Evaluate();
        scheduler.Schedule(EventRegion.NBA, new NbaConcatUpdateEvent(lhsSignals, evaluatedValue));

        yield break;
    }
}