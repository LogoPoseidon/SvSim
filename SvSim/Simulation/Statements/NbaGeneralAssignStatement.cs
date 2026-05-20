using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

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