using System.Numerics;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaBitAssignStatement(ISimLogicSignal signal, IExpression<SimLogic<BigInteger>> indexExpr, IExpression<SimLogic<BigInteger>> rhs, Engine.EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var idx = (int)indexExpr.Evaluate().Value;
        var value = rhs.Evaluate();
        scheduler.Schedule(Engine.EventRegion.Nba, new NbaSliceUpdateEvent(signal, idx, idx, value));
        yield break;
    }
}