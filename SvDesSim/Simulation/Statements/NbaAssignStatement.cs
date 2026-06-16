using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Expressions;
using SvDesSim.Simulation.Processes;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Statements;

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