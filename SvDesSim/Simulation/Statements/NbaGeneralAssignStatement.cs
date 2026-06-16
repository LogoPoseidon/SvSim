using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaGeneralAssignStatement(
    ISimLogicSignal lhs,
    IExpression<SimLogic<BigInteger>> rhs,
    EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhs.Evaluate();
        scheduler.Schedule(EventRegion.Nba, new NbaGeneralUpdateEvent(lhs, val));
        yield break;
    }
}