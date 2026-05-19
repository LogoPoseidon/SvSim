using System.Numerics;
using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

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