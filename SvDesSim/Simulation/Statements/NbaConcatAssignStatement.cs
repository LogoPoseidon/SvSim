using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaConcatAssignStatement(
    ISimLogicSignal[] lhsSignals, 
    IExpression<SimLogic<BigInteger>> rhs, 
    EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var evaluatedValue = rhs.Evaluate();
        scheduler.Schedule(EventRegion.Nba, new NbaConcatUpdateEvent(lhsSignals, evaluatedValue));

        yield break;
    }
}