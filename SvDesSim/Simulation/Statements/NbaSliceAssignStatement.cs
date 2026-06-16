using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

public class NbaSliceAssignStatement(
    ISimLogicSignal signal, 
    int msb, 
    int lsb, 
    IExpression<SimLogic<BigInteger>> rhs, 
    EventScheduler scheduler) : IStatement
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var evaluatedValue = rhs.Evaluate();

        scheduler.Schedule(EventRegion.Nba, new NbaSliceUpdateEvent(signal, msb, lsb, evaluatedValue));

        yield break;
    }
}