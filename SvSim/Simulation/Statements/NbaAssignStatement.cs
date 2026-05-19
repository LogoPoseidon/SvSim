using SvSim.Simulation.Engine;
using SvSim.Simulation.Expressions;
using SvSim.Simulation.Processes;
using SvSim.Simulation.Signal;

namespace SvSim.Simulation.Statements;

public class NbaAssignStatement<T>(SimVar<T> lhs, IExpression<T> rhs, EventScheduler scheduler) : IStatement 
    where T : IEquatable<T>
{
    public IEnumerable<YieldInstruction> Execute()
    {
        var val = rhs.Evaluate();
        scheduler.Schedule(EventRegion.Nba, new NbaUpdateEvent<T>(lhs, val));
        yield break;
    }
}