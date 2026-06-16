using System.Numerics;
using SvDesSim.Simulation.Engine;
using SvDesSim.Simulation.Signal;

namespace SvDesSim.Simulation.Expressions;

public class TimeExpr<T>(EventScheduler scheduler) : IExpression<SimLogic<T>> 
    where T : IBinaryInteger<T>
{
    public SimLogic<T> Evaluate()
    {
        return new SimLogic<T>(T.CreateTruncating(scheduler.CurrentTime), T.Zero);
    }
}